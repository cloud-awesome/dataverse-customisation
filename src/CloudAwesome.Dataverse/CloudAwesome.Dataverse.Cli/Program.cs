using System.Reflection;
using CloudAwesome.Dataverse.Cli.Commands;
using Spectre.Console.Cli;

namespace CloudAwesome.Dataverse.Cli;

public static class Program
{
	private const string CliName = "dvcli";
	
	static void Main(string[] args)
	{
		Console.WriteLine($"--------\n{CliName}, version {GetCliVersion()}\n--------");

		var cli = new CommandApp();

		cli.Configure(config =>
		{
			config.SetApplicationName(CliName);
			config.AddBranch("plugins", settings =>
			{
				/*settings.AddCommand<PluginRegistrationCommand>("register").WithData(true);
				settings.AddCommand<PluginRegistrationCommand>("unregister").WithData(false);*/
				settings.AddCommand<PlaceholderCommand>("register").WithData(true);
				settings.AddCommand<PlaceholderCommand>("unregister").WithData(false);
			});
	
			config.AddBranch("customisations", settings =>
			{
				settings.AddCommand<SetEnvironmentVariableCommand>("set-environment-variable");
			});
	
			config.AddBranch("processes", settings =>
			{
				settings.AddCommand<ProcessActivationCommand>("activate").WithData(true);
				settings.AddCommand<ProcessActivationCommand>("deactivate").WithData(false);
			});
	
			config.AddBranch("dependencies", settings =>
			{
				settings.AddCommand<PlaceholderCommand>("placeholder");
			});
	
			config.AddBranch("security", settings =>
			{
				settings.AddCommand<ExportSecurityRolesCommand>("export");
				settings.AddCommand<ImportSecurityRolesCommand>("import");
			});
	
			config.AddBranch("document", settings =>
			{
				settings.AddCommand<PlaceholderCommand>("placeholder");
			});
	
			config.AddBranch("project-ops", settings =>
			{
				settings.AddCommand<PlaceholderCommand>("placeholder");
			});
	
			config.AddBranch("test", settings =>
			{
				settings.AddCommand<WhoAmICommand>("who-am-i");
				settings.AddCommand<TestGetAccessToken>("get-access-token");
			});

			cli.Run(args);
		});
	}

	private static string GetCliVersion()
	{
		var version = Assembly.GetExecutingAssembly()
			.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
			.InformationalVersion;

		var cleanVersion = string.Empty;
		if (version != null)
		{
			cleanVersion = version.Split('+')[0];
		}
		
		return cleanVersion;
	}
}