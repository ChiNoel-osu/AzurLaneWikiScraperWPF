using AzurLaneWikiScraperWPF.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AzurLaneWikiScraperWPF.ViewModel
{
	public partial class HomePageRightPanelViewModel : ObservableObject
	{
		[ObservableProperty]
		ObservableCollection<ShipSkin> _Skins = new ObservableCollection<ShipSkin>();
		[ObservableProperty]
		ObservableCollection<SkinVariation> _Variations = new ObservableCollection<SkinVariation>();
		[ObservableProperty]
		int _DlPersent = 0;
		[ObservableProperty]
		bool _IsNotDownloading = true;
		[ObservableProperty]
		string _StatusText = "Standby....";
		[ObservableProperty]
		int _Index;

		readonly string rootDir = Directory.GetCurrentDirectory();

		public string saveDir;
		public string cacheDir;
		public string existPath;

		public async void GetSkins(string galleryLink, string shipName)    //Shows skins.
		{
			MainWindow.MainVM.HomePageLeftPanel.IsNotSearching = false; //Visual Change
			StatusText = "Getting " + shipName + "....";
			Skins.Clear();
			await Task.Run(() =>
			{   //TODO: Try-Catch the web request.
				HtmlDocument skinPage = new HtmlDocument();
				bool isFileExist = true;
				IEnumerator<string> enu = Directory.EnumerateFiles(cacheDir).GetEnumerator();
				do  //Check if cache exists.
					if (!enu.MoveNext())
					{   //If there's no file left, break out.
						isFileExist = false;
						break;
					}
				//When the "while" segment is true (no matching file), continue enumerating.
				//When it's false (file exists), break out to check isFileExist (it will be true).
				while (enu.Current[(enu.Current.LastIndexOf('\\') + 1)..] != shipName + ".html");
				if (isFileExist)    //File exist, load cache.
				{
					StatusText = "Loading from cache....";
					skinPage.Load(enu.Current);
				}
				else    //File not found, load from web and create cache.
				{
					StatusText = "Loading from web....";
					skinPage.LoadHtml(MainViewModel.httpClient.GetStringAsync(new Uri(galleryLink)).GetAwaiter().GetResult());
					File.WriteAllText(string.Format("{0}\\{1}.html", cacheDir, shipName), skinPage.ParsedText);
				}
				enu.Dispose();
				List<HtmlNode> shipSkins = new List<HtmlNode>();
				skinPage.DocumentNode.SelectNodes("//div[@class='shipskin']").ToList().ForEach(node => shipSkins.Add(node.ChildNodes[1].ChildNodes[1]));
				foreach (HtmlNode skin in shipSkins)
				{
					ShipSkin shipSkin = new ShipSkin() { Name = skin.ParentNode.ParentNode.ParentNode.Attributes["title"].Value, DivSkinNode = skin };
					Skins.Add(shipSkin);
				}
				GetVariations(shipSkins[0]);  //Preload the default skin.
			});
		}
		public void GetVariations(HtmlNode skin)  //Shows variations.
		{   //This does not need to be async as it'll be called on non-UI thread.
			Variations.Clear();
			byte variationNumber = 0;
			switch (skin.ChildNodes.Count)
			{
				case 1: //Normal Skin, no variations.
					Variations.Add(GetVari(skin, variationNumber++));
					break;
				case 2: //Two variations exists. Mainly Default and NOBG.
					foreach (HtmlNode node in skin.ChildNodes[1].ChildNodes)
						Variations.Add(GetVari(node, variationNumber++, true));
					break;
				default:
					throw new NotImplementedException();
			}
			SkinVariation GetVari(HtmlNode skin, byte num, bool isMultiVari = false)
			{
				//The HTML structure of a multiple-variation-skin is different from one that only has one variation.
				string skinImg = isMultiVari ? skin.FirstChild.FirstChild.FirstChild.GetAttributeValue("src", "") : skin.FirstChild.FirstChild.GetAttributeValue("src", "");
				//Your typical link to be dealt with would look much like this:
				//https://azurlane.netojuu.com/images/thumb/0/02/HammannChristmas.png/1187px-HammannChristmas.png
				//To convert it to original picture, use the following code:
				string skinOGImg = skinImg.Remove(skinImg.IndexOf("thumb/"), 6);
				skinOGImg = skinOGImg.Remove(skinOGImg.LastIndexOf('/'));
				bool isFileExist = true;
				IEnumerator<string> enu = Directory.EnumerateFiles(saveDir).GetEnumerator();
				do  //Check if fullsize file exists.
					if (!enu.MoveNext())
					{   //If there's no file left, break out.
						isFileExist = false;
						break;
					}
				//When the "while" segment is true (no matching file), continue enumerating.
				//When it's false (file exists), break out to check isFileExist (it will be true).
				while (enu.Current[(enu.Current.LastIndexOf('\\') + 1)..] != skinOGImg[(skinOGImg.LastIndexOf('/') + 1)..]);
				if (isFileExist)
				{
					StatusText = "Loaded from local.";
					skinImg = enu.Current;  //Change the display path to local path.
				}
				else
					StatusText = "Loading from web.";
				enu.Dispose();
				MainWindow.MainVM.HomePageLeftPanel.IsNotSearching = true;  //Visual Change
				return num switch
				{
					0 => new SkinVariation { Variation = "Def.", ShipImageLink = skinImg, ShipImageOGLink = skinOGImg },
					_ => new SkinVariation { Variation = "Alt." + num, ShipImageLink = skinImg, ShipImageOGLink = skinOGImg },
				};
			}
		}

		//HttpClient doesn't have progress reporting so yeah.
		WebClient webClient = new WebClient() { Proxy = MainViewModel.proxy };
		[RelayCommand]
		public void DownloadImg(string imgLink)
		{
			if (!string.IsNullOrWhiteSpace(imgLink))    //Check if link is valid
			{
				IsNotDownloading = false;   //Change button visual
				IEnumerator<string> enu = Directory.EnumerateFiles(saveDir).GetEnumerator();
				do  //Check if file exists.
					if (!enu.MoveNext())
					{   //File not exist, initiate download.
						DlPersent = 0;
						webClient.DownloadFileAsync(new Uri(imgLink), saveDir + '\\' + imgLink[(imgLink.LastIndexOf('/') + 1)..]);
						webClient.DownloadProgressChanged += (sender, e) => { DlPersent = e.ProgressPercentage; };
						webClient.DownloadFileCompleted += (sender, e) => { IsNotDownloading = true; };
						enu.Dispose();
						return;
					}
				while (enu.Current[(enu.Current.LastIndexOf('\\') + 1)..] != imgLink[(imgLink.LastIndexOf('/') + 1)..]);
				enu.Dispose();
				IsNotDownloading = true;
			}
			else
				StatusText = "Select a skin first.";
		}

		public HomePageRightPanelViewModel()
		{
			Directory.CreateDirectory(saveDir = rootDir + "\\Downloads");
			Directory.CreateDirectory(cacheDir = rootDir + "\\Cache\\Ships");
			BindingOperations.EnableCollectionSynchronization(Skins, new object());
			BindingOperations.EnableCollectionSynchronization(Variations, new object());

			Skins.CollectionChanged += (sender, e) => { if (e.NewStartingIndex == 0) Index = 0; };
		}
	}
}
