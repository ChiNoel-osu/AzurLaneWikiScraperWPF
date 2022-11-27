using AzurLaneWikiScraperWPF.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AzurLaneWikiScraperWPF.ViewModel
{
	public partial class DownloadWindowViewModel : ObservableObject
	{
		IReadOnlyCollection<ShipTreeItem> galleries;
		public string SaveDir { get; set; }
		readonly string cacheDir;

		[ObservableProperty]
		bool _Step1EN = true;
		[ObservableProperty]
		bool _Step2EN = true;
		[ObservableProperty]
		bool _Step3EN = false;
		[ObservableProperty]
		string _Status = "Standby....";
		[ObservableProperty]
		double _Progress = 0;
		[ObservableProperty]
		string _ProgressSegment = "(0/3)";

		private StringBuilder _Log = new StringBuilder("Standby....");
		public string LogB { get => _Log.ToString(); }


		[RelayCommand]
		public void WelYes(TabControl tabControl)
		{
			tabControl.SelectedIndex = 1;
		}
		[RelayCommand]
		public void WelNo(Window window)
		{
			window.Close();
		}
		[RelayCommand]
		public void SetProc(TabControl tabControl)
		{
			Step1EN = false; Step2EN = false; Step3EN = true;
			tabControl.SelectedIndex = 2;
			Task.Run(() =>
			{
				Stopwatch globalSW = new Stopwatch();
				globalSW.Start();
				HttpClient httpClient = new HttpClient(new HttpClientHandler { Proxy = MainViewModel.proxy });
				Stopwatch stopwatch = new Stopwatch();
				Stopwatch segmentSW = new Stopwatch();
				void UpdateStatus(string st)
				{   //Status update helper
					Status = st;
					lock (_Log)
						_Log.AppendLine(st);
					OnPropertyChanged(nameof(LogB));
				}
				UpdateStatus($"<!> {galleries.Count} galleries found.");

				segmentSW.Start();
				#region Segment1 DL Webpage
				ProgressSegment = "(1/3)";
				bool isFileExist;
				float eachFilePersent = 100f / galleries.Count;
				foreach (ShipTreeItem galleryLink in galleries)
				{
					UpdateStatus($"Downloading webpage {galleryLink.Link}");
					stopwatch.Start();
					isFileExist = true;
					IEnumerator<string> enu = Directory.EnumerateFiles(cacheDir).GetEnumerator();
					do  //Check if cache exists.
						if (!enu.MoveNext())
						{   //If there's no file left, set flag and break out.
							isFileExist = false;
							break;
						}
					while (enu.Current[(enu.Current.LastIndexOf('\\') + 1)..] != galleryLink.Name + ".html");
					if (!isFileExist)   //File not exist, cache it.
						File.WriteAllText(string.Format("{0}\\{1}.html", cacheDir, galleryLink.Name), httpClient.GetStringAsync(new Uri(galleryLink.Link)).GetAwaiter().GetResult());
					enu.Dispose();
					Progress = _Progress + eachFilePersent;
					stopwatch.Stop();
					UpdateStatus($"DL complete in {stopwatch.ElapsedMilliseconds}ms.");
				}
				Progress = 100;
				segmentSW.Stop();
				UpdateStatus($"<!> All required webpages downloaded in {segmentSW.Elapsed}.");
				#endregion

				segmentSW.Restart();
				#region Segment2 Get all DL links
				Progress = 0;
				ProgressSegment = "(2/3)";
				HtmlDocument htmlDocument = new HtmlDocument();
				List<HtmlNode> shipSkins = new List<HtmlNode>();
				List<string> links = new List<string>();
				byte addedLinkAmount;
				//Helper function
				string GetOGLink(HtmlNode skin, bool isMultiVari = false)
				{
					string skinImg = isMultiVari ? skin.FirstChild.FirstChild.FirstChild.GetAttributeValue("src", "") : skin.FirstChild.FirstChild.GetAttributeValue("src", "");
					string skinOGImg = skinImg.Remove(skinImg.IndexOf("thumb/"), 6);
					return skinOGImg = skinOGImg.Remove(skinOGImg.LastIndexOf('/'));
				}
				string[] htmlPaths = Directory.GetFiles(cacheDir, "*.html");
				eachFilePersent = 100f / htmlPaths.Length;
				foreach (string shipGalleryPath in htmlPaths)
				{
					UpdateStatus($"Parsing HTML file {shipGalleryPath[shipGalleryPath.LastIndexOf('\\')..]}");
					stopwatch.Restart();
					shipSkins.Clear();
					addedLinkAmount = 0;
					htmlDocument.DetectEncodingAndLoad(shipGalleryPath);
					htmlDocument.DocumentNode.SelectNodes("//div[@class='shipskin']").ToList().ForEach(node => shipSkins.Add(node.ChildNodes[1].ChildNodes[1]));
					foreach (HtmlNode skin in shipSkins)
					{
						switch (skin.ChildNodes.Count)
						{
							case 1:
								links.Add(GetOGLink(skin));
								addedLinkAmount++;
								break;
							case 2:
								foreach (HtmlNode node in skin.ChildNodes[1].ChildNodes)
								{
									links.Add(GetOGLink(node, true));
									addedLinkAmount++;
								}
								break;
							default:
								throw new NotImplementedException();
						}
					}
					File.WriteAllLinesAsync($"{SaveDir}\\DownloadLinks_{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt", links);
					Progress = _Progress + eachFilePersent;
					stopwatch.Stop();
					UpdateStatus($"Parsed in {stopwatch.ElapsedMilliseconds}ms and added {addedLinkAmount} DL links.");
				}
				segmentSW.Stop();
				Progress = 100;
				UpdateStatus($"<!> All HTML file parsed and download links are extracted in {segmentSW.Elapsed}.");
				#endregion

				segmentSW.Restart();
				#region Segment3 DL All imgs.
				Progress = 0;
				ProgressSegment = "(3/3)";
				eachFilePersent = 100f / links.Count;
				List<string> existingImgName = (from path in Directory.GetFiles(SaveDir) where path.EndsWith(".png") select path[(path.LastIndexOf('\\') + 1)..]).ToList<string>();
				bool imgExist;
				foreach (string link in links)
				{
					UpdateStatus($"Downloading image from {link}");
					stopwatch.Restart();
					imgExist = true;
					byte[] img = null;
					string shipName = link[(link.LastIndexOf('/') + 1)..];  //With .png suffix
					if (!existingImgName.Contains(shipName))
					{
						imgExist = false;
						img = httpClient.GetByteArrayAsync(link).GetAwaiter().GetResult();
						File.WriteAllBytesAsync($"{SaveDir}\\{shipName}", img);
					}
					Progress = _Progress + eachFilePersent;
					stopwatch.Stop();
					if (imgExist)
						UpdateStatus($"{shipName} already exists. Skipped.");
					else
						UpdateStatus($"{shipName} with {img.Length}bytes is downloaded in {stopwatch.ElapsedMilliseconds}ms.");
				}
				segmentSW.Stop();
				UpdateStatus($"<!> Download complete in {segmentSW.Elapsed}.");
				#endregion

				globalSW.Stop();
				Progress = 100;
				UpdateStatus($"<!> Action complete in {globalSW.Elapsed}.");
			});
		}

		public DownloadWindowViewModel(IReadOnlyCollection<ShipTreeItem> galleries, string saveDir)
		{
			this.galleries = galleries;
			Directory.CreateDirectory(SaveDir = saveDir);
			Directory.CreateDirectory(cacheDir = Directory.GetCurrentDirectory() + "\\Cache\\Ships");
		}
	}
}
