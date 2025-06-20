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
