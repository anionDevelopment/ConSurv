using ConSurvBackend.Core.Constants;
using ConSurvBackend.Core.Model.DTOs;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.ConcreteEnvironments;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ConSurvBackend.Core.Miscellaneous
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

        internal static string GetVideoTargetFile(string folder, string cameraId, bool timeInUTC, ITimeService timeService)
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
            string result = $"{folder}/{dateTime.Year.ToString().PadLeft(4, '0')}/{dateTime.Month.ToString().PadLeft(2, '0')}/{dateTime.Day.ToString().PadLeft(2, '0')}/{cameraId}_{dateTime.Year.ToString().PadLeft(4, '0')}_{dateTime.Month.ToString().PadLeft(2, '0')}_{dateTime.Day.ToString().PadLeft(2, '0')}_{dateTime.Hour.ToString().PadLeft(2, '0')}_{dateTime.Minute.ToString().PadLeft(2, '0')}_{dateTime.Second.ToString().PadLeft(2, '0')}.mp4";
            result = result.Replace("\\", "/");
            return result;
        }

        internal static bool IsRunningInContainer()
        {
            return "true".Equals(Environment.GetEnvironmentVariable("IsRunningInDockerContainer"));
        }

        internal static Process GetBackgroundProcess(string program, string argument, string? workingFolder, string configurationFolder, Action<Process>? configureProcess)
        {
            bool isDebug = false;
#if Development
            isDebug = true;
#endif
            Process process = new Process();
            process.StartInfo.FileName = program;
            process.StartInfo.Arguments = argument;
            if (workingFolder != null)
            {
                process.StartInfo.WorkingDirectory = workingFolder;
            }
            configureProcess?.Invoke(process);
            process.Start();
            if (isDebug)
            {
                string processListFile = Path.Combine(configurationFolder, "StartedProcesses.txt");
                GRYLibrary.Core.Misc.Utilities.EnsureFileExists(processListFile);
                GRYLibrary.Core.Misc.Utilities.AppendLineToFile(processListFile, process.Id.ToString(), _Encoding);
            }
            return process;
        }
    }
}
