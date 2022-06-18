using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Joined.ServiceInstance
{
	public static class DaemonBackgroundServiceExtensions
	{
		public static IServiceCollection ConfigureDaemonBackgroundServices(
			this IServiceCollection serviceCollection,
			IConfiguration configuration,
			IReadOnlyDictionary<Type, string> daemonNames,
			string[] args)
		{
			serviceCollection.Configure<DaemonBackgroundServiceConfig>(config =>
			{
				config.RestartDelay = configuration.GetValue(
					DaemonBackgroundServiceKeys.DaemonRestartDelay,
					DaemonBackgroundServiceValues.DaemonRestartDelay);
				config.ConsoleArgs = args;
				config.DaemonNames = daemonNames;
			});
			return serviceCollection;
		}
	}
}