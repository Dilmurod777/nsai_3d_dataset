using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Constants;
using UnityEngine;
using Action = Constants.Action;

public class CoroutineWithResult
{
	public Coroutine coroutine;
	public object result;
	private IEnumerator target;

	public CoroutineWithResult(MonoBehaviour owner, IEnumerator t)
	{
		target = t;
		coroutine = owner.StartCoroutine(Run());
	}

	private IEnumerator Run()
	{
		while (target.MoveNext())
		{
			result = target.Current;
			yield return result;
		}
	}
}

public class Testing : MonoBehaviour
{
	private List<Action> _actions;
	private int _activeIndex;
	private GameObject _figure, _rfm, _ifm;
	private Robot _robot;

	private static IEnumerator AdjustStructureCoroutine(GameObject objA, GameObject objB)
	{
		objA.transform.parent = objB.transform;
		yield return null;
	}

	private IEnumerator Sequence(List<IEnumerator> coroutines, float delay = 0.0f)
	{
		var responses = new List<TestingResponse>();

		yield return new WaitForSeconds(delay);
		foreach (var c in coroutines)
		{
			var coroutineWithResult = new CoroutineWithResult(this, c);
			yield return coroutineWithResult.coroutine;

			if (coroutineWithResult.result is TestingResponse response)
			{
				responses.Add(response);
			}
		}

		yield return responses;
	}

	public void Performer(List<IEnumerator> primitives)
	{
		IEnumerator InnerPerformer(List<IEnumerator> p, System.Action<List<TestingResponse>> callback = null)
		{
			var coroutineWithResult = new CoroutineWithResult(this, Sequence(p));
			yield return coroutineWithResult.coroutine;

			callback?.Invoke(coroutineWithResult.result as List<TestingResponse>);
		}

		StartCoroutine(InnerPerformer(primitives, PrintPrimitiveResponses));
		// StartCoroutine(InnerPerformer(primitives));
	}

	private void PrintPrimitiveResponses(List<TestingResponse> responses)
	{
		const string path = "Assets/Resources/output.txt";
		var writer = new StreamWriter(path);
		var output = "";

		foreach (var response in responses)
		{
			output += response.Type.ToString();

			if (response.ObjectName != default)
			{
				output += " object " + response.ObjectName;
			}

			if (response.InitialPosition != default)
			{
				output += " from " + response.InitialPosition;
			}

			if (response.InitialRotation != default)
			{
				output += " from " + response.InitialRotation;
			}

			if (response.FinalPosition != default)
			{
				output += " to " + response.FinalPosition;
			}

			if (response.FinalRotation != default)
			{
				output += " to " + response.FinalRotation;
			}

			if (response.CurrentPosition != default)
			{
				output += ". Current position is " + response.CurrentPosition;
			}

			if (response.CurrentRotation != default)
			{
				output += ". Current rotation is " + response.CurrentRotation;
			}

			if (response.PreviousMoveSpeed != default)
			{
				output += " to " + response.PreviousMoveSpeed;
			}

			if (response.CurrentMoveSpeed != default)
			{
				output += " | " + response.CurrentMoveSpeed;
			}

			output += "\n";
		}

		writer.Write(output);
		writer.Close();
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
			new Action {Name = "SmoothInstall", Components = new List<string> {"[41]", "[8]"}},
			new Action {Name = "StepInstall", Components = new List<string> {"[45]", "[8]"}},
			new Action {Name = "SmoothInstall", Components = new List<string> {"[44]", "[46]"}},
			new Action {Name = "SmoothScrew", Components = new List<string> {"[46]", "[8]"}},
			new Action {Name = "SmoothInstall", Components = new List<string> {"[43]", "[46]"}},
			new Action {Name = "StepScrew", Components = new List<string> {"[42]", "[46]"}}
		};

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
			var action = _actions[_activeIndex];
			_activeIndex += 1;

			var objA = HelperFunctions.FindObjectInFigure(_figure, action.Components[0]);
			var objB = HelperFunctions.FindObjectInFigure(_figure, action.Components[1]);

