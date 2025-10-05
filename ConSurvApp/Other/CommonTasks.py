import sys
import os
from pathlib import Path
from ScriptCollection.ScriptCollectionCore import ScriptCollectionCore
from ScriptCollection.GeneralUtilities import GeneralUtilities
from ScriptCollection.TasksForCommonProjectStructure import TasksForCommonProjectStructure


def common_tasks():
    cmd_args = sys.argv
    t = TasksForCommonProjectStructure()
    sc = ScriptCollectionCore()
    build_environment = t.get_targetenvironmenttype_from_commandline_arguments(cmd_args, "QualityCheck")
    verbosity = t.get_verbosity_from_commandline_arguments(cmd_args, 1)
    file = str(Path(__file__).absolute())
    codeunit_folder = GeneralUtilities.resolve_relative_path("..", os.path.dirname(file))
    codeunit_version = sc.get_semver_version_from_gitversion(GeneralUtilities.resolve_relative_path(
        "../..", os.path.dirname(file)))  # Should always be the same as the project-version
    additional_arguments_file = t.get_additionalargumentsfile_from_commandline_arguments(cmd_args, None)
    t.standardized_tasks_do_common_tasks(file, codeunit_version, verbosity, build_environment, True, additional_arguments_file, True, cmd_args)
    t.ensure_androidappbundletool_is_available(codeunit_folder)


if __name__ == "__main__":
    common_tasks()
