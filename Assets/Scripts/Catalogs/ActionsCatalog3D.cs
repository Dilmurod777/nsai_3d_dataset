using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Constants;
using CustomFunctionality;
using UnityEditor.UIElements;
using UnityEngine;
using Action = Constants.Action;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Catalogs
{
	public interface IActionsCatalog3DInterface
	{
		List<GameObject> Filter3DAttr(string args);
		Response Reset(string args);
		Response Highlight(string args);
		Response Rotate(string args);
		Response Scale(string args);
		Response ShowSide(string args);
		Response SideBySideLook(string args);
		Response CloseLook(string args);
		Response Animate(string args);
		Response Visibility(string args);
		Response SmoothInstall(string args);
		Response Align(string args);
		Response StepInstall(string args);
		Response SmoothScrew(string args);
		Response StepScrew(string args);
		List<Action> CreateActions(string args);
		string CheckActionsValidity(string args);
	}

	public class ActionsCatalog3D : MonoBehaviour, IActionsCatalog3DInterface
	{
		private IEnumerator Sequence(List<IEnumerator> coroutines, float delay = 0.0f)
		{
			yield return new WaitForSeconds(delay);
			foreach (var c in coroutines) yield return StartCoroutine(c);
		}

		private static void DestroyCloneObjects()
		{
			var cloneObjects = GameObject.FindGameObjectsWithTag("CloneObject");

			foreach (var cloneObj in cloneObjects)
			{
				Destroy(cloneObj);
			}
		}

		public List<GameObject> Filter3DAttr(string args)
		{
			var argsList = args.Split(GeneralConstants.ArgsSeparator);
			var attrName = argsList[0];
			var prev = Context.HasAttribute(argsList[1]) ? Context.GetAttribute(argsList[1]) : argsList[1];
			var parent = new List<GameObject>();
			if (argsList[2] != "root")
			{
				parent = Context.GetAttribute(argsList[2]);
			}

			if (prev.GetType() == "".GetType())
			{
				prev = new List<string> {prev};
			}

			var allObjects = new List<GameObject>();
			switch (attrName)
			{
				case "name":
					var objects = FindObjectsWithIds(prev, parent);
					var figures = FindFigureWithId(prev, parent);

					allObjects.AddRange(objects);
					allObjects.AddRange(figures);
					break;
				case "type":
					if (prev == "figure")
					{
						foreach (var obj in parent)
						{
							if (obj.CompareTag("Figure"))
							{
								allObjects.Add(obj);
							}
						}
					}

					break;
			}
			
			return allObjects;
		}

		// find object with part of name
		private static List<GameObject> FindObjectsWithIds(List<string> ids, List<GameObject> parentObjects)
		{
			var allObjects = parentObjects;
			if (parentObjects == null || parentObjects.Count == 0)
			{
				allObjects = GameObject.FindGameObjectsWithTag("Object").ToList();
			}

			var foundObs = new List<GameObject>();

			foreach (var id in ids)
			foreach (var obj in allObjects)
				if (obj.name.Contains($"{id}"))
					foundObs.Add(obj);

			return foundObs;
		}

		private static List<GameObject> FindFigureWithId(List<string> ids, List<GameObject> parentFigures)
		{
			var allFigures = parentFigures;
			if (parentFigures == null || parentFigures.Count == 0)
			{
				allFigures = GameObject.FindGameObjectsWithTag("Figure").ToList();
			}

			var foundFigs = new List<GameObject>();

			foreach (var fig in allFigures)
			foreach (var id in ids)
				if (fig.name.Equals(id))
					foundFigs.Add(fig);

			return foundFigs;
		}

		public Response Rotate(string args)
		{
			var argsList = args.Split(GeneralConstants.ArgsSeparator);
			GameObject obj = Context.GetAttribute(argsList[0]);
			if (obj == null) return null;

			List<string> restArgs = Context.GetAttribute(argsList[1]);
			var degree = float.Parse(restArgs[0]);
			var axisRegex = Regex.Match(Context.Instance.Query, @"[XYZ] axis").Value;

			var rotation = obj.transform.rotation;
			var axis = axisRegex.Split(' ')[0];
			var rotationX = axis == "X" ? degree : 0;
			var rotationY = axis == "Y" ? degree : 0;
			var rotationZ = axis == "Z" ? degree : 0;

			var newRotation = rotation * Quaternion.Euler(rotationX, rotationY, rotationZ);
			StartCoroutine(IRotateObject(obj, newRotation, 1.0f));

			return new Response(new Dictionary<string, dynamic>
			{
				{"name", "rotate"}
			}, new List<GameObject> {obj}, new Dictionary<string, dynamic>
			{
				{"degree", degree},
				{"axis", axis}
			});
		}

		public Response Scale(string args)
		{
			var argsList = args.Split(GeneralConstants.ArgsSeparator);
			GameObject obj = Context.GetAttribute(argsList[1]);
			if (obj == null) return null;

			List<string> restArgs = Context.GetAttribute(argsList[2]);
			var state = argsList[0];
			var scaleRatio = float.Parse(restArgs[0]);

			var currentLocalScale = obj.transform.localScale;
			var currentLocalScaleX = currentLocalScale.x;
			var currentLocalScaleY = currentLocalScale.y;
			var currentLocalScaleZ = currentLocalScale.z;
			var change = scaleRatio < 1
				? state == "up" ? 1 + scaleRatio : 1 - scaleRatio
				: state == "up"
					? scaleRatio
					: Mathf.Round(100f / scaleRatio) / 100f;

			var finalScale = new Vector3(currentLocalScaleX * change, currentLocalScaleY * change,
				currentLocalScaleZ * change);

			StartCoroutine(IScaleObject(obj, finalScale, 1.0f));

			return new Response(new Dictionary<string, dynamic>
			{
				{"name", "scale"},
				{"extra", state}
			}, new List<GameObject> {obj}, new Dictionary<string, dynamic>
			{
				{"scale", scaleRatio}
			});
		}

		public Response Reset(string args)
		{
			var argsList = args.Split(GeneralConstants.ArgsSeparator);

			GameObject obj = Context.GetAttribute(argsList[0]);
			if (obj == null) return null;

			Attributes attributes = Context.Instance.InitialAttributes[obj.name];
			var cameraAttributes = Context.Instance.InitialAttributes["camera"];

			var coroutines = new List<IEnumerator>
			{
				IResetObject(obj, attributes, 1.0f),
				IResetObject(Context.Instance.Camera.gameObject, cameraAttributes, 1.0f)
			};

			if (cameraAttributes.FoV != 0)
				coroutines.Add(IChangeFieldOfViewByValue(Context.Instance.Camera, cameraAttributes.FoV, 1.0f));

			StartCoroutine(Sequence(coroutines));

			return new Response(new Dictionary<string, dynamic>
			{
				{"name", "reset"}
			}, new List<GameObject> {obj}, new Dictionary<string, dynamic>());
		}

		public Response Highlight(string args)
		{
			var argsList = args.Split(GeneralConstants.ArgsSeparator);

			var state = argsList[0];

			List<GameObject> objs = Context.GetAttribute(argsList[1]);
			if (objs.Count == 0) return null;

			foreach (var obj in objs)
			{
				var outlineComponent = obj.GetComponent<Outline>();
				if (outlineComponent == null) outlineComponent = obj.AddComponent<Outline>();

				switch (state)
				{
					case "on":
						outlineComponent.OutlineMode = Outline.Mode.OutlineAll;
						outlineComponent.OutlineWidth = 5.0f;
						outlineComponent.OutlineColor = Color.blue;

						outlineComponent.enabled = true;
						break;
					case "off":
						outlineComponent.enabled = false;
						break;
				}
			}

			return new Response(new Dictionary<string, dynamic>
			{
				{"name", "highlight"},
				{"extra", state}
			}, objs, new Dictionary<string, dynamic>());
		}

		public Response ShowSide(string args)
		{
			var argsList = args.Split(GeneralConstants.ArgsSeparator);

			GameObject obj = Context.GetAttribute(argsList[1]);
			if (obj == null) return null;

			var figureSide = argsList[0];

			var sideRotation = figureSide switch
			{
				"front" => Quaternion.Euler(0, -90, -45),
				"back" => Quaternion.Euler(0, 90, 0),
				"right" => Quaternion.Euler(0, 0, 0),
				"left" => Quaternion.Euler(0, 180, 0),
				"top" => Quaternion.Euler(-45, 0, 0),
				"bottom" => Quaternion.Euler(135, 0, 0),
				_ => Quaternion.Euler(0, 0, 0)
			};

			var coroutines = new List<IEnumerator>
			{
				IRotateObject(obj, sideRotation, 1.0f)
			};

			StartCoroutine(Sequence(coroutines));

			return new Response(new Dictionary<string, dynamic>
			{
				{"name", "show side"},
				{"extra", figureSide}
			}, new List<GameObject> {obj}, new Dictionary<string, dynamic>
			{
				{"side", figureSide}
			});
		}

		public Response SideBySideLook(string args)
		{
			var argsList = args.Split(' ');
			GameObject fig = Context.GetAttribute(argsList[0]);
			if (fig == null) return null;

			var objs = new List<GameObject>();
			for (var i = 0; i < fig.transform.childCount; i++)
			{
				objs.Add(fig.transform.GetChild(i).gameObject);
			}

			CloseLookFunctionality(objs);

			return new Response(new Dictionary<string, dynamic>
			{
				{"name", "side by side look"}
			}, objs, new Dictionary<string, dynamic>());
		}

		public Response CloseLook(string args)
		{
			var argsList = args.Split(GeneralConstants.ArgsSeparator);

			List<GameObject> objs = Context.GetAttribute(argsList[0]);
			if (objs.Count == 0) return null;

			CloseLookFunctionality(objs);

			return new Response(new Dictionary<string, dynamic>
			{
				{"name", "close look"}
			}, objs, new Dictionary<string, dynamic>());
		}

		public Response Animate(string args)
		{
			var argsList = args.Split(GeneralConstants.ArgsSeparator);
			GameObject fig = Context.GetAttribute(argsList[1]);
			if (fig == null) return null;

			var state = argsList[0];
			Attributes attributes = Context.Instance.InitialAttributes[fig.name];

			void StartAnimatingFigure()
			{
				var infiniteRotationComponent = fig.GetComponent<InfiniteRotation>();
				if (infiniteRotationComponent == null)
				{
					infiniteRotationComponent = fig.AddComponent<InfiniteRotation>();
					infiniteRotationComponent.SetSpeed(25.0f);
				}
			}

			void StopAnimatingFigure()
			{
				var infiniteRotationComponent = fig.GetComponent<InfiniteRotation>();
				if (infiniteRotationComponent != null)
				{
					Destroy(infiniteRotationComponent);
				}

				StartCoroutine(IResetObject(fig, attributes, 1.0f));
			}

			var infiniteRotationComponent = fig.GetComponent<InfiniteRotation>();
			if (infiniteRotationComponent == null && state == "on")
			{
				StartCoroutine(IResetObject(fig, attributes, 1.0f));
			}

			StartCoroutine(state == "on"
				? IDelay(1.0f, StartAnimatingFigure)
				: IDelay(1.0f, StopAnimatingFigure));

			return new Response(new Dictionary<string, dynamic>
			{
				{"name", "animate"},
				{"extra", state}
			}, new List<GameObject> {fig}, new Dictionary<string, dynamic>());
		}

		public Response Visibility(string args)
		{
			var argsList = args.Split(GeneralConstants.ArgsSeparator);
			List<GameObject> objs = Context.GetAttribute(argsList[1]);
			if (objs.Count == 0) return null;

			var state = argsList[0];

			foreach (var obj in objs)
			{
				obj.GetComponent<MeshRenderer>().enabled = state == "on";
			}

			return new Response(new Dictionary<string, dynamic>
			{
				{"name", "visibility"},
				{"extra", state}
			}, objs, new Dictionary<string, dynamic>());
		}

		public Response SmoothInstall(string args)
		{
			var argsList = args.Split(GeneralConstants.ArgsSeparator);
			var valid = Context.GetAttribute(argsList[0]) == "yes";
			
			if (!valid) return null;
			
			List<Action> actionsList = Context.GetAttribute(argsList[1]);

			var action = actionsList[0];
			var figureID = Context.Instance.CurrentFigureID;
			var figure = GameObject.Find(figureID);
			var objA = HelperFunctions.FindObjectInFigure(figure, action.Components[0]);
			var objB = HelperFunctions.FindObjectInFigure(figure, action.Components[1]);

			StartCoroutine(Attach(figure, objA, objB, GeneralConstants.AttachTypes.SmoothInstall));
			
			return new Response(new Dictionary<string, dynamic>
			{
				{"name", "attach"}
			}, new List<GameObject>{objA, objB}, new Dictionary<string, dynamic>());
		}
		
		public Response Align(string args)
		{
			var argsList = args.Split(GeneralConstants.ArgsSeparator);
			var valid = Context.GetAttribute(argsList[0]) == "yes";
			
			if (!valid) return null;
			
			List<Action> actionsList = Context.GetAttribute(argsList[1]);

			var action = actionsList[0];
			var figureID = Context.Instance.CurrentFigureID;
			var figure = GameObject.Find(figureID);
			var objA = HelperFunctions.FindObjectInFigure(figure, action.Components[0]);
			var objB = HelperFunctions.FindObjectInFigure(figure, action.Components[1]);

			StartCoroutine(Attach(figure, objA, objB, GeneralConstants.AttachTypes.Align));
			
			return new Response(new Dictionary<string, dynamic>
			{
				{"name", "attach"}
			}, new List<GameObject>{objA, objB}, new Dictionary<string, dynamic>());
		}
		
		public Response StepInstall(string args)
		{
			var argsList = args.Split(GeneralConstants.ArgsSeparator);
			var valid = Context.GetAttribute(argsList[0]) == "yes";
			
			if (!valid) return null;
			
			List<Action> actionsList = Context.GetAttribute(argsList[1]);

			var action = actionsList[0];
			var figureID = Context.Instance.CurrentFigureID;
			var figure = GameObject.Find(figureID);
			var objA = HelperFunctions.FindObjectInFigure(figure, action.Components[0]);
			var objB = HelperFunctions.FindObjectInFigure(figure, action.Components[1]);

			StartCoroutine(Attach(figure, objA, objB, GeneralConstants.AttachTypes.StepInstall));
			
			return new Response(new Dictionary<string, dynamic>
			{
				{"name", "attach"}
			}, new List<GameObject>{objA, objB}, new Dictionary<string, dynamic>());
		}
		
		public Response SmoothScrew(string args)
		{
			var argsList = args.Split(GeneralConstants.ArgsSeparator);
			var valid = Context.GetAttribute(argsList[0]) == "yes";
			
			if (!valid) return null;
			
			List<Action> actionsList = Context.GetAttribute(argsList[1]);

			var action = actionsList[0];
			var figureID = Context.Instance.CurrentFigureID;
			var figure = GameObject.Find(figureID);
			var objA = HelperFunctions.FindObjectInFigure(figure, action.Components[0]);
			var objB = HelperFunctions.FindObjectInFigure(figure, action.Components[1]);

			StartCoroutine(Attach(figure, objA, objB, GeneralConstants.AttachTypes.SmoothScrew));
			
			return new Response(new Dictionary<string, dynamic>
			{
				{"name", "attach"}
			}, new List<GameObject>{objA, objB}, new Dictionary<string, dynamic>());
		}
		
		public Response StepScrew(string args)
		{
			var argsList = args.Split(GeneralConstants.ArgsSeparator);
			var valid = Context.GetAttribute(argsList[0]) == "yes";
			
			if (!valid) return null;
			
			List<Action> actionsList = Context.GetAttribute(argsList[1]);

			var action = actionsList[0];
			var figureID = Context.Instance.CurrentFigureID;
			var figure = GameObject.Find(figureID);
			var objA = HelperFunctions.FindObjectInFigure(figure, action.Components[0]);
			var objB = HelperFunctions.FindObjectInFigure(figure, action.Components[1]);

			StartCoroutine(Attach(figure, objA, objB, GeneralConstants.AttachTypes.StepScrew));
			
			return new Response(new Dictionary<string, dynamic>
			{
				{"name", "attach"}
			}, new List<GameObject>{objA, objB}, new Dictionary<string, dynamic>());
		}

		public List<Action> CreateActions(string args)
		{
			var argsList = args.Split(GeneralConstants.ArgsSeparator);
			var actionType = argsList[0];
			var refSpecified = argsList[1]; // always true for now
			List<string> idList = Context.GetAttribute(argsList[2]);

			var actionsList = new List<Action>();

			if (refSpecified == "yes")
			{
				var referenceId = idList[idList.Count - 1];

				for (var i = 0; i < idList.Count - 1; i++)
				{
						actionsList.Add(new Action
						{
							Name = actionType,
							Components = new List<string>{idList[i], referenceId}
						});
				}
			}

			return actionsList;
		}

		public string CheckActionsValidity(string args)
		{
			var argsList = args.Split(GeneralConstants.ArgsSeparator);
			List<Action> a = Context.GetAttribute(argsList[0]);
			List<Action> b = Context.GetAttribute(argsList[1]);
			
			// comparison logic
			
			return "yes";
		}

		private IEnumerator Attach(GameObject figure, GameObject objA, GameObject objB, GeneralConstants.AttachTypes type)
		{
			Vector3 delta;
			var steps = 3;
			
			var figureName = figure.name;
			var figureRfmName = figureName + "-RFM";
			var figureIfmName = figureName + "-IFM";

			var figureWrapper = figure.transform.parent;
			var rfm = GameObject.Find(figureRfmName);
			var ifm = GameObject.Find(figureIfmName);
			
			var rfmReferenceObjA = HelperFunctions.FindObjectInFigure(rfm, objA.name);
			var rfmReferenceObjB = HelperFunctions.FindObjectInFigure(rfm, objB.name);
			var rfmReferenceObjAPosition = rfmReferenceObjA.transform.position;
			var rfmReferenceObjBPosition = rfmReferenceObjB.transform.position;
					
			var diff = (rfmReferenceObjAPosition - rfmReferenceObjBPosition) / rfm.transform.localScale.x;
			var rfmFinalPosition = objB.transform.TransformPoint(diff);
			
			// (2.91591144, 6.9050045, 6.85317564)
			// (-0.0434860215, -0.142524645, 0.185333788)
				
			// var ifmReferenceObjA = HelperFunctions.FindObjectInFigure(ifm, objA.name);
			// var ifmReferenceObjB = HelperFunctions.FindObjectInFigure(ifm, objB.name);
			// var ifmReferenceObjAPosition = ifmReferenceObjA.transform.position;
			// var ifmReferenceObjBPosition = ifmReferenceObjB.transform.position;
			// 		
			// diff = ifmReferenceObjAPosition - ifmReferenceObjBPosition;
			// var ifmFinalPosition = (objB.transform.TransformPoint(diff) + figureWrapper.position) / ifm.transform.localScale.x;

			var routines = new List<IEnumerator>
			{
				IRotateObject(objA, objB.transform.rotation, 0.5f),
				IDelay(0.5f),
				IMoveObject(objA, rfmFinalPosition, 0.5f),
				IDelay(0.5f)
			};

			// switch (type)
			// {
			// 	case GeneralConstants.AttachTypes.SmoothInstall:
			// 	case GeneralConstants.AttachTypes.Align:
			// 		routines.Add(IMoveObject(objA, ifmFinalPosition, 1.0f));
			// 		break;
			// 	case GeneralConstants.AttachTypes.StepInstall:
			// 		delta = ifmFinalPosition - rfmFinalPosition;
			// 		
			// 		for (var i = 1; i <= steps; i++)
			// 		{
			// 			routines.Add(IMoveObject(objA, rfmFinalPosition + delta * i / steps, 0.5f));
			// 			routines.Add(IDelay(0.5f));
			// 		}
			//
			// 		break;
			// 	case GeneralConstants.AttachTypes.SmoothScrew:
			// 		routines.Add(IMoveObjectWithRotation(objA, ifmFinalPosition, 2.0f));
			// 		break;
			// 	case GeneralConstants.AttachTypes.StepScrew:
			// 		delta = ifmFinalPosition - rfmFinalPosition;
			//
			// 		for (var i = 1; i <= steps; i++)
			// 		{
			// 			routines.Add(IMoveObjectWithRotation(objA, rfmFinalPosition + delta * i / steps, 0.5f));
			// 			routines.Add(IDelay(0.5f));
			// 		}
			//
			// 		break;
			// 	default:
			// 		routines.Add(IMoveObject(objA, ifmFinalPosition, 1.0f));
			// 		break;
			// }
			
			// routines.Add(IAdjustStructure(objA, objB));
			
			StartCoroutine(Sequence(routines, 1.0f));

			yield return null;
		}

		private static void ScatterObjects(GameObject figure)
		{
			ScriptExecutor.IsScattered = true;
			
			var count = figure.transform.childCount;
			var deltaDegree = Mathf.Ceil(360.0f / count);

			for (var i = 0; i < count; i++)
			{
				var child = figure.transform.GetChild(i);
				
				var radius = Random.Range(8.0f, 10.0f);	
				var newPosX = radius * Mathf.Sin(deltaDegree * i);
				var newPosZ = radius * Mathf.Cos(deltaDegree * i);
				child.transform.position += new Vector3(newPosX, 0, newPosZ);

				var newRotX = Random.Range(15.0f, 30.0f);
				var newRotY = Random.Range(15.0f, 30.0f);
				var newRotZ = Random.Range(15.0f, 30.0f);
				child.transform.rotation = Quaternion.Euler(newRotX, newRotY, newRotZ);
			}
		}

		private void CloseLookFunctionality(List<GameObject> objs)
		{
			var parentFigure = objs[0].transform.parent.gameObject;
			var cloneObjects = new List<GameObject>();

			void CreateCloneObjects()
			{
				for (var i = 0; i < parentFigure.transform.childCount; i++)
				{
					var child = parentFigure.transform.GetChild(i);
					child.GetComponent<MeshRenderer>().enabled = false;
				}

				foreach (var obj in objs)
				{
					var cloneObject = Instantiate(obj);
					cloneObject.tag = "CloneObject";
					cloneObject.GetComponent<MeshRenderer>().enabled = true;
					cloneObject.transform.rotation = parentFigure.transform.rotation;
					cloneObject.transform.position = parentFigure.transform.position;
					cloneObjects.Add(cloneObject);
				}
			}

			void MoveCloneObjects()
			{
				var coroutines = new List<IEnumerator>();
				var width = Math.Max(20, 5 * cloneObjects.Count);
				for (var i = 0; i < cloneObjects.Count; i++)
				{
					coroutines.Add(IMoveObject(cloneObjects[i],
						new Vector3((i + 1) * width / (cloneObjects.Count + 1) - width / 2, 5.0f, 0), 0.2f));
				}

				StartCoroutine(Sequence(coroutines));
			}

			Attributes attributes = Context.Instance.InitialAttributes[parentFigure.name];
			var coroutines = new List<IEnumerator>
			{
				IResetObject(parentFigure, attributes, 0.2f),
				IDelay(0.2f, CreateCloneObjects),
				IDelay(0.2f, MoveCloneObjects)
			};

			StartCoroutine(Sequence(coroutines));
		}

		private static IEnumerator IDelay(float duration, State.VoidFunction method = null)
		{
			yield return new WaitForSeconds(duration);
			method?.Invoke();
		}

		private static IEnumerator IResetObject(GameObject obj, Attributes attributes, float duration)
		{
			ScriptExecutor.IsScattered = false;
			
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

			foreach(var o in obj.transform.GetComponentsInChildren<Transform>())
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

			ScriptExecutor.IsScattered = false;

			float counter = 0;
			while (counter < duration)
			{
				counter += Time.deltaTime;

				obj.transform.rotation = Quaternion.Lerp(currentRot, attributes.Rotation, counter / duration);
				obj.transform.localScale = Vector3.Lerp(currentScale, attributes.Scale, counter / duration);
				// obj.transform.position = Vector3.Lerp(obj.transform.position, attributes.Position, counter / duration);

				yield return null;
			}
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
		
		// smoothly move object
		private static IEnumerator IMoveObject(GameObject obj, Vector3 finalPosition, float duration)
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
		
		private static IEnumerator IMoveObjectWithRotation(GameObject obj, Vector3 finalPosition, float duration)
		{
			if (ScriptExecutor.IsInAction) yield break;
			ScriptExecutor.IsInAction = true;
			
			var infiniteRotationComponents = obj.GetComponents<InfiniteRotation>();
			InfiniteRotation infiniteRotationComponent;
			if (infiniteRotationComponents.Length == 0)
			{ 
				infiniteRotationComponent = obj.AddComponent<InfiniteRotation>();
			} else if (infiniteRotationComponents.Length > 1)
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
			
			
			infiniteRotationComponent.SetSpeed(100);
			infiniteRotationComponent.SetDirection(Vector3.forward);

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
		private static IEnumerator IRotateObject(dynamic obj, Quaternion finalRotation, float duration)
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

		private static IEnumerator IScaleObject(dynamic obj, Vector3 finalScale, float duration)
		{
			if (ScriptExecutor.IsInAction) yield break;
			ScriptExecutor.IsInAction = true;

			var currentScale = obj.transform.localScale;
			float counter = 0;
			while (counter < 1)
			{
				counter += Time.deltaTime / duration;

				obj.transform.localScale = Vector3.Lerp(currentScale, finalScale, counter);
				yield return null;
			}

			ScriptExecutor.IsInAction = false;
		}

		// smoothly change FoV of camera
		private static IEnumerator IChangeFieldOfViewByValue(Camera camera, float finalFoV, float duration)
		{
			if (ScriptExecutor.IsInAction) yield break;

			ScriptExecutor.IsInAction = true;

			var currentFOV = camera.fieldOfView;
			float counter = 0;
			while (counter < duration)
			{
				counter += Time.deltaTime;

				camera.fieldOfView = Mathf.Lerp(currentFOV, finalFoV, counter / duration);
				yield return null;
			}

			ScriptExecutor.IsInAction = false;
		}
	}
}