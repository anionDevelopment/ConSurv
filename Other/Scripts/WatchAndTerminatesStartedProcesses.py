import time
import psutil
import os
import argparse
from pathlib import Path
from ScriptCollection.GeneralUtilities import GeneralUtilities


def watch_and_terminate_started_processes():
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
    codeunit_folder = GeneralUtilities.resolve_relative_path("../../../ConSurvBackend", current_file)
    process_list_file = GeneralUtilities.resolve_relative_path("./Other/Workspace/Configuration/StartedProcesses.txt", codeunit_folder)
    if os.path.exists(process_list_file):
        for line in GeneralUtilities.read_lines_from_file(process_list_file):
            if GeneralUtilities.string_has_content(line):
                current_process_id = int(line.strip())
                kill_process(current_process_id,True)
        GeneralUtilities.write_text_to_file(process_list_file, "")
        GeneralUtilities.write_message_to_stdout("All started processes terminated.")
    else:
        GeneralUtilities.write_message_to_stdout(f"File {process_list_file} does not exist. No processes to terminate.")


if __name__ == "__main__":
    watch_and_terminate_started_processes()
