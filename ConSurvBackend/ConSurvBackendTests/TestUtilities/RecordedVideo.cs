using GRYLibrary.Core.ExecutePrograms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Globalization;
using System.IO;

namespace ConSurvBackend.Tests.TestUtilities
{
    /// <summary>
    /// A video which the application recorded. It can hand out the picture of a certain moment and compare it with
    /// the baseline-picture which belongs to a test-stream.
    /// </summary>
    public sealed class RecordedVideo
    {
        /// <summary>
        /// The amount of pixels a picture is allowed to differ from its baseline.
        /// </summary>
        /// <remarks>
        /// The value lies between the two amounts which were measured: a recording of the own stream differs from its
        /// baseline by almost no pixel at all (the picture is a flat color with sharp text, which the encoding of the
        /// recording reproduces without a visible loss at the tolerance below), while a recording which shows the
        /// other stream differs by thousands of pixels, because the name of the stream and the id of the camera are
        /// different ones.
        /// </remarks>
        public const int MaximalAmountOfDifferentPixels = 500;

        /// <summary>The amount by which the value of a single pixel may differ before it counts as different. The video-compression changes almost every pixel by a small amount.</summary>
        private const int TolerancePerPixel = 40;

        /// <summary>The color which is painted over the areas which can not be compared. It does not appear in the picture otherwise, so a mask which sits in the wrong place is visible immediately.</summary>
        private static readonly Rgb24 _ColorOfTheMask = new Rgb24(255, 0, 255);

        /// <summary>
        /// The area which shows the number of the stream.
        /// </summary>
        /// <remarks>
        /// The number is masked because it can not be part of a baseline-picture: it counts the seconds the stream is
        /// running, and how far the stream is advanced when the application begins to record is not predictable. That
        /// the number runs correctly is asserted separately by <see cref="CountTheChangesOfTheNumber"/>. The area
        /// spans the whole width, so it covers the number regardless of how many digits it has; nothing else is drawn
        /// in these rows.
        /// </remarks>
        private static readonly Rectangle _AreaOfTheNumber = new Rectangle(0, 430, 1920, 221);

        /// <summary>
        /// The area which shows the part of the text of the application which changes.
        /// </summary>
        /// <remarks>
        /// The application draws the name and the id of the camera and the current time into the lower right corner.
        /// Everything up to and including the century of the year stays visible and is compared - the name and the id
        /// are exactly what tells two cameras apart - while the rest of the time is masked, because it is a different
        /// one in every run. Measured at the picture-size 1920x1080: the text ends at the right edge, the part behind
        /// the century is 417 pixels wide and the box around the text covers the rows 1016 to 1079.
        /// </remarks>
        private static readonly Rectangle _AreaOfTheChangingPartOfTheTime = new Rectangle(1503, 1016, 417, 64);

        private static readonly Rectangle[] _AreasWhichCanNotBeCompared = new Rectangle[] { _AreaOfTheNumber, _AreaOfTheChangingPartOfTheTime };

        private readonly string _File;

        public RecordedVideo(string file)
        {
            this._File = file;
        }

        /// <summary>
        /// Whether the picture at the given moment of this video looks like the baseline-picture of the given stream.
        /// </summary>
        /// <param name="stream">The stream whose baseline-picture is used.</param>
        /// <param name="momentInSeconds">The moment inside the video, counted from its first picture.</param>
        public bool LooksLikeTheBaselineOfTheStream(RTSPTestStream stream, double momentInSeconds)
        {
            string baseline = BaselinePictures.GetFileOfBaseline(stream.Name);
            GRYLibrary.Core.Misc.Utilities.AssertCondition(File.Exists(baseline), $"The baseline-picture \"{baseline}\" does not exist. It is created by UpdateRecordedStreamBaselines.py of this codeunit.");
            return PicturesAreEqual(this.ExtractComparablePicture(momentInSeconds), baseline);
        }

