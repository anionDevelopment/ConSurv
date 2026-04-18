using ConSurvBackend.Core.Constants;
using ConSurvBackend.Core.Model.DTOs;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.ConcreteEnvironments;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using OpenCvSharp;
using OpenCvSharp.ImgHash;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Sprache;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Image = SixLabors.ImageSharp.Image;


namespace ConSurvBackend.Core.Misc
{
    internal static class Utilities
    {
        public static readonly Encoding _Encoding = new UTF8Encoding(false);
        /// <summary>
        /// Returns the <see cref="GRYEnvironment"/> that matches the compile-time build configuration
        /// (Development, QualityCheck, or Productive).
        /// </summary>
        /// <returns>The active <see cref="GRYEnvironment"/> instance.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">
        /// Thrown when none of the expected preprocessor symbols is defined.
        /// </exception>
        internal static GRYEnvironment GetEnvironmentTargetType()
        {
#if Development
            return Development.Instance;
#elif QualityCheck
            return QualityCheck.Instance;
#elif Productive
            return Productive.Instance;
#else
            throw new System.Collections.Generic.KeyNotFoundException("Unknown environmenttargettype.");
#endif
        }

        /// <summary>
        /// Maps a <see cref="User"/> entity to a <see cref="UserInformationDTO"/>, including flags
        /// that indicate whether the user holds the administrator or moderator role.
        /// </summary>
        /// <param name="user">The user entity to map.</param>
        /// <returns>A <see cref="UserInformationDTO"/> populated with the user's id, name, and role flags.</returns>
        internal static UserInformationDTO GetUserInformation(User user)
        {
            bool isAdmin = user.GetAllRoles().Where(r => r.Name == CodeUnitSpecificConstants.RolenameAdmins).Any();
            bool isModerator = user.GetAllRoles().Where(r => r.Name == CodeUnitSpecificConstants.RolenameModerators).Any();
            return new UserInformationDTO(user.Id, user.Name, isAdmin, isModerator);
        }

        /// <summary>
        /// Builds a video file name for a new recording segment using the camera identifier and the
        /// current timestamp.
        /// </summary>
        /// <param name="cameraId">Identifier of the camera whose recording is being named.</param>
        /// <param name="timeInUTC">
        /// When <see langword="true"/> the UTC timestamp is used; otherwise the local timestamp is used.
        /// </param>
        /// <param name="timeService">Service used to obtain the current date and time.</param>
        /// <returns>A file name in the format <c>{cameraId}_{yyyy}_{MM}_{dd}_{HH}_{mm}_{ss}.mp4</c>.</returns>
        internal static string GetVideoTargetFile(string cameraId, bool timeInUTC, ITimeService timeService)
        {
            DateTimeOffset dateTime;
            if (timeInUTC)
            {
                dateTime = timeService.GetCurrentTimeInUTCAsDateTimeOffset();
            }
            else
            {
                dateTime = timeService.GetCurrentLocalTimeAsDateTimeOffset();
            }
            string result = $"{cameraId}_{dateTime.Year.ToString().PadLeft(4, '0')}_{dateTime.Month.ToString().PadLeft(2, '0')}_{dateTime.Day.ToString().PadLeft(2, '0')}_{dateTime.Hour.ToString().PadLeft(2, '0')}_{dateTime.Minute.ToString().PadLeft(2, '0')}_{dateTime.Second.ToString().PadLeft(2, '0')}.mp4";
            result = result.Replace("\\", "/");
            return result;
        }

        /// <summary>
        /// Determines whether the process is executing inside a container by checking the
        /// <c>ISRUNNINGINCONTAINER</c> environment variable.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> when the environment variable equals <c>"true"</c> (case-sensitive);
        /// otherwise <see langword="false"/>.
        /// </returns>
        internal static bool IsRunningInContainer()
        {
            return "true".Equals(Environment.GetEnvironmentVariable("ISRUNNINGINCONTAINER"));
        }

