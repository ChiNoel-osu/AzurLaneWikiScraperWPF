using AzurLaneWikiScraperWPF.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace AzurLaneWikiScraperWPF.ViewModel
{
	public partial class NavBarViewModel : ObservableObject
	{
		HomePage HomePage = new HomePage();
		GraphicalListPage GraphicalListPage = new GraphicalListPage();

		[ObservableProperty]
		object _CurrentPage;

		public Button lastPage;	//This is set initially by some necessary code-behind in NavBar.xaml.cs

		[RelayCommand]
		void SwitchPage(Button page)
		{	//Navigation
			switch (page.Content)
			{
				case "Text List":
					CurrentPage = HomePage;
					break;
				case "Graphical List":
					CurrentPage = GraphicalListPage;
					break;
				case "Settings":
					break;
				default:
					break;
			}
			lastPage.IsEnabled = true;	//Enables the non-active page's button.
			lastPage = page;	//Set the lastPage(page switching from).
			lastPage.IsEnabled = false; //Disables the button as its page is active.
		}
		public NavBarViewModel()
		{
			CurrentPage = HomePage;
		}
	}
}
