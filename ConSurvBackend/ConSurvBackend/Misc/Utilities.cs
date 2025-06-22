using ConSurvBackend.Core.Constants;
using ConSurvBackend.Core.Model.DTOs;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.ConcreteEnvironments;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.ExecutePrograms;
using GRYLibrary.Core.ExecutePrograms.WaitingStates;
using GRYLibrary.Core.Logging.GRYLogger;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
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
            DateTime dateTime;
            if (timeInUTC)
            {
                dateTime = timeService.GetCurrentTimeInUTC();
            }
            else
            {
                dateTime = timeService.GetCurrentTime();
            }
            string result = $"{dateTime.Year.ToString().PadLeft(4, '0')}/{dateTime.Month.ToString().PadLeft(2, '0')}/{dateTime.Day.ToString().PadLeft(2, '0')}/{cameraId}_{dateTime.Year.ToString().PadLeft(4, '0')}_{dateTime.Month.ToString().PadLeft(2, '0')}_{dateTime.Day.ToString().PadLeft(2, '0')}_{dateTime.Hour.ToString().PadLeft(2, '0')}_{dateTime.Minute.ToString().PadLeft(2, '0')}_{dateTime.Second.ToString().PadLeft(2, '0')}.mp4";
            result = result.Replace("\\", "/");
            return result;
        }

        internal static bool IsRunningInContainer()
        {
            return "true".Equals(Environment.GetEnvironmentVariable("IsRunningInDockerContainer"));
        }

        internal static ExternalProgramExecutor GetBackgroundProcess(string program, string argument, string? workingFolder, string configurationFolder, Action<Process>? configureProcess, IGRYLog log, string purpose, bool runSynchronous,GRYEnvironment environment)
        {
            bool verbose = true;//can be changed to true temporary for debugging purposes
            string workingDirectory = workingFolder ?? Directory.GetCurrentDirectory();
            ExternalProgramExecutor e = new ExternalProgramExecutor(new ExternalProgramExecutorConfiguration()
            {
                Program = program,
                Argument = argument,
                WorkingDirectory = workingDirectory,

            });
            log.Log($"Started background process \"{workingDirectory}>{program} {argument}\" (Purpose: {purpose})", Microsoft.Extensions.Logging.LogLevel.Debug);
            e.Configuration.Verbosity = verbose ? Verbosity.Verbose : Verbosity.Quiet;
            if (runSynchronous)
            {
                e.Configuration.WaitingState = new RunSynchronously();
            }
            else
            {
                e.Configuration.WaitingState = new RunAsynchronously();
            }
            e.Run();
            if (environment is Development)
            {
                string processListFile = Path.Combine(configurationFolder, "StartedProcesses.txt");
                GRYLibrary.Core.Misc.Utilities.EnsureFileExists(processListFile);
                GRYLibrary.Core.Misc.Utilities.AppendLineToFile(processListFile, e.ProcessId.ToString(), _Encoding);
            }                      
            return e;
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
    }
}
