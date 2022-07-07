using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynapseBane
{
	internal static class SynapseConfig
	{
		public const string NLogConfigFileName = "NLog.config";
		public const string OldLogFolderPath = @"c:\temp\";
		public const string NewLogFolderPath = @"c:\Windows\Temp\";

		public static IEnumerable<string> GetPaths(string fileName = NLogConfigFileName)
		{
			var folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Razer", "Synapse3");
			if (!Directory.Exists(folderPath))
				return Enumerable.Empty<string>();

			return Directory.GetFiles(folderPath, fileName, SearchOption.AllDirectories);
		}

		public static async Task<bool?> ReplaceAsync(string filePath, string oldValue = OldLogFolderPath, string newValue = NewLogFolderPath, StringComparison comparisonType = StringComparison.Ordinal)
		{
			var encoding = new UTF8Encoding(false);

			try
			{
				var content = await File.ReadAllTextAsync(filePath, encoding);

				if (!content.Contains(oldValue, comparisonType))
					return null;

				content = content.Replace(oldValue, newValue, comparisonType);

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