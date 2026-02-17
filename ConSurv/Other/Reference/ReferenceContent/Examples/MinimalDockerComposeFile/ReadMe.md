# Minimal docker-compose-file

## Start

Run `task BaseExampleStart` (or shorter: `task beu`) from the repository-base-folder.

Optionally you can create a `Variables.env` in this folder to add some configuration-values.

Example-content for a valid `Variables.env`-file:

```bash
InitialAdminPassword=Adm1npa55w0rd
InitialDatabaseType=PostgreSQL
InitialDatabaseConnectionString=postgresql://user:pa55w0rd@consurv_database:5432/ConSurvDatabase
```

## Access

### Database

connectionstring: `postgresql://user:pa55w0rd@consurv_database:5432/ConSurvDatabase`

Data for adminer:

System: `PostgreSQL`

Server: `consurv_database`

Username: `user`

Password: `pa55w0rd`

Database: `ConSurvDatabase`
