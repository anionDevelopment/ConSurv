import os
from ScriptCollection.GeneralUtilities import GeneralUtilities, Platform
from ScriptCollection.TFCPS.DotNet.TFCPS_CodeUnitSpecific_DotNet import TFCPS_CodeUnitSpecific_DotNet_Functions,TFCPS_CodeUnitSpecific_DotNet_CLI
 
def build():
    platforms:list[Platform] = [
            Platform.Windows_AMD64,
            Platform.Linux_AMD64,
            Platform.Linux_ARM64,
    ]
    tf:TFCPS_CodeUnitSpecific_DotNet_Functions=TFCPS_CodeUnitSpecific_DotNet_CLI.parse(__file__)
    tf.build([GeneralUtilities.platform_to_dotnet_runtime_identifier(p)  for p in platforms], True)
    codeunit_folder: str = tf.get_codeunit_folder()
    for target_platform in platforms:
        mediamtx_src_folder: str = os.path.join(codeunit_folder, "Other", "Resources", f"MediaMTX_{GeneralUtilities.platform_to_dash_str(target_platform)}", "MediaMTX")
        mediamtx_trg_folder: str = os.path.join(codeunit_folder, "Other", "Artifacts", f"BuildResult_DotNet_{GeneralUtilities.platform_to_dotnet_runtime_identifier(target_platform)}", "MediaMTX")
        GeneralUtilities.ensure_folder_exists_and_is_empty(mediamtx_trg_folder)
        GeneralUtilities.copy_content_of_folder(mediamtx_src_folder, mediamtx_trg_folder)


if __name__ == "__main__":
    build()
