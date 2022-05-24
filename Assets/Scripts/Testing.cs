using System.Collections;
using System.Collections.Generic;
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

	private static IEnumerator AdjustStructureCoroutine(GameObject objA, GameObject objB)
	{
		objA.transform.parent = objB.transform;
		yield return null;
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

		var rotationDuration = 1.0f;
		var initialRotation = objA.transform.rotation.eulerAngles;
		var finalRotation = objB.transform.rotation.eulerAngles;

		var rotationX = new Vector3(finalRotation.x, initialRotation.y, initialRotation.z);
		var rotationY = new Vector3(finalRotation.x, finalRotation.y, initialRotation.z);
		var rotationZ = new Vector3(finalRotation.x, finalRotation.y, finalRotation.z);

		var xAngle = Quaternion.Angle(Quaternion.Euler(initialRotation), Quaternion.Euler(rotationX));
		var yAngle = Quaternion.Angle(Quaternion.Euler(rotationX), Quaternion.Euler(rotationY));
		var zAngle = Quaternion.Angle(Quaternion.Euler(rotationY), Quaternion.Euler(rotationZ));
		var xCount = xAngle / _robot.GetRotateDegree();
		var yCount = yAngle / _robot.GetRotateDegree();
		var zCount = zAngle / _robot.GetRotateDegree();

		primitives.Add(_robot.SetRotateSpeed(xAngle / rotationDuration));
		for (var i = 0; i < Mathf.CeilToInt(xCount); i++)
		{
			primitives.Add(_robot.Rotate(objA, rotationX, Vector3.right));
		}

		primitives.Add(_robot.SetRotateSpeed(yAngle / rotationDuration));
		for (var i = 0; i < Mathf.CeilToInt(yCount); i++)
		{
			primitives.Add(_robot.Rotate(objA, rotationY, Vector3.up));
		}

		primitives.Add(_robot.SetRotateSpeed(zAngle / rotationDuration));
		for (var i = 0; i < Mathf.CeilToInt(zCount); i++)
		{
			primitives.Add(_robot.Rotate(objA, rotationZ, Vector3.forward));
		}

		return primitives;
	}

	private List<IEnumerator> CreateRfmMovePrimitives(GameObject objA, GameObject objB)
	{
		var primitives = new List<IEnumerator>();

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

		return primitives;
	}

	private List<IEnumerator> CreateIfmMovePrimitives(GameObject objA, GameObject objB)
	{
		var primitives = new List<IEnumerator>();
		// ifm
		var ifmDuration = 1.0f;
		var ifmReferenceObjA = HelperFunctions.FindObjectInFigure(_ifm, objA.name);
		var ifmReferenceObjB = HelperFunctions.FindObjectInFigure(_ifm, objB.name);
		var rfmReferenceObjA = HelperFunctions.FindObjectInFigure(_rfm, objA.name);
		var rfmReferenceObjB = HelperFunctions.FindObjectInFigure(_rfm, objB.name);
		
		rfmReferenceObjA.transform.parent = rfmReferenceObjB.transform;
		var rfmDiff = rfmReferenceObjA.transform.localPosition;
		var rfmFinalPosition = objB.transform.TransformPoint(rfmDiff);
		
		// ifmReferenceObjA.transform.parent = ifmReferenceObjB.transform;
		var ifmDiff = ifmReferenceObjA.transform.localPosition;
		var ifmFinalPosition = objB.transform.TransformPoint(ifmDiff);
		// ifmReferenceObjA.transform.parent = ifmReferenceObjB.transform.parent;
	
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

	private List<IEnumerator> CreateIfmMoveWithRotationPrimitives(GameObject objA, GameObject objB)
	{
		var primitives = new List<IEnumerator>();
		// ifm
		var ifmDuration = 1.0f;
		var ifmReferenceObjA = HelperFunctions.FindObjectInFigure(_ifm, objA.name);
		var ifmReferenceObjB = HelperFunctions.FindObjectInFigure(_ifm, objB.name);
		var rfmReferenceObjA = HelperFunctions.FindObjectInFigure(_rfm, objA.name);
		var rfmReferenceObjB = HelperFunctions.FindObjectInFigure(_rfm, objB.name);
		
		rfmReferenceObjA.transform.parent = rfmReferenceObjB.transform;
		var rfmDiff = rfmReferenceObjA.transform.localPosition;
		var rfmFinalPosition = objB.transform.TransformPoint(rfmDiff);
		
		// ifmReferenceObjA.transform.parent = ifmReferenceObjB.transform;
		var ifmDiff = ifmReferenceObjA.transform.localPosition;
		var ifmFinalPosition = objB.transform.TransformPoint(ifmDiff);
		// ifmReferenceObjA.transform.parent = ifmReferenceObjB.transform.parent;

		var ifmDistance = Vector3.Distance(rfmFinalPosition, ifmFinalPosition);
		var ifmNewSpeed = ifmDistance / ifmDuration;
		primitives.Add(_robot.SetMoveSpeed(ifmNewSpeed));
		var ifmCount = Mathf.CeilToInt(ifmDuration / (_robot.GetMoveDistance() / ifmNewSpeed));

		for (var i = 0; i < ifmCount; i++)
		{
			primitives.Add(_robot.MoveWithRotation(objA, ifmFinalPosition, Vector3.forward));
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

		primitives.AddRange(CreateIfmMovePrimitives(objA, objB));

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

		primitives.AddRange(CreateIfmMoveWithRotationPrimitives(objA, objB));

		return primitives;
	}

	private List<IEnumerator> StepInstall(GameObject objA, GameObject objB, int steps = 3)
	{
		var primitives = new List<IEnumerator>();

		primitives.Add(AdjustStructureCoroutine(objA, objB));
		primitives.AddRange(CreateRotatePrimitives(objA, objB));

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

		primitives.Add(AdjustStructureCoroutine(objA, objB));
		primitives.AddRange(CreateRotatePrimitives(objA, objB));

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
		var rfmCount = rfmDistance / _robot.GetMoveDistance();

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
		var ifmCount = ifmDistance / _robot.GetMoveDistance();

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

	private void Performer(List<IEnumerator> primitives)
	{
		IEnumerator InnerPerformer(List<IEnumerator> primitivies, System.Action<List<TestingResponse>> callback)
		{
			var coroutineWithResult = new CoroutineWithResult(this, Sequence(primitives));
			yield return coroutineWithResult.coroutine;

			callback(coroutineWithResult.result as List<TestingResponse>);
		}

		StartCoroutine(InnerPerformer(primitives, PrintPrimitiveResponses));
	}

	private void PrintPrimitiveResponses(List<TestingResponse> responses)
	{
		foreach (var response in responses)
		{
			var output = response.Type.ToString();

			if (response.ObjectName != default)
			{
				output += " | " + response.ObjectName;
			}

			if (response.CurrentPosition != default)
			{
				output += " | " + response.CurrentPosition;
			}

			if (response.CurrentRotation != default)
			{
				output += " | " + response.CurrentRotation;
			}

			if (response.FinalPosition != default)
			{
				output += " | " + response.FinalPosition;
			}

			if (response.FinalRotation != default)
			{
				output += " | " + response.FinalRotation;
			}

			if (response.PreviousMoveSpeed != default)
			{
				output += " | " + response.PreviousMoveSpeed;
			}

			if (response.CurrentMoveSpeed != default)
			{
				output += " | " + response.CurrentMoveSpeed;
			}

			Debug.Log(output);
		}
	}
}