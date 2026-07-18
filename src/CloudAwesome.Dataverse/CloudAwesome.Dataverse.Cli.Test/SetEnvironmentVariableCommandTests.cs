using CloudAwesome.Dataverse.Cli.Commands;

namespace CloudAwesome.Dataverse.Cli.Test;

[TestFixture]
public class SetEnvironmentVariableCommandTests
{
	[Test]
	public void Execute_Returns_SuccessCode_When_Manifest_Deserialisation_Succeeds()
	{
		var command = new SetEnvironmentVariableCommand();

		// context is not used, so can be null
		var context = (Spectre.Console.Cli.CommandContext?)null;
		
		var manifestFile = Path.GetTempFileName();
		File.WriteAllText(manifestFile, 
			@"
					{
					  ""variables"": [
					    {
					      ""key"": ""new_testvariable1"",
					      ""value"": ""variable1""
					    },
					    {
					      ""key"": ""new_testvariable2"",
					      ""value"": ""variable2""
					    },
					    {
					      ""key"": ""new_testvariable3"",
					      ""value"": ""variable3""
					    }
					  ]
					}");

		var settings = new SetEnvironmentVariableSettings
		{
			Manifest = manifestFile
		};
		
		var result = command.Execute(context!, settings);
		
		Assert.That(result, Is.EqualTo(0));
	}
	
	[Test]
	public void Execute_Returns_ErrorCode_When_Manifest_Deserialisation_Fails()
	{
		var command = new SetEnvironmentVariableCommand();

		// context is not used, so can be null
		var context = (Spectre.Console.Cli.CommandContext?)null;
		
		var manifestFile = Path.GetTempFileName();
		File.WriteAllText(manifestFile, 
			@"
					{
					  ""variables"": [
					    {
					      ""key"": ""new_testvariable1"",
					      ""value"": ""variable1""
					    },
					    {
					      ""key"": ""new_testvariable2"",
					      ""value"": ""variable2""
					    },
					    {
					      ""key"": ""new_testvariable3"",
					      ""value"": ""variable3""
					    },
					  ]
					}");

		var settings = new SetEnvironmentVariableSettings
		{
			Manifest = manifestFile
		};
		
		var result = command.Execute(context!, settings);
		
		Assert.That(result, Is.EqualTo(-1));
	}
}