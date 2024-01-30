using Config.Settings;
using Models.Enums;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Data;
using Serilog.Sinks.MariaDB;
using Serilog.Sinks.MariaDB.Extensions;
using Utilities.Extensions;
using System.Collections.Concurrent;
using Serilog.Sinks.PostgreSQL;
using ColumnOptions = Serilog.Sinks.MSSqlServer.ColumnOptions;

namespace ServiceExtensions.Serilog;

public static class SerilogExtensions
{
    public static void Register(ApplicationSettings? applicationSettings)
    {
        if (applicationSettings is null)
        {
            throw new ArgumentNullException(nameof(applicationSettings));
        }

        var databaseSetting = applicationSettings.DatabaseSetting;
        var logSetting = applicationSettings.LogSetting;

        switch (databaseSetting.DatabaseProvider)
        {
            case DatabaseProvider.Postgres:
                PostgresRegistration(logSetting, databaseSetting.ConnectionStrings.Postgres!);
                break;

            case DatabaseProvider.SqlServer:
                SqlServerRegistration(logSetting, databaseSetting.ConnectionStrings.SqlServer!);
                break;

            case DatabaseProvider.MySql:
                MySqlRegistration(logSetting, databaseSetting.ConnectionStrings.MySql!);
                break;

            default:
                throw new Exception("Database provider not found.");
        }
    }

    private static void PostgresRegistration(LogSetting logSetting, string connectionString)
    {
        var options = new ConcurrentDictionary<string, ColumnWriterBase>()
        {
            //todo:if need postGre then RnD here
            //["MacAddress"] = new MacAddressColumnWriter(),
            //["IpAddress"] = new IpAddressColumnWriter()
        };
        Log.Logger = new LoggerConfiguration()
                    .WriteTo
                    .PostgreSQL(
                                connectionString: connectionString,
                                columnOptions: options,//
                                tableName: logSetting.TableName.ToSnakeCase(),
                                restrictedToMinimumLevel: (LogEventLevel)logSetting.MinimumLevelSerilog,
                                needAutoCreateTable: logSetting.AutoCreateSqlTable)
                    .CreateLogger();
    }

    private static void SqlServerRegistration(LogSetting logSetting, string connectionString)
    {
        var columnOptions = new ColumnOptions
        {
            AdditionalColumns = new Collection<SqlColumn>
            {
                new() { ColumnName = "MacAddress", DataType = SqlDbType.NVarChar, DataLength = 100 },
                new() { ColumnName = "IpAddress", DataType = SqlDbType.NVarChar, DataLength = 100 }
            }
        };

        var sqlServerSinkOptions = new MSSqlServerSinkOptions
        {
            TableName = logSetting.TableName,
            AutoCreateSqlDatabase = logSetting.AutoCreateDatabase,
            AutoCreateSqlTable = logSetting.AutoCreateSqlTable
        };

        Log.Logger = new LoggerConfiguration()
            .WriteTo.MSSqlServer(
                connectionString: connectionString,
                sinkOptions: sqlServerSinkOptions,
                columnOptions: columnOptions,
                restrictedToMinimumLevel: (LogEventLevel)logSetting.MinimumLevelSerilog)
            .CreateLogger();


    }
    private static void MySqlRegistration(LogSetting logSetting, string connectionString)
    {
        var columnOption = new MariaDBSinkOptions() // because MySql does not have sink
        {
            PropertiesToColumnsMapping =
            {
                ["ipAddress"] = "ipAddress",
                ["macAddress"] = "macAddress"
            }
        };
        Log.Logger = new LoggerConfiguration()
            .WriteTo.MariaDB( // because MySql does not have sink
                connectionString: connectionString,
                options: columnOption,
                tableName: logSetting.TableName,
                autoCreateTable:logSetting.AutoCreateSqlTable,
                restrictedToMinimumLevel: (LogEventLevel)logSetting.MinimumLevelSerilog)
            .CreateLogger();
    }

}
