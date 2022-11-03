using AzurLaneWikiScraperWPF.Model;
using AzurLaneWikiScraperWPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
				((MainViewModel)DataContext).HomePageRightPanel.GetSkins(((ShipTreeItem)e.NewValue).Link);
		}
	}
}
