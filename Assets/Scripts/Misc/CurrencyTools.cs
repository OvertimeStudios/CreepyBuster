using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

public class CurrencyTools : MonoBehaviour 
{
	private static Dictionary<string,string> map;

	private static bool isLoaded = false;

	public static void LoadCurrencies()
	{
		byte[] bytes = null;
		TextAsset asset = Resources.Load<TextAsset>("Currencies");

		ReadCSV(asset.text);

		isLoaded = true;
	}

	private static void ReadCSV(string csv)
	{
		map = new Dictionary<string, string>();

		string[] splitted = csv.Split("\n" [0]);

		foreach(string row in splitted)
		{
			string key = row.Substring(0, row.IndexOf(","));
			string value = row.Substring(row.IndexOf(",") + 1);
			value = value.Replace("\"", "");

			map.Add(key, value);
		}
	}

	public static string GetCurrencySymbol(string currencyCode)
	{
		if(!isLoaded)
			LoadCurrencies();

		//24 = $
		string hex = "24";

		//check if not locally stored
		if(map.ContainsKey(currencyCode))
			hex = map[currencyCode];

		return FromHex(hex);

		//generic currency symbol (USD = $ not US$, BRL = $, not R$)
		/*CultureInfo[] allCultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
		foreach(CultureInfo cultureInfo in allCultures)
		{
			try
			{
				RegionInfo myRI1 = new RegionInfo(cultureInfo.LCID);
				if(myRI1.ISOCurrencySymbol == currencyCode)
				{
					symbol = myRI1.CurrencySymbol;
					break;
				}
			}
			catch
			{

			}
		}*/
	}

	private static string FromHex(string hex)
	{
		string[] hexValuesSplit = hex.Split(',');
		string s = "";
		foreach (string h in hexValuesSplit)
		{
			int value = Int32.Parse(h, System.Globalization.NumberStyles.HexNumber);
			// Get the character corresponding to the integral value.
			string stringValue = Char.ConvertFromUtf32(value);
			char charValue = (char)value;

			s += charValue;
		}

		return s;
	}
}