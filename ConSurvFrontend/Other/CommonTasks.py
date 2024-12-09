import sys
import os
from pathlib import Path
from ScriptCollection.ScriptCollectionCore import ScriptCollectionCore
from ScriptCollection.GeneralUtilities import GeneralUtilities
from ScriptCollection.TasksForCommonProjectStructure import TasksForCommonProjectStructure
from ScriptCollection.ProgramRunnerEpew import ProgramRunnerEpew


def generate_api_client_from_dependent_codeunit_in_angular(s: TasksForCommonProjectStructure, file: str, name_of_api_providing_codeunit: str, generated_program_part_name: str) -> None:
    codeunit_folder = GeneralUtilities.resolve_relative_path("../..", file)
    target_subfolder_in_codeunit = f"src/app/generated/{generated_program_part_name}"
    language = "typescript-angular"
    s.ensure_openapigenerator_is_available(codeunit_folder)
    openapigenerator_jar_file = os.path.join(codeunit_folder, "Other", "Resources", "OpenAPIGenerator", "open-api-generator.jar")
    openapi_spec_file = os.path.join(codeunit_folder, "Other", "Resources", "DependentCodeUnits", name_of_api_providing_codeunit, "APISpecification", f"{name_of_api_providing_codeunit}.latest.api.json")
    target_folder = os.path.join(codeunit_folder, target_subfolder_in_codeunit)
    GeneralUtilities.ensure_folder_exists_and_is_empty(target_folder)
    ScriptCollectionCore().run_program("java", f'-jar {openapigenerator_jar_file} generate -i {openapi_spec_file} -g {language} -o {target_folder} --global-property supportingFiles --global-property models --global-property apis', codeunit_folder)


def common_tasks():
    file = str(Path(__file__).absolute())
    cmd_args = sys.argv
    t = TasksForCommonProjectStructure()
    sc = ScriptCollectionCore()
    build_environment = t.get_targetenvironmenttype_from_commandline_arguments(cmd_args, "QualityCheck")
    verbosity = t.get_verbosity_from_commandline_arguments(cmd_args, 1)
    file = str(Path(__file__).absolute())
    codeunit_version = sc.get_semver_version_from_gitversion(GeneralUtilities.resolve_relative_path("../..", os.path.dirname(file)))  # Should always be the same as the project-version
    folder_of_current_file = os.path.dirname(file)
    sc.program_runner = ProgramRunnerEpew()
    codeunit_folder = GeneralUtilities.resolve_relative_path("..", os.path.dirname(file))
    additional_arguments_file = t.get_additionalargumentsfile_from_commandline_arguments(cmd_args, None)
    t.replace_version_in_packagejson_file(GeneralUtilities.resolve_relative_path("../package.json", folder_of_current_file), codeunit_version)
    t.standardized_tasks_do_common_tasks(file, codeunit_version, verbosity, build_environment, True, additional_arguments_file, False, cmd_args)
    t.do_npm_install(codeunit_folder, True, verbosity=verbosity)
    generate_api_client_from_dependent_codeunit_in_angular(t, file, "ConSurvBackend", "con-surv-backend")


if __name__ == "__main__":
    common_tasks()
