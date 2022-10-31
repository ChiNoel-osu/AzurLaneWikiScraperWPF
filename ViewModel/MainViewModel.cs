using System;
using System.Collections.Generic;
using System.Text;

namespace AzurLaneWikiScraperWPF.ViewModel
{
	public class MainViewModel
	{
		public TopBarViewModel TopBar { get; set; }


		public MainViewModel()
		{
			TopBar = new TopBarViewModel();
		}
	}
}
