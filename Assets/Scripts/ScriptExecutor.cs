using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ScriptExecutor : MonoBehaviour
{
    public static bool IsInAction = false;

    private List<string> _currentObjectIds;
    private string _currentFigureId;

    public float zoomDuration = 1.0f;
    public float resetDuration = 1.0f;

    public State.FigureSide figureSide;
    public float figureSideDuration = 1.0f;

    public float closeLookDuration = 1.0f;

    private static Text _queryText;
    private static Text _replyText;
    private static Text _programsText;

    private static List<QueryMeta> _queryMetas;
    private static int _currentQueryMetaId;

    private void Start()
    {
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
            }
        }
        
        _queryMetas = new List<QueryMeta>
        {
            new QueryMeta
            {
                Query = "Rotate 402-32-11-61-990-802-A by 35 degrees in Y axis",
                Programs = new[]
                {
                    "ExtractNumbers query",
                    "FilterIds prev var1",
                    "Filter3DAttr name prev root",
                    "Filter3DAttr type figure prev",
                    "Unique prev",
                    "Rotate prev var1"
                },
                Reply = "402-32-11-61-990-802-A"
            },
            new QueryMeta
            {
                Query = "Rotate 402-32-11-61-990-802-A by 90 degrees in X axis",
                Programs = new[]
                {
                    "ExtractNumbers query",
                    "FilterIds prev var1",
                    "Filter3DAttr name prev root",
                    "Filter3DAttr type figure prev",
                    "Unique prev",
                    "Rotate prev var1"
                },
                Reply = "402-32-11-61-990-802-A"
            },
            new QueryMeta
            {
                Query = "Scale up 402-32-11-61-990-802-A by 0.5",
                Programs = new[]
                {
                    "ExtractNumbers query",
                    "FilterIds prev var1",
                    "Filter3DAttr name prev root",
                    "Filter3DAttr type figure prev",
                    "Unique prev",
                    "Scale prev var1"
                },
                Reply = "402-32-11-61-990-802-A"
            },
            new QueryMeta
            {
                Query = "Scale down 402-32-11-61-990-802-A by 2",
                Programs = new[]
                {
                    "ExtractNumbers query",
                    "FilterIds prev var1",
                    "Filter3DAttr name prev root",
                    "Filter3DAttr type figure prev",
                    "Unique prev",
                    "Scale prev var1"
                },
                Reply = "402-32-11-61-990-802-A"
            },
            new QueryMeta
            {
                Query = "Show highlight [41], [46]",
                Programs = new[]
                {
                    "ExtractNumbers query",
                    "FilterIds prev var1",
                    "Filter3DAttr name prev root",
                    "Highlight prev"
                },
                Reply = "[41], [46]"
            },
            new QueryMeta
            {
                Query = "Remove highlight [41], [46]",
                Programs = new[]
                {
                    "ExtractNumbers query",
                    "FilterIds prev var1",
                    "Filter3DAttr name prev root",
                    "Highlight prev"
                },
                Reply = "[41], [46]"
            },
            new QueryMeta
            {
                Query = "Animate on 402-32-11-61-990-802-A",
                Programs = new[]
                {
                    "ExtractNumbers query",
                    "FilterIds prev var1",
                    "Filter3DAttr name prev root",
                    "Filter3DAttr type figure prev",
                    "Unique prev",
                    "Animate prev"
                },
                Reply = "402-32-11-61-990-802-A"
            },
            new QueryMeta
            {
                Query = "Animate off 402-32-11-61-990-802-A",
                Programs = new[]
                {
                    "ExtractNumbers query",
                    "FilterIds prev var1",
                    "Filter3DAttr name prev root",
                    "Filter3DAttr type figure prev",
                    "Unique prev",
                    "Animate prev"
                },
                Reply = "402-32-11-61-990-802-A"
            },
            new QueryMeta
            {
                Query = "Show left side of figure 402-32-11-61-990-802-A",
                Programs = new[]
                {
                    "ExtractNumbers query",
                    "FilterIds prev var1",
                    "Filter3DAttr name prev root",
                    "Filter3DAttr type figure prev",
                    "Unique prev",
                    "ShowSide prev"
                },
                Reply = "402-32-11-61-990-802-A"
            },
            new QueryMeta
            {
                Query = "Show top side of figure 402-32-11-61-990-802-A",
                Programs = new[]
                {
                    "ExtractNumbers query",
                    "FilterIds prev var1",
                    "Filter3DAttr name prev root",
                    "Filter3DAttr type figure prev",
                    "Unique prev",
                    "ShowSide prev"
                },
                Reply = "402-32-11-61-990-802-A"
            },
            new QueryMeta
            {
                Query = "Show bottom side of figure 402-32-11-61-990-802-A",
                Programs = new[]
                {
                    "ExtractNumbers query",
                    "FilterIds prev var1",
                    "Filter3DAttr name prev root",
                    "Filter3DAttr type figure prev",
                    "Unique prev",
                    "ShowSide prev"
                },
                Reply = "402-32-11-61-990-802-A"
            },
            new QueryMeta
            {
                Query = "Show back side of figure 402-32-11-61-990-802-A",
                Programs = new[]
                {
                    "ExtractNumbers query",
                    "FilterIds prev var1",
                    "Filter3DAttr name prev root",
                    "Filter3DAttr type figure prev",
                    "Unique prev",
                    "ShowSide prev"
                },
                Reply = "402-32-11-61-990-802-A"
            },
            new QueryMeta
            {
                Query = "Reset 402-32-11-61-990-802-A",
                Programs = new[]
                {
                    "ExtractNumbers query",
                    "FilterIds prev var1",
                    "Filter3DAttr name prev root",
                    "Filter3DAttr type figure prev",
                    "Unique prev",
                    "Reset prev"
                },
                Reply = "402-32-11-61-990-802-A"
            }
        };
        
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
        }

        Context.Instance.CurrentTaskID = "1";
        Context.Instance.CurrentSubtaskID = "2";
        Context.Instance.CurrentInstructionOrder = "3";
    }

    private void Update()
    {
        // execute
        if (Input.GetKeyDown(KeyCode.E))
        {
            var currentQueryMeta = _queryMetas[_currentQueryMetaId];
            
            _queryText.text = currentQueryMeta.Query;
            _programsText.text = string.Join("\n", currentQueryMeta.Programs);

            var result = FunctionalProgramsExecutor.Instance.Execute(currentQueryMeta);
            print($"result: {result}");
            _replyText.text = result;
        }
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
