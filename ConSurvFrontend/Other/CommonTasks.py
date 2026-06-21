import os
import shutil
from ScriptCollection.TFCPS.NodeJS.TFCPS_CodeUnitSpecific_NodeJS import TFCPS_CodeUnitSpecific_NodeJS_Functions,TFCPS_CodeUnitSpecific_NodeJS_CLI


def sync_openapigenerator_version(codeunit_folder:str) -> None:
    # The OpenAPIGenerator-version is resolved from the repository-wide pinned version-file.
    # Keep the codeunit-local version-file in sync with it so both always reference the same version.
    relative_version_file = os.path.join("Other", "Resources", "Dependencies", "OpenAPIGenerator", "Version.txt")
    repository_folder = os.path.dirname(codeunit_folder)
    source_version_file = os.path.join(repository_folder, relative_version_file)
    target_version_file = os.path.join(codeunit_folder, relative_version_file)
    shutil.copyfile(source_version_file, target_version_file)


def common_tasks():
    tf:TFCPS_CodeUnitSpecific_NodeJS_Functions=TFCPS_CodeUnitSpecific_NodeJS_CLI.parse(__file__)
    tf.do_common_tasks(tf.get_version_of_project())#codeunit-version should alsways be the same as project-version
    codeunit_folder = tf.get_codeunit_folder()
    sync_openapigenerator_version(codeunit_folder)
    tf.tfcps_Tools_General.generate_api_client_from_dependent_codeunit(codeunit_folder,"ConSurvBackend","src/app/generated/con-surv-backend", "typescript-angular",tf.use_cache(),["apis","models","supportingFiles"])

if __name__ == "__main__":
    common_tasks()
 