using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActionsCatalog3DInterface
{
	GameObject FindObjectWithPartOfName(string partOfName);
	void ZoomHandler(string duration = "1.0");
	void ResetHandler(string id, string duration);
	void HighlightHandler(string state, string highlightWidthStr, string highlightColorStr);
	void RotateHandler(string degreeStr, string axisStr, string durationStr);
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
		
			coroutines.Add(RotateObject(Context.Instance.Camera, finalRotation, duration));
		}
		coroutines.Add(ChangeFieldOfViewByValue(Context.Instance.Camera, finalFieldOfView, duration));

		StartCoroutine(Sequence(coroutines));
	}

	// rotate
	public void RotateHandler(string degreeStr, string axisStr, string durationStr)
	{
		var degree = float.Parse(degreeStr);
		var duration = float.Parse(durationStr);
		
		Enum.TryParse(axisStr, out State.Axis axis);

		var obj = (GameObject) Context.Instance.Prev;
		var rotation = obj.transform.rotation;
		var rotationX = axis == State.Axis.X ? degree : 0;
		var rotationY = axis == State.Axis.Y ? degree : 0;
		var rotationZ = axis == State.Axis.Z ? degree : 0;
		
		var newRotation = rotation * Quaternion.Euler(rotationX, rotationY,rotationZ);
		StartCoroutine(RotateObject(obj, newRotation, duration));
	}

	// reset
	public void ResetHandler(string id, string durationStr)
	{
		var obj = (GameObject) Context.Instance.Prev;
		var attributes = Context.Instance.InitialAttributes[id];
		var duration = float.Parse(durationStr);

		var cameraAttributes = Context.Instance.InitialAttributes["camera"];
		
		var coroutines = new List<IEnumerator>();
		
		coroutines.Add(Reset(obj, attributes, duration));
		coroutines.Add(Reset(Context.Instance.Camera, cameraAttributes, duration));
    
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

	private void HighlightObject(GameObject obj, string state = State.On, float highlightWidth = 1.0f, Color highlightColor = default)
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
	private static IEnumerator RotateObject(dynamic obj, Quaternion finalRotation, float duration)
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
			obj.transform.rotation = Quaternion.Slerp(obj.transform.rotation, finalRotation, counter/duration);
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