using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using System.Windows.Media;
using System.Xml;

namespace DexpBugDetectorWpf
{
	public class Configuration : IConfigurationSectionHandler
	{
		static Configuration()
		{
			ConfigurationManager.GetSection("rects");
		}

		public static readonly List<RectInfo> Rects = new List<RectInfo>();

		public object Create(object parent, object configContext, XmlNode section)
		{
			try
			{
				XmlNodeList rectNodes = section.SelectNodes("rect");
				if (rectNodes != null)
				{
					foreach (XmlNode rectNode in rectNodes)
					{
						RectInfo rectInfo = new RectInfo();

						string colorStr = rectNode.Attributes["color"].Value;
						if (!colorStr.StartsWith("#"))
						{
							colorStr = "#" + colorStr;
						}
						rectInfo.Color = (Color)ColorConverter.ConvertFromString(colorStr);

						string pos = rectNode.Attributes["pos"].Value;
						string[] posParts = pos.Split(',', ';');
						rectInfo.X = int.Parse(posParts[0].Trim());
						rectInfo.Y = int.Parse(posParts[1].Trim());
						rectInfo.Width = int.Parse(posParts[2].Trim());
						rectInfo.Height = int.Parse(posParts[3].Trim());

						Rects.Add(rectInfo);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			return null;
		}

		public class RectInfo
		{
			public int X;
			public int Y;
			public int Width;
			public int Height;
			public Color Color;
		}
	}
}
