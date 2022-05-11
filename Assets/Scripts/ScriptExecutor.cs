using UnityEngine;
using System.Collections.Generic;
using Constants;
using UnityEngine.UI;

public class ScriptExecutor : MonoBehaviour
{
    public static bool IsInAction = false;

    private static Text _queryText;
    private static Text _replyText;
    private static Text _programsText;
    private static Text _knowledgeText;

    private static List<QueryMeta> _queryMetas;
    private static int _currentQueryMetaId;
    

    private void Start()
    {
        Context.Instance.CurrentFigureID = "402-32-11-61-990-802-C";
        Context.Instance.CurrentTaskID = "32-11-61-400-802";
        Context.Instance.CurrentSubtaskID = "32-11-61-420-014";
        Context.Instance.CurrentInstructionOrder = "1";
        
        var textObjects = FindObjectsOfType<Text>();

        foreach (var textObj in textObjects)
        {
            switch (textObj.name)
            {
                case "QueryText":
                    _queryText = textObj;
                    break;
                case "ProgramsText":
                    _programsText = textObj;
                    break;
                case "ReplyText":
                    _replyText = textObj;
                    break;
                case "KnowledgeText":
                    _knowledgeText = textObj;
                    break;
            }
        }

        HideAllUnnecessaryGameObjects();
        InitProgram();
    }

    private void Update()
    {
        // execute
        if (Input.GetKeyDown(KeyCode.E))
        {
            ExecuteQuery();
        }
    }

    private static void HideAllUnnecessaryGameObjects()
    {
        var allFigures = GameObject.FindGameObjectsWithTag("Figure");

        foreach (var figure in allFigures)
        {
            var wrapper = figure.transform.parent;
            wrapper.localScale = figure.name == Context.Instance.CurrentFigureID ? Vector3.one : Vector3.zero;
        }

        var referenceObjects = GameObject.FindGameObjectsWithTag("ReferenceObject");

        foreach (var obj in referenceObjects)
        {
            var meshRenderer = obj.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.enabled = false;
            }
        }
    }

    public static void InitProgram()
    {
        _queryMetas = Queries.GetAllQueries();

        _currentQueryMetaId = 0;
        _queryText.text = _queryMetas[_currentQueryMetaId].Query;
        
        var mainCamera = Camera.main;
        Context.Instance.Camera = mainCamera;
        if (mainCamera != null)
        {
            var cameraLocation = GameObject.Find(Context.Instance.CurrentFigureID + "-Wrapper")?.transform.Find("CameraLocation")?.transform;
            var cameraTransform = mainCamera.transform;
            
            if (cameraLocation)
            {
                cameraTransform.position = cameraLocation.position;
                cameraTransform.rotation = cameraLocation.rotation;
                cameraTransform.localScale = cameraLocation.localScale;
            }

            var cameraAttributes = new Attributes
            {
                Position =  cameraTransform.position,
                Rotation = cameraTransform.rotation,
                Scale = cameraTransform.localScale,
                FoV =  mainCamera.fieldOfView
            };
            if (Context.Instance.InitialAttributes.ContainsKey("camera"))
            {
                Context.Instance.InitialAttributes["camera"] = cameraAttributes;
            }
            else
            {
                Context.Instance.InitialAttributes.Add("camera", cameraAttributes);
            }
        }
        
        var allFigures = GameObject.FindGameObjectsWithTag("Figure");

        foreach (var fig in allFigures)
        {
            Context.Instance.InitialAttributes[fig.name] = new Attributes
            {
                Rotation = fig.transform.rotation,
                Scale = fig.transform.localScale
            };

            foreach (Transform child in fig.transform)
            {
                var keyName = fig.name + GeneralConstants.ArgsSeparator + child.name;
                var childAttributes = new Attributes
                {
                    Position = child.position,
                    Rotation = child.rotation,
                    Scale = child.localScale
                };

                if (Context.Instance.InitialAttributes.ContainsKey(keyName))
                {
                    Context.Instance.InitialAttributes[keyName] = childAttributes;
                }
                else
                {
                    Context.Instance.InitialAttributes.Add(keyName, childAttributes);
                }
            }
        }
    }
    
    public static void ExecuteQuery()
    {
        var currentQueryMeta = _queryMetas[_currentQueryMetaId];
            
        _queryText.text = currentQueryMeta.Query;
        _programsText.text = string.Join("\n", currentQueryMeta.Programs);
        _knowledgeText.text = currentQueryMeta.Knowledge;

        var result = FunctionalProgramsExecutor.Instance.Execute(currentQueryMeta);
        print($"result: {result}");
        _replyText.text = result;
    }
    
    public static void NextInstruction()
    {
        _currentQueryMetaId += 1;

        if (_currentQueryMetaId >= _queryMetas.Count)
        {
            _currentQueryMetaId = 0;
        }

        _queryText.text = _queryMetas[_currentQueryMetaId].Query;
        _programsText.text = "";
        _replyText.text = "";
    }
    
    public static void PreviousInstruction()
    {
        _currentQueryMetaId -= 1;

        if (_currentQueryMetaId < 0)
        {
            _currentQueryMetaId = _queryMetas.Count - 1;
        }

        _queryText.text = _queryMetas[_currentQueryMetaId].Query;
        _programsText.text = "";
        _replyText.text = "";
    }
}
