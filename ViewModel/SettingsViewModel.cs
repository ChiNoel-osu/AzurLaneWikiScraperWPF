using AzurLaneWikiScraperWPF.Model;
using AzurLaneWikiScraperWPF.View;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace AzurLaneWikiScraperWPF.ViewModel
{
	public partial class SettingsViewModel
	{
		readonly string rootDir = Directory.GetCurrentDirectory();
		string defaultSaveDir;

		public string SaveDir { get; set; }

		[RelayCommand]
		public void SaveAll(MainViewModel vmThatHasTheData)
		{
			List<ShipTreeItem> ships = new List<ShipTreeItem>();
			Directory.CreateDirectory(SaveDir);
			foreach (RootTreeItem item in vmThatHasTheData.HomePageLeftPanel.TreeViewSource)
				if (item.Name == "Retrofitted Ships") continue;
				else
					foreach (ShipTreeItem ship in item.Children)
						ships.Add(ship);
			if (ships.Count > 0)
			{
				DownloadWindow wnd = new DownloadWindow(ships.AsReadOnly(), SaveDir);
				wnd.Show();
			}
			else
			{
				MessageBox.Show("Please update the Text List first :)", "Nope", MessageBoxButton.OK, MessageBoxImage.Asterisk);
			}
		}

		public SettingsViewModel()
		{
			SaveDir = defaultSaveDir = rootDir + "\\Downloads\\Everyone";
		}
	}
}
