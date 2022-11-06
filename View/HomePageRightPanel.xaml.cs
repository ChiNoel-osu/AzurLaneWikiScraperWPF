using AzurLaneWikiScraperWPF.Model;
using AzurLaneWikiScraperWPF.ViewModel;
using System.Windows.Controls;

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
			VariBox.SelectedIndex = 0;
		}
	}
}
