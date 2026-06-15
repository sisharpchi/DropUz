namespace DropUz.Api.Extensions;

internal static class ModuleConfigurationExtensions
{
    public static IConfigurationBuilder AddModuleConfiguration(
        this IConfigurationBuilder configuration,
        IReadOnlyCollection<string> moduleNames)
    {
        foreach (string moduleName in moduleNames)
        {
            configuration.AddJsonFile(
                $"modules.{moduleName}.json",
                optional: true,
                reloadOnChange: true);

            configuration.AddJsonFile(
                $"modules.{moduleName}.{GetEnvironmentName()}.json",
                optional: true,
                reloadOnChange: true);
        }

        return configuration;
    }

    private static string GetEnvironmentName()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environments.Production;
    }
}
