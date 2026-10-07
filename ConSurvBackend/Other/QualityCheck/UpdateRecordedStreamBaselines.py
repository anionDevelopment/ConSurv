import os
from pathlib import Path
from ScriptCollection.GeneralUtilities import GeneralUtilities
from ScriptCollection.ScriptCollectionCore import ScriptCollectionCore

# The name of the environment-variable which makes the testcase write the baseline-pictures instead of comparing
# against them. It has to be the one in RecordedStreamVisualRegressionTests.
_name_of_the_variable_which_updates_the_baselines: str = "ConSurvUpdateRecordedStreamBaselines"

# The testcase which records the streams and writes the pictures.
_name_of_the_testcase: str = "TheRecordedVideoOfEveryStreamLooksLikeItsBaselinePicture"

# The streams whose pictures are written. The names have to be the ones in RecordedStreamVisualRegressionTests.
_names_of_the_streams: list[str] = ["TestStreamA", "TestStreamB"]


def update_recorded_stream_baselines():
    """Creates the pictures which the testcase of the recorded streams compares against.

    The pictures are taken from a video which the application really recorded and not from a generated one: what
    has to be compared is what the application produces, including the text which it draws into the picture. The
    testcase itself writes them, because it is the one which brings the application into the state in which it
    records. Everything which can not be reproduced - the number of the stream and the changing part of the time -
    is masked by the testcase before a picture is written.

    Always run the testcases afterwards: a freshly written baseline-picture says nothing about whether the
    application records reproducibly, only the comparison shows that."""
    current_file = str(Path(__file__).absolute())
    codeunit_folder = GeneralUtilities.resolve_relative_path("../../..", current_file)
    codeunit_name = os.path.basename(codeunit_folder)
    test_project_file = os.path.join(codeunit_folder, f"{codeunit_name}Tests", f"{codeunit_name}Tests.csproj")
    GeneralUtilities.assert_file_exists(test_project_file)
    os.environ[_name_of_the_variable_which_updates_the_baselines] = "true"
    try:
        # The testcase reports itself as inconclusive after writing the pictures, which dotnet counts as a run
        # which was not successful, so the exit-code does not tell whether the update worked. That the pictures
        # exist afterwards does.
        ScriptCollectionCore().run_program_argsasarray(
            "dotnet",
            ["test", test_project_file, "--configuration", "QualityCheck", "--filter", _name_of_the_testcase],
            codeunit_folder,
            throw_exception_if_exitcode_is_not_zero=False)
    finally:
        del os.environ[_name_of_the_variable_which_updates_the_baselines]
    target_folder = os.path.join(codeunit_folder, "Other", "Resources", "RecordedStreamBaselines")
    for name_of_the_stream in _names_of_the_streams:
        GeneralUtilities.assert_file_exists(os.path.join(target_folder, f"{name_of_the_stream}.png"))
        GeneralUtilities.write_message_to_stdout(f"Created the baseline-picture of {name_of_the_stream}.")


if __name__ == "__main__":
    update_recorded_stream_baselines()
