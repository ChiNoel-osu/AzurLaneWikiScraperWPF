using AzurLaneWikiScraperWPF.Model;
using AzurLaneWikiScraperWPF.ViewModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace AzurLaneWikiScraperWPF.View
{
	/// <summary>
	/// GraphicalListPage.xaml 的交互逻辑
	/// </summary>
	public partial class GraphicalListPage : UserControl
	{
		public GraphicalListPage()
		{
			InitializeComponent();
		}

		private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{   //Make scrollviewer always handle mousewheel event
			ScrollViewer scv = (ScrollViewer)sender;
			scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
			e.Handled = true;
		}   //https://www.codenong.com/16234522

		private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{	//TODO: Initial implementation, might have bugs.
			((MainViewModel)DataContext).HomePageRightPanel.GetSkins(((GraphicalListItem)e.AddedItems[0]).Link, ((ShipTreeItem)e.AddedItems[0]).Name);
		}
	}
}
