using System.Collections;
using UnityEngine;

public interface IActionsCatalog3DInterface
{
	GameObject FindObjectWithPartOfName(string partOfName);
	void ZoomHandler(string duration = "1.0");
	void ResetHandler(GameObject obj, Attributes attributes, float duration);
}

public class ActionsCatalog3D : MonoBehaviour, IActionsCatalog3DInterface
{
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
		
		if (!Physics.Raycast(Context.Instance.Camera.transform.position, Context.Instance.Camera.transform.TransformDirection(Vector3.forward), Mathf.Infinity))
		{
			var finalRotation = Quaternion.LookRotation(((GameObject) Context.Instance.Prev).transform.position - cameraTransform.position);
		
			StartCoroutine(ChangeRotation(Context.Instance.Camera, finalRotation, duration));
		}
		StartCoroutine(ChangeFieldOfViewByValue(Context.Instance.Camera, finalFieldOfView, duration));
	}

	// reset
	public void ResetHandler(GameObject obj, Attributes attributes, float duration)
	{
		StartCoroutine(Reset(obj, attributes, duration));
	}

	private static IEnumerator Reset(GameObject obj, Attributes attributes, float duration)
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

			obj.transform.rotation = Quaternion.Lerp(currentRot, attributes.rotation, counter / duration);
			obj.transform.localScale = Vector3.Lerp(obj.transform.localScale, attributes.scale, counter / duration);
            
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