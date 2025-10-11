from ScriptCollection.TFCPS.Flutter.TFCPS_CodeUnitSpecific_Flutter import TFCPS_CodeUnitSpecific_Flutter_Functions,TFCPS_CodeUnitSpecific_Flutter_CLI


def common_tasks():
    tf:TFCPS_CodeUnitSpecific_Flutter_Functions=TFCPS_CodeUnitSpecific_Flutter_CLI.parse(__file__)    
    tf.do_common_tasks(tf.get_version_of_project())#codeunit-version should alsways be the same as project-version
    tf.tfcps_Tools_General.generate_api_client_from_dependent_codeunit(tf.get_codeunit_folder(),"ConSurvBackend","con-surv-app/lib/generated/backend-api", "dart",tf.use_cache(),["apis","models"])
 
if __name__ == "__main__":
    common_tasks()
 