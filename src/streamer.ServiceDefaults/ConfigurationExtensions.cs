using Microsoft.Extensions.Configuration;

namespace streamer.ServiceDefaults;

public static class ConfigurationExtensions
{
    public static string GetRequiredValue(this IConfiguration configuration, string name) =>
        configuration[name] ?? throw new InvalidOperationException($"Configuration missing value for: {(configuration is IConfigurationSection s ? s.Path + ":" + name : name)}");
    
    public static TOptions BindOptions<TOptions>(this IConfiguration configuration, string section)
        where TOptions : new()
    {
        // note: with using Get<>() if there is no configuration in appsettings it just returns default value (null) for the configuration type
        // but if we use Bind() we can pass a instantiated type with its default value (for example in its property initialization) to bind method for binding configurations from appsettings
        // https://www.twilio.com/blog/provide-default-configuration-to-dotnet-applications
        var options = new TOptions();

        var optionsSection = configuration.GetSection(section);
        optionsSection.Bind(options);

        return options;
    }
    
    public static TOptions BindOptions<TOptions>(this IConfiguration configuration)
        where TOptions : new()
    {
        return BindOptions<TOptions>(configuration, typeof(TOptions).Name);
    }
}
