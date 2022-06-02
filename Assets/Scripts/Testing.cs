using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Constants;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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
	public Material InProgressMaterial;
	public Material CompletedMaterial;
	public TextMeshProUGUI ActionsText;

	private List<Action> _actions = new List<Action>();
	private int _activeIndex;
	private GameObject _figure, _rfm, _ifm;
	private Robot _robot;

	private static bool _isPerforming;
	private List<Vector3> _cameraPositions = new List<Vector3>();
	private List<Quaternion> _cameraRotations = new List<Quaternion>();


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
		IEnumerator InnerPerformer(List<IEnumerator> p, Action<List<TestingResponse>> callback = null)
		{
			var coroutineWithResult = new CoroutineWithResult(this, Sequence(p));
			yield return coroutineWithResult.coroutine;

			callback?.Invoke(coroutineWithResult.result as List<TestingResponse>);
		}

		var finalPrimitives = new List<IEnumerator>();

		finalPrimitives.Add(SetIsPerforming(true));
		finalPrimitives.AddRange(primitives);
		finalPrimitives.Add(SetIsPerforming(false));

		StartCoroutine(InnerPerformer(finalPrimitives, PrintPrimitiveResponses));
	}

	private IEnumerator SetIsPerforming(bool state)
	{
		_isPerforming = state;
		yield return null;
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

		var upperSideInstallationActions = new List<Action>
		{
			new Action {Name = "Attach", Components = new List<string> {"[3]", "[15]"}}, // 1
			new Action {Name = "Attach", Components = new List<string> {"[22]", "[3]"}}, // 2
			new Action {Name = "Attach", Components = new List<string> {"[21]", "[20]", "[22]"}}, // 3
			new Action {Name = "Attach", Components = new List<string> {"[16]", "[22]"}}, // 4
			new Action {Name = "Attach", Components = new List<string> {"[17]", "[18]", "[16]"}}, // 5
			new Action {Name = "Attach", Components = new List<string> {"[19]", "[16]"}}, // 6

			new Action {Name = "Attach", Components = new List<string> {"[8]", "[3]"}}, // 7
			new Action {Name = "Attach", Components = new List<string> {"[7]", "[8]"}}, // 8
			new Action {Name = "Attach", Components = new List<string> {"[9]", "[7]"}}, // 9
			new Action {Name = "Attach", Components = new List<string> {"[10]", "[7]"}}, // 10
			new Action {Name = "Attach", Components = new List<string> {"[11]", "[7]"}}, // 11
			new Action {Name = "Attach", Components = new List<string> {"[12]", "[14]", "[11]"}}, // 12
			new Action {Name = "Attach", Components = new List<string> {"[13]", "[11]"}}, // 13

			new Action {Name = "Attach", Components = new List<string> {"[1]", "[3]"}}, // 14
			new Action {Name = "Attach", Components = new List<string> {"[2]", "[1]"}}, // 15
			new Action {Name = "Attach", Components = new List<string> {"[4]", "[5]", "[2]"}}, // 16
			new Action {Name = "Attach", Components = new List<string> {"[6]", "[2]"}} // 17
		};

		var lowerSideInstallationActions = new List<Action>
		{
			new Action {Name = "Attach", Components = new List<string> {"[3]", "[8]"}},
			new Action {Name = "Attach", Components = new List<string> {"[7]", "[3]"}},
			new Action {Name = "Attach", Components = new List<string> {"[9]", "[7]"}},
			new Action {Name = "Attach", Components = new List<string> {"[11]", "[7]"}},
			new Action {Name = "Attach", Components = new List<string> {"[12]", "[14]", "[11]"}},
			new Action {Name = "Attach", Components = new List<string> {"[13]", "[11]"}},

			new Action {Name = "Attach", Components = new List<string> {"[8]", "[51]"}},
			new Action {Name = "Attach", Components = new List<string> {"[52]", "[8]"}},
			new Action {Name = "Attach", Components = new List<string> {"[53]", "[54]", "[52]"}},
			new Action {Name = "Attach", Components = new List<string> {"[47]", "[52]"}},
			new Action {Name = "Attach", Components = new List<string> {"[48]", "[50]", "[47]"}},
			new Action {Name = "Attach", Components = new List<string> {"[49]", "[47]"}},

			new Action {Name = "Attach", Components = new List<string> {"[41]", "[8]"}},
			new Action {Name = "Attach", Components = new List<string> {"[45]", "[41]"}},
			new Action {Name = "Attach", Components = new List<string> {"[4]", "[46]"}},
			new Action {Name = "Attach", Components = new List<string> {"[46]", "[41]"}},
			new Action {Name = "Attach", Components = new List<string> {"[43]", "[42]", "[46]"}}
		};

		var sideSideInstallationActions = new List<Action>
		{
			new Action {Name = "Attach", Components = new List<string> {"[3]", "[15]"}},
			new Action {Name = "Attach", Components = new List<string> {"[22]", "[3]"}},
			new Action {Name = "Attach", Components = new List<string> {"[22]", "[3]"}},
			new Action {Name = "Attach", Components = new List<string> {"[21]", "[20]", "[22]"}},
			new Action {Name = "Attach", Components = new List<string> {"[16]", "[22]"}},
			new Action {Name = "Attach", Components = new List<string> {"[17]", "[18]", "[16]"}},
			new Action {Name = "Attach", Components = new List<string> {"[19]", "[16]"}},

			new Action {Name = "Attach", Components = new List<string> {"[8]", "[51]"}},
			new Action {Name = "Attach", Components = new List<string> {"[52]", "[8]"}},
			new Action {Name = "Attach", Components = new List<string> {"[53]", "[54]", "[52]"}},
			new Action {Name = "Attach", Components = new List<string> {"[47]", "[52]"}},
			new Action {Name = "Attach", Components = new List<string> {"[48]", "[50]", "[47]"}},
			new Action {Name = "Attach", Components = new List<string> {"[49]", "[47]"}},

			new Action {Name = "Attach", Components = new List<string> {"[41]", "[8]"}},
			new Action {Name = "Attach", Components = new List<string> {"[45]", "[41]"}},
			new Action {Name = "Attach", Components = new List<string> {"[44]", "[46]"}},
			new Action {Name = "Attach", Components = new List<string> {"[46]", "[41]"}},
			new Action {Name = "Attach", Components = new List<string> {"[43]", "[42]", "[46]"}},

			new Action {Name = "Attach", Components = new List<string> {"[1]", "[3]"}},
			new Action {Name = "Attach", Components = new List<string> {"[2]", "[1]"}},
			new Action {Name = "Attach", Components = new List<string> {"[4]", "[5]", "[2]"}},
			new Action {Name = "Attach", Components = new List<string> {"[6]", "[2]"}},
		};

		_actions.AddRange(upperSideInstallationActions);
		// _actions.AddRange(lowerSideInstallationActions);
		// _actions.AddRange(sideSideInstallationActions);

		var figureName = "MainLandingGear";
		var figureRfmName = figureName + "-RFM";
		var figureIfmName = figureName + "-IFM";

		_figure = GameObject.Find(figureName);
		_rfm = GameObject.Find(figureRfmName);
		_ifm = GameObject.Find(figureIfmName);

		_robot = new Robot();

		var figureWrapper = GameObject.Find(figureName + "-Wrapper");
		var positionsWrapper = HelperFunctions.FindObjectInFigure(figureWrapper, "CameraPositions");
		Debug.Log(positionsWrapper);
		if (positionsWrapper != null)
		{
			for (var i = 0; i < positionsWrapper.transform.childCount; i++)
			{
				_cameraPositions.Add(positionsWrapper.transform.GetChild(i).position);
				_cameraRotations.Add(positionsWrapper.transform.GetChild(i).rotation);
			}
		}
	}


	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.E))
		{
			if (_isPerforming) return;

			var action = _actions[_activeIndex];

			StartCoroutine(ExecuteAction(action));
		}
	}

	private IEnumerator ExecuteAction(Action action)
	{
		var attachingObjNames = action.Components.Take(action.Components.Count - 1);
		var attachingObjs = new List<GameObject>();
		foreach (var objName in attachingObjNames)
		{
			attachingObjs.Add(HelperFunctions.FindObjectInFigure(_figure, objName));
		}

		var referenceObj = HelperFunctions.FindObjectInFigure(_figure, action.Components[action.Components.Count - 1]);

		if (attachingObjs.Count != 0 && referenceObj != null)
		{
			var primitives = new List<IEnumerator>();
			foreach (var attachingObj in attachingObjs)
			{
				var objectMeta = attachingObj.GetComponent<ObjectMeta>();

				primitives.Add(AdaptCamera(_cameraPositions[_activeIndex], _cameraRotations[_activeIndex]));
				primitives.Add(ChangeMaterialColor(attachingObj, GeneralConstants.AttachingObjectState.InProgress));

				if (objectMeta != null)
				{
					var attachRotationAxis = objectMeta.AttachRotationAxis;

					var attachRotationVector = attachRotationAxis switch
					{
						RotationAxisEnum.X => Vector3.right,
						RotationAxisEnum.negX => Vector3.left,
						RotationAxisEnum.Y => Vector3.up,
						RotationAxisEnum.negY => Vector3.down,
						RotationAxisEnum.Z => Vector3.forward,
						RotationAxisEnum.negZ => Vector3.back,
						_ => Vector3.forward
					};

					var text = "- ";
					var delimiter = " and ";
					switch (objectMeta.AttachType)
					{
						case GeneralConstants.AttachTypes.SmoothInstall:
							text += "Smooth Install ";
							primitives.AddRange(SmoothInstall(attachingObj, referenceObj));
							break;
						case GeneralConstants.AttachTypes.StepInstall:
							text += "Step Install ";
							primitives.AddRange(StepInstall(attachingObj, referenceObj));
							break;
						case GeneralConstants.AttachTypes.SmoothScrew:
							text += "Smooth Screw ";
							primitives.AddRange(SmoothScrew(attachingObj, referenceObj, attachRotationVector));
							break;
						case GeneralConstants.AttachTypes.StepScrew:
							text += "Step Screw ";
							primitives.AddRange(StepScrew(attachingObj, referenceObj, attachRotationVector));
							break;
						default:
							primitives.AddRange(SmoothInstall(attachingObj, referenceObj));
							break;
					}

					text += attachingObj.name + delimiter + referenceObj.name;
					if (ActionsText.text.Trim() == "")
					{
						ActionsText.text = text;
					}
					else
					{
						ActionsText.text += "\n" + text;
					}
				}
				else
				{
					primitives.AddRange(SmoothInstall(attachingObj, referenceObj));
				}

				primitives.Add(ChangeMaterialColor(attachingObj, GeneralConstants.AttachingObjectState.Completed));
				primitives.Add(_robot.Stop(0.1f));
			}

			Performer(primitives);

			_activeIndex += 1;
		}
		else
		{
			Debug.Log("Some of objects were not found.");
		}

		yield return null;
	}

	private IEnumerator ChangeMaterialColor(GameObject obj, GeneralConstants.AttachingObjectState state)
	{
		var objRenderer = obj.GetComponent<Renderer>();
		if (objRenderer != null)
		{
			var newMaterials = new Material[objRenderer.materials.Length];

			switch (state)
			{
				case GeneralConstants.AttachingObjectState.Initial:
					var ifmObj = HelperFunctions.FindObjectInFigure(_ifm, obj.name);
					newMaterials = ifmObj.GetComponent<Renderer>().materials;
					break;
				case GeneralConstants.AttachingObjectState.InProgress:
					for (var i = 0; i < objRenderer.materials.Length; i++)
					{
						newMaterials[i] = InProgressMaterial;
					}

					break;
				case GeneralConstants.AttachingObjectState.Completed:
					for (var i = 0; i < objRenderer.materials.Length; i++)
					{
						newMaterials[i] = CompletedMaterial;
					}

					break;
			}

			objRenderer.materials = newMaterials;
		}

		yield return null;
	}

	private IEnumerator AdaptCamera(Vector3 position, Quaternion rotation)
	{
		var mainCamera = Camera.main;
		var mainCameraTransform = mainCamera.transform;
		var initialRotation = mainCameraTransform.rotation;
		var initialPosition = mainCameraTransform.position;

		const float moveDuration = 0.5f;
		var count = 0.0f;
		while (count < moveDuration)
		{
			count += Time.fixedDeltaTime;

			mainCamera.transform.position =
				Vector3.Slerp(initialPosition, position, count / moveDuration);

			yield return null;
		}

		const float rotateDuration = 0.5f;
		count = 0.0f;
		while (count < rotateDuration)
		{
			count += Time.fixedDeltaTime;

			mainCamera.transform.rotation =
				Quaternion.Slerp(initialRotation, rotation,
					count / rotateDuration);

			yield return null;
		}

		yield return null;
	}

	private List<IEnumerator> CreateRotatePrimitives(GameObject objA, GameObject objB)
	{
		var primitives = new List<IEnumerator>();

		var initialRotation = objA.transform.eulerAngles;
		var finalRotation = objB.transform.eulerAngles;
		var angle = Quaternion.Angle(objA.transform.rotation, objB.transform.rotation);

		var duration = _robot.GetRotateDuration();
		var count = duration / Time.fixedDeltaTime;

		primitives.Add(_robot.SetRotateDegree(angle / count));
		for (var i = 0; i < Mathf.CeilToInt(count); i++)
		{
			primitives.Add(_robot.Rotate(objA, finalRotation));
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

	private List<IEnumerator> SmoothScrew(GameObject objA, GameObject objB, Vector3 direction = default)
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
		primitives.Add(_robot.SetRotateDegree(5.0f));
		for (var i = 0; i < ifmCount; i++)
		{
			if (direction == default) direction = Vector3.forward;
			primitives.Add(_robot.MoveWithRotation(objA, ifmFinalPosition, direction));
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

	private List<IEnumerator> StepScrew(GameObject objA, GameObject objB, Vector3 direction = default, int steps = 3)
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
		primitives.Add(_robot.SetRotateDegree(5.0f));

		var delayDelta = Mathf.CeilToInt(ifmCount / steps);
		var delayCounter = 1;
		for (var i = 0; i < Mathf.CeilToInt(ifmCount); i++)
		{
			if (i == delayDelta * delayCounter)
			{
				delayCounter += 1;
				primitives.Add(_robot.Stop(0.5f));
			}

			if (direction == default) direction = Vector3.forward;
			primitives.Add(_robot.MoveWithRotation(objA, ifmFinalPosition, direction));
		}

		return primitives;
	}
}