using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Constants;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Action = Constants.Action;
using Random = UnityEngine.Random;

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
	public GameObject Table;
	public GameObject FigureImage;
	public Text CurrentInstructionText;
	public Text CurrentTaskText;

	private Dictionary<string, dynamic> _tasks = new Dictionary<string, dynamic>();
	private int _subtaskIndex;
	private int _taskIndex;
	private GameObject _figure, _rfm, _ifm;
	private Robot _robot;

	private static bool _isPerforming;
	private static bool _isInitilizing = true;
	private List<Vector3> _cameraPositions = new List<Vector3>();
	private List<Quaternion> _cameraRotations = new List<Quaternion>();
	private string _fileName = "output.txt";
	private float _moveAwayOffset = 0.5f;

	private List<string> TaskIds;


	private static IEnumerator AdjustStructureCoroutine(GameObject objA, GameObject objB)
	{
		objA.transform.parent = objB.transform;
		yield return null;
	}

	private IEnumerator SequenceCoroutine(List<IEnumerator> coroutines, float delay = 0.0f)
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

	private static IEnumerator DelayCoroutine(float duration, State.VoidFunction method = null)
	{
		yield return new WaitForSeconds(duration);
		method?.Invoke();
	}

	private void Performer(List<IEnumerator> primitives)
	{
		IEnumerator InnerPerformer(List<IEnumerator> p, Action<List<TestingResponse>> callback = null)
		{
			var coroutineWithResult = new CoroutineWithResult(this, SequenceCoroutine(p));
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
		string path = "Assets/Resources/" + _fileName;
		var writer = new StreamWriter(path, true);
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

	private void Start()
	{
		_subtaskIndex = 0;
		_taskIndex = 0;
		
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

		_tasks = new Dictionary<string, dynamic>
		{
			{
				"TASK 32-11-61-400-801", new Dictionary<string, dynamic>
				{
					{"figureImage", "figure_c"},
					{"subtasks", upperSideInstallationActions}
				}
			}
		};
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

		// hide reference objects
		var referenceObjects = GameObject.FindGameObjectsWithTag("ReferenceObject");
		foreach (var obj in referenceObjects)
		{
			var meshRenderer = obj.GetComponent<MeshRenderer>();
			if (meshRenderer != null)
			{
				meshRenderer.enabled = false;
			}
		}

		StartCoroutine(DelayCoroutine(2.0f, () =>
		{
			var objects = GameObject.FindGameObjectsWithTag("Object");

			foreach (var obj in objects)
			{
				var rb = obj.GetComponent<Rigidbody>();

				if (rb != null)
				{
					rb.isKinematic = true;
					rb.useGravity = false;
				}
			}

			_isInitilizing = false;
		}));

		// clear output file
		string path = "Assets/Resources/" + _fileName;
		var writer = new StreamWriter(path);
		writer.Write("");
		writer.Close();
	}


	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.E))
		{
			if (_isPerforming || _isInitilizing) return;

			var imageComponent = FigureImage.GetComponent<Image>();

			var taskId = _tasks.Keys.ToList()[_taskIndex];
			var task = _tasks[taskId];
			var figureName = task["figureImage"];
			
			var figureASprite = Resources.Load<Sprite>("Images/" + figureName); //FULL
			if (imageComponent != null)
			{
				if (!imageComponent.enabled) imageComponent.enabled = true;

				imageComponent.sprite = figureASprite;
			}

			Action action = task["subtasks"][_subtaskIndex];
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
				primitives.Add(AdaptCamera(_cameraPositions[_subtaskIndex], _cameraRotations[_subtaskIndex]));
				primitives.Add(ChangeMaterialColor(attachingObj, GeneralConstants.AttachingObjectState.InProgress));

				primitives.AddRange(MoveObjectAwayFromTable(attachingObj));

				primitives.AddRange(CreateRotatePrimitives(attachingObj, referenceObj));
				primitives.Add(AdjustStructureCoroutine(attachingObj, referenceObj));

				primitives.AddRange(CreateRfmMovePrimitives(attachingObj, referenceObj));

				var objectMeta = attachingObj.GetComponent<ObjectMeta>();
				// delay
				primitives.Add(_robot.Stop(0.5f));

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

					var text = "";
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
						ActionsText.text = "- " + text;
					}
					else
					{
						ActionsText.text += "\n" + text;
					}

					CurrentInstructionText.text = text;
					
					CurrentTaskText.text = _tasks.Keys.ToList()[_taskIndex];
				}
				else
				{
					primitives.AddRange(SmoothInstall(attachingObj, referenceObj));
				}

				primitives.Add(ChangeMaterialColor(attachingObj, GeneralConstants.AttachingObjectState.Completed));
				primitives.Add(_robot.Stop(0.1f));
			}

			Performer(primitives);

			_subtaskIndex += 1;
		}
		else
		{
			Debug.Log("Some of objects were not found.");
		}

		yield return null;
	}

	private List<IEnumerator> MoveObjectAwayFromTable(GameObject objA)
	{
		var primitives = new List<IEnumerator>();

		var duration = _robot.GetMoveDuration();

		var finalPosition = objA.transform.TransformPoint(objA.transform.up * _moveAwayOffset);
		var finalCount = duration / Time.fixedDeltaTime;
		var distance = Vector3.Distance(finalPosition, objA.transform.position);

		var prevMoveDistance = _robot.GetMoveDistance();
		primitives.Add(_robot.SetMoveDistance(distance / finalCount));
		for (var i = 0; i < Mathf.CeilToInt(finalCount); i++)
		{
			primitives.Add(_robot.Move(objA, finalPosition));
		}

		primitives.Add(_robot.SetMoveDistance(prevMoveDistance));
		primitives.Add(_robot.Stop(0.1f));

		return primitives;
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
					var color = new Color(Random.Range(10, 200) / 255.0f, Random.Range(10, 200) / 255.0f,
						Random.Range(10, 200) / 255.0f, 1);
					for (var i = 0; i < objRenderer.materials.Length; i++)
					{
						newMaterials[i] = Instantiate(CompletedMaterial);
						newMaterials[i].color = color;
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

		var previousParent = rfmReferenceObjA.transform.parent;
		rfmReferenceObjA.transform.SetParent(rfmReferenceObjB.transform);
		var rfmDiff = rfmReferenceObjA.transform.localPosition;
		var rfmFinalPosition = objB.transform.TransformPoint(rfmDiff);
		rfmReferenceObjA.transform.SetParent(previousParent);

		var duration = _robot.GetMoveDuration();
		var objAPosition = objA.transform.TransformPoint(objA.transform.up * _moveAwayOffset);
		var rfmDistance = Vector3.Distance(objAPosition, rfmFinalPosition);
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

		// ifm
		var ifmReferenceObjA = HelperFunctions.FindObjectInFigure(_ifm, objA.name);
		var ifmReferenceObjB = HelperFunctions.FindObjectInFigure(_ifm, objB.name);
		var rfmReferenceObjA = HelperFunctions.FindObjectInFigure(_rfm, objA.name);
		var rfmReferenceObjB = HelperFunctions.FindObjectInFigure(_rfm, objB.name);

		var prevParent = rfmReferenceObjA.transform.parent;
		rfmReferenceObjA.transform.parent = rfmReferenceObjB.transform;
		var rfmDiff = rfmReferenceObjA.transform.localPosition;
		var rfmFinalPosition = objB.transform.TransformPoint(rfmDiff);
		rfmReferenceObjA.transform.parent = prevParent;

		prevParent = ifmReferenceObjA.transform.parent;
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

		// ifm
		var ifmReferenceObjA = HelperFunctions.FindObjectInFigure(_ifm, objA.name);
		var ifmReferenceObjB = HelperFunctions.FindObjectInFigure(_ifm, objB.name);
		var rfmReferenceObjA = HelperFunctions.FindObjectInFigure(_rfm, objA.name);
		var rfmReferenceObjB = HelperFunctions.FindObjectInFigure(_rfm, objB.name);

		var prevParent = rfmReferenceObjA.transform.parent;
		rfmReferenceObjA.transform.parent = rfmReferenceObjB.transform;
		var rfmDiff = rfmReferenceObjA.transform.localPosition;
		var rfmFinalPosition = objB.transform.TransformPoint(rfmDiff);
		rfmReferenceObjA.transform.parent = prevParent;

		prevParent = ifmReferenceObjA.transform.parent;
		ifmReferenceObjA.transform.parent = ifmReferenceObjB.transform;
		var ifmDiff = ifmReferenceObjA.transform.localPosition;
		var ifmFinalPosition = objB.transform.TransformPoint(ifmDiff);
		ifmReferenceObjA.transform.parent = prevParent;

		var duration = _robot.GetMoveDuration();
		var ifmDistance = Vector3.Distance(rfmFinalPosition, ifmFinalPosition);
		var ifmCount = duration / Time.fixedDeltaTime;

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

		var ifmReferenceObjA = HelperFunctions.FindObjectInFigure(_ifm, objA.name);
		var ifmReferenceObjB = HelperFunctions.FindObjectInFigure(_ifm, objB.name);
		var rfmReferenceObjA = HelperFunctions.FindObjectInFigure(_rfm, objA.name);
		var rfmReferenceObjB = HelperFunctions.FindObjectInFigure(_rfm, objB.name);

		var prevParent = rfmReferenceObjA.transform.parent;
		rfmReferenceObjA.transform.parent = rfmReferenceObjB.transform;
		var rfmDiff = rfmReferenceObjA.transform.localPosition;
		var rfmFinalPosition = objB.transform.TransformPoint(rfmDiff);
		rfmReferenceObjA.transform.parent = prevParent;

		prevParent = ifmReferenceObjA.transform.parent;
		ifmReferenceObjA.transform.parent = ifmReferenceObjB.transform;
		var ifmDiff = ifmReferenceObjA.transform.localPosition;
		var ifmFinalPosition = objB.transform.TransformPoint(ifmDiff);
		ifmReferenceObjA.transform.parent = prevParent;

		var duration = _robot.GetMoveDuration();
		var ifmDistance = Vector3.Distance(rfmFinalPosition, ifmFinalPosition);
		var ifmCount = duration / Time.fixedDeltaTime;

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

		var ifmReferenceObjA = HelperFunctions.FindObjectInFigure(_ifm, objA.name);
		var ifmReferenceObjB = HelperFunctions.FindObjectInFigure(_ifm, objB.name);
		var rfmReferenceObjA = HelperFunctions.FindObjectInFigure(_rfm, objA.name);
		var rfmReferenceObjB = HelperFunctions.FindObjectInFigure(_rfm, objB.name);

		var prevParent = rfmReferenceObjA.transform.parent;
		rfmReferenceObjA.transform.parent = rfmReferenceObjB.transform;
		var rfmDiff = rfmReferenceObjA.transform.localPosition;
		var rfmFinalPosition = objB.transform.TransformPoint(rfmDiff);
		rfmReferenceObjA.transform.parent = prevParent;

		prevParent = ifmReferenceObjA.transform.parent;
		ifmReferenceObjA.transform.parent = ifmReferenceObjB.transform;
		var ifmDiff = ifmReferenceObjA.transform.localPosition;
		var ifmFinalPosition = objB.transform.TransformPoint(ifmDiff);
		ifmReferenceObjA.transform.parent = prevParent;

		var duration = _robot.GetMoveDuration();
		var ifmDistance = Vector3.Distance(rfmFinalPosition, ifmFinalPosition);
		var ifmCount = duration / Time.fixedDeltaTime;

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