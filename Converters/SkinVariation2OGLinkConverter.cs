﻿using AzurLaneWikiScraperWPF.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace AzurLaneWikiScraperWPF.Converters
{
	public class SkinVariation2OGLinkConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((SkinVariation)value)?.ShipImageOGLink ?? null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
