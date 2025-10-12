import sys
from ScriptCollection.TFCPS.NodeJS.TFCPS_CodeUnitSpecific_NodeJS import TFCPS_CodeUnitSpecific_NodeJS_Functions,TFCPS_CodeUnitSpecific_NodeJS_CLI


def common_tasks():
    sys.arv=sys.argv+["-v","5"]
    tf:TFCPS_CodeUnitSpecific_NodeJS_Functions=TFCPS_CodeUnitSpecific_NodeJS_CLI.parse(__file__)    
    tf.do_common_tasks(tf.get_version_of_project())#codeunit-version should alsways be the same as project-version
    tf.tfcps_Tools_General.generate_api_client_from_dependent_codeunit(tf.get_codeunit_folder(),"ConSurvBackend","src/app/generated/con-surv-backend", "typescript-angular",tf.use_cache(),["apis","models","supportingFiles"])
 
if __name__ == "__main__":
    common_tasks()
 