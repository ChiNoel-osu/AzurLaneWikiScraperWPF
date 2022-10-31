using AzurLaneWikiScraperWPF.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace AzurLaneWikiScraperWPF.Converters
{
	public class WindowActionConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			return new WindowActionModel
			{
				window = (System.Windows.Window)values[0],
				tag = (string)values[1]
			};
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
