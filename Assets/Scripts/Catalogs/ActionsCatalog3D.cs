using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Catalogs
{
    public interface IActionsCatalog3DInterface
    {
        List<string> FilterIds(string args);
        List<GameObject> Filter3DAttr(string args);
        GameObject Reset(string args);
        List<GameObject> Highlight(string args);
        GameObject Rotate(string args);
        GameObject Scale(string args);
        GameObject ShowSide(string args);
        GameObject SideBySideLook(string args);
        List<GameObject> CloseLook(string args);
        GameObject Animate(string args);
        List<GameObject> Visibility(string args);
    }

    public class ActionsCatalog3D : MonoBehaviour, IActionsCatalog3DInterface
    {
        private IEnumerator Sequence(List<IEnumerator> coroutines)
        {
            foreach (var c in coroutines) yield return StartCoroutine(c);
        }

        private bool ExtractState(string query)
        {
            var positiveSuffixes = new[]{"up", "on"};
            var negativeSuffixes = new[]{"down", "off"};
            var positivePrefixes = new[] {"show"};
            var negativePrefixes = new[] {"remove", "hide"};
            var positiveInnerWords = new[] {"visible"};
            var negativeInnerWords = new[] {"invisible"};

            var words = query.Split(' ');
            var stateSuffix = words[1].ToLower();
            var statePrefix = words[0].ToLower();
            
            if (positiveSuffixes.Contains(stateSuffix) || positivePrefixes.Contains(statePrefix) || words.Any(x => positiveInnerWords.Any(y => y == x)))
            {
                return true;
            }

            if (negativeSuffixes.Contains(stateSuffix) || negativePrefixes.Contains(statePrefix) || words.Any(x => negativeInnerWords.Any(y => y == x)))
            {
                return false;
            }

            return false;
        }

        private string ExtractSide(string query)
        {
            var words = query.Split(' ').ToList();
            var sideIndex = words.IndexOf("side") - 1;

            return words[sideIndex].ToLower();
        }

        private static void DestroyCloneObjects()
        {
            var cloneObjects = GameObject.FindGameObjectsWithTag("CloneObject");

            foreach (var cloneObj in cloneObjects)
            {
                Destroy(cloneObj);
            }
        }
        
        public List<string> FilterIds(string args)
        {
            var argsList = args.Split('#');
            var prev = Context.GetAttribute(argsList[0]);
            var varName = argsList[1];

            var ids = new List<string>();
            var rest = new List<string>();
            foreach (var number in prev)
            {
                if (Regex.IsMatch(number, @"((\d|-)+[A-Z]+|(\[\d+\]))", RegexOptions.IgnoreCase))
                {
                    ids.Add(number);
                }
                else
                {
                    rest.Add(number);
                }
            }

            switch (varName)
            {
                case "var1":
                    Context.Instance.Var1 = rest;
                    break;
                case "var2":
                    Context.Instance.Var2 = rest;
                    break;
            }

            return ids;
        }
        
        // filter 3d attr
        public List<GameObject> Filter3DAttr(string args)
        {
            var argsList = args.Split('#');
            var attrName = argsList[0];
            var prev = Context.HasAttribute(argsList[1]) ? Context.GetAttribute(argsList[1]) : argsList[1];
            var parent = Context.GetAttribute(argsList[2]);


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
                            if(((GameObject) obj).CompareTag("Figure"))
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

            foreach (var obj in allObjects)
            foreach (var id in ids)
                if (obj.name.Contains($"{id}"))
                    foundObs.Add(obj);

            return foundObs;
        }

        // close look
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
        
        // rotate
        public GameObject Rotate(string args) 
        {
            var argsList = args.Split('#');
            GameObject obj = Context.GetAttribute(argsList[0]);
            if (obj == null) return null;

            List<string> restArgs = Context.GetAttribute(argsList[1]);
            var degree = float.Parse(restArgs[0]);
            var axis = Regex.Match(Context.Instance.Query, @"[XYZ] axis").Value;
            var duration = 1.0f;
            
            var rotation = obj.transform.rotation;
            var rotationX = axis.StartsWith("X") ? degree : 0;
            var rotationY = axis.StartsWith("Y") ? degree : 0;
            var rotationZ = axis.StartsWith("Z") ? degree : 0;

            var newRotation = rotation * Quaternion.Euler(rotationX, rotationY, rotationZ);
            StartCoroutine(IRotateObject(obj, newRotation, duration));

            return obj;
        }

        public GameObject Scale(string args)
        {
            var argsList = args.Split('#');
            GameObject obj = Context.GetAttribute(argsList[0]);
            if (obj == null) return null;
            
            List<string> restArgs = Context.GetAttribute(argsList[1]);
            var state = ExtractState(Context.Instance.Query);
            var scaleRatio = float.Parse(restArgs[0]);

            var currentLocalScale = obj.transform.localScale;
            var currentLocalScaleX = currentLocalScale.x;
            var currentLocalScaleY = currentLocalScale.y;
            var currentLocalScaleZ = currentLocalScale.z;
            var change = scaleRatio < 1 
                ? state ? 1 + scaleRatio : 1 - scaleRatio
                : state ? scaleRatio : Mathf.Round(100f / scaleRatio) / 100f;
        
            var finalScale = new Vector3(currentLocalScaleX * change, currentLocalScaleY * change, currentLocalScaleZ * change);
        
            StartCoroutine(IScaleObject(obj, finalScale, 1.0f));

            return obj;
        }
    
        // reset
        public GameObject Reset(string args)
        {
            var argsList = args.Split('#');

            GameObject obj = Context.GetAttribute(argsList[0]);
            if (obj == null) return null;
        
            Attributes attributes = Context.Instance.InitialAttributes[obj.name];
            var cameraAttributes = Context.Instance.InitialAttributes["camera"];

            var coroutines = new List<IEnumerator>
            {
                IResetObject(obj, attributes, 1.0f),
                IResetObject(Context.Instance.Camera, cameraAttributes, 1.0f)
            };

            if (cameraAttributes.FoV != 0)
                coroutines.Add(IChangeFieldOfViewByValue(Context.Instance.Camera, cameraAttributes.FoV, 1.0f));

            StartCoroutine(Sequence(coroutines));

            return obj;
        }
    
        // highlight
        public List<GameObject> Highlight(string args)
        {
            var argsList = args.Split('#');
        
            var state = ExtractState(Context.Instance.Query);

            List<GameObject> objs = Context.GetAttribute(argsList[0]);
            if (objs.Count == 0) return null;
        
            HighlightObject(objs, state, 5.0f, Color.blue);

            return objs;
        }

        private void HighlightObject(List<GameObject> objs, bool state = true, float highlightWidth = 1.0f, Color highlightColor = default)
        {
            foreach (var obj in objs)
            {
                var outlineComponent = obj.GetComponent<Outline>();
                if (outlineComponent == null) outlineComponent = obj.AddComponent<Outline>();

                switch (state)
                {
                    case true:
                        outlineComponent.OutlineMode = Outline.Mode.OutlineAll;
                        outlineComponent.OutlineWidth = highlightWidth;
                        outlineComponent.OutlineColor = highlightColor;

                        outlineComponent.enabled = true;
                        break;
                    case false:
                        outlineComponent.enabled = false;
                        break;
                }
            }
        }

        public GameObject ShowSide(string args)
        {
            var argsList = args.Split('#');
            
            GameObject obj = Context.GetAttribute(argsList[0]);
            if (obj == null) return null;
            
            var figureSide = ExtractSide(Context.Instance.Query);

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
            
            Attributes attributes = Context.Instance.InitialAttributes[obj.name];

            var coroutines = new List<IEnumerator>
            {
                IRotateObject(obj, sideRotation, 1.0f)
            };
            
            StartCoroutine(Sequence(coroutines));

            return obj;
        }

        public GameObject SideBySideLook(string args)
        {
            var argsList = args.Split(' ');
            var fig = Context.GetAttribute(argsList[0]);
            if (fig == null) return null;
            //
            // var figObjects = 
            //
            // void MoveObjects()
            // {
            //     var coroutines = new List<IEnumerator>();
            //     var width = Math.Max(20, 5 * cloneObjects.Count);
            //     for (var i = 0; i < cloneObjects.Count; i++)
            //     {
            //         coroutines.Add(IMoveObject(cloneObjects[i], new Vector3((i+1) * width / (cloneObjects.Count + 1) - width / 2, 5.0f, 0), 0.2f));
            //     }
            //
            //     StartCoroutine(Sequence(coroutines));
            // }
            //
            // Attributes attributes = Context.Instance.InitialAttributes[fig.name];
            // var coroutines = new List<IEnumerator>
            // {
            //     IResetObject(fig, attributes, 0.2f),
            //     IDelay(0.2f, MoveObjects)
            // };
            //
            // StartCoroutine(Sequence(coroutines));
            //
            //
            return fig;
        }
        
        public List<GameObject> CloseLook(string args)
        {
            var argsList = args.Split('#');

            List<GameObject> objs = Context.GetAttribute(argsList[0]);
            if (objs.Count == 0) return null;
            
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
                    coroutines.Add(IMoveObject(cloneObjects[i], new Vector3((i+1) * width / (cloneObjects.Count + 1) - width / 2, 5.0f, 0), 0.2f));
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
            
            return objs;
        }

        public GameObject Animate(string args)
        {
            var argsList = args.Split('#');
            GameObject fig = Context.GetAttribute(argsList[0]);
            if (fig == null) return null;
            
            var state = ExtractState(Context.Instance.Query);
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
            if (infiniteRotationComponent == null && state)
            {
                StartCoroutine(IResetObject(fig, attributes, 1.0f));
            }
        
            StartCoroutine(state
                ? IDelay(1.0f, StartAnimatingFigure)
                : IDelay(1.0f, StopAnimatingFigure));

            return fig;
        }

        public List<GameObject> Visibility(string args)
        {
            var argsList = args.Split('#');
            List<GameObject> objs = Context.GetAttribute(argsList[0]);
            if (objs.Count == 0) return null;
            
            var state = ExtractState(Context.Instance.Query);
            
            foreach (var obj in objs)
            {
                obj.GetComponent<MeshRenderer>().enabled = state;
            }

            return objs;
        }

        private static IEnumerator IDelay(float duration, State.VoidFunction method = null)
        {
            yield return new WaitForSeconds(duration);
            method?.Invoke();
        }
    
        private static IEnumerator IResetObject(dynamic obj, Attributes attributes, float duration)
        {
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

            var objs = GameObject.FindGameObjectsWithTag("Object");
            foreach (var o in objs)
            {
                o.GetComponent<MeshRenderer>().enabled = true;
            }

            float counter = 0;
            while (counter < duration)
            {
                counter += Time.deltaTime;

                obj.transform.rotation = Quaternion.Lerp(currentRot, attributes.Rotation, counter / duration);
                obj.transform.localScale = Vector3.Lerp(currentScale, attributes.Scale, counter / duration);
                // obj.transform.position = Vector3.Lerp(obj.transform.position, attributes.Position, counter / duration);

                yield return null;
            }

            ScriptExecutor.IsInAction = false;
        }
    
        // smoothly move object
        private static IEnumerator IMoveObject(dynamic obj, Vector3 finalPosition, float duration)
        {
            if (ScriptExecutor.IsInAction) yield break;
            ScriptExecutor.IsInAction = true;

            float counter = 0;
            while (counter < duration)
            {
                counter += Time.deltaTime;

                obj.transform.position =Vector3.Lerp(obj.transform.position, finalPosition, counter / duration);
                yield return null;
            }

            ScriptExecutor.IsInAction = false;
        }
    
        // smoothly change rotation of camera
        private static IEnumerator IRotateObject(dynamic obj, Quaternion finalRotation, float duration)
        {
            if (ScriptExecutor.IsInAction) yield break;
            ScriptExecutor.IsInAction = true;

            float counter = 0;
            while (counter < duration)
            {
                counter += Time.deltaTime;

                obj.transform.rotation = Quaternion.Slerp(obj.transform.rotation, finalRotation, counter / duration);
                yield return null;
            }

            ScriptExecutor.IsInAction = false;
        }
    
        private static IEnumerator IScaleObject(dynamic obj, Vector3 finalScale, float duration)
        {
            if (ScriptExecutor.IsInAction) yield break;
            ScriptExecutor.IsInAction = true;

            float counter = 0;
            while (counter < duration)
            {
                counter += Time.deltaTime;

                obj.transform.localScale = Vector3.Lerp(obj.transform.localScale, finalScale, counter / duration);
                yield return null;
            }

            ScriptExecutor.IsInAction = false;
        }

        // smoothly change FoV of camera
        private static IEnumerator IChangeFieldOfViewByValue(Camera camera, float finalFoV, float duration)
        {
            if (ScriptExecutor.IsInAction) yield break;

            ScriptExecutor.IsInAction = true;

            float counter = 0;
            while (counter < duration)
            {
                counter += Time.deltaTime;

                camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, finalFoV, counter / duration);
                yield return null;
            }

            ScriptExecutor.IsInAction = false;
        }
    }
}