from pathlib import Path
import argparse
from ScriptCollection.GeneralUtilities import GeneralUtilities
from ScriptCollection.ScriptCollectionCore import ScriptCollectionCore
from ScriptCollection.TFCPS.TFCPS_Tools_General import TFCPS_Tools_General


def start_dockerfile_example():
    parser = argparse.ArgumentParser()
    parser.add_argument('-v', '--verbose',  required=False, action='store_true', default=False)
    args = parser.parse_args()
    current_file = str(Path(__file__).absolute())
    repository_folder:str=GeneralUtilities.resolve_relative_path("../../../../../../..",current_file)
    env_file_name = "Variables.env"
    sc=ScriptCollectionCore()
    t: TFCPS_Tools_General = TFCPS_Tools_General(sc)
    env_values=dict({
        'InitialAdminPassword': 'admin',
        'InitialDatabaseType': 'PostgreSQL',
        'InitialDatabaseConnectionString': 'postgresql://user:pa55w0rd@consurv_database:5432/ConSurvDatabase',
        'InitialCameraAddresses': 'rtsp://192.168.1.141/stream1;rtsp://192.168.1.142/stream1',
        "image_postgresql":t.oci_image_manager.get_registry_address_for_image_with_default_tag(repository_folder,"PostgreSQL"),
        "image_adminer":t.oci_image_manager.get_registry_address_for_image_with_default_tag(repository_folder,"Adminer"),
    })
    if args.verbose:
        env_values["InitialVerboseValue"] = GeneralUtilities.empty_string#env variable must just be there to get rexognized
    t.ensure_env_file_is_generated(current_file, env_file_name,env_values)
    t.start_dockerfile_example(current_file, True, False, env_file_name)


if __name__ == "__main__":
    start_dockerfile_example()
