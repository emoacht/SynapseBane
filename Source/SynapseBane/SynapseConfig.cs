using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SynapseBane
{
	internal static class SynapseConfig
	{
		private const string NLogConfigFileName = "NLog.config";
		private const string RootTempPath = @"c:\temp\";
		private const string SystemTempPath = @"c:\windows\temp\";

		private static readonly Lazy<string> _systemTempPath = new(() =>
			Environment.GetEnvironmentVariable("TEMP", EnvironmentVariableTarget.Machine)?.ToLower() ?? SystemTempPath);

		public static IEnumerable<string> GetPaths(string fileName = NLogConfigFileName)
		{
			var folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Razer", "Synapse3");
			if (!Directory.Exists(folderPath))
				return Enumerable.Empty<string>();

			return Directory.GetFiles(folderPath, fileName, SearchOption.AllDirectories);
		}

		public static async Task<bool?> ReplaceRootTempAsync(string filePath)
		{
			var encoding = new UTF8Encoding(false);
			var internalLogFilePattern = new Regex(@"internalLogFile=""(?<path>[^""]+)""");

			try
			{
				var content = await File.ReadAllTextAsync(filePath, encoding);

				var match = internalLogFilePattern.Match(content);
				if (!match.Success)
					return null;

				var path = match.Groups["path"].Value;
				if (!path.StartsWith(RootTempPath, StringComparison.Ordinal))
					return null;

				var oldValue = match.Value;
				var newValue = $@"internalLogFile=""{Path.Combine(_systemTempPath.Value, path[RootTempPath.Length..])}""";

				content = content.Replace(oldValue, newValue, StringComparison.Ordinal);

				await File.WriteAllTextAsync(filePath, content, encoding);
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}