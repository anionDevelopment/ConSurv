# 7. Deployment View

## Infrastructure-overview

ConSurv will be deployed as OCI-container so the administrator needs a container-runtime to run ConSurv.

## Infrastructure-requirements

ConSurv is supposed to be run behind a reverseproxy to protect certain maintenance-routes and to manage the TLS-overhead.

## Deployment-processes

### General

ConSurv is supposed to be run only as container.
Updates will be applied by restarting the container with a newer image-version.

### Migrations

You do not have to care about internal data-migrations due to new versions.
Just run a newer version by using an updated image and then ConSurv will run all required migrations.
