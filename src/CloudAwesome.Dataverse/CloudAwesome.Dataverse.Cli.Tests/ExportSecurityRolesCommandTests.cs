using CloudAwesome.Dataverse.Cli.Commands;
using CloudAwesome.Dataverse.Security;
using NUnit.Framework;

namespace CloudAwesome.Dataverse.Cli.Tests;

[TestFixture]
public class ExportSecurityRolesCommandTests
{
    [Test]
    public void ApplyOutputFilePathOverride_replaces_manifest_value_when_cli_value_is_provided()
    {
        var manifest = new ExportSecurityRolesManifest
        {
            OutputFilePath = "manifest-output.json"
        };

        ExportSecurityRolesCommand.ApplyOutputFilePathOverride(manifest, "cli-output.json");

        Assert.That(manifest.OutputFilePath, Is.EqualTo("cli-output.json"));
    }

    [Test]
    public void ApplyOutputFilePathOverride_preserves_manifest_value_when_cli_value_is_not_provided()
    {
        var manifest = new ExportSecurityRolesManifest
        {
            OutputFilePath = "manifest-output.json"
        };

        ExportSecurityRolesCommand.ApplyOutputFilePathOverride(manifest, null);

        Assert.That(manifest.OutputFilePath, Is.EqualTo("manifest-output.json"));
    }
}
