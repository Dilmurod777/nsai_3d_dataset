using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Constants;
using CustomFunctionality;
using UnityEngine;
using UnityEngine.UI;
using Action = Constants.Action;

public class Testing : MonoBehaviour
{
	private List<Action> _actions;
	private int _activeIndex;
	private GameObject _figure, _rfm, _ifm;
	private Robot _robot;

	private IEnumerator Sequence(List<IEnumerator> coroutines, float delay = 0.0f)
	{
		yield return new WaitForSeconds(delay);
		foreach (var c in coroutines) yield return StartCoroutine(c);
	}

	private static IEnumerator DelayCoroutine(float duration, State.VoidFunction method = null)
	{
		yield return new WaitForSeconds(duration);
		method?.Invoke();
	}

	private static IEnumerator ResetObjectCoroutine(GameObject obj, Attributes attributes, float duration)
	{
		if (ScriptExecutor.IsInAction) yield break;
		ScriptExecutor.IsInAction = true;

		var currentRot = obj.transform.rotation;
		var currentScale = obj.transform.localScale;

		var infiniteRotationComponent = obj.GetComponent<InfiniteRotation>();
		if (infiniteRotationComponent != null)
		{
			Destroy(infiniteRotationComponent);
		}

		DestroyCloneObjects();

		foreach (var o in obj.transform.GetComponentsInChildren<Transform>())
		{
			var oTransform = o.transform;
			var meshRenderer = o.GetComponent<MeshRenderer>();
			if (meshRenderer != null)
			{
				o.GetComponent<MeshRenderer>().enabled = true;
			}

			if (Context.Instance.InitialAttributes.ContainsKey(obj.name + GeneralConstants.ArgsSeparator + o.name))
			{
				var attr = Context.Instance.InitialAttributes[obj.name + GeneralConstants.ArgsSeparator + o.name];
				oTransform.rotation = attr.Rotation;
				oTransform.position = attr.Position;
				oTransform.localScale = attr.Scale;
			}

			o.transform.parent = obj.transform;
		}

		float counter = 0;
		while (counter < 1)
		{
			counter += Time.deltaTime / duration;

			obj.transform.rotation = Quaternion.Lerp(currentRot, attributes.Rotation, counter);
			obj.transform.localScale = Vector3.Lerp(currentScale, attributes.Scale, counter);
			obj.transform.position = Vector3.Lerp(obj.transform.position, attributes.Position, counter);

			yield return null;
		}
	}

	private static IEnumerator AdjustStructureCoroutine(GameObject objA, GameObject objB)
	{
		objA.transform.parent = objB.transform;
		yield return null;
	}

	// smoothly move object
	private static IEnumerator MoveObjectCoroutine(GameObject obj, Vector3 finalPosition, float duration)
	{
		if (ScriptExecutor.IsInAction) yield break;
		ScriptExecutor.IsInAction = true;

		float counter = 0;
		var currentPos = obj.transform.position;
		while (counter < 1)
		{
			counter += Time.deltaTime / duration;

			obj.transform.position = Vector3.Lerp(currentPos, finalPosition, counter);
			yield return null;
		}

		ScriptExecutor.IsInAction = false;
	}

	private static IEnumerator MoveObjectWithRotationCoroutine(GameObject obj, Vector3 finalPosition, float duration,
		Vector3 direction = default)
	{
		if (ScriptExecutor.IsInAction) yield break;
		ScriptExecutor.IsInAction = true;

		var infiniteRotationComponents = obj.GetComponents<InfiniteRotation>();
		InfiniteRotation infiniteRotationComponent;
		if (infiniteRotationComponents.Length == 0)
		{
			infiniteRotationComponent = obj.AddComponent<InfiniteRotation>();
		}
		else if (infiniteRotationComponents.Length > 1)
		{
			for (var i = 1; i < infiniteRotationComponents.Length; i++)
			{
				Destroy(infiniteRotationComponents[i]);
			}

			infiniteRotationComponent = infiniteRotationComponents[0];
		}
		else
		{
			infiniteRotationComponent = infiniteRotationComponents[0];
		}


		infiniteRotationComponent.SetSpeed(150);
		infiniteRotationComponent.SetDirection(direction);

		var currentPos = obj.transform.position;
		float counter = 0;
		while (counter < 1)
		{
			counter += Time.deltaTime / duration;
			obj.transform.position = Vector3.Lerp(currentPos, finalPosition, counter);
			yield return null;
		}

		Destroy(infiniteRotationComponent);

		ScriptExecutor.IsInAction = false;

		yield return null;
	}

