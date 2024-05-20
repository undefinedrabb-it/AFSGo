# ASFGo

## Description

This application provides extendable translator interface.
Currently supported translators:
- FunTranslations 
  - Leetspeak
- DummyTranslator (Just return the same text)

Extensibility is provided by `ITranslationService` interface, and `TranslationFactory` class.

Application has sign up and sign in functionality.
To sign in provide Secret Token from the configuration file `appsettings.json`.
```
"RegisterSettings": {
"token": "secret"
}
```

Authorized user is able to view the history of translations.

## Requirements

- dotnet sdk `8.0.104` version
- dotnet ef `8.0.5` version (`dotnet tool install --global dotnet-ef` from bash (linux))

In order to use other database than SQLite [see `Changing the database` section](./README.md#changing-the-database).

### Configuration

This two section of the `appsettings.json`, could be (and if on production should be) setup.
For local development, you can use the default values. (Especially if you don't use Release configuration).

```
"RegisterSettings": {
"token": "secret"
},
"FunTranslator": {
"baseUrl": "https://api.funtranslations.com/translate/",
"apiKey": "SECERT_API_KEY"
},
```

I encourage you to use the `appsettings.Development.json` file for local development.
Because you don't need to call external api, because in repo is ones mock.

### Setup

The following instructions fetch dependencies and apply migration against the database.

(linux)

```bash
dotnet restore
dotnet ef database update --project src/ASFGo.Web
```
## Usage


### External api:
(linux)
```bash
dotnet run --project src/ASFGo.Web --property:Configuration=Release 

```

### Local api:
(linux)
```bash
dotnet run --project src/ASFGo.Api
dotnet run --project src/ASFGo.Web
```
 
## Architecture

Project is divided into 3 main parts:

- `AFSGo.Web` - contains the main application
- `AFSGo.Api` - contains a mock api for external service
- `AFSGo.Tests` - contains tests suites for the application

## Changing the database

Change the `appsettings.json` file to use different database authentication method. And in `Program.cs`:

```csharp
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services
    .AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));
```

NOTE: Different databases may have additional requirements. When in doubt, refer to database or database provider documentation. 