import os
from ScriptCollection.GeneralUtilities import GeneralUtilities
from ScriptCollection.TFCPS.DotNet.TFCPS_CodeUnitSpecific_DotNet import TFCPS_CodeUnitSpecific_DotNet_Functions,TFCPS_CodeUnitSpecific_DotNet_CLI
 
def build():

    platforms = ["win-x64", "linux-x64"]
    tf:TFCPS_CodeUnitSpecific_DotNet_Functions=TFCPS_CodeUnitSpecific_DotNet_CLI.parse(__file__)
    tf.build(platforms, True) 
    codeunit_folder: str = tf.get_codeunit_folder()

    for target_platform in platforms:

        # copy mediamtx to output folder
        os_name: str
        if target_platform == "win-x64":
            os_name = "Windows"
        elif target_platform == "linux-x64":
            os_name = "Linux"
        else:
            raise ValueError(f"Unknown OS: \"{os_name}\"")
        mediamtx_src_folder: str = os.path.join(codeunit_folder, "Other", "Resources", f"MediaMTX_{os_name}", "MediaMTX")
        mediamtx_trg_folder: str = os.path.join(codeunit_folder, "Other", "Artifacts", f"BuildResult_DotNet_{target_platform}", "MediaMTX")
        GeneralUtilities.ensure_folder_exists_and_is_empty(mediamtx_trg_folder)
        GeneralUtilities.copy_content_of_folder(mediamtx_src_folder, mediamtx_trg_folder)

        # copy font to output folder
        font_src_folder: str = os.path.join(codeunit_folder, "Other", "Resources", "Fonts")
        font_trg_folder: str = os.path.join(codeunit_folder, "Other", "Artifacts", f"BuildResult_DotNet_{target_platform}", "Fonts")
        GeneralUtilities.ensure_folder_exists_and_is_empty(font_trg_folder)
        GeneralUtilities.copy_content_of_folder(font_src_folder, font_trg_folder)


if __name__ == "__main__":
    build()
