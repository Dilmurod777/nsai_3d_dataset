﻿using UnityEngine;

public class ControlsExecutor: MonoBehaviour
{
	public void LeftButtonClickHandler()
	{
		ScriptExecutor.PreviousInstruction();
	}

	public void RightButtonClickHandler()
	{
		ScriptExecutor.NextInstruction();
	}

	public void ExecuteClickHandler()
	{
		ScriptExecutor.ExecuteQuery();
	}

	public void Switch2Figure(string figureName)
	{
		// disable old figure
		GameObject.Find(Context.Instance.CurrentFigureID + "-Wrapper").transform.localScale = Vector3.zero;
		
		// enable new figure
		Context.Instance.CurrentFigureID = figureName;
		GameObject.Find(figureName+ "-Wrapper").transform.localScale = Vector3.one;
		
		ScriptExecutor.InitProgram();
	}
}