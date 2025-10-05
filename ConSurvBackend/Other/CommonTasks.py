import os
import zipfile
import tarfile
from ScriptCollection.GeneralUtilities import GeneralUtilities
from ScriptCollection.TFCPS.TFCPS_Tools_General import TFCPS_Tools_General
from ScriptCollection.TFCPS.DotNet.TFCPS_CodeUnitSpecific_DotNet import TFCPS_CodeUnitSpecific_DotNet_Functions,TFCPS_CodeUnitSpecific_DotNet_CLI
from ScriptCollection.TFCPS.DotNet.CertificateGeneratorInformationGenerate import CertificateGeneratorInformationGenerate

 
@GeneralUtilities.check_arguments
def ensure_mediamtx_is_available(t: TFCPS_Tools_General, target_folder: str) -> None:
    def download_and_extract(osname: str, osname_in_github_asset: str, extension: str):
        resource_name: str = f"MediaMTX_{osname}"
        zip_filename: str = f"{resource_name}.{extension}"
        downloaded_file=t.ensure_file_from_github_assets_is_available_with_retry(target_folder, "bluenviron", "mediamtx", resource_name, zip_filename, lambda latest_version: f"mediamtx_{latest_version}_{osname_in_github_asset}_amd64.{extension}")
        resource_folder: str = os.path.join(target_folder, "Other", "Resources", resource_name)
        target_folder_extracted = os.path.join(resource_folder, "MediaMTX")
        GeneralUtilities.ensure_folder_exists_and_is_empty(target_folder_extracted)
        if extension == "zip":
            with zipfile.ZipFile(downloaded_file, 'r') as zip_ref:
                zip_ref.extractall(target_folder_extracted)
        elif extension == "tar.gz":
            with tarfile.open(downloaded_file, "r:gz") as tar:
                tar.extractall(path=target_folder_extracted)
        else:
            raise ValueError(f"Unknown extension: \"{extension}\"")

    download_and_extract("Windows", "windows", "zip")
    download_and_extract("Linux", "linux", "tar.gz")
    download_and_extract("MacOS", "darwin", "tar.gz")


def common_tasks():
    tf:TFCPS_CodeUnitSpecific_DotNet_Functions=TFCPS_CodeUnitSpecific_DotNet_CLI.parse(__file__)    
    tf.tfcps_Tools_General.get_resource_from_global_resource(tf.get_codeunit_folder(), "DevelopmentCertificate")
    tf.do_common_tasks(tf.get_version_of_project(),CertificateGeneratorInformationGenerate(),True)#codeunit-version should alsways be the same as project-version
    ensure_mediamtx_is_available(tf.tfcps_Tools_General,tf.get_codeunit_folder())

if __name__ == "__main__":
    common_tasks()
