using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Runtime;
using Orleans.Statistics;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using System;
using HelloWorldGrains;

namespace Silo
{
    class Program
    {
        static Task Main(string[] args)
        {
            return new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true);
                    config.AddEnvironmentVariables();

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .UseOrleans((context, builder) =>
                {
                    builder
                          .AddStartupTask(async (IServiceProvider services, CancellationToken cancellation) =>
                          {
                              // Use the service provider to get the grain factory.
                              var grainFactory = services.GetRequiredService<IGrainFactory>();

                              // Get a reference to a grain and call a method on it.
                              var grain = grainFactory.GetGrain<IHelloWorldGrain>(0);
                              await grain.SayHello("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
                          })
                        .UsePerfCounterEnvironmentStatistics()
                        .UseDashboard(options => { })
                        .UseLocalhostClustering()
                        .Configure<ClusterOptions>(options =>
                        {
                            //options.ClusterId = "dev";
                            //options.ServiceId = "HelloWorldApp";
                        })
                        .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
                        .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(HelloWorldGrain).Assembly).WithReferences())
                        //.ConfigureApplicationParts(parts => parts.AddFromApplicationBaseDirectory())
                        .AddMemoryGrainStorage(name: "ParrotStorage")
                        .AddMemoryGrainStorage(name: "PubSubStore")
                        .AddSimpleMessageStreamProvider(name: "Parrot")
                    //    .AddAzureBlobGrainStorage(
                    //        name: "profileStore",
                    //        configureOptions: options =>
                    //        {
                    //// Use JSON for serializing the state in storage
                    //options.UseJson = true;

                    //// Configure the storage connection key
                    //options.ConnectionString = "DefaultEndpointsProtocol=https;AccountName=data1;AccountKey=SOMETHING1";
                    //        })
                    ;
                })
                .ConfigureServices(services =>
                {
                    services.Configure<ConsoleLifetimeOptions>(options =>
                    {
                        options.SuppressStatusMessages = true;
                    });
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                })
                .RunConsoleAsync();
        }
    }
}
