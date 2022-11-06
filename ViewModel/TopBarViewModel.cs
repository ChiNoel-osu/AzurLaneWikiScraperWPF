using AzurLaneWikiScraperWPF.Model;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows;

namespace AzurLaneWikiScraperWPF.ViewModel
{
	public partial class TopBarViewModel
	{
		[RelayCommand]
		public void WndAct(WindowActionModel action)
		{
			switch (action.tag)
			{
				case "CloseBtn":
					action.window.Close();
					break;
				case "ChangeStBtn":
					if (action.window.WindowState == WindowState.Normal)
						action.window.WindowState = WindowState.Maximized;
					else if (action.window.WindowState == WindowState.Maximized)
						action.window.WindowState = WindowState.Normal;
					break;
				case "MinBtn":
					action.window.WindowState = WindowState.Minimized;
					break;
				default:
					throw new NotImplementedException();
			}

		}
	}
}
