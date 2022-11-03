using AzurLaneWikiScraperWPF.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace AzurLaneWikiScraperWPF.ViewModel
{
	public partial class HomePageRightPanelViewModel : ObservableObject
	{
		[ObservableProperty]
		ObservableCollection<ShipSkin> _Skins = new ObservableCollection<ShipSkin>();
		[ObservableProperty]
		ObservableCollection<SkinVariation> _Variations = new ObservableCollection<SkinVariation>();

		HtmlWeb htmlWeb = new HtmlWeb();
		public async void GetSkins(string galleryLink)    //Shows skins.
		{
			MainWindow.MainVM.HomePageLeftPanel.IsNotSearching = false; //Visual Change
			Skins.Clear();
			await Task.Run(() =>
			{   //TODO: Try-Catch the web request. Cache the galleryLink.
				HtmlDocument skinPage = htmlWeb.Load(galleryLink);
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
			static SkinVariation GetVari(HtmlNode skin, byte num, bool isMultiVari = false)
			{
				string skinImg = isMultiVari ? skin.FirstChild.FirstChild.FirstChild.GetAttributeValue("src", "") : skin.FirstChild.FirstChild.GetAttributeValue("src", "");
				//TODO: Display thumb but save original.
				//Your typical link to be dealt with would look much like this:
				//https://azurlane.netojuu.com/images/thumb/0/02/HammannChristmas.png/1187px-HammannChristmas.png
				//To convert it to original picture, use the following code:
				string skinOGImg = skinImg.Remove(skinImg.IndexOf("thumb/"), 6);
				skinOGImg = skinOGImg.Remove(skinOGImg.LastIndexOf('/'));
				MainWindow.MainVM.HomePageLeftPanel.IsNotSearching = true;	//Visual Change
				return num switch
				{
					0 => new SkinVariation { Variation = "Def.", ShipImageLink = skinImg, ShipImageOGLink = skinOGImg },
					_ => new SkinVariation { Variation = "Alt." + num, ShipImageLink = skinImg, ShipImageOGLink = skinOGImg },
				};
			}
		}

		public HomePageRightPanelViewModel()
		{
			BindingOperations.EnableCollectionSynchronization(Skins, new object());
			BindingOperations.EnableCollectionSynchronization(Variations, new object());
		}
	}
}
