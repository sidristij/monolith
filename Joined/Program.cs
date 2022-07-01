using System;
using System.Collections.Generic;
using First;
using First.Services;
using Joined.ServiceInstance;
using Joined.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Second;
using Second.Services;

namespace Joined
{
	public class Program
	{
		public static Dictionary<Type, string> DaemonNames = new()
		{
			[typeof(First.Program)] = "first-http",
			[typeof(Second.Program)] = "second-http"
		};
		
		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureServices((context, services) =>
				{
					var configuration = context.Configuration;
					services.ConfigureDaemonBackgroundServices(
						configuration,
						DaemonNames,
						args);

					services.AddFirstServices();

					services.AddSecondServices(configuration);
                    services.AddScoped<IFirstClient, LocalFirstClient>();

					services.AddHostedService<DaemonBackgroundService<First.Program>>();
					services.AddHostedService<DaemonBackgroundService<Second.Program>>();
				})
				.ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
	}
}