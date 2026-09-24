using CloudAwesome.Dataverse.Core.Models;
using NUnit.Framework;
using System.Text.Json.Serialization;

namespace CloudAwesome.Dataverse.Core.Tests;

[TestFixture]
public class SerialisationWrapperTests
{
    [Test]
    public void DeserialiseJsonFromFile_reads_camel_case_enum_values()
    {
        var filePath = Path.Combine(TestContext.CurrentContext.WorkDirectory, $"{Guid.NewGuid()}.json");
        File.WriteAllText(filePath, """
            {
              "connectionType": "connectionString"
            }
            """);

        var result = SerialisationWrapper.DeserialiseJsonFromFile<TestConnectionModel>(filePath);

        Assert.That(result.ConnectionType, Is.EqualTo(DataverseConnectionType.ConnectionString));
    }

    [Test]
    public void SerialiseJsonToFile_writes_camel_case_enum_values()
    {
        var filePath = Path.Combine(TestContext.CurrentContext.WorkDirectory, $"{Guid.NewGuid()}.json");
        var model = new TestConnectionModel
        {
            ConnectionType = DataverseConnectionType.BearerToken
        };

        SerialisationWrapper.SerialiseJsonToFile(filePath, model);

        var json = File.ReadAllText(filePath);
        Assert.That(json, Does.Contain("\"connectionType\": \"bearerToken\""));
    }

    [Test]
    public void DeserialiseJsonFromFile_throws_clear_exception_when_json_is_null()
    {
        var filePath = Path.Combine(TestContext.CurrentContext.WorkDirectory, $"{Guid.NewGuid()}.json");
        File.WriteAllText(filePath, "null");

        var exception = Assert.Throws<InvalidOperationException>(
            () => SerialisationWrapper.DeserialiseJsonFromFile<TestConnectionModel>(filePath));

        Assert.That(exception!.Message, Does.Contain($"Could not deserialize '{filePath}'"));
        Assert.That(exception.Message, Does.Contain(nameof(TestConnectionModel)));
    }

    private sealed class TestConnectionModel
    {
        [JsonPropertyName("connectionType")]
        public DataverseConnectionType ConnectionType { get; set; }
    }
}
