// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using BackgroundTask.HostedServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

try
{
    var build = Host.CreateDefaultBuilder(args)
        .ConfigureServices(services =>
        {
            services.AddHostedService<MyBackgroundHostedService>();
        })
        .Build();
    
    await build.RunAsync();

}
catch(Exception ex)
{

}