			if (objA != null && objB != null)
			{
				switch (action.Name)
				{
					case "SmoothInstall":
						Performer(SmoothInstall(objA, objB));
						break;
					case "StepInstall":
						Performer(StepInstall(objA, objB));
						break;
					case "SmoothScrew":
						Performer(SmoothScrew(objA, objB));
						break;
					case "StepScrew":
						Performer(StepScrew(objA, objB));
						break;
					default:
						Performer(SmoothInstall(objA, objB));
						break;
				}
			}
			else
			{
				Debug.Log("Some of objects were not found.");
			}
		}
	}
	
	private List<IEnumerator> CreateRotatePrimitives(GameObject objA, GameObject objB)
	{
		var primitives = new List<IEnumerator>();

		var initialRotation = objA.transform.eulerAngles;
		var finalRotation = objB.transform.eulerAngles;
		var angle = Quaternion.Angle(objA.transform.rotation, objB.transform.rotation);
		
		var duration = _robot.GetRotateDuration();
		var count = duration / Time.fixedDeltaTime;
		
		// primitives.Add(_robot.SetRotateDegree(angle / count));
		// for (var i = 0; i < Mathf.CeilToInt(count); i++)
		// {
		// 	primitives.Add(_robot.Rotate(objA, finalRotation));
		// }
		
		// var initialRotationX = initialRotation.x > 180 ? initialRotation.x - 360 : initialRotation.x;
		// var initialRotationY = initialRotation.y > 180 ? initialRotation.y - 360 : initialRotation.y;
		// var initialRotationZ = initialRotation.z > 180 ? initialRotation.z - 360 : initialRotation.z;
		// initialRotation = new Vector3(initialRotationX, initialRotationY, initialRotationZ);
		//
		// var finalRotationX = finalRotation.x > 180 ? finalRotation.x - 360 : finalRotation.x;
		// var finalRotationY = finalRotation.y > 180 ? finalRotation.y - 360 : finalRotation.y;
		// var finalRotationZ = finalRotation.z > 180 ? finalRotation.z - 360 : finalRotation.z;
		// finalRotation = new Vector3(finalRotationX, finalRotationY, finalRotationZ);
		//
		// var rotationX = new Vector3(finalRotation.x, initialRotation.y, initialRotation.z);
		// var rotationY = new Vector3(finalRotation.x, finalRotation.y, initialRotation.z);
		//
		// var xAngle = Quaternion.Angle(Quaternion.Euler(initialRotation), Quaternion.Euler(rotationX));
		// var yAngle = Quaternion.Angle(Quaternion.Euler(rotationX), Quaternion.Euler(rotationY));
		// var zAngle = Quaternion.Angle(Quaternion.Euler(rotationY), Quaternion.Euler(finalRotation));
		//
		// var duration = _robot.GetRotateDuration();
		// var count = duration / Time.fixedDeltaTime;

		primitives.Add(_robot.SetRotateDegree(angle / count));
		for (var i = 0; i < Mathf.CeilToInt(count); i++)
		{
			primitives.Add(_robot.Rotate(objA, finalRotation, Vector3.right));
		}
		
		primitives.Add(_robot.SetRotateDegree(angle / count));
		for (var i = 0; i < Mathf.CeilToInt(count); i++)
		{
			primitives.Add(_robot.Rotate(objA, finalRotation, Vector3.up));
		}
		
		primitives.Add(_robot.SetRotateDegree(angle / count));
		for (var i = 0; i < Mathf.CeilToInt(count); i++)
		{
			primitives.Add(_robot.Rotate(objA, finalRotation, Vector3.forward));
		}

		return primitives;
	}

	private List<IEnumerator> CreateRfmMovePrimitives(GameObject objA, GameObject objB)
	{
		var primitives = new List<IEnumerator>();

		var rfmReferenceObjA = HelperFunctions.FindObjectInFigure(_rfm, objA.name);
		var rfmReferenceObjB = HelperFunctions.FindObjectInFigure(_rfm, objB.name);

		rfmReferenceObjA.transform.parent = rfmReferenceObjB.transform;
		var rfmDiff = rfmReferenceObjA.transform.localPosition;
		var rfmFinalPosition = objB.transform.TransformPoint(rfmDiff);
		rfmReferenceObjA.transform.parent = rfmReferenceObjB.transform.parent;

		var duration = _robot.GetMoveDuration();
		var rfmDistance = Vector3.Distance(objA.transform.position, rfmFinalPosition);
		var rfmCount = duration / Time.fixedDeltaTime;

		primitives.Add(_robot.SetMoveDistance(rfmDistance / rfmCount));
		for (var i = 0; i < Mathf.CeilToInt(rfmCount); i++)
		{
			primitives.Add(_robot.Move(objA, rfmFinalPosition));
		}

		return primitives;
	}

	private List<IEnumerator> SmoothInstall(GameObject objA, GameObject objB)
	{
		var primitives = new List<IEnumerator>();

		primitives.AddRange(CreateRotatePrimitives(objA, objB));
		primitives.Add(AdjustStructureCoroutine(objA, objB));

		primitives.AddRange(CreateRfmMovePrimitives(objA, objB));

		// delay
		primitives.Add(_robot.Stop(0.5f));

		// ifm
		var ifmReferenceObjA = HelperFunctions.FindObjectInFigure(_ifm, objA.name);
		var ifmReferenceObjB = HelperFunctions.FindObjectInFigure(_ifm, objB.name);
		var rfmReferenceObjA = HelperFunctions.FindObjectInFigure(_rfm, objA.name);
		var rfmReferenceObjB = HelperFunctions.FindObjectInFigure(_rfm, objB.name);

		rfmReferenceObjA.transform.parent = rfmReferenceObjB.transform;
		var rfmDiff = rfmReferenceObjA.transform.localPosition;
		var rfmFinalPosition = objB.transform.TransformPoint(rfmDiff);

		var prevParent = ifmReferenceObjA.transform.parent;
		ifmReferenceObjA.transform.parent = ifmReferenceObjB.transform;
		var ifmDiff = ifmReferenceObjA.transform.localPosition;
		var ifmFinalPosition = objB.transform.TransformPoint(ifmDiff);
		ifmReferenceObjA.transform.parent = prevParent;

		var duration = _robot.GetMoveDuration();
		var ifmDistance = Vector3.Distance(rfmFinalPosition, ifmFinalPosition);
		var ifmCount = duration / Time.fixedDeltaTime;

		primitives.Add(_robot.SetMoveDistance(ifmDistance / ifmCount));
		for (var i = 0; i < ifmCount; i++)
		{
			primitives.Add(_robot.Move(objA, ifmFinalPosition));
		}

		return primitives;
	}

	private List<IEnumerator> SmoothScrew(GameObject objA, GameObject objB)
	{
		var primitives = new List<IEnumerator>();

		primitives.AddRange(CreateRotatePrimitives(objA, objB));
		primitives.Add(AdjustStructureCoroutine(objA, objB));

		primitives.AddRange(CreateRfmMovePrimitives(objA, objB));

		// delay
		primitives.Add(_robot.Stop(0.5f));

		// ifm
		var ifmDuration = _robot.GetMoveDuration();
		var ifmReferenceObjA = HelperFunctions.FindObjectInFigure(_ifm, objA.name);
		var ifmReferenceObjB = HelperFunctions.FindObjectInFigure(_ifm, objB.name);
		var rfmReferenceObjA = HelperFunctions.FindObjectInFigure(_rfm, objA.name);
		var rfmReferenceObjB = HelperFunctions.FindObjectInFigure(_rfm, objB.name);

		rfmReferenceObjA.transform.parent = rfmReferenceObjB.transform;
		var rfmDiff = rfmReferenceObjA.transform.localPosition;
		var rfmFinalPosition = objB.transform.TransformPoint(rfmDiff);

		var prevParent = ifmReferenceObjA.transform.parent;
		ifmReferenceObjA.transform.parent = ifmReferenceObjB.transform;
		var ifmDiff = ifmReferenceObjA.transform.localPosition;
		var ifmFinalPosition = objB.transform.TransformPoint(ifmDiff);
		ifmReferenceObjA.transform.parent = prevParent;

		var ifmDistance = Vector3.Distance(rfmFinalPosition, ifmFinalPosition);
		var ifmCount = ifmDuration / Time.fixedDeltaTime;

		primitives.Add(_robot.SetMoveDistance(ifmDistance / ifmCount));
		for (var i = 0; i < ifmCount; i++)
		{
			primitives.Add(_robot.MoveWithRotation(objA, ifmFinalPosition, Vector3.forward));
		}

		return primitives;
	}

	private List<IEnumerator> StepInstall(GameObject objA, GameObject objB, int steps = 3)
	{
		var primitives = new List<IEnumerator>();

		primitives.AddRange(CreateRotatePrimitives(objA, objB));
		primitives.Add(AdjustStructureCoroutine(objA, objB));

		primitives.AddRange(CreateRfmMovePrimitives(objA, objB));

		// delay
		primitives.Add(_robot.Stop(0.5f));

		// ifm
		var ifmDuration = _robot.GetMoveDuration();
		var ifmReferenceObjA = HelperFunctions.FindObjectInFigure(_ifm, objA.name);
		var ifmReferenceObjB = HelperFunctions.FindObjectInFigure(_ifm, objB.name);
		var rfmReferenceObjA = HelperFunctions.FindObjectInFigure(_rfm, objA.name);
		var rfmReferenceObjB = HelperFunctions.FindObjectInFigure(_rfm, objB.name);

		rfmReferenceObjA.transform.parent = rfmReferenceObjB.transform;
		var rfmDiff = rfmReferenceObjA.transform.localPosition;
		var rfmFinalPosition = objB.transform.TransformPoint(rfmDiff);

		var prevParent = ifmReferenceObjA.transform.parent;
		ifmReferenceObjA.transform.parent = ifmReferenceObjB.transform;
		var ifmDiff = ifmReferenceObjA.transform.localPosition;
		var ifmFinalPosition = objB.transform.TransformPoint(ifmDiff);
		ifmReferenceObjA.transform.parent = prevParent;

		var ifmDistance = Vector3.Distance(rfmFinalPosition, ifmFinalPosition);
		var ifmCount = ifmDuration / Time.fixedDeltaTime;

		primitives.Add(_robot.SetMoveDistance(ifmDistance / ifmCount));

		var delayDelta = Mathf.CeilToInt(ifmCount / steps);
		var delayCounter = 1;
		for (var i = 0; i < ifmCount; i++)
		{
			if (i == delayDelta * delayCounter)
			{
				delayCounter += 1;
				primitives.Add(_robot.Stop(0.5f));
			}

			primitives.Add(_robot.Move(objA, ifmFinalPosition));
		}

		return primitives;
	}

	private List<IEnumerator> StepScrew(GameObject objA, GameObject objB, int steps = 3)
	{
		var primitives = new List<IEnumerator>();

		primitives.AddRange(CreateRotatePrimitives(objA, objB));
		primitives.Add(AdjustStructureCoroutine(objA, objB));

		primitives.AddRange(CreateRfmMovePrimitives(objA, objB));

		// delay
		primitives.Add(_robot.Stop(0.5f));

		// ifm
		var ifmDuration = _robot.GetMoveDuration();
		var ifmReferenceObjA = HelperFunctions.FindObjectInFigure(_ifm, objA.name);
		var ifmReferenceObjB = HelperFunctions.FindObjectInFigure(_ifm, objB.name);
		var rfmReferenceObjA = HelperFunctions.FindObjectInFigure(_rfm, objA.name);
		var rfmReferenceObjB = HelperFunctions.FindObjectInFigure(_rfm, objB.name);

		rfmReferenceObjA.transform.parent = rfmReferenceObjB.transform;
		var rfmDiff = rfmReferenceObjA.transform.localPosition;
		var rfmFinalPosition = objB.transform.TransformPoint(rfmDiff);

		var prevParent = ifmReferenceObjA.transform.parent;
		ifmReferenceObjA.transform.parent = ifmReferenceObjB.transform;
		var ifmDiff = ifmReferenceObjA.transform.localPosition;
		var ifmFinalPosition = objB.transform.TransformPoint(ifmDiff);
		ifmReferenceObjA.transform.parent = prevParent;

		var ifmDistance = Vector3.Distance(rfmFinalPosition, ifmFinalPosition);
		var ifmCount = ifmDuration / Time.fixedDeltaTime;

		primitives.Add(_robot.SetMoveDistance(ifmDistance / ifmCount));

		var delayDelta = Mathf.CeilToInt(ifmCount / steps);
		var delayCounter = 1;
		for (var i = 0; i < Mathf.CeilToInt(ifmCount); i++)
		{
			if (i == delayDelta * delayCounter)
			{
				delayCounter += 1;
				primitives.Add(_robot.Stop(0.5f));
			}

			primitives.Add(_robot.MoveWithRotation(objA, ifmFinalPosition, Vector3.forward));
		}

		return primitives;
	}
}