	// smoothly change rotation of camera
	private static IEnumerator RotateObjectCoroutine(dynamic obj, Quaternion finalRotation, float duration)
	{
		if (ScriptExecutor.IsInAction) yield break;
		ScriptExecutor.IsInAction = true;

		var currentRot = obj.transform.rotation;
		float counter = 0;
		while (counter < 1)
		{
			counter += Time.deltaTime / duration;

			obj.transform.rotation = Quaternion.Slerp(currentRot, finalRotation, counter);
			yield return null;
		}

		ScriptExecutor.IsInAction = false;
	}

	private List<string> CreateHierarchy(GameObject figure)
	{
		var hierarchy = new List<string>();

		var figureName = figure.name;
		foreach (var child in figure.GetComponentsInChildren<Transform>())
		{
			if (child.name == figureName) continue;

			var id = Regex.Match(child.name, @"\d+").Value;

			if (child.parent.name == figureName)
			{
				if (figureName.Contains("-RFM")) figureName = figureName.Replace("-RFM", "");
				if (figureName.Contains("-IFM")) figureName = figureName.Replace("-IFM", "");
				hierarchy.Add(figureName + "#" + id);
			}
			else
			{
				var parentId = Regex.Match(child.parent.name, @"\d+").Value;
				hierarchy.Add(parentId + "#" + id);
			}
		}

		return hierarchy;
	}

	private static void DestroyCloneObjects()
	{
		var cloneObjects = GameObject.FindGameObjectsWithTag("CloneObject");

		foreach (var cloneObj in cloneObjects)
		{
			Destroy(cloneObj);
		}
	}

