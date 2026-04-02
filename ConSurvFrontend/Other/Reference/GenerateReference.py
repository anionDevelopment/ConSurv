from ScriptCollection.TFCPS.NodeJS.TFCPS_CodeUnitSpecific_NodeJS import TFCPS_CodeUnitSpecific_NodeJS_Functions,TFCPS_CodeUnitSpecific_NodeJS_CLI
  
def generate_reference():
    
    tf:TFCPS_CodeUnitSpecific_NodeJS_Functions=TFCPS_CodeUnitSpecific_NodeJS_CLI.parse(__file__)
    tf.generate_reference()
    tf.tfcps_Tools_General.try_update_basic_codeunitreference_from_examples_repository(tf.get_codeunit_folder(),"AngularCodeUnit")


if __name__ == "__main__":
    generate_reference()
