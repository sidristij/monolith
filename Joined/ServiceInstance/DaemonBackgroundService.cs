using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Joined.ServiceInstance
{
	public class DaemonBackgroundService<T> : BackgroundService
	{
		private const string DefaultHostBuilderMethodName = "CreateHostBuilder";

		private static readonly HashSet<string> AspNetCorePrefixes = new()
		{
			"ASPNETCORE_"
		};

		private readonly IOptions<DaemonBackgroundServiceConfig> _config;
		private readonly ILogger _logger;
		private readonly ILoggerFactory _loggerFactory;
		private readonly IConfiguration _configuration;
		

		private readonly Type _programType;

		private MethodInfo _hostBuilderMethod;
		private string _daemonName;
		private readonly Dictionary<string, string> _daemonConfiguration;

		public DaemonBackgroundService(
			IOptions<DaemonBackgroundServiceConfig> config,
			ILoggerFactory loggerFactory,
			IConfiguration configuration)
		{
			_config = config;

			_loggerFactory = loggerFactory;
			_configuration = configuration;
			_hostBuilderMethod = null;
			_daemonName = string.Empty;
			_daemonConfiguration = new Dictionary<string, string>();
			_programType = typeof(T);

			_logger = _loggerFactory.CreateLogger(typeof(T).FullName);
		}

		public override Task StartAsync(CancellationToken stoppingToken)
		{
			InitHostBuilderMethod();
			InitDaemonName();
			InitDaemonConfiguration();
			
			return base.StartAsync(stoppingToken);
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			if (_hostBuilderMethod == null)
			{
				throw new ApplicationException($"Unexpected null value of {nameof(_hostBuilderMethod)}");
			}

			var args = _config.Value.ConsoleArgs;
			var parameters = args == null ? Array.Empty<object>() : new object[] { args };

			while (true)
			{
				stoppingToken.ThrowIfCancellationRequested();

				var result = _hostBuilderMethod.Invoke(obj: null, parameters);

				if (result == null)
				{
					throw new ApplicationException($"Unexpected null result of host builder method.");
				}

				if (result is not IHostBuilder hostBuilder)
				{
					throw new ApplicationException(
						$"Unexpected type of result of host builder method : {result.GetType().Name}");
				}

				ConfigureDaemonConfiguration(hostBuilder);
				var host = hostBuilder.Build();

				try
				{
					await host.RunAsync(stoppingToken);
					_logger.LogInformation($"Daemon '{_daemonName}' has shut down successfully.");
					return;
				}
				catch (Exception e)
				{
					_logger.LogError(e, $"Exception in daemon with name '{_daemonName}'.");
				}
				finally
				{
					host?.Dispose();
				}

				await Task.Delay(_config.Value.RestartDelay, stoppingToken);
				_logger.LogInformation($"Trying to restart daemon with name '{_daemonName}'.");
			}
		}

		private void InitHostBuilderMethod()
		{
			var methodInfos = _programType.GetMethods();

			var hostBuilderMethod =
				methodInfos.FirstOrDefault(methodInfo => methodInfo.Name == DefaultHostBuilderMethodName);

			if (hostBuilderMethod == null)
			{
				throw new ApplicationException($"No daemon host builder method found. " +
				                               $"Use default method name - '{DefaultHostBuilderMethodName}'");
			}

			ValidateHostBuilderMethod(hostBuilderMethod);
			_hostBuilderMethod = hostBuilderMethod;
		}

		private void InitDaemonName()
		{
			if (!_config.Value.DaemonNames.TryGetValue(_programType, out _daemonName))
			{
				_daemonName = string.Empty;
			}
		}

		private void InitDaemonConfiguration()
		{
			if (string.IsNullOrWhiteSpace(_daemonName))
			{
				return;
			}

			var prefix = _daemonName
				.ToUpper()
				.Replace(oldChar: '-', newChar: '_');
			var daemonConfigurationPrefix = $"{prefix}_";
			var ptDaemonConfigurationPrefix = $"{HostingConstants.ConfigurationPrefix}{daemonConfigurationPrefix}";
			
			foreach (var (key, value) in _configuration.AsEnumerable())
			{
				if (key.StartsWith(ptDaemonConfigurationPrefix))
				{
					var daemonKey = key.Substring(ptDaemonConfigurationPrefix.Length);
					daemonKey = RemoveNetCorePrefixes(daemonKey);
					
					_daemonConfiguration.Add($"{HostingConstants.ConfigurationPrefix}{daemonKey}", value);
				}
				else if (key.StartsWith(daemonConfigurationPrefix))
				{
					var daemonKey = key.Substring(daemonConfigurationPrefix.Length);
					daemonKey = RemoveNetCorePrefixes(daemonKey);
					
					_daemonConfiguration.Add(daemonKey, value);
				}
			}
		}

		private void ConfigureDaemonConfiguration(IHostBuilder hostBuilder)
		{
			hostBuilder.ConfigureAppConfiguration((context, builder) =>
			{
				builder.Sources.Clear();
				builder.AddInMemoryCollection(_daemonConfiguration);
			});
		}

		private string RemoveNetCorePrefixes(string daemonKey)
		{
			foreach (var prefix in AspNetCorePrefixes)
			{
				if (daemonKey.StartsWith(prefix))
				{
					return daemonKey.Substring(prefix.Length);
				}
			}

			return daemonKey;
		}
		
		private void ValidateHostBuilderMethod(MethodInfo methodInfo)
		{
			if (methodInfo.ReturnType != typeof(IHostBuilder))
			{
				throw new ApplicationException(
					$"Method '{methodInfo.Name}' must have '{typeof(IHostBuilder)}' return type.");
			}

			var parameters = methodInfo.GetParameters();

			if (parameters.Length != 1 ||
			    parameters[0].ParameterType != typeof(string[]))
			{
				throw new ApplicationException(
					$"Method '{methodInfo.Name}' must have one parameter of type '{typeof(string[])}'.");
			}
		}

		private static class HostingConstants
		{
			public static string ConfigurationPrefix = "CFG_";
		}
	}
}