using System.Windows.Controls;

namespace AzurLaneWikiScraperWPF.View
{
	/// <summary>
	/// TopBar.xaml 的交互逻辑
	/// </summary>
	public partial class TopBar : UserControl
	{
		public TopBar()
		{
			InitializeComponent();
			DataContext = MainWindow.MainVM;
		}
	}
}
