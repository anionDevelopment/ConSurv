import time
import psutil
import os
import argparse
from pathlib import Path
from ScriptCollection.GeneralUtilities import GeneralUtilities


def process_is_running_by_id(process_id: int) -> bool:  # TODO move this to scriptcollection
    processes: list[psutil.Process] = list(psutil.process_iter())
    for p in processes:
        if p.pid == process_id:
            return True
    return False


def watch_and_terminate_started_processes():
    while True:
        time.sleep(1)
        GeneralUtilities.write_message_to_stdout("check...")
        # parser = argparse.ArgumentParser(description='Watches programs started by ConSurvBackend in the debug-mode and terminates the remaining processes ')
        # parser.add_argument('-p', '--processid')
        # args = parser.parse_args()
        # process_id = args.processid
        # GeneralUtilities.assert_condition(process_is_running_by_id(process_id), f"Process with id {process_id} is not running.")
        # while process_is_running_by_id(process_id):
        #    time.sleep(1)
        # GeneralUtilities.write_message_to_stdout(f"Process with id {process_id} is not running anymore. Start terminating remaining processes.")
        current_file = str(Path(__file__).absolute())
        codeunit_folder = GeneralUtilities.resolve_relative_path("../..", current_file)
        process_list_file = GeneralUtilities.resolve_relative_path("./Other/Workspace/Configuration/StartedProcesses.txt", codeunit_folder)
        if os.path.exists(process_list_file):
            for line in GeneralUtilities.read_lines_from_file(process_list_file):
                current_process_id = int(line.strip())
                if process_is_running_by_id(current_process_id):
                    GeneralUtilities.write_message_to_stdout(f"Process with id {current_process_id} is running. Terminating it...")
                    process = psutil.Process(current_process_id)
                    process.kill()
                    process.wait()
                else:
                    GeneralUtilities.write_message_to_stdout(f"Process with id {current_process_id} is not running anymore.")
            GeneralUtilities.write_text_to_file(process_list_file, "")
            GeneralUtilities.write_message_to_stdout("All started processes terminated.")
        else:
            GeneralUtilities.write_message_to_stdout(f"File {process_list_file} does not exist. No processes to terminate.")


if __name__ == "__main__":
    watch_and_terminate_started_processes()
