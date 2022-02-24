using System.Collections.Generic;
using System.Linq;
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

    public static string JoinListIntoString(IEnumerable<dynamic> list, string delimiter = " ")
    {
        return string.Join(delimiter, list);
    }

    public static List<string> SplitStringIntoList(string str, char delimiter = ' ')
    {
        return str.Split(delimiter).ToList();
    }
}