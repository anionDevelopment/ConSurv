import os
from ScriptCollection.GeneralUtilities import GeneralUtilities, Platform
from ScriptCollection.TFCPS.DotNet.TFCPS_CodeUnitSpecific_DotNet import TFCPS_CodeUnitSpecific_DotNet_Functions,TFCPS_CodeUnitSpecific_DotNet_CLI


def copy_resource_next_to_the_binary(source_folder: str, target_folder: str) -> None:
    """Puts a resource which the application resolves relative to its own assembly next to that assembly."""
    GeneralUtilities.ensure_folder_exists_and_is_empty(target_folder)
    GeneralUtilities.copy_content_of_folder(source_folder, target_folder)


def build():
    platforms:list[Platform] = [
            Platform.Windows_AMD64,
            Platform.Linux_AMD64,
            Platform.Linux_ARM64,
    ]
    tf:TFCPS_CodeUnitSpecific_DotNet_Functions=TFCPS_CodeUnitSpecific_DotNet_CLI.parse(__file__)
    tf.build(True)
    codeunit_folder: str = tf.get_codeunit_folder()
    resources_folder: str = os.path.join(codeunit_folder, "Other", "Resources")
    artifacts_folder: str = os.path.join(codeunit_folder, "Other", "Artifacts")
    # The overlay-text which is drawn into the stream of a camera uses the font which is delivered here and not one
    # of the operating-system, so the picture of a camera looks the same in every environment.
    fonts_source_folder: str = os.path.join(resources_folder, "Fonts")
    GeneralUtilities.assert_file_exists(os.path.join(fonts_source_folder, "Noto", "NotoSans_Condensed-Regular.ttf"))
    # The application resolves its mediamtx and its font relative to its own assembly, and the build-result of a
    # target-platform needs the mediamtx of exactly that platform. The test-assembly gets both from the project-file
    # of the testcases instead, because dotnet-test deploys it into a temporary folder and not into this folder.
    for target_platform in platforms:
        mediamtx_source_folder: str = os.path.join(resources_folder, f"MediaMTX_{GeneralUtilities.platform_to_dash_str(target_platform)}", "MediaMTX")
        build_result_folder: str = os.path.join(artifacts_folder, f"BuildResult_DotNet_{GeneralUtilities.platform_to_dotnet_runtime_identifier(target_platform)}")
        copy_resource_next_to_the_binary(mediamtx_source_folder, os.path.join(build_result_folder, "MediaMTX"))
        copy_resource_next_to_the_binary(fonts_source_folder, os.path.join(build_result_folder, "Fonts"))


if __name__ == "__main__":
    build()
