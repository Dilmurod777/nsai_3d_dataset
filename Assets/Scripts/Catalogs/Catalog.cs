using System.Collections.Generic;
using Constants;
using UnityEngine;

namespace Catalogs
{
    public class Catalog : IGeneralCatalogInterface, IActionsCatalog3DInterface
    {
        private static Catalog _instance;
        private readonly ActionsCatalog3D _actionsCatalog3D;
        private readonly GeneralCatalog _generalCatalog;

        private Catalog()
        {
            var gameObject = Object.FindObjectOfType<ScriptExecutor>().gameObject;

            _generalCatalog = new GeneralCatalog();
            _actionsCatalog3D = gameObject.GetComponent<ActionsCatalog3D>()
                ? gameObject.GetComponent<ActionsCatalog3D>()
                : gameObject.AddComponent<ActionsCatalog3D>();
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
    }
}