using CloudAwesome.Dataverse.Core.Authentication;
using CloudAwesome.Dataverse.Core.Models;
using NUnit.Framework;

namespace CloudAwesome.Dataverse.Core.Tests;

[TestFixture]
public class DataverseInteractiveAuthenticatorTests
{
	[Test]
	public void GetScopes_uses_dataverse_user_impersonation_scope()
	{
		var scopes = DataverseInteractiveAuthenticator.GetScopes("https://example.crm11.dynamics.com/");

		Assert.That(scopes, Is.EqualTo(new[] { "https://example.crm11.dynamics.com/user_impersonation" }));
	}

	[Test]
	public void NormalizeEnvironmentUrl_rejects_missing_url()
	{
		var exception = Assert.Throws<InvalidOperationException>(
			() => DataverseInteractiveAuthenticator.NormalizeEnvironmentUrl(null));

		Assert.That(exception!.Message, Does.Contain("environment URL is required"));
	}

	[Test]
	public void GetServiceClient_rejects_username_and_password_with_clear_message()
	{
		var connection = new DataverseConnection
		{
			ConnectionType = DataverseConnectionType.UserNameAndPassword,
			Url = "https://example.crm11.dynamics.com",
			UserName = "someone@example.com",
			Password = "password"
		};

		var exception = Assert.Throws<NotSupportedException>(
			() => DataverseConnectionExtensions.GetServiceClient(connection));

		Assert.That(exception!.Message, Does.Contain("Username/password authentication is not supported"));
	}
}
