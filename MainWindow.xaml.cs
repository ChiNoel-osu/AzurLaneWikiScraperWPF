using AzurLaneWikiScraperWPF.ViewModel;
using System.Windows;
using System.Windows.Input;

namespace AzurLaneWikiScraperWPF
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public static MainViewModel MainVM { get; set; }
		public MainWindow()
		{
			MainVM = new MainViewModel();
			DataContext = MainVM;
			InitializeComponent();
		}

		private void TopBar_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
				DragMove();
		}
	}
}
