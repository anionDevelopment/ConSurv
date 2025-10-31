from pathlib import Path
from ScriptCollection.ScriptCollectionCore import ScriptCollectionCore
from ScriptCollection.TFCPS.TFCPS_Tools_General import TFCPS_Tools_General


def start_dockerfile_example():
    current_file = str(Path(__file__).absolute())
    env_file_name = "Variables.env"
    sc=ScriptCollectionCore()
    t: TFCPS_Tools_General = TFCPS_Tools_General(sc)
    t.ensure_env_file_is_generated(current_file, env_file_name, dict({
        'InitialAdminPassword': 'admin',
        'InitialCameraAddresses': 'rtsp://@192.168.1.141/stream1;rtsp://@192.168.1.142/stream1',
    }))
    t.start_dockerfile_example(current_file, True, False,  env_file_name)


if __name__ == "__main__":
    start_dockerfile_example()
