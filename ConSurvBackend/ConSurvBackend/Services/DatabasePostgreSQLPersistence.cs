using ConSurvBackend.Core.Configuration;
using ConSurvBackend.Core.Database;
using ConSurvBackend.Core.Misc;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Services.Trans;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.Logging.GRYLogger;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Data.Common;

namespace ConSurvBackend.Core.Services
{
    public sealed class DatabasePostgreSQLPersistence : GenericDatabasePersistence, IPersistence
    {
        public DatabasePostgreSQLPersistence(DbContextOptions<DatabaseContext> options, ITimeService timeService, IDatabaseManager databaseManager, IGRYLog log, ISQLProvider sqlProvider, IPersistedAPIServerConfiguration<CodeUnitSpecificConfiguration> persistedAPIServerConfiguration) : base(options, timeService, databaseManager, log, sqlProvider, persistedAPIServerConfiguration)
        {
        }

        public override DbParameter GetParameter(string parameterName, object? value, Type type)//TODO extract this frmo grylib
        {
            object formattedValue = this.FormatValue(value);
            Type adaptedType = this.AdaptType(type);
            NpgsqlDbType dbType = this.GetType(adaptedType);
            return new NpgsqlParameter()
            {
                Value = formattedValue,
                NpgsqlDbType = dbType,
            };
        }

        private Type AdaptType(Type type)//TODO extract this frmo grylib
        {
            return type switch
            {
                var t when t == typeof(UInt16) => typeof(Int16),
                var t when t == typeof(UInt32) => typeof(Int32),
                var t when t == typeof(UInt64) => typeof(Int64),
                var t when t == typeof(UInt128) => typeof(Int128),
                _ => type
            };
        }

        private object FormatValue(object? value)//TODO extract this frmo grylib
        {
            object result;
            if (value == null)
            {
                result = DBNull.Value;
            }
            /*
            else if (value is DateTime typedValue)
            {
                 result= typedValue.ToString("yyyy-MM-dd'T'HH:mm:ss");
            }
            */
            else if (value is UInt16 valusAsUInt16)
            {
                result = (Int16)valusAsUInt16;
            }
            else if (value is UInt32 valusAsUInt32)
            {
                result = (Int32)valusAsUInt32;
            }
            else if (value is UInt64 valusAsUInt64)
            {
                result = (Int64)valusAsUInt64;
            }
            else if (value is UInt128 valusAsUInt128)
            {
                result = (Int128)valusAsUInt128;
            }
            else
            {
                result = value;
            }
            return result;
        }

        private NpgsqlDbType GetType(Type type)
        {
            return type switch
            {
                var t when t == typeof(string) => NpgsqlDbType.Varchar,
                var t when t == typeof(int) => NpgsqlDbType.Integer,
                var t when t == typeof(long) => NpgsqlDbType.Bigint,
                var t when t == typeof(short) => NpgsqlDbType.Smallint,
                var t when t == typeof(bool) => NpgsqlDbType.Boolean,
                var t when t == typeof(DateTime) => NpgsqlDbType.Timestamp,
                var t when t == typeof(float) => NpgsqlDbType.Real,
                var t when t == typeof(double) => NpgsqlDbType.Double,
                var t when t == typeof(decimal) => NpgsqlDbType.Numeric,
                var t when t == typeof(Guid) => NpgsqlDbType.Uuid,
                var t when t == typeof(byte[]) => NpgsqlDbType.Bytea,
                var t when t == typeof(char) => NpgsqlDbType.Varchar,
                var t when t == typeof(TimeSpan) => NpgsqlDbType.Interval,
                var t when t == typeof(byte) => NpgsqlDbType.Smallint,

                _ => throw new NotSupportedException($"Type '{type.FullName}' is not supported.")
            };
        }
    }
}
