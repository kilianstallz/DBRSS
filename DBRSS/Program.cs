// See https://aka.ms/new-console-template for more information
using DBRSS.Buffer;
using DBRSS.DataConnector;
using PubSub;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DBRSS;
public class DBRSS
{

    public static Application GetApp()
    {

        IServiceCollection serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);

        IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
        var app = serviceProvider.GetService<Application>();

        serviceProvider.GetService<ThresholdService>();
        serviceProvider.GetService<IBufferService>();

        return app;
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        IConfigurationRoot configuration = GetConfiguration();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddSingleton<ThresholdService>();
        services.AddSingleton<IBufferService, BufferService>();
        services.AddSingleton(Hub.Default);
        services.AddSingleton<MqttConnector>();
        
        services.AddOptions();
        services.AddTransient<Application>();
    }

    private static IConfigurationRoot GetConfiguration()
    {
        IConfigurationBuilder configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables();

        return configuration.Build();
    }
}
