using System.Collections.Generic;
using Constants;
using SimpleJSON;
using UnityEngine;
using Action = Constants.Action;

namespace Catalogs
{
    public class Catalog : IGeneralCatalogInterface, IActionsCatalog3DInterface, IKnowledgeCatalogInterface
    {
        private static Catalog _instance;
        private readonly ActionsCatalog3D _actionsCatalog3D;
        private readonly GeneralCatalog _generalCatalog;
        private readonly KnowledgeCatalog _knowledgeCatalog;

        private Catalog()
        {
            var gameObject = Object.FindObjectOfType<ScriptExecutor>().gameObject;

            _generalCatalog = new GeneralCatalog();
            _actionsCatalog3D = gameObject.GetComponent<ActionsCatalog3D>()
                ? gameObject.GetComponent<ActionsCatalog3D>()
                : gameObject.AddComponent<ActionsCatalog3D>();
            _knowledgeCatalog = new KnowledgeCatalog();
        }

        public static Catalog Instance => _instance ??= new Catalog();

        public Response Reset(string args)
        {
            return _actionsCatalog3D.Reset(args);
        }

        public Response Highlight(string args)
        {
            return _actionsCatalog3D.Highlight(args);
        }

        public Response Rotate(string args)
        {
            return _actionsCatalog3D.Rotate(args);
        }

        public Response Scale(string args)
        {
            return _actionsCatalog3D.Scale(args);
        }

        public Response ShowSide(string args)
        {
            return _actionsCatalog3D.ShowSide(args);
        }

        public Response SideBySideLook(string args)
        {
            return _actionsCatalog3D.SideBySideLook(args);
        }

        public Response CloseLook(string args)
        {
            return _actionsCatalog3D.CloseLook(args);
        }

        public Response Animate(string args)
        {
            return _actionsCatalog3D.Animate(args);
        }

        public Response Visibility(string args)
        {
            return _actionsCatalog3D.Visibility(args);
        }

        public Response SmoothInstall(string args)
        {
            return _actionsCatalog3D.SmoothInstall(args);
        }
        
        public Response Align(string args)
        {
            return _actionsCatalog3D.Align(args);
        }
        
        public Response StepInstall(string args)
        {
            return _actionsCatalog3D.StepInstall(args);
        }
        
        public Response SmoothScrew(string args)
        {
            return _actionsCatalog3D.SmoothScrew(args);
        }
        
        public Response StepScrew(string args)
        {
            return _actionsCatalog3D.StepScrew(args);
        }

        public List<Action> CreateActions(string args)
        {
            return _actionsCatalog3D.CreateActions(args);
        }

        public string CheckActionsValidity(string args)
        {
            return _actionsCatalog3D.CheckActionsValidity(args);
        }

        public List<GameObject> Filter3DAttr(string args)
        {
            return _actionsCatalog3D.Filter3DAttr(args);
        }

        public object SaveVal2Var(string args)
        {
            return _generalCatalog.SaveVal2Var(args);
        }

        public int Count(object[] objects)
        {
            return _generalCatalog.Count(objects);
        }

        public bool Exist(object[] objects)
        {
            return _generalCatalog.Exist(objects);
        }

        public object Unique(string args)
        {
            return _generalCatalog.Unique(args);
        }

        public List<string> ExtractNumbers(string value)
        {
            return _generalCatalog.ExtractNumbers(value);
        }

        public List<string> ExtractID(string args)
        {
            return _generalCatalog.ExtractID(args);
        }

        public string Same(string args)
        {
            return _generalCatalog.Same(args);
        }

        public List<JSONNode> FilterAttr(string args)
        {
            return _knowledgeCatalog.FilterAttr(args);
        }

        public List<JSONNode> FilterType(string args)
        {
            return _knowledgeCatalog.FilterType(args);
        }

        public string QueryAttr(string args)
        {
            return _knowledgeCatalog.QueryAttr(args);
        }

        public string ShowInfo(List<JSONNode> dataObjects)
        {
            return _knowledgeCatalog.ShowInfo(dataObjects);
        }
    }
}