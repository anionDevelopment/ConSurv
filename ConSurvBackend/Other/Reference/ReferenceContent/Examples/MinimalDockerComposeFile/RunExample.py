from pathlib import Path
from ScriptCollection.ScriptCollectionCore import ScriptCollectionCore
from ScriptCollection.TFCPS.TFCPS_Tools_General import TFCPS_Tools_General


def run_example():
    sc=ScriptCollectionCore()
    t=TFCPS_Tools_General(sc)
    env_file_name = "Variables.env"
    t.start_dockerfile_example(str(Path(__file__).absolute()), True, False, env_file_name)
   # HINT now open your browser and open http://127.0.0.1:80

if __name__ == "__main__":
    run_example()
