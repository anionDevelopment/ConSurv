import sys
from pathlib import Path
import os
import re
from lxml import etree
from ScriptCollection.GeneralUtilities import GeneralUtilities
from ScriptCollection.TasksForCommonProjectStructure import TasksForCommonProjectStructure


def run_testcases():
    TasksForCommonProjectStructure().standardized_tasks_run_testcases_for_flutter_project_in_common_project_structure(str(Path(__file__).absolute()),  1, sys.argv, "con_surv_app", "QualityCheck", True)


if __name__ == "__main__":
    run_testcases()