        /// <summary>
        /// Returns how often the number of the stream changes between the two given moments of this video.
        /// </summary>
        /// <remarks>
        /// This is what makes the speed of the recording checkable without knowing which number is shown, which the
        /// masked baseline-picture can not tell: the number of a test-stream changes exactly once per second, so a
        /// part of a recording which lasts n seconds contains exactly n changes. A recording which is played back too
        /// fast or too slow contains more or fewer of them, and a recording whose picture is frozen contains none.
        /// The step has to be shorter than a second, otherwise a change could lie between two moments unnoticed.
        /// </remarks>
        public int CountTheChangesOfTheNumber(double firstMomentInSeconds, double lastMomentInSeconds, double stepInSeconds)
        {
            GRYLibrary.Core.Misc.Utilities.AssertCondition(stepInSeconds < 1, $"The step has to be shorter than a second, but it is {stepInSeconds}.");
            int result = 0;
            string previousPicture = this.ExtractPicture(firstMomentInSeconds);
            for (double moment = firstMomentInSeconds + stepInSeconds; moment <= lastMomentInSeconds; moment += stepInSeconds)
            {
                string currentPicture = this.ExtractPicture(moment);
                if (!AreasAreEqual(previousPicture, currentPicture, _AreaOfTheNumber))
                {
                    result += 1;
                }
                previousPicture = currentPicture;
            }
            return result;
        }

        /// <summary>Writes the picture at the given moment as the baseline-picture of the given stream. This is how the baselines are created; see UpdateRecordedStreamBaselines.py of this codeunit.</summary>
        public void WriteBaseline(RTSPTestStream stream, double momentInSeconds)
        {
            string pictureOfTheVideo = this.ExtractComparablePicture(momentInSeconds);
            string baseline = BaselinePictures.GetFileOfBaseline(stream.Name);
            Directory.CreateDirectory(Path.GetDirectoryName(baseline)!);
            File.Copy(pictureOfTheVideo, baseline, true);
        }

        /// <summary>Extracts the picture at the given moment, paints over the areas which can not be compared and returns the file which contains it.</summary>
        public string ExtractComparablePicture(double momentInSeconds)
        {
            string file = this.ExtractPicture(momentInSeconds);
            using (Image<Rgb24> picture = Image.Load<Rgb24>(file))
            {
                foreach (Rectangle area in _AreasWhichCanNotBeCompared)
                {
                    GRYLibrary.Core.Misc.Utilities.AssertCondition(area.Right <= picture.Width && area.Bottom <= picture.Height, $"The area which has to be masked does not lie inside a picture of the size {picture.Width}x{picture.Height}.");
                    for (int y = area.Top; y < area.Bottom; y++)
                    {
                        for (int x = area.Left; x < area.Right; x++)
                        {
                            picture[x, y] = _ColorOfTheMask;
                        }
                    }
                }
                picture.Save(file);
            }
            return file;
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
            return AreasAreEqual(first, second, new Rectangle(0, 0, first.Width, first.Height));
        }

        /// <summary>Whether the two pictures look the same inside the given area.</summary>
        private static bool AreasAreEqual(string firstFile, string secondFile, Rectangle area)
        {
            using Image<Rgb24> first = Image.Load<Rgb24>(firstFile);
            using Image<Rgb24> second = Image.Load<Rgb24>(secondFile);
            return AreasAreEqual(first, second, area);
        }

        private static bool AreasAreEqual(Image<Rgb24> first, Image<Rgb24> second, Rectangle area)
        {
            if (first.Width != second.Width || first.Height != second.Height)
            {
                return false;
            }
            int amountOfDifferentPixels = 0;
            for (int y = area.Top; y < area.Bottom; y++)
            {
                for (int x = area.Left; x < area.Right; x++)
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

    /// <summary>The baseline-pictures: one per test-stream.</summary>
    public static class BaselinePictures
    {
        public static string GetFileOfBaseline(string nameOfTheStream)
        {
            return Path.Combine(Constants.GeneralConstants.CodeUnitFolder, "Other", "Resources", "RecordedStreamBaselines", $"{nameOfTheStream}.png");
        }
    }
}
