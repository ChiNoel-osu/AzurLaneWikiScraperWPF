using AzurLaneWikiScraperWPF.Model;
using AzurLaneWikiScraperWPF.ViewModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace AzurLaneWikiScraperWPF.View
{
	/// <summary>
	/// DownloadWindow.xaml 的交互逻辑
	/// </summary>
	public partial class DownloadWindow : Window
	{
		public DownloadWindow(IReadOnlyCollection<ShipTreeItem> galleries, string saveDir)
		{
			InitializeComponent();
			DataContext = new DownloadWindowViewModel(galleries, saveDir);
		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			((TextBox)sender).ScrollToEnd();
		}
	}
}
