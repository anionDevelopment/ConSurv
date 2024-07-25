using ContinuousSurveillanceBackend.Core.Database.Contexts;
using ContinuousSurveillanceBackend.Core.Model.RecordingModes;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Services.Trans;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using GRYLibrary.Core.Misc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Data;

namespace ContinuousSurveillanceBackend.Core.Services
{
    public sealed class DatabasePersistence : IPersistence
    {
        private readonly DatabaseContext _DatabaseContext;
        private static readonly object _Lock = new object();
        private readonly Semaphore _Semaphore = new Semaphore();
        public DatabasePersistence(DbContextOptions<DatabaseContext> options, IGeneralLogger logger, ITimeService timeService, IDatabaseManager databaseManager)
        {
            this._DatabaseContext = new DatabaseContext(options, logger, timeService, databaseManager);
        }

        #region AccessDatabase
        private void AccessDatabase(Action<DatabaseContext> action)
        {
            this.AccessDatabase((database) =>
            {
                action(database);
                return new object();
            });
        }
        private T AccessDatabase<T>(Func<DatabaseContext, T> func)
        {
            lock (_Lock)
            {
                this._Semaphore.Lock();
                try
                {
                    return func(this._DatabaseContext);
                }
                finally
                {
                    this._Semaphore.Unlock();
                }
            }
        }
        public void RunTransaction(params Action<MySqlCommand>[] actions)
        {
            this.RunTransaction(actions.Select<Action<MySqlCommand>, Func<MySqlCommand, object>>(action => (command) =>
            {
                action(command);
                return null;
            }
            ).ToArray());
        }
        public T[] RunTransaction<T>(params Func<MySqlCommand, T>[] functions)
        {
            List<T> results = new List<T>();
            this.AccessDatabase(context =>
            {
                MySqlConnection connection = context.Connection;
                using MySqlTransaction transaction = connection.BeginTransaction();
                bool commit = true;
                try
                {
                    foreach (Func<MySqlCommand, T> function in functions)
                    {
                        using (MySqlCommand cmd = connection.CreateCommand())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.CommandTimeout = 300;
                            cmd.Transaction = transaction;
                            try
                            {
                                T result = function(cmd);
                                results.Add(result);
                            }
                            catch
                            {
                                commit = false;
                                throw;
                            }
                        };
                    }
                }
                finally
                {
                    if (commit)
                    {
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                    }
                }
            });
            return results.ToArray();
        }
        #endregion

        public string CreateCamera(string name, NoRecording notRecording)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveCamera(string cameraId)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateCamera(string cameraId, string name, RecordMode recordMode)
        {
            throw new System.NotImplementedException();
        }

        public bool IsAvailable()
        {
            return this.AccessDatabase((databaseContext) =>
            {
                try
                {
                    return databaseContext.Database.CanConnect();
                }
                catch
                {
                    return false;
                }
            });
        }

        public void Dispose()
        {
            this._DatabaseContext.Dispose();
        }
    }
}
