using System.Collections;
using UnityEngine;

public class HelperFunctions : MonoBehaviour
{
	public static string ConvertColorToString(Color color)
	{
		const char delimiter = '-';
		var r = color.r;
		var g = color.g;
		var b = color.b;
		var a = color.a;
		
		
		return $"{r}{delimiter}{g}{delimiter}{b}{delimiter}{a}";
	}

	public static Color ConvertStringToColor(string color)
	{
		const char delimiter = '-';
		var values = color.Split(delimiter);
		
		return new Color(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3]));
	}
}