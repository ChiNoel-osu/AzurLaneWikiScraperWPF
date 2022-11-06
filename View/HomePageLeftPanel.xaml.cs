using AzurLaneWikiScraperWPF.Model;
using AzurLaneWikiScraperWPF.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace AzurLaneWikiScraperWPF.View
{
	/// <summary>
	/// HomePageLeftPanel.xaml 的交互逻辑
	/// </summary>
	public partial class HomePageLeftPanel : UserControl
	{
		public HomePageLeftPanel()
		{
			InitializeComponent();
		}

		private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (e.NewValue is not null && e.NewValue.GetType().Name == nameof(ShipTreeItem))
				((MainViewModel)DataContext).HomePageRightPanel.GetSkins(((ShipTreeItem)e.NewValue).Link, ((ShipTreeItem)e.NewValue).Name);
		}
	}
}
