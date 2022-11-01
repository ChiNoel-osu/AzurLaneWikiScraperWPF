using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Controls;

namespace AzurLaneWikiScraperWPF.ViewModel
{
	public partial class HomePageLeftPanelViewModel : ObservableObject
	{
		[ObservableProperty]
		ObservableCollection<TreeViewItem> _TreeViewSource = new ObservableCollection<TreeViewItem>();

		public void PopulateTree()
		{
			
		}

		public HomePageLeftPanelViewModel()
		{
			PopulateTree();
		}
	}
}
