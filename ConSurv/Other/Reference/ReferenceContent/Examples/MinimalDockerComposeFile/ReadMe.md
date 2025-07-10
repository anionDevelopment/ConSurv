# Minimal docker-compose-file

## Start

Run `task BaseExampleStart` from the repository-base-folder.

Optionally you can create a `Variables.env` in this folder to add some configuration-values.

Example-content for a valid `Variables.env`-file:

```bash
InitialAdminPassword=Adm1npa55w0rd
InitialCameraAddresses="rtsp://192.168.1.42/stream1 rtsp://192.168.1.43/stream2"
```

## Access

### Database

connectionstring: `postgresql://root:R00tpa55w0rd@consurv_database:5432/ConSurvDatabase`

Data for adminer:

System: `PostgreSQL`

Server: `consurv_database`

Username: `root`

Password: `R00tpa55w0rd`

Database: `ConSurvDatabase`
