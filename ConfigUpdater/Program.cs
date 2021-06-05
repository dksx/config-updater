using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace ConfigUpdater
{
    class Program
    {
        internal static IConfigurationRoot configuration;
        static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                IHostEnvironment env = context.HostingEnvironment;
                config.AddEnvironmentVariables()
                      .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                      .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                      .AddCommandLine(args);
                configuration = config.Build();
            })
            .ConfigureServices(services =>
            {
                services.AddSingleton<IHostedService, HostedService>();
            })
            .Build()
            .RunAsync();

            /*var task = Task.Run(async () => await Asy()).GetAwaiter().GetResult();
            foreach (var i in task)
            {
                Console.WriteLine(i);
            }*/
        }

        public static async Task<IEnumerable<string>> Asy()
        {
            var taskA = txA();
            var taskB = txB();
            await Task.WhenAll(new Task[] { taskA, taskB });
            return new string[] { taskA.Result, taskB.Result };
        }

        public static async Task<string> txA()
        {
            Console.WriteLine("A pausing 4s");
            await Task.Delay(TimeSpan.FromSeconds(4));
            Console.WriteLine("A returning");
            return "endedA";
        }

        public static async Task<string> txB()
        {
            Console.WriteLine("B pausing 2s");
            await Task.Delay(TimeSpan.FromSeconds(2));
            Console.WriteLine("B returning");
            return "endedB";
        }
    }
}
