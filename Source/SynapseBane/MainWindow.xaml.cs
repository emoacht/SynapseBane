using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SynapseBane
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			this.TitleBar.MouseDown += (_, e) =>
			{
				if (Mouse.LeftButton == MouseButtonState.Pressed)
					this.DragMove();
			};
			this.TitleBar.MouseUp += (_, _) => RevertAeroSnap();
			this.TitleBar.MouseLeave += (_, _) => RevertAeroSnap();

			void RevertAeroSnap()
			{
				if (WindowState is WindowState.Maximized)
					WindowState = WindowState.Normal;
			}
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e) => this.Close();
	}
}