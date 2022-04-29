using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Action = Constants.Action;

public class Testing : MonoBehaviour
{
	private List<Action> _actions;
	private int _activeIndex;

	public static bool IsInitialized = false;

	private IEnumerator Sequence(List<IEnumerator> coroutines, float delay = 0.0f)
	{
		yield return new WaitForSeconds(delay);
		foreach (var c in coroutines) yield return StartCoroutine(c);
	}

	private static IEnumerator IAdjustStructure(GameObject objA, GameObject objB, float duration = 0.1f)
	{
		objA.transform.parent = objB.transform;

		float elapsedTime = 0;
		while (elapsedTime <= duration)
		{
			objA.transform.parent = objB.transform;
			elapsedTime += Time.deltaTime;
			yield return null;
		}
	}

	private static IEnumerator IMoveObject(GameObject obj, Vector3 finalPosition, float duration=1.0f, bool isLocal = false)
	{
		if (ScriptExecutor.IsInAction) yield break;
		ScriptExecutor.IsInAction = true;

		float counter = 0;
		while (counter < duration)
		{
			counter += Time.deltaTime;

			if (isLocal)
			{
				obj.transform.localPosition = Vector3.Lerp(obj.transform.position, finalPosition, counter / duration);
			}
			else
			{
				obj.transform.position = Vector3.Lerp(obj.transform.position, finalPosition, counter / duration);
			}
			
			yield return null;
		}

		ScriptExecutor.IsInAction = false;
	}

	private static IEnumerator IRotateObject(GameObject obj, Quaternion finalRotation, float duration=1.0f, bool isLocal = false)
	{
		if (ScriptExecutor.IsInAction) yield break;
		ScriptExecutor.IsInAction = true;

		float counter = 0;
		while (counter < duration)
		{
			counter += Time.deltaTime;

			if (isLocal)
			{
				obj.transform.localRotation = Quaternion.Slerp(obj.transform.rotation, finalRotation, counter / duration);
			}
			else
			{
				obj.transform.rotation = Quaternion.Slerp(obj.transform.rotation, finalRotation, counter / duration);
			}
			
			yield return null;
		}

		ScriptExecutor.IsInAction = false;
	}

	private List<string> CreateHierarchy(GameObject obj)
	{
		var hierarchy = new List<string>();

		
		
		return hierarchy;
	}
	
	private void Start()
	{
		_activeIndex = 0;
		_actions = new List<Action>
		{
			new Action {Name = "attach", Components = new List<string> {"[41]", "[8]"}},
			new Action {Name = "attach", Components = new List<string> {"[45]", "[8]"}},
			new Action {Name = "attach", Components = new List<string> {"[44]", "[46]"}},
			new Action {Name = "attach", Components = new List<string> {"[46]", "[8]"}},
			new Action {Name = "attach", Components = new List<string> {"[43]", "[46]"}},
			new Action {Name = "attach", Components = new List<string> {"[42]", "[46]"}}
		};
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.E))
		{
			Attach(_actions[_activeIndex]);
			_activeIndex += 1;
		}
	}

	private void Attach(Action action)
	{
		var routines = new List<IEnumerator>();

		var figureName = "402-32-11-61-990-802-A";
		var figureRfmName = figureName + "-RFM";
		var figureIfmName = figureName + "-IFM";

		var figure = GameObject.Find(figureName);
		var rfm = GameObject.Find(figureRfmName);
		var ifm = GameObject.Find(figureIfmName);

		var objA = HelperFunctions.FindObjectInFigure(figure, action.Components[0]);
		var objB = HelperFunctions.FindObjectInFigure(figure, action.Components[1]);

		var rfmReferenceObjA = rfm.transform.Find(objA.name).gameObject;
		var rfmReferenceObjB = rfm.transform.Find(objB.name).gameObject;
		var rfmReferenceObjAPosition = rfmReferenceObjA.transform.position;
		var rfmReferenceObjBPosition = rfmReferenceObjB.transform.position;
		var objBRotation = objB.transform.rotation;
		
		var diff = rfmReferenceObjAPosition - rfmReferenceObjBPosition;

		var rfmFinalPosition = objB.transform.TransformPoint(diff);

		var ifmReferenceObjA = ifm.transform.Find(objA.name).gameObject;
		var ifmReferenceObjB = ifm.transform.Find(objB.name).gameObject;
		var ifmReferenceObjAPosition = ifmReferenceObjA.transform.position;
		var ifmReferenceObjBPosition = ifmReferenceObjB.transform.position;
		
		diff = ifmReferenceObjAPosition - ifmReferenceObjBPosition;

		var ifmFinalPosition = objB.transform.TransformPoint(diff);
		
		routines.Add(IRotateObject(objA, objBRotation, 1.0f));
		routines.Add(IMoveObject(objA, rfmFinalPosition, 1.0f));
		routines.Add(IMoveObject(objA, ifmFinalPosition, 1.0f));
		routines.Add(IAdjustStructure(objA, objB));

		StartCoroutine(Sequence(routines, 1.0f));
	}
}