using UnityEngine;
using System.Collections.Generic;
using Constants;
using UnityEngine.UI;

public class ScriptExecutor : MonoBehaviour
{
    public static bool IsInAction = false;
    public static bool IsScattered = false;

    private static Text _queryText;
    private static Text _replyText;
    private static Text _programsText;
    private static Text _knowledgeText;

    private static List<QueryMeta> _queryMetas;
    private static int _currentQueryMetaId;

    private void Start()
    {
        Context.Instance.CurrentFigureID = "402-32-11-61-990-802-A";
        Context.Instance.CurrentTaskID = "1";
        Context.Instance.CurrentSubtaskID = "2";
        Context.Instance.CurrentInstructionOrder = "3";
        
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

        _queryMetas = Queries.GetAllQueries();
        
        _queryText.text = _queryMetas[_currentQueryMetaId].Query;

        var mainCamera = Camera.main;
        Context.Instance.Camera = mainCamera;
        if (mainCamera != null)
        {
            var cameraTransform = mainCamera.transform;
            Context.Instance.InitialAttributes.Add("camera", new Attributes
            {
                Position =  cameraTransform.position,
                Rotation = cameraTransform.rotation,
                Scale = cameraTransform.localScale,
                FoV =  mainCamera.fieldOfView
            });
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
                Context.Instance.InitialAttributes[fig.name + GeneralConstants.ArgsSeparator + child.name] = new Attributes
                {
                    Position = child.position,
                    Rotation = child.rotation,
                    Scale = child.localScale
                };
            }
        }
    }

    private void Update()
    {
        // execute
        if (Input.GetKeyDown(KeyCode.E))
        {
            ExecuteQuery();
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
