using ConSurvBackend.Core.Services;
using ConSurvBackend.Tests.TestUtilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GUtilities = GRYLibrary.Core.Misc.Utilities;
using System.IO;
using System.Linq;
using System.Threading;

namespace ConSurvBackend.Tests.Testcases.BackgroundServices
{
    /// <summary>
    /// Checks that what the application records is really the picture of the camera it belongs to.
    ///
    /// Two test-streams are recorded at the same time. Each of them shows its own name and a number which is
    /// counted up once per second, so a picture of a recording says two things at once: which stream it came from
    /// and how much time passed inside it. A recording which contains the other camera, which is frozen or which
    /// runs at a wrong speed therefore fails here, and none of that is visible in a testcase which only checks
    /// that a file was written.
    ///
    /// What is compared is a picture of the recording of the application, so it contains everything the application
    /// puts into it, including the text with the name and the id of the camera. Two areas of it can not be part of a
    /// baseline-picture and are masked in both pictures: the current time, and the number of the stream. The number
    /// belongs to the timeline of the *stream* while the compared moments belong to the timeline of the *recording*,
    /// and both do not begin at the same instant - the application only starts recording after it could probe the
    /// camera and bring up its media-processes (see CameraManagementService), so how far the stream is already
    /// advanced at that point is not predictable.
    ///
    /// That the number nevertheless runs correctly is asserted without knowing it: it changes exactly once per
    /// second, so the amount of its changes within a part of the recording has to be the duration of that part in
    /// seconds. A frozen picture produces fewer changes and a wrong playback-speed produces another amount.
    /// </summary>
    [TestClass]
    public class RecordedStreamVisualRegressionTests
    {
        private const string _NameOfTheFirstStream = "TestStreamA";
        private const string _NameOfTheSecondStream = "TestStreamB";

        /// <summary>The name of the environment-variable which makes this testcase write the baseline-pictures instead of comparing against them. It is set by UpdateRecordedStreamBaselines.py of this codeunit.</summary>
        private const string _NameOfTheVariableWhichUpdatesTheBaselines = "ConSurvUpdateRecordedStreamBaselines";

        /// <summary>How long is recorded. It has to be longer than the last checked moment, otherwise the picture of that moment does not exist.</summary>
        private static readonly TimeSpan _RecordingDuration = TimeSpan.FromSeconds(5);

        /// <summary>The moment whose picture is compared with the baseline-picture of the stream.</summary>
        private const double _CheckedMomentInSeconds = 1;

        /// <summary>The moments between which the changes of the number are counted, and the step in which they are sampled. The step is shorter than a second, so no change can lie between two of them unnoticed.</summary>
        private const double _FirstMomentOfTheCountingInSeconds = 1;
        private const double _LastMomentOfTheCountingInSeconds = 4;
        private const double _StepOfTheCountingInSeconds = 0.25;

        /// <summary>How often the number has to change between those two moments: the number of a test-stream changes once per second, so it is the duration between them in seconds.</summary>
        private const int _ExpectedAmountOfChangesOfTheNumber = (int)(_LastMomentOfTheCountingInSeconds - _FirstMomentOfTheCountingInSeconds);

        /// <summary>How long the application gets to bring up the media-processes of a camera before the testcase gives up.</summary>
        private static readonly TimeSpan _TimeoutForTheRecordingToAppear = TimeSpan.FromMinutes(2);

        [TestMethod]
        [Ignore("This testcase fails sporadically, for example when the build-machine is under load.")]
        [TestProperty(nameof(GRYLibrary.Core.Misc.TestKind), nameof(GRYLibrary.Core.Misc.TestKind.IntegrationTest))]
        public void TheRecordedVideoOfEveryStreamLooksLikeItsBaselinePicture()
        {
            // arrange
            using RTSPTestServer server = new RTSPTestServer(_NameOfTheFirstStream, _NameOfTheSecondStream);
            using RTSPTestStream firstStream = new RTSPTestStream(server, _NameOfTheFirstStream);
            using RTSPTestStream secondStream = new RTSPTestStream(server, _NameOfTheSecondStream);
            // The application only records when its background-services run, so this testcase asks for them explicitly.
            using IntegrationTestFramework framework = new IntegrationTestFramework(new IntegrationTestConfiguration() { RunBackgroundProcesses = true }, true);
            IBusinessLogicService businessLogicService = GUtilities.AssertNotNull(framework._BusinessLogicService, nameof(framework._BusinessLogicService));

            // act
            // A new camera records always (see the constructor of Camera), so creating it is enough.
            string firstCameraId = businessLogicService.CreateCamera(_NameOfTheFirstStream, firstStream.Address);
            string secondCameraId = businessLogicService.CreateCamera(_NameOfTheSecondStream, secondStream.Address);
            RecordedVideo firstRecording = this.WaitForTheRecordingOfCamera(framework, firstCameraId);
            RecordedVideo secondRecording = this.WaitForTheRecordingOfCamera(framework, secondCameraId);

            // assert
            if (Environment.GetEnvironmentVariable(_NameOfTheVariableWhichUpdatesTheBaselines) == "true")
            {
                firstRecording.WriteBaseline(firstStream, _CheckedMomentInSeconds);
                secondRecording.WriteBaseline(secondStream, _CheckedMomentInSeconds);
                Assert.Inconclusive("The baseline-pictures were written instead of being compared against. Run the testcases again without that environment-variable to see whether the application really records what the baseline-pictures show.");
            }
            AssertTheRecordingShowsItsStream(firstRecording, firstStream);
            AssertTheRecordingShowsItsStream(secondRecording, secondStream);
        }

