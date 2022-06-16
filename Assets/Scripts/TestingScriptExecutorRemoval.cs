using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Constants;
using CustomFunctionality;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Action = Constants.Action;
using Random = UnityEngine.Random;

public class TestingScriptExecutorRemoval : MonoBehaviour
{
	[FormerlySerializedAs("InProgressMaterial")]
	public Material inProgressMaterial;

	[FormerlySerializedAs("CompletedMaterial")]
	public Material completedMaterial;

	[FormerlySerializedAs("ActionsText")] public Text actionsText;
	[FormerlySerializedAs("FigureImage")] public GameObject figureImage;

	[FormerlySerializedAs("CurrentInstructionText")]
	public Text currentInstructionText;

	[FormerlySerializedAs("CurrentTaskText")]
	public Text currentTaskText;

	[FormerlySerializedAs("ScrollRect")] public ScrollRect scrollRect;

	public static GameObject CurrentAttachingObj;

	private Dictionary<string, Dictionary<string, dynamic>> _tasks = new Dictionary<string, Dictionary<string, dynamic>>();
	private int _instructionIndex;
	private int _subtaskIndex;
	private int _taskIndex;
	private GameObject _figure, _rfm, _ifm;
	private Robot _robot;

	private static bool _isPerforming, _isInitializing, _isCompleted;
	private Dictionary<string, Dictionary<string, List<Vector3>>> _cameraPositions = new Dictionary<string, Dictionary<string, List<Vector3>>>();
	private Dictionary<string, Dictionary<string, List<Quaternion>>> _cameraRotations = new Dictionary<string, Dictionary<string, List<Quaternion>>>();
	private string _fileName = "output.txt";
	private float _moveAwayOffset = 2.0f;

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

	private static IEnumerator SetCurrentAttachingObject(GameObject obj)
	{
		CurrentAttachingObj = obj;
		yield return null;
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
		_isInitializing = true;
		_instructionIndex = 0;
		_subtaskIndex = 0;
		_taskIndex = 0;

		_tasks = MLGInstallationTasks.tasks;

		var figureName = "MainLandingGear";
		var figureRfmName = figureName + "-RFM";
		var figureIfmName = figureName + "-IFM";

		_figure = GameObject.Find(figureName);
		_rfm = GameObject.Find(figureRfmName);
		_ifm = GameObject.Find(figureIfmName);

		_robot = new Robot();

		var figureWrapper = GameObject.Find(figureName + "-Wrapper");
		var positionsWrapper = HelperFunctions.FindObjectInFigure(figureWrapper, "CameraPositions");
		var taskIds = _tasks.Keys.ToList();
		if (positionsWrapper != null)
		{
			for (var i = 0; i < positionsWrapper.transform.childCount; i++)
			{
				for (var j = 0; j < taskIds.Count; j++)
				{
					var subtaskIds = new List<string>(_tasks[taskIds[j]]["subtasks"].Keys);

					if (!_cameraPositions.ContainsKey(taskIds[j]))
					{
						_cameraPositions.Add(taskIds[j], new Dictionary<string, List<Vector3>>());
						_cameraRotations.Add(taskIds[j], new Dictionary<string, List<Quaternion>>());
					}

					for (var k = 0; k < subtaskIds.Count; k++)
					{
						var taskCameraLocationsRange = _tasks[taskIds[j]]["subtasks"][subtaskIds[k]]["cameraLocationsRange"];
						var start = taskCameraLocationsRange[0];
						var end = taskCameraLocationsRange[1];

						if (i >= start && i <= end)
						{
							if (_cameraPositions[taskIds[j]].ContainsKey(subtaskIds[k]))
							{
								_cameraPositions[taskIds[j]][subtaskIds[k]].Add(positionsWrapper.transform.GetChild(i).position);
								_cameraRotations[taskIds[j]][subtaskIds[k]].Add(positionsWrapper.transform.GetChild(i).rotation);
							}
							else
							{
								_cameraPositions[taskIds[j]].Add(subtaskIds[k], new List<Vector3> {positionsWrapper.transform.GetChild(i).position});
								_cameraRotations[taskIds[j]].Add(subtaskIds[k], new List<Quaternion> {positionsWrapper.transform.GetChild(i).rotation});
							}
						}
					}
				}
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

		var objects = GameObject.FindGameObjectsWithTag("Object");

		foreach (var obj in objects)
		{
			var rb = obj.GetComponent<Rigidbody>();

			if (rb == null)
			{
				rb = obj.AddComponent<Rigidbody>();
				rb.mass = 1;
				rb.drag = 0;
				rb.useGravity = true;
			}

			var objCollider = obj.GetComponent<BoxCollider>();

			if (objCollider == null)
			{
				obj.AddComponent<BoxCollider>();
			}
		}

		// make them stop falling
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

			_isInitializing = false;
		}));

