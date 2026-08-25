using ConSurvBackend.Core.Services;
using ConSurvBackend.Tests.TestUtilities;
using GRYLibrary.Core.Misc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GUtilities = GRYLibrary.Core.Misc.Utilities;
using System.Collections.Generic;
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
    /// About the moment which is compared: the number in the picture belongs to the timeline of the *stream*,
    /// while the moments below belong to the timeline of the *recording*. Both do not begin at the same instant:
    /// the application only starts recording after it could probe the camera (see CameraManagementService), so how
    /// far the stream is already advanced when the recording begins is not predictable. The testcase therefore
    /// does not expect one certain number but expects that the picture is one of the baseline-pictures of its
    /// stream and that the number one second later is the successor of it.
    /// </summary>
    [TestClass]
    public class RecordedStreamVisualRegressionTests
    {
        private const string _NameOfTheFirstStream = "TestStreamA";
        private const string _NameOfTheSecondStream = "TestStreamB";

        /// <summary>How long is recorded. It has to be longer than the last checked moment, otherwise the picture of that moment does not exist.</summary>
        private static readonly TimeSpan _RecordingDuration = TimeSpan.FromSeconds(3);

        private const double _FirstCheckedMomentInSeconds = 1.5;
        private const double _SecondCheckedMomentInSeconds = 2.5;

        /// <summary>How long the application gets to bring up the media-processes of a camera before the testcase gives up.</summary>
        private static readonly TimeSpan _TimeoutForTheRecordingToAppear = TimeSpan.FromMinutes(2);

        [TestMethod]
        [TestProperty(nameof(GRYLibrary.Core.Misc.TestKind), nameof(GRYLibrary.Core.Misc.TestKind.IntegrationTest))]
        public void TheRecordedVideoOfEveryStreamLooksLikeItsBaselinePictures()
        {
            // arrange
            using RTSPTestServer server = new RTSPTestServer(_NameOfTheFirstStream, _NameOfTheSecondStream);
            using RTSPTestStream firstStream = new RTSPTestStream(server, _NameOfTheFirstStream);
            using RTSPTestStream secondStream = new RTSPTestStream(server, _NameOfTheSecondStream);
            using IntegrationTestFramework framework = new IntegrationTestFramework(true);
            IBusinessLogicService businessLogicService = GUtilities.AssertNotNull(framework._BusinessLogicService, nameof(framework._BusinessLogicService));

            // act
            // A new camera records always (see the constructor of Camera), so creating it is enough.
            string firstCameraId = businessLogicService.CreateCamera(_NameOfTheFirstStream, firstStream.Address);
            string secondCameraId = businessLogicService.CreateCamera(_NameOfTheSecondStream, secondStream.Address);
            RecordedVideo firstRecording = this.WaitForTheRecordingOfCamera(firstCameraId);
            RecordedVideo secondRecording = this.WaitForTheRecordingOfCamera(secondCameraId);

            // assert
            AssertTheRecordingShowsItsStream(firstRecording, firstStream);
            AssertTheRecordingShowsItsStream(secondRecording, secondStream);
        }

        private static void AssertTheRecordingShowsItsStream(RecordedVideo recording, RTSPTestStream stream)
        {
            int? numberAtTheFirstMoment = recording.GetShownNumber(stream, _FirstCheckedMomentInSeconds);
            int? numberAtTheSecondMoment = recording.GetShownNumber(stream, _SecondCheckedMomentInSeconds);
            Assert.IsNotNull(numberAtTheFirstMoment, $"The picture at second {_FirstCheckedMomentInSeconds} of the recording of \"{stream.Name}\" looks like none of the baseline-pictures of that stream. Either it shows another stream or the picture changed. The actual picture is in the artifacts-folder of the codeunit.");
            Assert.IsNotNull(numberAtTheSecondMoment, $"The picture at second {_SecondCheckedMomentInSeconds} of the recording of \"{stream.Name}\" looks like none of the baseline-pictures of that stream.");
            // One second of the recording has to be one second of the stream. A recording which is played back too
            // fast or too slow, or which repeats pictures because the stream stalled, is wrong even though every
            // single picture of it looks like a baseline-picture.
            Assert.AreEqual(numberAtTheFirstMoment + 1, numberAtTheSecondMoment, $"Between second {_FirstCheckedMomentInSeconds} and second {_SecondCheckedMomentInSeconds} of the recording of \"{stream.Name}\" the number has to be counted up exactly once.");
        }

        /// <summary>
        /// Waits until the application recorded enough of the given camera and returns that recording.
        /// The application needs a moment for this: it probes the camera, starts its media-hub and only then the
        /// recording begins.
        /// </summary>
        private RecordedVideo WaitForTheRecordingOfCamera(string cameraId)
        {
            string folderOfTheRecordings = this.GetFolderOfTheRecordings(cameraId);
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

        /// <summary>Returns the folder into which the application writes the recordings of the given camera (see CameraManagementService).</summary>
        private string GetFolderOfTheRecordings(string cameraId)
        {
            string workspaceFolder = Path.Combine(TestUtilities.Constants.GeneralConstants.CodeUnitFolder, "Other", "Workspace");
            return Path.Combine(workspaceFolder, "Data", "CameraData", cameraId, "Recordings");
        }
    }
}
