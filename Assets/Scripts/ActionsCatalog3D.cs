using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IActionsCatalog3DInterface
{
	GameObject FindObjectWithPartOfName(string partOfName);
	void ZoomHandler(string duration = "1.0");
	void ResetHandler(string id, string duration);
	void HighlightHandler(string state, string highlightWidthStr, string highlightColorStr);
}

public class ActionsCatalog3D : MonoBehaviour, IActionsCatalog3DInterface
{
	private IEnumerator Sequence(List<IEnumerator> coroutines)
	{
		foreach (var c in coroutines)
		{
			yield return StartCoroutine(c);
		}
	}
	
	// find object with part of name
	public GameObject FindObjectWithPartOfName(string partOfName)
	{
		var allObjects = FindObjectsOfType<GameObject>();

		foreach (var obj in allObjects)
		{
			if (obj.name.Contains($"[{partOfName}]"))
			{
				return obj;
			}
		}

		return null;
	}
	
	// close look
	public void ZoomHandler(string durationStr = "1.0")
	{
		const float changeFieldOfView = -5;
		var duration = float.Parse(durationStr);
		
		var finalFieldOfView = Context.Instance.Camera.fieldOfView + changeFieldOfView;
		var cameraTransform = Context.Instance.Camera.transform;

		var coroutines = new List<IEnumerator>();

		if (!Physics.Raycast(Context.Instance.Camera.transform.position, Context.Instance.Camera.transform.TransformDirection(Vector3.forward), Mathf.Infinity))
		{
			var finalRotation = Quaternion.LookRotation(((GameObject) Context.Instance.Prev).transform.position - cameraTransform.position);
		
			coroutines.Add(ChangeRotation(Context.Instance.Camera, finalRotation, duration));
		}
		coroutines.Add(ChangeFieldOfViewByValue(Context.Instance.Camera, finalFieldOfView, duration));

		StartCoroutine(Sequence(coroutines));
	}

	// reset
	public void ResetHandler(string id, string durationStr)
	{
		var obj = (GameObject) Context.Instance.Prev;
		var attributes = Context.Instance.InitialAttributes[id];
		var duration = float.Parse(durationStr);

		var cameraAttributes = Context.Instance.InitialAttributes["camera"];
		
		var coroutines = new List<IEnumerator>();
		
		coroutines.Add(Reset(Context.Instance.Camera, cameraAttributes, duration));
		coroutines.Add(Reset(obj, attributes, duration));
    
    if (cameraAttributes.FoV != 0)
    {
	    coroutines.Add(ChangeFieldOfViewByValue(Context.Instance.Camera, cameraAttributes.FoV, duration));
    }

    StartCoroutine(Sequence(coroutines));
	}

	// highlight
	public void HighlightHandler(string state, string highlightWidthStr, string highlightColorStr)
	{
		var highlightWidth = float.Parse(highlightWidthStr);
		var highlightColor = HelperFunctions.ConvertStringToColor(highlightColorStr);
		
		HighlightObject((GameObject) Context.Instance.Prev, state, highlightWidth, highlightColor);
	}

	private void HighlightObject(GameObject obj, string state, float highlightWidth, Color highlightColor)
	{
		var outlineComponent = obj.GetComponent<Outline>();
		if (outlineComponent == null)
		{
			outlineComponent = obj.AddComponent<Outline>();
		}
		
		switch (state)
		{
			case State.On:
				outlineComponent.OutlineMode = Outline.Mode.OutlineAll;
				outlineComponent.OutlineWidth = highlightWidth;
				outlineComponent.OutlineColor = highlightColor;

				outlineComponent.enabled = true;
				break;
			case State.Off:
				outlineComponent.enabled = false;
				break;
		}
	}

	
	private static IEnumerator Reset(dynamic obj, Attributes attributes, float duration)
	{
		if (ScriptExecutor.IsInAction)
		{
			yield break;
		}
		ScriptExecutor.IsInAction = true;
		
		var currentRot = obj.transform.rotation;
        
		float counter = 0;
		while (counter < duration)
		{
			counter += Time.deltaTime;

			obj.transform.rotation = Quaternion.Lerp(currentRot, attributes.Rotation, counter / duration);
			obj.transform.localScale = Vector3.Lerp(obj.transform.localScale, attributes.Scale, counter / duration);
			obj.transform.position = Vector3.Lerp(obj.transform.position, attributes.Position, counter / duration);

			yield return null;
		}
		
		ScriptExecutor.IsInAction = false;
	}
	
	// smoothly change rotation of camera
	private static IEnumerator ChangeRotation(Component camera, Quaternion finalRotation, float duration)
	{
		if (ScriptExecutor.IsInAction)
		{
			yield break;
		}
		ScriptExecutor.IsInAction = true;
		
		float counter = 0;
		while (counter < duration)
		{
			counter += Time.deltaTime;

			yield return null;
			camera.transform.rotation = Quaternion.Slerp(camera.transform.rotation, finalRotation, Time.deltaTime);
		}
		
		ScriptExecutor.IsInAction = false;
	}
	
	// smoothly change FoV of camera
	private static IEnumerator ChangeFieldOfViewByValue(Camera camera, float finalFoV, float duration)
	{
		if (ScriptExecutor.IsInAction)
		{
			yield break;
		}

		ScriptExecutor.IsInAction = true;
		
		float counter = 0;
		while (counter < duration)
		{
			counter += Time.deltaTime;
            
			camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, finalFoV, counter / duration);
			yield return null;
		}

		ScriptExecutor.IsInAction = false;
	}
}