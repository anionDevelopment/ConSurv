from pathlib import Path
from ScriptCollection.TFCPS.Flutter.TFCPS_CodeUnitSpecific_Flutter import TFCPS_CodeUnitSpecific_Flutter_Functions,TFCPS_CodeUnitSpecific_Flutter_CLI


def update_dependencies():
    tf:TFCPS_CodeUnitSpecific_Flutter_Functions=TFCPS_CodeUnitSpecific_Flutter_CLI.parse(__file__)    
    update_dependencies_script_file = str(Path(__file__).absolute())
    tf.update_dependencies()
    tf.tfcps_Tools_General.set_version_of_openapigenerator(tf.get_codeunit_folder(),tf.tfcps_Tools_General.get_latest_version_of_openapigenerator())


if __name__ == "__main__":
    update_dependencies()
 