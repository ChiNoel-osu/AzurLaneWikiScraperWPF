using AzurLaneWikiScraperWPF.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AzurLaneWikiScraperWPF.ViewModel
{
	public partial class HomePageLeftPanelViewModel : ObservableObject
	{
		[ObservableProperty]
		ObservableCollection<RootTreeItem> _TreeViewSource = new ObservableCollection<RootTreeItem>();
		[ObservableProperty]
		bool _IsNotSearching = true;

		string wikiRoot = "https://azurlane.koumakan.jp";
		string ListOfShipsPage = "https://azurlane.koumakan.jp/wiki/List_of_Ships";
		string mainCachePath = Directory.GetCurrentDirectory() + "\\Cache\\MainSiteCache.txt";

		[RelayCommand]
		public async void PopulateTree()
		{
			TreeViewSource.Clear();
			IsNotSearching = false;
			string responseStr;
			//TODO: Try-Catch the web request. Make cache configurable.
			if (File.Exists(mainCachePath))
				responseStr = File.ReadAllText(mainCachePath);
			else
				using (HttpClient client = new HttpClient())
				{   //I guess we can use HtmlWeb here given that we have Html Agility Pack?
					responseStr = await client.GetStringAsync(new Uri(ListOfShipsPage));
					File.WriteAllText(mainCachePath, responseStr);
				}   //Uh, whatever.
					//Start HTML extracting....
			await Task.Run(() =>
			{
				HtmlDocument htmlDocument = new HtmlDocument();
				htmlDocument.LoadHtml(responseStr);
				List<string> h2Contents = new List<string>();   //Ship classifications
				List<string> tableHTMLs = new List<string>();   //Ship classifications table
				htmlDocument.DocumentNode.Descendants("h2").ToList().ForEach(node => h2Contents.Add(node.InnerText));
				string[] undesiredH2 = { "Contents", "Other Charts", "Navigation menu" };
				foreach (string h2 in undesiredH2)  //Remove extra H2 lines, leaving only Ship classifications.
					h2Contents.Remove(h2);
				htmlDocument.DocumentNode.Descendants("table").ToList().ForEach(node => tableHTMLs.Add(node.OuterHtml));
				byte tableIndex = 0; RootTreeItem tItem;
				foreach (string tableHTML in tableHTMLs)
				{
					htmlDocument.LoadHtml(tableHTML);
					List<string> linkHTMLs = new List<string>();
					//Search inside tableHTML and select every single <a> in the second appearance of <td> reletive to their parent block (in this case <tr>).	[XPath]
					htmlDocument.DocumentNode.SelectNodes("//td[2]/a").ToList().ForEach(a => linkHTMLs.Add(a.OuterHtml));
					//Start adding stuff to treeview.
					tItem = new RootTreeItem();
					tItem.Name = h2Contents[tableIndex++];
					foreach (string linkHTML in linkHTMLs)  //Adding each individual ship.
					{
						htmlDocument.LoadHtml(linkHTML);
						ShipTreeItem shipTreeItem = new ShipTreeItem();
						shipTreeItem.Name = htmlDocument.DocumentNode.InnerText;
						shipTreeItem.Link = wikiRoot + htmlDocument.DocumentNode.ChildNodes[0].Attributes["href"].Value.ToString() + "/Gallery";
						tItem.Children.Add(shipTreeItem);
					}
					TreeViewSource.Add(tItem);
				}
			});
			IsNotSearching = true;
		}

		public HomePageLeftPanelViewModel()
		{
			BindingOperations.EnableCollectionSynchronization(TreeViewSource, new object());
		}
	}
}