        /// <summary>
        /// Resizes the given image to the specified dimensions, preserving the original image format.
        /// </summary>
        /// <param name="image">Raw bytes of the source image.</param>
        /// <param name="height">Target height in pixels.</param>
        /// <param name="width">Target width in pixels.</param>
        /// <returns>Raw bytes of the resized image in the same format as the input.</returns>
        internal static byte[] ResizeImage(byte[] image, uint height, uint width)
        {
            MemoryStream inputStream = new MemoryStream(image);
            Image<Rgba32> sourceImage = Image.Load<Rgba32>(inputStream);
            IImageFormat format = sourceImage.Metadata.DecodedImageFormat;
            int targetWidth = (int)width;
            int targetHeight = (int)height;
            sourceImage.Mutate(ctx => ctx.Resize(targetWidth, targetHeight));
            MemoryStream outputStream = new MemoryStream();
            sourceImage.Save(outputStream, format);
            byte[] result = outputStream.ToArray();
            sourceImage.Dispose();
            inputStream.Dispose();
            outputStream.Dispose();

            return result;
        }
        /// <summary>
        /// Center-crops the given image to the specified dimensions, preserving the original image format.
        /// If the requested dimensions exceed the source image size, the source dimensions are used.
        /// </summary>
        /// <param name="image">Raw bytes of the source image.</param>
        /// <param name="height">Target height in pixels.</param>
        /// <param name="width">Target width in pixels.</param>
        /// <returns>Raw bytes of the cropped image in the same format as the input.</returns>
        internal static byte[] CropImage(byte[] image, uint height, uint width)
        {
            MemoryStream inputStream = new MemoryStream(image);
            Image<Rgba32> sourceImage = Image.Load<Rgba32>(inputStream);
            IImageFormat format = sourceImage.Metadata.DecodedImageFormat;
            int targetWidth = (int)width;
            int targetHeight = (int)height;
            int cropX = Math.Max((sourceImage.Width - targetWidth) / 2, 0);
            int cropY = Math.Max((sourceImage.Height - targetHeight) / 2, 0);
            int cropWidth = Math.Min(targetWidth, sourceImage.Width);
            int cropHeight = Math.Min(targetHeight, sourceImage.Height);
            Rectangle cropArea = new Rectangle(cropX, cropY, cropWidth, cropHeight);
            sourceImage.Mutate(ctx => ctx.Crop(cropArea));
            MemoryStream outputStream = new MemoryStream();
            sourceImage.Save(outputStream, format);
            byte[] result = outputStream.ToArray();
            sourceImage.Dispose();
            inputStream.Dispose();
            outputStream.Dispose();
            return result;
        }

        /// <summary>
        /// Removes the password from a Basic-Auth RTSP URL, leaving only the scheme and host portion
        /// so that the URL can be safely logged or displayed.
        /// </summary>
        /// <param name="rtspLink">An RTSP (or other scheme) URL that may contain Basic-Auth credentials.</param>
        /// <returns>
        /// The URL with the user-info segment removed (e.g. <c>rtsp://host/path</c> instead of
        /// <c>rtsp://user:pass@host/path</c>).
        /// </returns>
        public static string EscapeBasicAuthPasswords(string rtspLink)
        {
            string pattern = @"(?<scheme>[a-z]+):\/\/(?<user>[^:\s@]+):(?<pass>[^@\s]+)@";

            string result = Regex.Replace(rtspLink, pattern, m =>
            {
                string scheme = m.Groups["scheme"].Value;
                string user = m.Groups["user"].Value;
                return $"{scheme}://";
            });

            return result;
        }
        /// <summary>
        /// Determines whether two images are perceptually different by comparing their similarity
        /// score against the given threshold.
        /// </summary>
        /// <param name="img1Bytes">Raw bytes of the first image.</param>
        /// <param name="img2Bytes">Raw bytes of the second image.</param>
        /// <param name="thresold">
        /// A value between 0 and 1.  Images are considered different when their similarity score is
        /// below this threshold.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the similarity score is below <paramref name="thresold"/>;
        /// otherwise <see langword="false"/>.
        /// </returns>
        /// <exception cref="Exception">Thrown when <paramref name="thresold"/> is outside [0, 1].</exception>
        public static bool ImagesAreDifferent(byte[] img1Bytes, byte[] img2Bytes, double thresold)
        {
            GRYLibrary.Core.Misc.Utilities.AssertCondition(thresold >= 0 && thresold <= 1, "Thresold must be between 0 and 1.");
            return CalculateImageSimilarity(img1Bytes, img2Bytes) < thresold;
        }
        /// <summary>
        /// Calculates a perceptual similarity score between two images using OpenCV's PHash algorithm.
        /// </summary>
        /// <param name="img1Bytes">Raw bytes of the first image (must be decodable by OpenCV).</param>
        /// <param name="img2Bytes">Raw bytes of the second image (must be decodable by OpenCV).</param>
        /// <returns>
        /// A value in the range [0, 100] where 100 means the images are identical and 0 means they
        /// are completely different.
        /// </returns>
        public static double CalculateImageSimilarity(byte[] img1Bytes, byte[] img2Bytes)
        {
            Mat img1 = Cv2.ImDecode(img1Bytes, ImreadModes.Grayscale);
            Mat img2 = Cv2.ImDecode(img2Bytes, ImreadModes.Grayscale);

            var phash = PHash.Create();

            Mat hash1 = new Mat();
            Mat hash2 = new Mat();

            phash.Compute(img1, hash1);
            phash.Compute(img2, hash2);

            double distance = Cv2.Norm(hash1, hash2, NormTypes.Hamming);

            double similarity = (1.0 - distance / 64.0) * 100.0;

            return similarity;
        }
    }
}