        private static void AssertTheRecordingShowsItsStream(RecordedVideo recording, RTSPTestStream stream)
        {
            // The picture says which stream it came from: the name of the stream and, in the text which the
            // application draws into it, the name and the id of the camera. Everything which can not be reproduced -
            // the number of the stream and the changing part of the time - is masked in both pictures.
            Assert.IsTrue(recording.LooksLikeTheBaselineOfTheStream(stream, _CheckedMomentInSeconds), $"The picture at second {_CheckedMomentInSeconds} of the recording of \"{stream.Name}\" does not look like the baseline-picture of that stream. Either it shows another stream or the picture changed.");
            // One second of the recording has to be one second of the stream. A recording which is played back too
            // fast or too slow, or in which the picture is frozen, is wrong even though every single picture of it
            // looks like the baseline-picture.
            int amountOfChanges = recording.CountTheChangesOfTheNumber(_FirstMomentOfTheCountingInSeconds, _LastMomentOfTheCountingInSeconds, _StepOfTheCountingInSeconds);
            Assert.AreEqual(_ExpectedAmountOfChangesOfTheNumber, amountOfChanges, $"Between second {_FirstMomentOfTheCountingInSeconds} and second {_LastMomentOfTheCountingInSeconds} of the recording of \"{stream.Name}\" the number has to change exactly {_ExpectedAmountOfChangesOfTheNumber} times, because it changes once per second.");
        }

        /// <summary>
        /// Waits until the application recorded enough of the given camera and returns that recording.
        /// The application needs a moment for this: it probes the camera, starts its media-hub and only then the
        /// recording begins.
        /// </summary>
        private RecordedVideo WaitForTheRecordingOfCamera(IntegrationTestFramework framework, string cameraId)
        {
            string folderOfTheRecordings = this.GetFolderOfTheRecordings(framework, cameraId);
            DateTime timeout = DateTime.UtcNow + _TimeoutForTheRecordingToAppear;
            while (DateTime.UtcNow < timeout)
            {
                if (Directory.Exists(folderOfTheRecordings))
                {
                    // The oldest file is the right one: it is the one which was started first, so it contains the
                    // begin of the recording.
                    string? file = Directory.EnumerateFiles(folderOfTheRecordings, "*.mp4").OrderBy(File.GetCreationTimeUtc).FirstOrDefault();
                    if (file != null && RecordingIsLongEnough(file))
                    {
                        return new RecordedVideo(file);
                    }
                }
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            throw new TimeoutException($"The application did not record camera {cameraId} within {_TimeoutForTheRecordingToAppear}. Expected folder: \"{folderOfTheRecordings}\".");
        }

        /// <summary>Whether the recording contains at least the duration which the testcase checks. The file grows while it is written, so this is what tells that waiting is over.</summary>
        private static bool RecordingIsLongEnough(string file)
        {
            try
            {
                using GRYLibrary.Core.ExecutePrograms.ExternalProgramExecutor executor = new GRYLibrary.Core.ExecutePrograms.ExternalProgramExecutor("ffprobe", $"-v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 \"{file}\"");
                executor.Run();
                return executor.ExitCode == 0
                    && double.TryParse(string.Join(string.Empty, executor.AllStdOutLines).Trim(), System.Globalization.CultureInfo.InvariantCulture, out double durationInSeconds)
                    && _RecordingDuration.TotalSeconds <= durationInSeconds;
            }
            catch (Exception)
            {
                // While ffmpeg is writing the file it can be unreadable for a moment. That is not an error, it
                // only means that waiting continues.
                return false;
            }
        }

        /// <summary>
        /// Returns the folder into which the application writes the recordings of the given camera (see
        /// CameraManagementService). The data-folder is asked from the running application: in a test-run it is a fresh
        /// temporary folder and not the workspace-folder of the codeunit, so it cannot be built from the repository.
        /// </summary>
        private string GetFolderOfTheRecordings(IntegrationTestFramework framework, string cameraId)
        {
            return Path.Combine(framework.GetDataFolder(), "CameraData", cameraId, "Recordings");
        }
    }
}