	private void Start()
	{
		// _activeIndex = 0;
		// _actions = new List<Action>
		// {
		// 	// new Action {Name = "attach", Components = new List<string> {"[41]", "[8]"}},
		// 	// new Action {Name = "attach", Components = new List<string> {"[45]", "[8]"}},
		// 	// new Action {Name = "attach", Components = new List<string> {"[44]", "[46]"}},
		// 	// new Action {Name = "attach", Components = new List<string> {"[46]", "[8]"}},
		// 	// new Action {Name = "attach", Components = new List<string> {"[43]", "[46]"}},
		// 	new Action {Name = "attach", Components = new List<string> {"[42]", "[46]"}}
		// };

		var figureName = "402-32-11-61-990-802-A";
		var figureRfmName = figureName + "-RFM";
		var figureIfmName = figureName + "-IFM";

		_figure = GameObject.Find(figureName);
		_rfm = GameObject.Find(figureRfmName);
		_ifm = GameObject.Find(figureIfmName);

		_robot = new Robot();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.E))
		{
			// Attach(_actions[_activeIndex]);
			// _activeIndex += 1;

			var objA = HelperFunctions.FindObjectInFigure(_figure, "[42] NUT");
			var objB = HelperFunctions.FindObjectInFigure(_figure, "[46] BOLT");

			if (objA != null && objB != null)
			{
				// Performer(SmoothInstall(objA, objB));
				// Performer(StepInstall(objA, objB));
				// Performer(StepScrew(objA, objB));
			}
			else
			{
				Debug.Log("Some of objects were not found.");
			}
		}
	}

	private List<IEnumerator> SmoothInstall(GameObject objA, GameObject objB)
	{
		var primitives = new List<IEnumerator>();

		primitives.Add(AdjustStructureCoroutine(objA, objB));

		// rfm
		var rfmDuration = 1.0f;
		var rfmReferenceObjA = HelperFunctions.FindObjectInFigure(_rfm, objA.name);
		var rfmReferenceObjB = HelperFunctions.FindObjectInFigure(_rfm, objB.name);

		rfmReferenceObjA.transform.parent = rfmReferenceObjB.transform;
		var rfmDiff = rfmReferenceObjA.transform.localPosition;
		var rfmFinalPosition = objB.transform.TransformPoint(rfmDiff);
		rfmReferenceObjA.transform.parent = rfmReferenceObjB.transform.parent;

		var rfmDistance = Vector3.Distance(objA.transform.position, rfmFinalPosition);
		var rfmNewSpeed = rfmDistance / rfmDuration;
		primitives.Add(_robot.SetMoveSpeed(rfmNewSpeed));
		var rfmCount = rfmDuration / (_robot.GetMoveDistance() / rfmNewSpeed);

		for (var i = 0; i < Mathf.CeilToInt(rfmCount); i++)
		{
			primitives.Add(_robot.Move(objA, rfmFinalPosition));
		}

		// delay
		primitives.Add(_robot.Stop(0.5f));

		// ifm
		var ifmDuration = 1.0f;
		var ifmReferenceObjA = HelperFunctions.FindObjectInFigure(_ifm, objA.name);
		var ifmReferenceObjB = HelperFunctions.FindObjectInFigure(_ifm, objB.name);

		ifmReferenceObjA.transform.parent = ifmReferenceObjB.transform;
		var ifmDiff = ifmReferenceObjA.transform.localPosition;
		var ifmFinalPosition = objB.transform.TransformPoint(ifmDiff);
		ifmReferenceObjA.transform.parent = ifmReferenceObjB.transform.parent;

		var ifmDistance = Vector3.Distance(rfmFinalPosition, ifmFinalPosition);
		var ifmNewSpeed = ifmDistance / ifmDuration;
		primitives.Add(_robot.SetMoveSpeed(ifmNewSpeed));
		var ifmCount = Mathf.CeilToInt(ifmDuration / (_robot.GetMoveDistance() / ifmNewSpeed));

		for (var i = 0; i < ifmCount; i++)
		{
			primitives.Add(_robot.Move(objA, ifmFinalPosition));
		}

		return primitives;
	}

	private List<IEnumerator> StepScrew(GameObject objA, GameObject objB)
	{
		var primitives = new List<IEnumerator>();

		primitives.Add(AdjustStructureCoroutine(objA, objB));

		// rfm
		var rfmDuration = 1.0f;
		var rfmReferenceObjA = HelperFunctions.FindObjectInFigure(_rfm, objA.name);
		var rfmReferenceObjB = HelperFunctions.FindObjectInFigure(_rfm, objB.name);

		rfmReferenceObjA.transform.parent = rfmReferenceObjB.transform;
		var rfmDiff = rfmReferenceObjA.transform.localPosition;
		var rfmFinalPosition = objB.transform.TransformPoint(rfmDiff);
		rfmReferenceObjA.transform.parent = rfmReferenceObjB.transform.parent;

		var rfmDistance = Vector3.Distance(objA.transform.position, rfmFinalPosition);
		var rfmNewSpeed = rfmDistance / rfmDuration;
		primitives.Add(_robot.SetMoveSpeed(rfmNewSpeed));
		var rfmCount = rfmDuration / (_robot.GetMoveDistance() / rfmNewSpeed);

		for (var i = 0; i < Mathf.CeilToInt(rfmCount); i++)
		{
			primitives.Add(_robot.Move(objA, rfmFinalPosition));
		}

		// delay
		primitives.Add(_robot.Stop(0.5f));

		// ifm
		var ifmDuration = 1.0f;
		var ifmReferenceObjA = HelperFunctions.FindObjectInFigure(_ifm, objA.name);
		var ifmReferenceObjB = HelperFunctions.FindObjectInFigure(_ifm, objB.name);

		ifmReferenceObjA.transform.parent = ifmReferenceObjB.transform;
		var ifmDiff = ifmReferenceObjA.transform.localPosition;
		var ifmFinalPosition = objB.transform.TransformPoint(ifmDiff);
		ifmReferenceObjA.transform.parent = ifmReferenceObjB.transform.parent;

		var ifmDistance = Vector3.Distance(rfmFinalPosition, ifmFinalPosition);
		var ifmNewSpeed = ifmDistance / ifmDuration;
		primitives.Add(_robot.SetMoveSpeed(ifmNewSpeed));
		var ifmCount = ifmDuration / (_robot.GetMoveDistance() / ifmNewSpeed);

		for (var i = 0; i < Mathf.CeilToInt(ifmCount); i++)
		{
			primitives.Add(_robot.MoveWithRotation(objA, ifmFinalPosition, Vector3.forward));
		}

		return primitives;
	}

	private List<IEnumerator> StepInstall(GameObject objA, GameObject objB)
	{
		var primitives = new List<IEnumerator>();

		primitives.Add(AdjustStructureCoroutine(objA, objB));

		// rfm
		var rfmDuration = 1.0f;
		var rfmReferenceObjA = HelperFunctions.FindObjectInFigure(_rfm, objA.name);
		var rfmReferenceObjB = HelperFunctions.FindObjectInFigure(_rfm, objB.name);

		rfmReferenceObjA.transform.parent = rfmReferenceObjB.transform;
		var rfmDiff = rfmReferenceObjA.transform.localPosition;
		var rfmFinalPosition = objB.transform.TransformPoint(rfmDiff);
		rfmReferenceObjA.transform.parent = rfmReferenceObjB.transform.parent;

		var rfmDistance = Vector3.Distance(objA.transform.position, rfmFinalPosition);
		var rfmNewSpeed = rfmDistance / rfmDuration;
		primitives.Add(_robot.SetMoveSpeed(rfmNewSpeed));
		var rfmCount = rfmDuration / (_robot.GetMoveDistance() / rfmNewSpeed);

		var steps1 = 4;
		var delayDelta1 = Mathf.CeilToInt(rfmCount / steps1);
		var delayCounter1 = 1;
		for (var i = 0; i < Mathf.CeilToInt(rfmCount); i++)
		{
			if (i == delayDelta1 * delayCounter1)
			{
				delayCounter1 += 1;
				primitives.Add(_robot.Stop(0.5f));
			}
			primitives.Add(_robot.Move(objA, rfmFinalPosition));
		}

		// delay
		primitives.Add(_robot.Stop(0.5f));

		// ifm
		var ifmDuration = 1.0f;
		var ifmReferenceObjA = HelperFunctions.FindObjectInFigure(_ifm, objA.name);
		var ifmReferenceObjB = HelperFunctions.FindObjectInFigure(_ifm, objB.name);

		ifmReferenceObjA.transform.parent = ifmReferenceObjB.transform;
		var ifmDiff = ifmReferenceObjA.transform.localPosition;
		var ifmFinalPosition = objB.transform.TransformPoint(ifmDiff);
		ifmReferenceObjA.transform.parent = ifmReferenceObjB.transform.parent;

		var ifmDistance = Vector3.Distance(rfmFinalPosition, ifmFinalPosition);
		var ifmNewSpeed = ifmDistance / ifmDuration;
		primitives.Add(_robot.SetMoveSpeed(ifmNewSpeed));
		var ifmCount = Mathf.CeilToInt(ifmDuration / (_robot.GetMoveDistance() / ifmNewSpeed));

		// var steps = 3;
		// var delayDelta = Mathf.CeilToInt(ifmCount / steps);
		// var delayCounter = 1;
		for (var i = 0; i < ifmCount; i++)
		{
			// if (i == delayDelta * delayCounter)
			// {
			// 	delayCounter += 1;
			// 	primitives.Add(_robot.Stop(0.5f));
			// }
			
			primitives.Add(_robot.Move(objA, ifmFinalPosition));
		}

		return primitives;
	}

	private void Performer(List<IEnumerator> primitives)
	{
		StartCoroutine(Sequence(primitives));
	}
}