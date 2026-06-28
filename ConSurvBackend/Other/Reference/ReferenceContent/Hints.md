# Hints

## Requirements

The following tools from the [tools-list](https://github.com/anionDev/ScriptCollection/blob/main/ScriptCollection/Other/Reference/ReferenceContent/Articles/RequirementsForCommonProjectStructure.md#Tools) are required to build this code-unit:

- `docfx`
- `dotnet`
- `ffmpeg`
- `git`
- `gitversion`
- `python`
- `reportgenerator`
- `scriptcollection`
- `swagger`

## IDE

The recommended IDE for this code-unit is [Visual Studio](https://visualstudio.microsoft.com/).

Start the project using the usual debug-button in Visual Studio.
When you start the backend the first time then a configuration-file will be generated which will be used.
This configuration-file is located in `<repository-root>\ConSurvBackend\Other\Workspace\Configuration\Configuration.xml`.
This configuration-file will always be used if available when running the backend locally.
If a debugger is attached (which is typically the case when developing and starting the backend in the IDE) when the configuration-file is generated then an internal transient database-storage will be used instead of a persistent database.
So to develop/debug the backend you do not have to run a local database, but you can if you want.
If you want to use a real database when debugging then you have to configure it in `Configuration.xml`. by adjusting the `DatabasePersistenceConfiguration`-section accordingly:
In this `DatabasePersistenceConfiguration`-section you can edit the `DatabaseType`-value and the `DatabaseConnectionString`-value.
For running transient set `DatabaseType` to `Transient`.
(In this case `DatabasePersistenceConfiguration` is not used obviously.)
Apart from the transient-mode the supported values for `DatabaseType` are:

- `PostgreSQL`
- `MariaDB`

To reset all your local backend-configuration-values etc. to a plain state you can simply remove the entire `<repository-root>\ConSurvBackend\Other\Workspace`-folder.

## Windows-specific hints

### Database-port cannot be bound (e. g. "ports are not available ... bind: An attempt was made to access a socket in a way forbidden by its access permissions")

When building or running the local test-services (e. g. via `scbuildcodeunits` or `task LocaltestserviceMariadbStart`) the start of the database-container can fail with an error like:

```
Error response from daemon: ports are not available: exposing port TCP 0.0.0.0:3306 -> 127.0.0.1:0: listen tcp 0.0.0.0:3306: bind: An attempt was made to access a socket in a way forbidden by its access permissions.
```

This usually does not mean the port is already in use by another program. On Windows, Hyper-V/WinNAT reserves dynamic port-ranges, and the required database-port (e. g. `3306` for MariaDB or `5432` for PostgreSQL) can fall into one of those reserved ranges, which prevents Docker from binding it.

To check whether the port is inside a reserved range, run:

```cmd
netsh int ipv4 show excludedportrange protocol=tcp
```

If the port lies within one of the listed ranges, restart the WinNAT-service (in a command prompt as Administrator) to re-roll the reserved ranges:

```cmd
net stop winnat
net start winnat
```

If stopping WinNAT fails because of dependent services, stop Docker first and start it again afterwards:

```cmd
net stop com.docker.service
net stop winnat
net start winnat
net start com.docker.service
```

For a permanent fix you can reserve the port explicitly so the dynamic Hyper-V-ranges avoid it (replace `3306` with the affected port):

```cmd
net stop winnat
netsh int ipv4 add excludedportrange protocol=tcp startport=3306 numberofports=1 store=persistent
net start winnat
```
