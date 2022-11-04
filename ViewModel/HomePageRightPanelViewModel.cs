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
using System.Windows;
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
		[ObservableProperty]
		int _DlPersent = 0;
		[ObservableProperty]
		bool _IsNotDownloading = true;

		readonly string rootDir = Directory.GetCurrentDirectory();

		public string saveDir;
		public string cacheDir;

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
				MainWindow.MainVM.HomePageLeftPanel.IsNotSearching = true;  //Visual Change
				return num switch
				{
					0 => new SkinVariation { Variation = "Def.", ShipImageLink = skinImg, ShipImageOGLink = skinOGImg },
					_ => new SkinVariation { Variation = "Alt." + num, ShipImageLink = skinImg, ShipImageOGLink = skinOGImg },
				};
			}
		}

		BitmapImage image;
		[RelayCommand]
		public void DownloadImg(string imgLink)
		{
			if (!string.IsNullOrWhiteSpace(imgLink))
			{
				var enu = Directory.EnumerateFiles(saveDir).GetEnumerator();
				do  //Check if file exists.
					if (!enu.MoveNext())
					{   //File not exist, initiate download.
						DlPersent = 0;
						IsNotDownloading = false;
						BitmapImage newImg = new BitmapImage(new Uri(imgLink));
						if (image is not null && (image.UriSource == newImg.UriSource))
						{
							MessageBox.Show("Please send your use case to Issue.", "This message box should not appear.");
							IsNotDownloading = true;
						}
						else
						{
							image = newImg;
							image.DownloadProgress += Image_DownloadProgress;
							image.DownloadCompleted += Image_DownloadCompleted;
						}
						enu.Dispose();
						return; //break;
					}
				while (enu.Current[(enu.Current.LastIndexOf('\\') + 1)..] != imgLink[(imgLink.LastIndexOf('/') + 1)..]);
				string cur = enu.Current;
				MessageBox.Show("File exists");
			}
		}

		void Image_DownloadProgress(object sender, DownloadProgressEventArgs e)
		{
			DlPersent = e.Progress;
		}
		void Image_DownloadCompleted(object sender, EventArgs e)
		{
			BitmapImage completedImg = sender as BitmapImage;
			SaveBitmapImageIntoFile((BitmapImage)sender, saveDir + '\\' + completedImg.UriSource.Segments[^1]);
			IsNotDownloading = true;
			GC.Collect();
		}
		void SaveBitmapImageIntoFile(BitmapImage bitmapImage, string filePath)
		{
			BitmapEncoder encoder = new PngBitmapEncoder();
			encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
			using (FileStream fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
			{
				encoder.Save(fileStream);
			}
		}


		public HomePageRightPanelViewModel()
		{
			Directory.CreateDirectory(saveDir = rootDir + "\\Downloads");
			Directory.CreateDirectory(cacheDir = rootDir + "\\Cache");
			BindingOperations.EnableCollectionSynchronization(Skins, new object());
			BindingOperations.EnableCollectionSynchronization(Variations, new object());
		}
	}
}
