using System.Collections.Generic;

namespace AzurLaneWikiScraperWPF.Model
{
	public class GraphicalRootItem
	{
		public string Name { get; set; }
		public List<GraphicalListItem> Children { get; set; } = new List<GraphicalListItem>();
	}
}
