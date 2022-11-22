using System;
using System.Net;
using System.Net.Http;

namespace AzurLaneWikiScraperWPF.ViewModel
{
	public class MainViewModel
	{
		public static HttpClient httpClient;
		public static WebProxy proxy;

		public TopBarViewModel TopBar { get; set; }
		public NavBarViewModel NavBar { get; set; }
		public HomePageLeftPanelViewModel HomePageLeftPanel { get; set; }
		public HomePageRightPanelViewModel HomePageRightPanel { get; set; }
		public GraphicalListPageViewModel GraphicalListPage { get; set; }
		public SettingsViewModel SettingsViewModel { get; set; }

		public MainViewModel()
		{
			#region Setup global HttpClient, proxy and stuff
			proxy = new WebProxy
			{
				Address = HttpClient.DefaultProxy.GetProxy(new Uri($"https://www.microsoft.com/")),
				BypassProxyOnLocal = true,
				UseDefaultCredentials = true
			};
			HttpClientHandler handler = new HttpClientHandler { Proxy = proxy };
			httpClient = new HttpClient(handler);
			httpClient.MaxResponseContentBufferSize = 10485760;  //10MB
			#endregion
			TopBar = new TopBarViewModel();
			NavBar = new NavBarViewModel();
			HomePageLeftPanel = new HomePageLeftPanelViewModel();
			HomePageRightPanel = new HomePageRightPanelViewModel();
			GraphicalListPage = new GraphicalListPageViewModel();
			SettingsViewModel = new SettingsViewModel();
		}
	}
}
