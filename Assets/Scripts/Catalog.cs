using System.Collections.Generic;
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

    public GameObject FindFigureWithId(string id)
    {
        return _actionsCatalog3D.FindFigureWithId(id);
    }

    public void ZoomHandler(string args)
    {
        _actionsCatalog3D.ZoomHandler(args);
    }

    public void ResetHandler(string args)
    {
        _actionsCatalog3D.ResetHandler(args);
    }

    public void HighlightHandler(string args)
    {
        _actionsCatalog3D.HighlightHandler(args);
    }

    public void RotateHandler(string args)
    {
        _actionsCatalog3D.RotateHandler(args);
    }

    public void ScaleHandler(string args)
    {
        _actionsCatalog3D.ScaleHandler(args);
    }

    public void ShowFigureSideHandler(string args)
    {
        _actionsCatalog3D.ShowFigureSideHandler(args);
    }

    public void CloseLookHandler(string args)
    {
        _actionsCatalog3D.CloseLookHandler(args);
    }

    public void AnimateFigureHandler(string args)
    {
        _actionsCatalog3D.AnimateFigureHandler(args);
    }

    public List<GameObject> FindObjectsWithIds(string ids)
    {
        // Debug.Log($"catalog {ids0} {ids1}");
        return _actionsCatalog3D.FindObjectsWithIds(ids);
    }

    public object SaveVal2Var(object value, string varName)
    {
        return _generalCatalog.SaveVal2Var(value, varName);
    }

    public int Count(object[] objects)
    {
        return _generalCatalog.Count(objects);
    }

    public bool Exist(object[] objects)
    {
        return _generalCatalog.Exist(objects);
    }

    public object Unique(object[] objects)
    {
        return _generalCatalog.Unique(objects);
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