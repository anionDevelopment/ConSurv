import sys
import os
from pathlib import Path
from ScriptCollection.GeneralUtilities import GeneralUtilities
from ScriptCollection.TasksForCommonProjectStructure import TasksForCommonProjectStructure


def build():
    t = TasksForCommonProjectStructure()
    file = str(Path(__file__).absolute())
    cmd_args = sys.argv
    verbosity = t.get_verbosity_from_commandline_arguments(cmd_args, 1)
    target_windows = "win-x64"
    target_platforms = [target_windows, "linux-x64"]
    t.standardized_tasks_build_for_dotnet_project(str(Path(__file__).absolute()), "QualityCheck", t.get_default_target_environmenttype_mapping(), target_platforms, verbosity, cmd_args)
    t.generate_openapi_file(file, target_windows, verbosity, cmd_args)
    codeunit_folder: str = GeneralUtilities.resolve_relative_path("../../..", file)
    for target_platform in target_platforms:
        os_name: str
        if target_platform == "win-x64":
            os_name = "Windows"
        elif target_platform == "linux-x64":
            os_name = "Linux"
        else:
            raise ValueError(f"Unknown OS: \"{os_name}\"")
        src_folder: str = os.path.join(codeunit_folder, "Other", "Resources", f"MediaMTX_{os_name}", "MediaMTX")
        trg_folder: str = os.path.join(codeunit_folder, "Other", "Artifacts", f"BuildResult_DotNet_{target_platform}", "MediaMTX")
        GeneralUtilities.ensure_folder_exists_and_is_empty(trg_folder)
        GeneralUtilities.copy_content_of_folder(src_folder, trg_folder)


if __name__ == "__main__":
    build()
