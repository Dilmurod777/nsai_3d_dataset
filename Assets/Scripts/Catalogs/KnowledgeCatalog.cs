using System;
using System.Collections;
using System.Collections.Generic;
using Constants;
using UnityEngine;
using SimpleJSON;

namespace Catalogs
{
	public interface IKnowledgeCatalogInterface
	{
		List<JSONNode> FilterAttr(string args);
		List<JSONNode> FilterType(string args);
		string QueryAttr(string args);
		string ShowInfo(List<JSONNode> dataObjects);
	}

	public class KnowledgeCatalog : IKnowledgeCatalogInterface
	{
		public KnowledgeCatalog()
		{
			InitRoot();
		}

		private void Update()
		{
			// if (Input.GetKeyDown(KeyCode.C))
			// {
			//     List<JSONNode> dataObjects1 = FilterType("tasks", root);
			//     List<JSONNode> dataObjects2 = FilterType("subtasks", dataObjects1);
			//     List<JSONNode> dataObjects3 = FilterType("instructions", dataObjects2);
			//     List<JSONNode> dataObjects4 = FilterType("actions", dataObjects3);
			//     
			//     foreach (JSONNode dataObject in dataObjects4)
			//     {
			//         Debug.Log(dataObject);
			//     }
			// }
			//
			// if (Input.GetKeyDown(KeyCode.V))
			// {
			//     List<JSONNode> dataObjects1 = FilterType("tasks", root);
			//     List<JSONNode> dataObjects2 = FilterType("subtasks", dataObjects1);
			//     List<JSONNode> dataObjects3 = FilterAttr("subtask_id", "32-11-61-020-007", dataObjects2);
			//     string res = ShowInfo(dataObjects3);
			//     Debug.Log(res);
			// }
		}

		private void InitRoot()
		{
			Debug.Log("InitRoot");
			Context.Instance.Root = new List<JSONNode>();

			string jsonDocument = "knowledge-v3";
			var jsonContent = Resources.Load<TextAsset>(jsonDocument);

			JSONNode items = JSON.Parse(jsonContent.ToString());

			foreach (JSONNode item in items)
			{
				Context.Instance.Root.Add(item);
			}
		}

		public List<JSONNode> FilterAttr(string args)
		{
			ScriptExecutor.AddNewProgram("FilterAttr " + args.Replace(GeneralConstants.ArgsSeparator.ToString(), " "));
			
			var argsList = args.Split(GeneralConstants.ArgsSeparator);
			var attr = argsList[0];
			var attrValue = Context.GetAttribute(argsList[1]).ToString();
			List<JSONNode> dataObjects = Context.GetAttribute(argsList[2]);

			var resultObjects = new List<JSONNode>();
			for (var i = 0; i < dataObjects.Count; i++)
			{
				if (dataObjects[i][attr] == attrValue)
				{
					resultObjects.Add(dataObjects[i]);
					ScriptExecutor.SetKnowledgeText(HelperFunctions.GetValueFromJSONNodeByKey("content", dataObjects[i]));
					ScriptExecutor.SetKnowledgeInfo();


					if (attr == "order")
					{
						Context.Instance.CurrentInstructionOrder += 1;
					}
				}
			}

			return resultObjects;
		}

		public List<JSONNode> FilterType(string args)
		{
			ScriptExecutor.AddNewProgram("FilterType " + args.Replace(GeneralConstants.ArgsSeparator.ToString(), " "));

			var argsList = args.Split(GeneralConstants.ArgsSeparator);
			var type = argsList[0];
			List<JSONNode> dataObjects = Context.GetAttribute(argsList[1]);

			var resultObjects = new List<JSONNode>();
			foreach (var dataObject in dataObjects)
			{
				foreach (JSONNode item in dataObject[type])
				{
					resultObjects.Add(item);
				}
			}

			return resultObjects;
		}

		public string QueryAttr(string args)
		{
			ScriptExecutor.AddNewProgram("QueryAttr " + args.Replace(GeneralConstants.ArgsSeparator.ToString(), " "));
			
			var argsList = args.Split(GeneralConstants.ArgsSeparator);
			var attr = argsList[0];
			JSONNode dataObject = Context.GetAttribute(argsList[1]);

			string result = dataObject[attr];
			return result;
		}

		public string ShowInfo(List<JSONNode> dataObjects)
		{
			string result = "";
			foreach (JSONNode dataObject in dataObjects)
			{
				foreach (var item in dataObject)
				{
					if (!item.Value.IsArray)
					{
						result += item.Key + ": " + item.Value + "\n";
					}
				}
			}

			return result;
		}
	}
}