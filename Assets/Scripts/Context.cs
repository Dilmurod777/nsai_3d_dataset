using UnityEngine;

public class Context
{
	public Camera Camera;
	
	public string Query;
	public string[] Programs;
	public string CurrentTaskID;
	public string CurrentSubtaskID;
	public string CurrentInstructionOrder;
	public dynamic Prev;
	public object Var1;
	public object Var2;
	public object Root;
	
	private Context() {}  
	private static Context _instance = null;  
	public static Context Instance => _instance ??= new Context();
}
