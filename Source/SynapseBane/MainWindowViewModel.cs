using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace SynapseBane
{
	public class MainWindowViewModel : ObservableObject
	{
		public ObservableCollection<FileViewModel> ConfigFiles { get; } = new();

		public ICommand ReplaceCommand { get; }

		public MainWindowViewModel()
		{
			foreach (var filePath in SynapseConfig.GetPaths())
				ConfigFiles.Add(new FileViewModel(filePath));

			ReplaceCommand = new AsyncRelayCommand(
				() => Task.WhenAll(ConfigFiles.Select(x => x.ReplaceAsync())),
				() => ConfigFiles.Any());
		}
	}

	public class FileViewModel : ObservableObject
	{
		public string Path { get; }

		public Status Status
		{
			get => _status;
			private set => SetProperty(ref _status, value);
		}
		private Status _status;

		public FileViewModel(string path) => Path = path;

		internal async Task ReplaceAsync()
		{
			var success = await SynapseConfig.ReplaceRootTempAsync(Path);

			Status = success switch
			{
				null => Status.AlreadyReplaced,
				true => Status.NewlyReplaced,
				false => Status.Failed
			};
		}
	}

	public enum Status
	{
		Unknown = 0,
		AlreadyReplaced,
		NewlyReplaced,
		Failed
	}
}