		// clear output file
		var path = "Assets/Resources/" + _fileName;
		var writer = new StreamWriter(path);
		writer.Write("");
		writer.Close();
	}


	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.E) && !_isCompleted && !_isInitializing)
		{
			StartCoroutine(SequentialExecuteAllActions());
		}
	}

	private IEnumerator SequentialExecuteAllActions()
	{
		while (!_isCompleted)
		{
			if (_isCompleted) break;
			if (_isPerforming)
			{
				yield return new WaitForSeconds(0.5f);
				continue;
			}

			_isPerforming = true;
			var taskId = _tasks.Keys.ToList()[_taskIndex];
			var task = _tasks[taskId];
			Dictionary<string, Dictionary<string, dynamic>> subtasks = task["subtasks"];
			var subtaskId = subtasks.Keys.ToList()[_subtaskIndex];
			var subtask = subtasks[subtaskId];
			List<Action> instructions = subtask["instructions"];

			if (_instructionIndex >= instructions.Count)
			{
				_subtaskIndex += 1;
				_instructionIndex = 0;

				if (_subtaskIndex >= subtasks.Count)
				{
					_taskIndex += 1;
					_subtaskIndex = 0;

					if (_taskIndex >= _tasks.Keys.Count)
					{
						_isCompleted = true;
						continue;
					}

					taskId = _tasks.Keys.ToList()[_taskIndex];
					task = _tasks[taskId];
					subtasks = task["subtasks"];
					subtaskId = subtasks.Keys.ToList()[_subtaskIndex];
					subtask = subtasks[subtaskId];
					instructions = subtask["instructions"];
				}
				else
				{
					subtaskId = subtasks.Keys.ToList()[_subtaskIndex];
					subtask = subtasks[subtaskId];
					instructions = subtask["instructions"];
				}
			}

			var action = instructions[_instructionIndex];

			ExecuteAction(action, _taskIndex, _subtaskIndex, _instructionIndex);
			_instructionIndex += 1;
			yield return null;
		}

		yield return null;
	}

	private IEnumerator ChangeFigureImage(string fileName)
	{
		var figureASprite = Resources.Load<Sprite>("Images/" + fileName); //FULL

		var imageComponent = figureImage.GetComponent<Image>();
		if (imageComponent != null)
		{
			if (!imageComponent.enabled) imageComponent.enabled = true;

			imageComponent.sprite = figureASprite;
		}

		yield return null;
	}

	private void ExecuteAction(Action action, int taskIndex, int subtaskIndex, int instructionIndex)
	{
		var primitives = new List<IEnumerator>();

		var attachingObjNames = action.Components.Take(action.Components.Count - 1);
		var attachingObjs = new List<GameObject>();
		foreach (var objName in attachingObjNames)
		{
			attachingObjs.Add(HelperFunctions.FindObjectInFigure(_figure, objName));
		}

		var referenceObj = HelperFunctions.FindObjectInFigure(_figure, action.Components[action.Components.Count - 1]);

		if (attachingObjs.Count != 0 && referenceObj != null)
		{
			var taskId = new List<string>(_tasks.Keys)[taskIndex];
			foreach (var attachingObj in attachingObjs)
			{
				var objectMeta = attachingObj.GetComponent<ObjectMeta>();

				var shouldAddPrimitives = objectMeta.currentStatus == Status.Initial;

				var text = "";
				const string delimiter = " and ";

				text += objectMeta.AttachType switch
				{
					GeneralConstants.AttachTypes.SmoothInstall => "Smooth Install ",
					GeneralConstants.AttachTypes.StepInstall => "Step Install ",
					GeneralConstants.AttachTypes.SmoothScrew => "Smooth Screw ",
					GeneralConstants.AttachTypes.StepScrew => "Step Screw ",
					_ => "Smooth Install "
				};

				text += attachingObj.name + delimiter + referenceObj.name;
				primitives.Add(UpdateText(actionsText, "- " + text, false));

				primitives.Add(UpdateText(currentInstructionText, text));

				string taskFullName = taskId + "\n" + _tasks[taskId]["title"];
				var subtaskIds = new List<string>(_tasks[taskId]["subtasks"].Keys);
				var subtaskId = subtaskIds[subtaskIndex];
				string subtaskFullName = _tasks[taskId]["subtasks"][subtaskId]["title"];
				string figureFullName = _tasks[taskId]["subtasks"][subtaskId]["figureImage"];
				primitives.Add(ChangeFigureImage(figureFullName));
				primitives.Add(UpdateText(currentTaskText, taskFullName + "\n" + subtaskFullName));

				primitives.Add(SetCurrentAttachingObject(attachingObj));
				primitives.Add(AdaptCamera(_cameraPositions[taskId][subtaskId][instructionIndex],
					_cameraRotations[taskId][subtaskId][instructionIndex]));
				// make status attached
				if (shouldAddPrimitives)
				{
					objectMeta.currentStatus = Status.Completed;
					primitives.Add(ChangeMaterialColor(attachingObj, GeneralConstants.AttachingObjectState.InProgress));

					primitives.AddRange(MoveObjectAwayFromTable(attachingObj));
					primitives.AddRange(CreateRotatePrimitives(attachingObj, referenceObj));
					primitives.Add(AdjustStructureCoroutine(attachingObj, referenceObj));

					primitives.AddRange(CreateRfmMovePrimitives(attachingObj, referenceObj));

					// delay
					primitives.Add(_robot.Stop(0.5f));
				}
				else
				{
					primitives.Add(SetCurrentAttachingObject(null));
				}

				if (objectMeta != null)
				{
					var rotationAxis = objectMeta.AttachRotationAxis;

					var attachRotationVector = rotationAxis switch
					{
						RotationAxisEnum.X => Vector3.right,
						RotationAxisEnum.negX => Vector3.left,
						RotationAxisEnum.Y => Vector3.up,
						RotationAxisEnum.negY => Vector3.down,
						RotationAxisEnum.Z => Vector3.forward,
						RotationAxisEnum.negZ => Vector3.back,
						_ => Vector3.forward
					};

					if (shouldAddPrimitives)
					{
						switch (objectMeta.AttachType)
						{
							case GeneralConstants.AttachTypes.SmoothInstall:
								primitives.AddRange(SmoothInstall(attachingObj, referenceObj));
								break;
							case GeneralConstants.AttachTypes.StepInstall:
								primitives.AddRange(StepInstall(attachingObj, referenceObj));
								break;
							case GeneralConstants.AttachTypes.SmoothScrew:
								primitives.AddRange(SmoothScrew(attachingObj, referenceObj, attachRotationVector));
								break;
							case GeneralConstants.AttachTypes.StepScrew:
								primitives.AddRange(StepScrew(attachingObj, referenceObj, attachRotationVector));
								break;
							default:
								primitives.AddRange(SmoothInstall(attachingObj, referenceObj));
								break;
						}
					}
				}
				else
				{
					if (shouldAddPrimitives)
					{
						primitives.AddRange(SmoothInstall(attachingObj, referenceObj));
					}
				}

				if (shouldAddPrimitives)
				{
					primitives.Add(ChangeMaterialColor(attachingObj, GeneralConstants.AttachingObjectState.Completed));
					primitives.Add(_robot.Stop(0.1f));
				}
			}

			Performer(primitives);
		}
		else
		{
			Debug.Log("Some of objects were not found.");
			StartCoroutine(SetCurrentAttachingObject(null));
		}
	}

	private IEnumerator UpdateText(Text textElement, string value, bool alwaysRewrite = true)
	{
		if (alwaysRewrite)
		{
			textElement.text = value;
		}
		else
		{
			if (textElement.text.Trim() == "")
			{
				textElement.text = value;
			}
			else
			{
				textElement.text += "\n" + value;
			}
		}

		if (scrollRect.isActiveAndEnabled)
		{
			Canvas.ForceUpdateCanvases();
			scrollRect.verticalNormalizedPosition = 0;
		}

		yield return null;
	}

	private Vector3 GetMoveAwayVector(GameObject obj)
	{
		var objVectorUp = new Vector3(0, 1, 0);
		var angle = Vector3.Angle(objVectorUp, Vector3.up);

		if (angle < 95 && angle > 85)
		{
			objVectorUp = new Vector3(0, 0, 1);
		}

		if (angle < 185 && angle > 175)
		{
			objVectorUp = new Vector3(0, -1, 0);
		}

		return obj.transform.position + objVectorUp * _moveAwayOffset;
	}

	private List<IEnumerator> MoveObjectAwayFromTable(GameObject objA)
	{
		var primitives = new List<IEnumerator>();

		var duration = _robot.GetMoveDuration();

		var finalPosition = GetMoveAwayVector(objA);
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
						newMaterials[i] = inProgressMaterial;
					}

					break;
				case GeneralConstants.AttachingObjectState.Completed:
					var color = new Color(Random.Range(10, 200) / 255.0f, Random.Range(10, 200) / 255.0f,
						Random.Range(10, 200) / 255.0f, 1);
					for (var i = 0; i < objRenderer.materials.Length; i++)
					{
						newMaterials[i] = Instantiate(completedMaterial);
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
		var objAPosition = GetMoveAwayVector(objA);
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