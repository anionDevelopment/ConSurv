using ConSurvBackend.Core;
using ConSurvBackend.Core.Services;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.Exceptions;
using GRYLibrary.Core.Logging.GRYLogger;
using GRYLibrary.Core.Misc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;

namespace ConSurvBackend.Tests.TestUtilities
{
    public sealed class IntegrationTestFramework : IDisposable
    {
        private bool Started = false;
        private readonly IDictionary<User, string> _UserPasswords = new Dictionary<User, string>();
        private Program? _Program = null;
        private readonly IntegrationTestConfiguration _IntegrationTestConfiguration;
        internal IBusinessLogicService? _BusinessLogicService;
        internal IGRYLog? _Log;
        /// <summary>The program which hosts the server. It only exists after the server was started.</summary>
        private Program RunningProgram => GRYLibrary.Core.Misc.Utilities.AssertNotNull(this._Program, nameof(this._Program));
        /// <summary>The business-logic-service of the running server. It only exists after the server was started.</summary>
        private IBusinessLogicService RunningBusinessLogicService => GRYLibrary.Core.Misc.Utilities.AssertNotNull(this._BusinessLogicService, nameof(this._BusinessLogicService));
        public IntegrationTestFramework(bool startServer) : this(new IntegrationTestConfiguration(), startServer)
        {
        }
        public IntegrationTestFramework(IntegrationTestConfiguration integrationTestConfiguration, bool startServer)
        {
            this._IntegrationTestConfiguration = integrationTestConfiguration;
            if (startServer)
            {
                this.StartServer();
            }
        }
        public void StartServer()
        {
            Action action = () =>
            {
                try
                {
                    this._Program = new Program
                    {
                        RunAsync = !this._IntegrationTestConfiguration.RunInOwnThread,
                        ListenOnEveryIP = false,
                        SetupMocks = this._IntegrationTestConfiguration.SetupMocks
                    };

                    //TODO add option to pass more configuration-values for the test-run like port etc. so that this can not go wrong due to a different configuration from a previous (manual) run.
                    string[] args = this._IntegrationTestConfiguration.RunBackgroundProcesses
                        ? new string[] { $"--{nameof(ConSurvBackend.Core.Configuration.CommandlineParameter.RunBackgroundProcesses)}", "true" }
                        : Array.Empty<string>();
                    int exitCode = this._Program.MainImplementation(args);
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                    GRYLibrary.Core.Misc.Utilities.AssertCondition(exitCode == 0, () =>
                    {
                        string message = $"Exitode of main-method was {exitCode}.";
                        if (this._Program._Log != null)//TODO this condition should not be required. but for unknown reasons the _log-property is null and this causes problems whille retrieving the logs here which would be useful.
                        {
                            IGRYLog log = GRYLibrary.Core.Misc.Utilities.AssertNotNull(this._Program._Log, nameof(Program._Log));
                            LogItem[] logMessages = log.LastLogEntries.GetEntries();
                            if (logMessages.Any())
                            {
                                message = $"{message} Last log entries:\n" + string.Join("\n", logMessages.Select(item =>
                                {
                                    item.Format(this._Program._Log.Configuration, out string result, out int _, out int _, out ConsoleColor _, GRYLogLogFormat.GRYLogFormat);
                                    return result;
                                }));
                            }
                        }
                        return message;
                    });

                }
                catch
                {
                    throw;
                }
            };
            if (this._IntegrationTestConfiguration.RunInOwnThread)
            {
                Thread t = new Thread(() => action())
                {
                    Name = nameof(Program)
                };
                t.Start();
            }
            else
            {
                action();
            }
            Exception? lastException = null;
            if (!GRYLibrary.Core.Misc.Utilities.RunWithTimeout(() =>
            {
                while (!this.IsReady(out lastException))
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            }, TimeSpan.
            FromSeconds(120)))
            {
                if (lastException == null)
                {
                    throw new DependencyNotAvailableException("Could not start service.");
                }
                else
                {
                    throw lastException;
                }
            }
            this.Started = true;
            this._BusinessLogicService = this.RunningProgram._BusinessLogicService;
            this._Log = this.RunningProgram._Log;
        }

        private bool IsReady(out Exception? exception)
        {
            try
            {
                using HttpClient client = this.GetClient();
                string url = $"{this.GetServerURL()}{ServerConfiguration.APIRoutePrefix}/Other/Maintenance/HealthCheck";
                HttpResponseMessage response = client.GetAsync(url).WaitAndGetResult();
                Assert.IsTrue(response.IsSuccessStatusCode);
                string content = response.Content.ReadAsStringAsync().WaitAndGetResult();
                dynamic obj = GRYLibrary.Core.Misc.Utilities.AssertNotNull(JsonConvert.DeserializeObject(content), "deserialized content of the health-check-response");
                int status = (int)obj["status"];
                exception = null;
                return status == 2;//2 means healthy.
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
        }

        public HttpClient GetClient(User? user = null)
        {
            HttpClient result = new HttpClient();
            if (user != null)
            {
                result.DefaultRequestHeaders.Add("X-Accesstoken", this.RunningBusinessLogicService.Login(user.Name, this._UserPasswords[user]).Value);
            }
            return result;
        }
        public User GetUser()
        {
            string username = Guid.NewGuid().ToString();
            string password = Guid.NewGuid().ToString();
            string userId = this.RunningBusinessLogicService.Register(username, password);
            User user = this.RunningBusinessLogicService.GetUser(userId);
            this._UserPasswords[user] = password;
            return user;
        }
        /// <summary>
        /// Returns the folder into which the started application writes its data. Which folder that is depends on the
        /// execution-mode (a test-run works in a fresh temporary folder), so a caller has to ask the application for it
        /// instead of building the path itself.
        /// </summary>
        public string GetDataFolder()
        {
            return GRYLibrary.Core.Misc.Utilities.AssertNotNull(this.RunningProgram._ApplicationConstants, nameof(Program._ApplicationConstants)).GetDataFolder();
        }
        public string GetServerURL()
        {
            return $"http://127.0.0.1:{HTTP.DefaultPort}";
        }
        public void Dispose()
        {
            this.EnsureServerIsStopped();
        }

        private void EnsureServerIsStopped()
        {
            if (this.Started)
            {
                this.RunningProgram.Stop();
                this.Started = false;
            }
        }
    }
}
