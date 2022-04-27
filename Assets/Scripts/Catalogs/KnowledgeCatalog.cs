using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

namespace Catalogs
{
    public interface IKnowledgeCatalogInterface
    {
        List<JSONNode> FilterAttr(string attr, string attrValue, List<JSONNode> dataObjects);
        List<JSONNode> FilterType(string type, List<JSONNode> dataObjects);
        string QueryAttr(string attr, JSONNode dataObject);
        string ShowInfo(List<JSONNode> dataObjects);
    }
    public class KnowledgeCatalog : IKnowledgeCatalogInterface
    {
        private List<JSONNode> root;
        
        private void Awake()
        {
            InitRoot();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                List<JSONNode> dataObjects1 = FilterType("tasks", root);
                List<JSONNode> dataObjects2 = FilterType("subtasks", dataObjects1);
                List<JSONNode> dataObjects3 = FilterType("instructions", dataObjects2);
                List<JSONNode> dataObjects4 = FilterType("actions", dataObjects3);
                
                foreach (JSONNode dataObject in dataObjects4)
                {
                    Debug.Log(dataObject);
                }
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                List<JSONNode> dataObjects1 = FilterType("tasks", root);
                List<JSONNode> dataObjects2 = FilterType("subtasks", dataObjects1);
                List<JSONNode> dataObjects3 = FilterAttr("subtask_id", "32-11-61-020-007", dataObjects2);
                string res = ShowInfo(dataObjects3);
                Debug.Log(res);
            }
            
        }

        private void InitRoot()
        {
            root = new List<JSONNode>();
            
            string jsonDocument = "knowledge-v2";
            var jsonContent = Resources.Load<TextAsset>(jsonDocument);
            
            JSONNode items = JSON.Parse(jsonContent.ToString());
            
            foreach (JSONNode item in items)
            {
                root.Add(item);
            }
        }

        public List<JSONNode> FilterAttr(string attr, string attrValue, List<JSONNode> dataObjects)
        {
            List<JSONNode> resultObjects = new List<JSONNode>();
            foreach (JSONNode dataObject in dataObjects)
            {
                if (dataObject[attr] == attrValue)
                {
                    resultObjects.Add(dataObject);
                }
            }
            return resultObjects;
        }

        public List<JSONNode> FilterType(string type, List<JSONNode> dataObjects)
        {
            List<JSONNode> resultObjects = new List<JSONNode>();
            foreach (JSONNode dataObject in dataObjects)
            {
                foreach (JSONNode item in dataObject[type])
                {
                    resultObjects.Add(item);    
                }
            }
            return resultObjects;
        }

        public string QueryAttr(string attr, JSONNode dataObject)
        {
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

