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

    public static string GetOperationForResponse(string operation, string extra = null)
    {
        switch (operation)
        {
            case "rotate":
                return "Rotating";
            case "scale":
                if (extra == "on") return "Scaling up";
                if (extra == "down") return "Scaling down";
                return "Scaling";
            case "reset":
                return "Resetting";
            case "highlight":
                if (extra == "on") return "Showing highlight of";
                if (extra == "off") return "Removing highlight from";
                return "Highlighting";
            case "show side":
                return "Showing " + extra + " side of";
            case "animate":
                if (extra == "on") return "Starting animation of";
                if (extra == "off") return "Stopping animation of";
                return "Animating";
            case "visibility":
                if (extra == "on") return "Showing";
                if (extra == "off") return "Hiding";
                return "Showing";
            case "close look":
                return "Showing close look of ";
            case "side by side look":
                return "Showing side by side look of";
            case "attach":
                return "Attaching";
            default:
                return "Doing";
        }
    }

    public static GameObject FindObjectInFigure(GameObject figure, string name)
    {
        foreach (var child in figure.transform.GetComponentsInChildren<Transform>())
        {
            if (child.name.Contains(name) || child.name.Equals(name))
            {
                return child.gameObject;
            }
        }

        return null;
    }
}