# Hints

## Requirements

The following tools from the [tools-list](https://github.com/anionDev/ScriptCollection/blob/main/ScriptCollection/Other/Reference/ReferenceContent/Articles/RequirementsForCommonProjectStructure.md#Tools) are required to build this code-unit:

- `docfx`
- `docker`
- `git`
- `gitversion`
- `python`
- `reportgenerator`
- `scriptcollection`

## IDE

The recommended IDE for this codeunit is [Visual Studio Code](https://code.visualstudio.com/).

Debugging this codeunit is not possible in that way because the Dockerfile is the main content of this codeunit.
To run the codeunit see the [minimal docker-compose-example](./Examples/MinimalDockerComposeFile/ReadMe.md).
If something should be debugged then it is recommended to debug the frontend and the backend specific IDEs following the general [development-steps](https://github.com/anionDev/ConSurv?tab=readme-ov-file#run-locally).

## Hosting

ConSurv is supposed to be hosted behind a reverse-proxy.
Even if ConSurv comes with an own certificate:
The own certificate is just there to be technically ready to host a HTTPS-service which is required for testing-purposes.
And because complexity should be reduced and the development-, quality-check- and productive-environment should be differ as little as possible this behavior with hosting using TLS on port 443 will also be applied in an productive-environment.

## Security-considerations

The routes `/API/Other/Maintenance` may contain sensitive information.
Please block or protect them in your reverse-proxy if you do not want them to be accessable from outside.
The currently available maintenance-routes are:

- `/API/Other/Maintenance/AvailabilityCheck`
- `/API/Other/Maintenance/CurrentVersion`
- `/API/Other/Maintenance/HealthCheck`
- `/API/Other/Maintenance/Metrics`
- `/API/Other/Maintenance/ShowAllEndpoints`
