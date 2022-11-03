using System;
using System.Collections.Generic;
using System.Text;

namespace AzurLaneWikiScraperWPF.ViewModel
{
	public class MainViewModel
	{
		public TopBarViewModel TopBar { get; set; }
		public HomePageLeftPanelViewModel HomePageLeftPanel { get; set; }
		public HomePageRightPanelViewModel HomePageRightPanel { get; set; }

		public MainViewModel()
		{
			TopBar = new TopBarViewModel();
			HomePageLeftPanel = new HomePageLeftPanelViewModel();
			HomePageRightPanel = new HomePageRightPanelViewModel();
		}
	}
}
