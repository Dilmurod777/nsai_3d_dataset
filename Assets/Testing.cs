using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Action = Constants.Action;

public class Testing : MonoBehaviour
{
	private List<Action> _actions;
	private int _activeIndex;
	private GameObject _figure, _rfm, _ifm;
	private List<string> ifmHierarchy;
	private bool _isTheSame;
	
	private Image _isCompleteIndicatorPanel;
	public static bool IsInitialized = false;

	
	public Testing(List<string> ifmHierarchy)
	{
		this.ifmHierarchy = ifmHierarchy;
	}

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

	private IEnumerator CheckIsFigureComplete()
	{
		var figureHierarchy = CreateHierarchy(_figure);

		_isTheSame = true;
		for (var i = 0; i < ifmHierarchy.Count; i++)
		{
			if (figureHierarchy.Contains(ifmHierarchy[i])) continue;
			_isTheSame = false;
			break;
		}

		if (_isTheSame)
		{
			_isCompleteIndicatorPanel.color = Color.green;
		}else
		{
			_isCompleteIndicatorPanel.color = Color.red;
		}
		
		yield return null;
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
		
		var figureName = "402-32-11-61-990-802-A";
		var figureRfmName = figureName + "-RFM";
		var figureIfmName = figureName + "-IFM";

		_figure = GameObject.Find(figureName);
		_rfm = GameObject.Find(figureRfmName);
		_ifm = GameObject.Find(figureIfmName);

		ifmHierarchy = CreateHierarchy(_ifm);

		_isCompleteIndicatorPanel = GameObject.Find("IsCompleteIndicator").GetComponent<Image>();
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
		_isCompleteIndicatorPanel.color = Color.gray;
		
		var routines = new List<IEnumerator>();

		var objA = HelperFunctions.FindObjectInFigure(_figure, action.Components[0]);
		var objB = HelperFunctions.FindObjectInFigure(_figure, action.Components[1]);

		var rfmReferenceObjA = HelperFunctions.FindObjectInFigure(_rfm, objA.name);
		var rfmReferenceObjB = HelperFunctions.FindObjectInFigure(_rfm, objB.name);
		var rfmReferenceObjAPosition = rfmReferenceObjA.transform.position;
		var rfmReferenceObjBPosition = rfmReferenceObjB.transform.position;
		var objBRotation = objB.transform.rotation;
		
		var diff = rfmReferenceObjAPosition - rfmReferenceObjBPosition;

		var rfmFinalPosition = objB.transform.TransformPoint(diff);

		var ifmReferenceObjA = HelperFunctions.FindObjectInFigure(_ifm, objA.name);
		var ifmReferenceObjB = HelperFunctions.FindObjectInFigure(_ifm, objB.name);
		var ifmReferenceObjAPosition = ifmReferenceObjA.transform.position;
		var ifmReferenceObjBPosition = ifmReferenceObjB.transform.position;
		
		diff = ifmReferenceObjAPosition - ifmReferenceObjBPosition;

		var ifmFinalPosition = objB.transform.TransformPoint(diff);
		
		routines.Add(IRotateObject(objA, objBRotation, 1.0f));
		routines.Add(IMoveObject(objA, rfmFinalPosition, 1.0f));
		routines.Add(IMoveObject(objA, ifmFinalPosition, 1.0f));
		routines.Add(IAdjustStructure(objA, objB));
		routines.Add(CheckIsFigureComplete());

		StartCoroutine(Sequence(routines, 1.0f));
	}
}