using System.Reflection;
using CloudAwesome.Dataverse.Cli.CommandInterfaces;
using CloudAwesome.Dataverse.Core.Models;
using NUnit.Framework;

namespace CloudAwesome.Dataverse.Cli.Tests;

[TestFixture]
public class SupportsDataverseConnectionTests
{
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
