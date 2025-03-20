using CloudAwesome.Dataverse.Cli.Commands;
using Spectre.Console.Cli;

namespace CloudAwesome.Dataverse.Cli;

public static class Program
{
	static void Main(string[] args)
	{
		Console.WriteLine("Hello, World!");

		var cli = new CommandApp();

		cli.Configure(config =>
		{
			config.SetApplicationName("dvcli");
			config.AddBranch("plugins", settings =>
			{
				settings.AddCommand<PlaceholderCommand>("placeholder");
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
			});

			cli.Run(args);
		});
	}
}