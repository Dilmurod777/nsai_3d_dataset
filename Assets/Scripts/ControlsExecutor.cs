using UnityEngine;

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
}