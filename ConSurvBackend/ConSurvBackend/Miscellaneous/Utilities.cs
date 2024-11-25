using ConSurvBackend.Core.Constants;
using ConSurvBackend.Core.Model;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.ConcreteEnvironments;
using System;
using System.Linq;

namespace ConSurvBackend.Core.Miscellaneous
{
    internal static class Utilities
    {
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

        internal static UserInformation GetUserInformation(User user)
        {
            return new UserInformation(user.Id,user.Name,user.GetAllRoles().Where(r=>r.Name==CodeUnitSpecificConstants.RolenameAdmins).Any());
        }

        internal static string GetVideoTargetFile(string folder,string cameraId, bool timeInUTC)
        {
            DateTime dateTime;
            if (timeInUTC)
            {
                dateTime = DateTime.UtcNow;
            }
            else
            {
                dateTime = DateTime.Now;
            }
            return $"{folder}/{dateTime.Year.ToString().PadLeft(4, '0')}/{dateTime.Month.ToString().PadLeft(2, '0')}/{dateTime.Day.ToString().PadLeft(2, '0')}/{cameraId}_{dateTime.Year.ToString().PadLeft(4, '0')}_{dateTime.Month.ToString().PadLeft(2, '0')}_{dateTime.Day.ToString().PadLeft(2, '0')}_{dateTime.Hour.ToString().PadLeft(2, '0')}_{dateTime.Minute.ToString().PadLeft(2, '0')}_{dateTime.Second.ToString().PadLeft(2, '0')}.mp4";
        }
    }
}
