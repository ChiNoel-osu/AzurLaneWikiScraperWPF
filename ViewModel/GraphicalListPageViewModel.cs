using AzurLaneWikiScraperWPF.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AzurLaneWikiScraperWPF.ViewModel
{
	public partial class GraphicalListPageViewModel : ObservableObject
	{
		[ObservableProperty]
		ObservableCollection<GraphicalRootItem> _GraphicalList = new ObservableCollection<GraphicalRootItem>();

		string imgListCachePath = Directory.GetCurrentDirectory() + "\\Cache\\GraphicalListCache.html";

		[RelayCommand]
		void RefreshGraphicalList()
		{
			Task.Run(() =>
			{
				GraphicalList.Clear();
				string graphList;
				if (File.Exists(imgListCachePath))
				{
					graphList = File.ReadAllText(imgListCachePath);
				}
				else
				{
					graphList = MainViewModel.httpClient.GetStringAsync(new Uri($"https://azurlane.koumakan.jp/wiki/List_of_Ships_by_Image")).GetAwaiter().GetResult();
					File.WriteAllText(imgListCachePath, graphList);
				}
				HtmlDocument htmlDocument = new HtmlDocument();
				htmlDocument.LoadHtml(graphList);
				HtmlNode contentDiv = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='mw-parser-output']");
				List<string> shipClassifications = new List<string>();  //"Standard Ships, Retrofitted Ships etc.
				contentDiv.SelectNodes("//span[@class='mw-headline']").ToList().ForEach(cl => shipClassifications.Add(cl.InnerText));
				byte classificationsIndex = 0;
				contentDiv.SelectNodes("//div[@style='display:flex; flex-flow:row wrap;']").ToList().ForEach(node =>
				{
					GraphicalRootItem graphicalRootItem = new GraphicalRootItem();
					graphicalRootItem.Name = shipClassifications[classificationsIndex++];
					node.ChildNodes.Remove(0);  //Remove the "link" section, leaving only the "div" sections.
					foreach (HtmlNode ship in node.ChildNodes)
					{
						string reletiveLink = HomePageLeftPanelViewModel.wikiRoot + ship.LastChild.LastChild.Attributes[0].Value + "/Gallery";
						string shipName = ship.LastChild.LastChild.Attributes[1].Value;
						string thumbnailLink = ship.LastChild.LastChild.LastChild.Attributes["src"].Value;
						graphicalRootItem.Children.Add(new GraphicalListItem() { Link = reletiveLink, Name = shipName, ThumbnailLink = thumbnailLink });
					}
					GraphicalList.Add(graphicalRootItem);
				});
			});
		}

		public GraphicalListPageViewModel()
		{
			BindingOperations.EnableCollectionSynchronization(GraphicalList, new object());
		}
	}
}
