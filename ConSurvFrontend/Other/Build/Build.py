from pathlib import Path
from ScriptCollection.GeneralUtilities import GeneralUtilities
from ScriptCollection.TasksForCommonProjectStructure import TasksForCommonProjectStructure


def standardized_tasks_build_for_angular_extract_i18n(t: TasksForCommonProjectStructure, scriptfile: str):
    codeunit_folder: str = GeneralUtilities.resolve_relative_path("../../..", scriptfile)
    t.run_with_epew("ng", "extract-i18n --format=xlf2", codeunit_folder)


def build():
    t = TasksForCommonProjectStructure()
    file = str(Path(__file__).absolute())
    # t.standardized_tasks_build_for_angular_codeunit(file, "QualityCheck",  1, sys.argv)
    standardized_tasks_build_for_angular_extract_i18n(t, file)


if __name__ == "__main__":
    build()
