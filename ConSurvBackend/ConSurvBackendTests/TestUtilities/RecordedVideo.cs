using GRYLibrary.Core.ExecutePrograms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ConSurvBackend.Tests.TestUtilities
{
    /// <summary>
    /// A video which the application recorded. It can hand out the picture of a certain moment and compare it with
    /// the baseline-pictures which belong to a test-stream.
    /// </summary>
    public sealed class RecordedVideo
    {
        /// <summary>
        /// The amount of pixels a picture is allowed to differ from its baseline.
        /// The value is a compromise: it is small enough to detect a wrong number or a wrong stream (which changes
        /// thousands of pixels) but large enough to tolerate the noise which the video-compression adds to a
        /// picture. With the size of the test-stream one picture has 640*480=307200 pixels, so 2000 pixels are
        /// about 0.65 percent of it.
        /// </summary>
        public const int MaximalAmountOfDifferentPixels = 2000;

        /// <summary>The amount by which the value of a single pixel may differ before it counts as different. The video-compression changes almost every pixel by a small amount.</summary>
        private const int TolerancePerPixel = 40;

        private readonly string _File;

        public RecordedVideo(string file)
        {
            this._File = file;
        }

        /// <summary>
        /// Returns the number which the picture at the given moment of this video shows, by comparing that picture
        /// with the baseline-pictures of the given stream.
        /// </summary>
        /// <param name="stream">The stream whose baseline-pictures are used.</param>
        /// <param name="momentInSeconds">The moment inside the video, counted from its first picture.</param>
        /// <returns>The number which is shown, or null when the picture matches none of the baseline-pictures.</returns>
        public int? GetShownNumber(RTSPTestStream stream, double momentInSeconds)
        {
            string pictureOfTheVideo = this.ExtractPicture(momentInSeconds);
            foreach (int number in Enumerable.Range(0, BaselinePictures.AmountOfPreparedNumbers))
            {
                string baseline = BaselinePictures.GetFileOfBaseline(stream.Name, number);
                if (File.Exists(baseline) && PicturesAreEqual(pictureOfTheVideo, baseline))
                {
                    return number;
                }
            }
            return null;
        }

        /// <summary>Writes the picture at the given moment into the folder of the baseline-pictures. This is how the baselines are created; see the ReadMe of the codeunit.</summary>
        public void WriteBaseline(RTSPTestStream stream, double momentInSeconds, int number)
        {
            string pictureOfTheVideo = this.ExtractPicture(momentInSeconds);
            string baseline = BaselinePictures.GetFileOfBaseline(stream.Name, number);
            Directory.CreateDirectory(Path.GetDirectoryName(baseline)!);
            File.Copy(pictureOfTheVideo, baseline, true);
        }

        /// <summary>Extracts the picture at the given moment and returns the file which contains it.</summary>
        public string ExtractPicture(double momentInSeconds)
        {
            string targetFile = Path.Combine(Path.GetTempPath(), $"ConSurvRecordedFrame_{Guid.NewGuid():N}.png");
            string moment = momentInSeconds.ToString(CultureInfo.InvariantCulture);
            // "-ss" before "-i" seeks to the moment before decoding, which is what makes the moment relate to the
            // timeline of the file itself and not to the moment the recording was started.
            using ExternalProgramExecutor executor = new ExternalProgramExecutor("ffmpeg", $"-hide_banner -loglevel error -ss {moment} -i \"{this._File}\" -frames:v 1 -y \"{targetFile}\"");
            executor.Run();
            GRYLibrary.Core.Misc.Utilities.AssertCondition(executor.ExitCode == 0, () => $"The picture at second {moment} of \"{this._File}\" could not be extracted. StdErr: {string.Join(Environment.NewLine, executor.AllStdErrLines)}");
            GRYLibrary.Core.Misc.Utilities.AssertCondition(File.Exists(targetFile), () => $"The picture at second {moment} of \"{this._File}\" was not written. The video is probably shorter than {moment} seconds.");
            return targetFile;
        }

        /// <summary>Whether the two pictures look the same, with the tolerances which the video-compression requires.</summary>
        public static bool PicturesAreEqual(string firstFile, string secondFile)
        {
            using Image<Rgb24> first = Image.Load<Rgb24>(firstFile);
            using Image<Rgb24> second = Image.Load<Rgb24>(secondFile);
            if (first.Width != second.Width || first.Height != second.Height)
            {
                return false;
            }
            int amountOfDifferentPixels = 0;
            for (int y = 0; y < first.Height; y++)
            {
                for (int x = 0; x < first.Width; x++)
                {
                    Rgb24 pixelOfTheFirst = first[x, y];
                    Rgb24 pixelOfTheSecond = second[x, y];
                    if (TolerancePerPixel < Math.Abs(pixelOfTheFirst.R - pixelOfTheSecond.R)
                        || TolerancePerPixel < Math.Abs(pixelOfTheFirst.G - pixelOfTheSecond.G)
                        || TolerancePerPixel < Math.Abs(pixelOfTheFirst.B - pixelOfTheSecond.B))
                    {
                        amountOfDifferentPixels += 1;
                        if (MaximalAmountOfDifferentPixels < amountOfDifferentPixels)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }

    /// <summary>The baseline-pictures: one per test-stream and number.</summary>
    public static class BaselinePictures
    {
        /// <summary>For how many numbers a baseline-picture exists. A recording of a few seconds never shows a higher number, because the stream is started shortly before the recording.</summary>
        public const int AmountOfPreparedNumbers = 10;

        public static string GetFileOfBaseline(string nameOfTheStream, int number)
        {
            return Path.Combine(Constants.GeneralConstants.CodeUnitFolder, "Other", "Resources", "RecordedStreamBaselines", $"{nameOfTheStream}_{number}.png");
        }
    }
}
