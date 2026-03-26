import os
from ScriptCollection.TFCPS.NodeJS.TFCPS_CodeUnitSpecific_NodeJS import TFCPS_CodeUnitSpecific_NodeJS_Functions,TFCPS_CodeUnitSpecific_NodeJS_CLI
from ScriptCollection.GeneralUtilities import GeneralUtilities
from ScriptCollection.ScriptCollectionCore import ScriptCollectionCore

import xml.etree.ElementTree as ET
import copy
from pathlib import Path

@GeneralUtilities.check_arguments
def is_xliff2_file(file: str) -> bool:
    tree = ET.parse(file)
    root = tree.getroot()

    tag = root.tag  # "{urn:oasis:names:tc:xliff:document:2.0}xliff"

    if tag.startswith("{"):
        namespace, localname = tag[1:].split("}", 1)
    else:
        namespace = None
        localname = tag

    if localname != "xliff":
        return False

    if namespace != "urn:oasis:names:tc:xliff:document:2.0":
        return False

    if root.get("version") != "2.0":
        return False

    return True

@GeneralUtilities.check_arguments
def __sync_xlf2_files(base_file:ET.ElementTree, language_files:dict [
    str,#filepath
    ET.ElementTree#parsed file
    ]):
    """This function assumes that all files are valid xliff2 files and that the base file is the reference for syncing.
    This function adds new entries from the base file to the language files if they do not already exist using the value from base_file.
    This function removes entries from the language files if they do not exist in the base file anymore.
    In the end the updated language files are written to the disk. The base file is not changed."""
    #The file which was parsed looks like:
    #<?xml version="1.0" encoding="UTF-8" ?>
    #<xliff version="2.0" xmlns="urn:oasis:names:tc:xliff:document:2.0" srcLang="en">
    #  <file id="ngi18n" original="ng.template">
    #    <unit id="logingreeting">
    #      <notes>
    #        <note category="location">src/app/modules/home-page/login-form/login-form.component.html:2,4</note>
    #      </notes>
    #      <segment>
    #        <source>Welcome back, please login</source>
    #      </segment>
    #    </unit>
    #    <unit id="username">
    #      <notes>
    #        <note category="location">src/app/modules/home-page/login-form/login-form.component.html:5,6</note>
    #      </notes>
    #      <segment>
    #        <source>Username</source>
    #      </segment>
    #    </unit>
    #    <unit id="password">
    #      <notes>
    #        <note category="location">src/app/modules/home-page/login-form/login-form.component.html:12,13</note>
    #      </notes>
    #      <segment>
    #        <source>Password</source>
    #      </segment>
    #    </unit>
    #  </file>
    #</xliff>

    NS = "urn:oasis:names:tc:xliff:document:2.0"
    NSMAP = {"x": NS}
    base_root = base_file.getroot()
    base_file_element = base_root.find("x:file", namespaces=NSMAP)
    if base_file_element is None:
        raise ValueError("Invalid XLIFF base file: <file> element not found")

    # Collect base units
    base_units = {
        unit.get("id"): unit
        for unit in base_file_element.findall("x:unit", namespaces=NSMAP)
    }
    base_ids = set(base_units.keys())
    for filepath, lang_tree in language_files.items():
        lang_root = lang_tree.getroot()
        lang_file_element = lang_root.find("x:file", namespaces=NSMAP)
        if lang_file_element is None:
            raise ValueError(f"{filepath}: <file> element not found")

        # Collect language units
        lang_units = {
            unit.get("id"): unit
            for unit in lang_file_element.findall("x:unit", namespaces=NSMAP)
        }
        lang_ids = set(lang_units.keys())

        # Remove obsolete units
        obsolete_ids = lang_ids - base_ids
        for unit_id in obsolete_ids:
            lang_file_element.remove(lang_units[unit_id])

        # Add missing units
        missing_ids = base_ids - lang_ids
        for unit_id in missing_ids:
            new_unit = copy.deepcopy(base_units[unit_id])
            lang_file_element.append(new_unit)

        # Reorder units to match base order
        current_units = {
            unit.get("id"): unit
            for unit in lang_file_element.findall("x:unit", namespaces=NSMAP)
        }
        for unit in list(lang_file_element.findall("x:unit", namespaces=NSMAP)):
            lang_file_element.remove(unit)
        for unit_id in base_units.keys():
            if unit_id in current_units:
                lang_file_element.append(current_units[unit_id])

        # Write file back to disk
        Path(filepath).write_bytes(
            ET.tostring(
                lang_tree.getroot(),
                xml_declaration=True,
                encoding="UTF-8"
            )
        )
        ScriptCollectionCore().format_xml_file(filepath)

@GeneralUtilities.check_arguments
def sync_xlf2_files(prefix:str, languages:list[str], folder:str):
    #languages=["de", "fr"] for example. the default-language (usually english) must not be included.
    base_file=os.path.join(folder, f"{prefix}.xlf")
    base_file_xml:ET.ElementTree=ET.parse(base_file)
    GeneralUtilities.assert_condition(is_xliff2_file(base_file), f"The base file '{base_file}' is not a valid XLIFF 2.0 file.")
    GeneralUtilities.assert_file_exists(base_file)
    if len(languages)==0:
        raise ValueError("No files provided for syncing.")
    if len(languages)==1:
        return
    language_files_list=[os.path.join(folder, f"{prefix}.{language}.xlf") for language in languages]
    language_files_with_content:dict[str,ET.ElementTree]=dict()
    not_existing_files:list[str]=[]
    for language_file in language_files_list:
        if os.path.isfile(language_file):
            GeneralUtilities.assert_condition(is_xliff2_file(base_file), f"The base file '{base_file}' is not a valid XLIFF 2.0 file.")
            language_files_with_content[language_file]=ET.parse(language_file)
        else:
            not_existing_files.append(language_file)

    #create not existing files
    for not_existing_file in not_existing_files:
        GeneralUtilities.ensure_directory_exists(os.path.dirname(not_existing_file))
        GeneralUtilities.ensure_file_exists(not_existing_file)
        GeneralUtilities.write_text_to_file(not_existing_file, GeneralUtilities.read_text_from_file(base_file))
    
    #sync existing files
    __sync_xlf2_files(base_file_xml, language_files_with_content)


def build():
    tf:TFCPS_CodeUnitSpecific_NodeJS_Functions=TFCPS_CodeUnitSpecific_NodeJS_CLI.parse(__file__)
    #tf.build()
    #tf.add_culture_chooser(tf.get_product_name(),tf.get_available_cultures_for_angular_app())
    #tf.add_maintenance_site(tf.get_product_name())
    #tf._protected_sc.run_program()
    tf._protected_sc.run_with_epew("ng","extract-i18n",tf.get_codeunit_folder())
    #tf._protected_sc.sync_xlf2_files("messages",["de","fr"],os.path.join(tf.get_codeunit_folder(),"Other","Resources","Translations"))
    sync_xlf2_files("messages",["de","fr"],os.path.join(tf.get_codeunit_folder(),"Other","Resources","Translations"))



if __name__ == "__main__":
    build()
