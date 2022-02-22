using UnityEngine;

public class Catalog : IGeneralCatalogInterface, IActionsCatalog3DInterface
{
	private readonly GeneralCatalog _generalCatalog;
	private readonly ActionsCatalog3D _actionsCatalog3D ;

	private Catalog()
	{
		var gameObject = Object.FindObjectOfType<ScriptExecutor>().gameObject; 
		
		_generalCatalog = new GeneralCatalog();
		_actionsCatalog3D = gameObject.GetComponent<ActionsCatalog3D>() ? gameObject.GetComponent<ActionsCatalog3D>() : gameObject.AddComponent<ActionsCatalog3D>();
	}  
	private static Catalog _instance;

	public static Catalog Instance => _instance ??= new Catalog();

  public object SaveVal2Var(object value, string varName)
	{
		return _generalCatalog.SaveVal2Var(value, varName);
	}

  public GameObject FindObjectWithPartOfName(string partOfName)
  {
	  return _actionsCatalog3D.FindObjectWithPartOfName(partOfName);
  }

  public void ZoomHandler(string duration = "1.0")
	{
		_actionsCatalog3D.ZoomHandler(duration);
	}

	public void ResetHandler(GameObject obj, Attributes attributes, float duration)
	{
		_actionsCatalog3D.ResetHandler(obj, attributes, duration);
	}
}