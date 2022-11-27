using System.Windows.Controls;

namespace AzurLaneWikiScraperWPF.View
{
	/// <summary>
	/// NavBar.xaml 的交互逻辑
	/// </summary>
	public partial class NavBar : UserControl
	{
		public NavBar()
		{
			InitializeComponent();
			MainWindow.MainVM.NavBar.lastPage = HomePageBtn;
		}
	}
}
