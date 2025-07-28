# Minimal docker-compose-file

## Start

Run `task BaseExampleStart` (or shorter: `task beu`) from the repository-base-folder.

Optionally you can create a `Variables.env` in this folder to add some configuration-values.

Example-content for a valid `Variables.env`-file:

```bash
InitialAdminPassword=Adm1npa55w0rd
InitialCameraAddresses="rtsp://192.168.1.42/stream1;rtsp://192.168.1.43/stream2"
```

(This file will automagically generateed when running `StartExample.py`.)

## Access

### Database

connectionstring: `postgresql://user:pa55w0rd@consurv_database:5432/ConSurvDatabase`
Data to acces the databas through adminer (`localhost:8080`):

System: `PostgreSQL`

Server: `consurv_database`

Username: `user`

Password: `pa55w0rd`

Database: `ConSurvDatabase`
