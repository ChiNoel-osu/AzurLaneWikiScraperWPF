using System.Collections.Generic;

namespace AzurLaneWikiScraperWPF.Model
{
	public class RootTreeItem
	{
		public string Name { get; set; }
		//Use List of ShipTreeItem instead of just TreeItem to save some resources
		//bc it only go as far as two levels anyway.
		public List<ShipTreeItem> Children { get; set; } = new List<ShipTreeItem>();
	}
}
