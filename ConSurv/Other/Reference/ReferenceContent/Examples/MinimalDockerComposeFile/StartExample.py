import sys
from pathlib import Path
from ScriptCollection.TasksForCommonProjectStructure import TasksForCommonProjectStructure


def start_dockerfile_example():
    current_file=str(Path(__file__).absolute())
    env_file_name="ConSurv.env"
    t:TasksForCommonProjectStructure=TasksForCommonProjectStructure()
    t.ensure_env_file_is_generated(current_file,env_file_name,dict({
        'A': 'v1',
        'B': 'v2', 
    }))
    t.start_dockerfile_example(current_file, 3, True, True, sys.argv,env_file_name)


if __name__ == "__main__":
    start_dockerfile_example()
