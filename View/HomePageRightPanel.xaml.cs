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
	/// HomePageRightPanel.xaml 的交互逻辑
	/// </summary>
	public partial class HomePageRightPanel : UserControl
	{
		public HomePageRightPanel()
		{
			InitializeComponent();
		}

		private void Skin_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count == 1)
				((MainViewModel)DataContext).HomePageRightPanel.GetVariations(((ShipSkin)e.AddedItems[0]).DivSkinNode);
		}
	}
}
