using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ScriptExecutor : MonoBehaviour
{
    public static bool IsInAction = false;

    private List<string> _currentObjectIds;
    private string _currentFigureId;
    
    public Color highlightColor = Color.magenta;
    public int highlightWidth = 4;
    
    public float scaleRatio = 0.1f;
    public float scaleDuration = 1.0f;

    public float zoomDuration = 1.0f;
    public float resetDuration = 1.0f;

    public State.FigureSide figureSide;
    public float figureSideDuration = 1.0f;

    public float closeLookDuration = 1.0f;

    public float infiniteRotationSpeed = 15.0f;

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
                    "RotateHandler prev var1"
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
                    "RotateHandler prev var1"
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
                    "ScaleHandler on prev var1"
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
                    "ScaleHandler off prev var1"
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
                    "HighlightHandler on prev"
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
                    "HighlightHandler off prev"
                },
                Reply = "[41], [46]"
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
        
        // zoom in
        if (Input.GetKeyDown(KeyCode.Z))
        {
            var queryMeta = new QueryMeta
            {
                Query = $"Zoom object {_currentObjectIds[0]}",
                Programs = new[]
                {
                    $"FindObjectsWithIds {_currentObjectIds[0]}",
                    $"ZoomHandler {zoomDuration}"
                },
                Reply = "zoom"
            };
            var result = FunctionalProgramsExecutor.Instance.Execute(queryMeta);
            print($"result: {result}");
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            var queryMeta = new QueryMeta
            {
                Query = $"Show {figureSide} side of {_currentFigureId}",
                Programs = new[]
                {
                    $"FindFigureWithId {_currentFigureId}",
                    $"ShowFigureSideHandler {figureSide} {figureSideDuration}"
                },
                Reply = "show side"
            };
            var result = FunctionalProgramsExecutor.Instance.Execute(queryMeta);
            print($"result: {result}");
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            var queryMeta = new QueryMeta
            {
                Query = $"Show close look of {HelperFunctions.JoinListIntoString(_currentObjectIds)}",
                Programs = new[]
                {
                    $"FindObjectsWithIds {HelperFunctions.JoinListIntoString(_currentObjectIds)}",
                    $"CloseLookHandler {_currentFigureId} {closeLookDuration}"
                },
                Reply = "show close look"
            };
            var result = FunctionalProgramsExecutor.Instance.Execute(queryMeta);
            print($"result: {result}");
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            var queryMeta = new QueryMeta
            {
                Query = $"Animate on {_currentFigureId}",
                Programs = new[]
                {
                    $"FindFigureWithId {_currentFigureId}",
                    $"AnimateFigureHandler {State.On} {infiniteRotationSpeed}"
                },
                Reply = "animate figure"
            };
            var result = FunctionalProgramsExecutor.Instance.Execute(queryMeta);
            print($"result: {result}");
        }
        
        if (Input.GetKeyDown(KeyCode.W))
        {
            var queryMeta = new QueryMeta
            {
                Query = $"Animate off {_currentFigureId}",
                Programs = new[]
                {
                    $"FindFigureWithId {_currentFigureId}",
                    $"AnimateFigureHandler {State.Off} {infiniteRotationSpeed}"
                },
                Reply = "animate figure"
            };
            var result = FunctionalProgramsExecutor.Instance.Execute(queryMeta);
            print($"result: {result}");
        }
        
        if (Input.GetKeyDown(KeyCode.I))
        {
            var queryMeta = new QueryMeta
            {
                Query = $"Make visible objects {HelperFunctions.JoinListIntoString(_currentObjectIds)}",
                Programs = new[]
                {
                    $"FindObjectsWithIds {HelperFunctions.JoinListIntoString(_currentObjectIds)}",
                    $"VisibilityHandler {State.On}"
                },
                Reply = "hide"
            };
            var result = FunctionalProgramsExecutor.Instance.Execute(queryMeta);
            print($"result: {result}");
        }
        
        if (Input.GetKeyDown(KeyCode.O))
        {
            var queryMeta = new QueryMeta
            {
                Query = $"Hide objects {HelperFunctions.JoinListIntoString(_currentObjectIds)}",
                Programs = new[]
                {
                    $"FindObjectsWithIds {HelperFunctions.JoinListIntoString(_currentObjectIds)}",
                    $"VisibilityHandler {State.Off}"
                },
                Reply = "hide"
            };
            var result = FunctionalProgramsExecutor.Instance.Execute(queryMeta);
            print($"result: {result}");
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
