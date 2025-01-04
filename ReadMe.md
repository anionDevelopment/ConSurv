# ConSurv

## Purpose

`ConSurv` (abbreviation for "Continuous surveillance") is an IP-camera-management-system.

## Features

- Capturing any RTSP-stream.

## Not implemented features

- Video-Recording must be implemented.
- Live-Video-Viewing in the browser must be implemented.
- Motion-detection must be implemented.
- Video-blacken must be implemented.
- Video-control using ONVIF-commands for cameras which supports ONVIF must be implemented.
- Smartphone-app which does all features from the user-area must be implemented.
- Texts must be translated.
- OpenID-Login must be configurable.
- Group-membership of users must be changeable.
- Design (including logo/favicon/dark-mode) must be implemented.
- Transport-encryption for streams must be possible.

## Development

## Run locally

First: Run `scbuildcodeunits`.
Then, create an entry into your local `hosts`-file to make the domain `consurv.test.local` get resolved to `127.0.0.1`.
To run the backend locally see its [ReadMe-file](./ConSurvBackend/ReadMe.md).
To run the frontend locally see its [ReadMe-file](./ConSurvFrontend/ReadMe.md).
To be able to make ConSurv accessable in a webbrowser run `task StartDevelopmentReverseProxy` (or shorter: `task sdrpu`) and then just open `https://consurv.test.local`.

## Contribue

Contributions are always welcome.

This product has the contribution-requirements defines by [DefaultOpenSourceContributionProcess](https://projects.aniondev.de/PublicProjects/Common/ProjectTemplates/-/blob/main/Conventions/Contributing/DefaultOpenSourceContributionProcess/DefaultOpenSourceContributionProcess.md).

## Repository-structure

This product uses the [CommonProjectStructure](https://projects.aniondev.de/PublicProjects/Common/ProjectTemplates/-/blob/main/Conventions/RepositoryStructure/CommonProjectStructure/CommonProjectStructure.md) as repository-structure.

## Branching-system

This product follows the [GitFlowSimplified](https://projects.aniondev.de/PublicProjects/Common/ProjectTemplates/-/blob/main/Conventions/BranchingSystem/GitFlowSimplified/GitFlowSimplified.md)-branching-system.

## Image-properties

The image-artifacts of this product must fulfill the image-properties defined by [DefaultImageUsabilityRequirements](https://projects.aniondev.de/PublicProjects/Common/ProjectTemplates/-/blob/main/Conventions/ImageProperties/DefaultImageUsabilityRequirements/DefaultImageUsabilityRequirements.md).

## Versioning

This product follows the [SemVerPractise](https://projects.aniondev.de/PublicProjects/Common/ProjectTemplates/-/blob/main/Conventions/Versioning/SemVerPractise/SemVerPractise.md)-versioning-system.

## License

See [License.txt](./License.txt) for license-information.
