using System.Reflection;
using CloudAwesome.Dataverse.Cli.CommandInterfaces;
using CloudAwesome.Dataverse.Core.Models;
using NUnit.Framework;

namespace CloudAwesome.Dataverse.Cli.Tests;

[TestFixture]
public class SupportsDataverseConnectionTests
{
    [Test]
    public void Validate_defaults_url_only_connection_to_interactive_user()
    {
        var settings = new TestConnectionSettings
        {
            Url = "https://example.crm11.dynamics.com"
        };

        var result = settings.Validate();
        var connection = GetConnectionDetails(settings);

        Assert.That(result.Successful, Is.True);
        Assert.That(connection.ConnectionType, Is.EqualTo(DataverseConnectionType.InteractiveUser));
    }

    [Test]
    public void Validate_uses_client_id_as_interactive_override_when_client_secret_is_absent()
    {
        var settings = new TestConnectionSettings
        {
            Url = "https://example.crm11.dynamics.com",
            ClientId = "ec8c9d8a-ac00-4871-9c4a-0696f1bac679"
        };

        var result = settings.Validate();
        var connection = GetConnectionDetails(settings);

        Assert.That(result.Successful, Is.True);
        Assert.That(connection.ConnectionType, Is.EqualTo(DataverseConnectionType.InteractiveUser));
        Assert.That(connection.ClientId, Is.EqualTo("ec8c9d8a-ac00-4871-9c4a-0696f1bac679"));
    }

    [Test]
    public void Validate_defaults_connection_string_when_connection_string_is_supplied()
    {
        var settings = new TestConnectionSettings
        {
            ConnectionString = "AuthType=ClientSecret;Url=https://example.crm11.dynamics.com"
        };

        var result = settings.Validate();
        var connection = GetConnectionDetails(settings);

        Assert.That(result.Successful, Is.True);
        Assert.That(connection.ConnectionType, Is.EqualTo(DataverseConnectionType.ConnectionString));
    }

    [Test]
    public void Validate_defaults_bearer_token_when_bearer_token_is_supplied()
    {
        var settings = new TestConnectionSettings
        {
            Url = "https://example.crm11.dynamics.com",
            BearerToken = "token"
        };

        var result = settings.Validate();
        var connection = GetConnectionDetails(settings);

        Assert.That(result.Successful, Is.True);
        Assert.That(connection.ConnectionType, Is.EqualTo(DataverseConnectionType.BearerToken));
    }

    [Test]
    public void Validate_rejects_username_and_password_authentication()
    {
        var settings = new TestConnectionSettings
        {
            ConnectionType = DataverseConnectionType.UserNameAndPassword,
            Url = "https://example.crm11.dynamics.com",
            UserName = "someone@example.com",
            UserPassword = "password"
        };

        var result = settings.Validate();

        Assert.That(result.Successful, Is.False);
        Assert.That(result.Message, Does.Contain("Username/password authentication is not supported"));
    }

    [Test]
    public void UserPassword_sets_connection_password()
    {
        var settings = new TestConnectionSettings
        {
            UserPassword = "expected-password"
        };

        var connection = GetConnectionDetails(settings);

        Assert.That(connection.Password, Is.EqualTo("expected-password"));
        Assert.That(connection.UserName, Is.Null);
    }

    private static DataverseConnection GetConnectionDetails(SupportsDataverseConnection settings)
    {
        var field = typeof(SupportsDataverseConnection)
            .GetField("ConnectionDetails", BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.That(field, Is.Not.Null);
        return (DataverseConnection)field!.GetValue(settings)!;
    }

    private sealed class TestConnectionSettings : SupportsDataverseConnection
    {
    }
}
