using System;
using System.Collections.Generic;

namespace Joined.ServiceInstance
{
	public class DaemonBackgroundServiceConfig
	{
		public TimeSpan RestartDelay { get; set; }
		
		public string[] ConsoleArgs { get; set; }
		
		public IReadOnlyDictionary<Type, string> DaemonNames { get; set; }
	}
}