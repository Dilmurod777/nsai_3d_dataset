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
}