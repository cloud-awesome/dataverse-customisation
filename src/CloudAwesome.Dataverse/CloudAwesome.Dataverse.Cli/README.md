# dataverse-customisation
Automate common, time-consuming, error prone, or otherwise non-automatable tasks during customisation and configuration of Dataverse/Dynamics 365 CE

## Interactive user authentication

For local manual usage, pass the Dataverse environment URL and let the CLI authenticate the signed-in user with Microsoft Entra:

```powershell
dvcli test who-am-i --url https://example.crm11.dynamics.com
```

When no connection string, bearer token, client secret, or explicit connection type is supplied, `--url` defaults to interactive browser authentication. The default public-client app registration is:

```text
ec8c9d8a-ac00-4871-9c4a-0696f1bac679
```

Optional flags:

```powershell
dvcli test who-am-i --url https://example.crm11.dynamics.com --tenant-id dc6f038f-9dfc-4cf9-92fc-bc99fe1322b5
dvcli test who-am-i --url https://example.crm11.dynamics.com --client-id <your-public-client-app-id>
```

The app registration must allow public-client browser sign-in with a `http://localhost` redirect URI and delegated Dataverse consent for `<environment-url>/user_impersonation`. The CLI attempts cached silent token acquisition first, then opens the browser when interaction or consent is required.
