using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Context
{
	public Camera Camera;
	public readonly Dictionary<string, Attributes> InitialAttributes = new Dictionary<string, Attributes>();
	
	public string Query;
	public string[] Programs;
	public string CurrentTaskID;
	public string CurrentSubtaskID;
	public string CurrentInstructionOrder;
	public string CurrentFigureID;
	public dynamic Prev;
	public object Var1;
	public object Var2;
	public object Root;
	
	private Context() {}  
	private static Context _instance;  
	public static Context Instance => _instance ??= new Context();

	public static dynamic GetAttribute(string name)
	{
		switch (name)
		{
			case "query":
				return Instance.Query;
			case "programs":
				return Instance.Programs;
			case "var1":
				return Instance.Var1;
			case "var2":
				return Instance.Var2;
			case "prev":
				return Instance.Prev;
			case "root":
				return Instance.Root;
			default:
				return null;
		}
	}

	public static bool HasAttribute(string name)
	{
		var attributes = new[] {"var1", "var2", "prev", "root", "query", "programs"};

		return attributes.Contains(name);
	}
}
