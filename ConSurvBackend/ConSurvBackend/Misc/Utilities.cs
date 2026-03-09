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

        internal static UserInformationDTO GetUserInformation(User user)
        {
            bool isAdmin = user.GetAllRoles().Where(r => r.Name == CodeUnitSpecificConstants.RolenameAdmins).Any();
            bool isModerator = user.GetAllRoles().Where(r => r.Name == CodeUnitSpecificConstants.RolenameModerators).Any();
            return new UserInformationDTO(user.Id, user.Name, isAdmin, isModerator);
        }

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

        internal static bool IsRunningInContainer()
        {
            return "true".Equals(Environment.GetEnvironmentVariable("IsRunningInDockerContainer"));
        }

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
        public static bool ImagesAreDifferent(byte[] img1Bytes, byte[] img2Bytes, double thresold)
        {
            GRYLibrary.Core.Misc.Utilities.AssertCondition(thresold >= 0 && thresold <= 1, "Thresold must be between 0 and 1.");
            return CalculateImageSimilarity(img1Bytes, img2Bytes) < thresold;
        }
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
