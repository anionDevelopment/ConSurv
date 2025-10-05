from ScriptCollection.TFCPS.Flutter.TFCPS_CodeUnitSpecific_Flutter import TFCPS_CodeUnitSpecific_Flutter_Functions,TFCPS_CodeUnitSpecific_Flutter_CLI


def common_tasks():
    tf:TFCPS_CodeUnitSpecific_Flutter_Functions=TFCPS_CodeUnitSpecific_Flutter_CLI.parse(__file__)    
    tf.do_common_tasks(tf.get_version_of_project())#codeunit-version should alsways be the same as project-version
 
if __name__ == "__main__":
    common_tasks()
 