using System.Collections.Generic;
using Catalogs;
using UnityEngine;

public class Catalog : IGeneralCatalogInterface, IActionsCatalog3DInterface
{
    private static Catalog _instance;
    private readonly ActionsCatalog3D _actionsCatalog3D;
    private readonly GeneralCatalog _generalCatalog;

    private Catalog()
    {
        var gameObject = Object.FindObjectOfType<ScriptExecutor>().gameObject;

        _generalCatalog = new GeneralCatalog();
        _actionsCatalog3D = gameObject.GetComponent<ActionsCatalog3D>()
            ? gameObject.GetComponent<ActionsCatalog3D>()
            : gameObject.AddComponent<ActionsCatalog3D>();
    }

    public static Catalog Instance => _instance ??= new Catalog();


    public void ZoomHandler(string args)
    {
        _actionsCatalog3D.ZoomHandler(args);
    }

    public void ResetHandler(string args)
    {
        _actionsCatalog3D.ResetHandler(args);
    }

    public List<GameObject> HighlightHandler(string args)
    {
        return _actionsCatalog3D.HighlightHandler(args);
    }

    public GameObject RotateHandler(string args)
    {
        return _actionsCatalog3D.RotateHandler(args);
    }

    public GameObject ScaleHandler(string args)
    {
        return _actionsCatalog3D.ScaleHandler(args);
    }

    public GameObject ShowFigureSideHandler(string args)
    {
        return _actionsCatalog3D.ShowFigureSideHandler(args);
    }

    public void CloseLookHandler(string args)
    {
        _actionsCatalog3D.CloseLookHandler(args);
    }

    public void AnimateFigureHandler(string args)
    {
        _actionsCatalog3D.AnimateFigureHandler(args);
    }

    public void VisibilityHandler(string args)
    {
        _actionsCatalog3D.VisibilityHandler(args);
    }

    public List<string> FilterIds(string args)
    {
        return _actionsCatalog3D.FilterIds(args);
    }

    public List<GameObject> Filter3DAttr(string args)
    {
        return _actionsCatalog3D.Filter3DAttr(args);
    }

    public object SaveVal2Var(string args)
    {
        return _generalCatalog.SaveVal2Var(args);
    }

    public int Count(object[] objects)
    {
        return _generalCatalog.Count(objects);
    }

    public bool Exist(object[] objects)
    {
        return _generalCatalog.Exist(objects);
    }

    public object Unique(string args)
    {
        return _generalCatalog.Unique(args);
    }

    public string[] ExtractNumbers(string value)
    {
        return _generalCatalog.ExtractNumbers(value);
    }

    public string ExtractID(string attrId, string query)
    {
        return _generalCatalog.ExtractID(attrId, query);
    }
}