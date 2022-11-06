using AzurLaneWikiScraperWPF.Model;
using System;
using System.Globalization;
using System.Windows.Data;

namespace AzurLaneWikiScraperWPF.Converters
{
	public class VariBoxSelectedItem2StringConverter : IValueConverter
	{
		private string lastValid;   //Preserve last image.
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return lastValid = ((SkinVariation)value)?.ShipImageLink ?? lastValid;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
