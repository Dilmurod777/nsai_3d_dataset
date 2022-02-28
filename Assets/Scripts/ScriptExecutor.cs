using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class ScriptExecutor : MonoBehaviour
{
    public static bool IsInAction = false;

    private List<string> _currentObjectIds;
    private string _currentFigureId;
    
    public Color highlightColor = Color.magenta;
    public int highlightWidth = 4;

    public float rotationAngle = 30;
    public float rotationDuration = 1.0f;

    public float scaleRatio = 0.1f;
    public float scaleDuration = 1.0f;

    public float zoomDuration = 1.0f;
    public float resetDuration = 1.0f;

    public State.FigureSide figureSide;
    public float figureSideDuration = 1.0f;

    public float closeLookDuration = 1.0f;

    public float infiniteRotationSpeed = 15.0f;

    private void Start()
    {
        _currentObjectIds = new List<string> {"46"};
        _currentFigureId = "402-32-11-61-990-802-A";
        
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
        // show highlight
        if (Input.GetKeyDown(KeyCode.H))
        {
            var queryMeta = new QueryMeta
            {
                Query = $"Show highlight object {HelperFunctions.JoinListIntoString(_currentObjectIds)}",
                Programs = new[]
                {
                    $"FindObjectsWithIds {HelperFunctions.JoinListIntoString(_currentObjectIds)}",
                    $"HighlightHandler {State.On} {highlightWidth} {HelperFunctions.ConvertColorToString(highlightColor)}"
                },
                Reply = "highlight on"
            };
            var result = FunctionalProgramsExecutor.Instance.Execute(queryMeta);
            print($"result: {result}");
        }

        // hide highlight
        if (Input.GetKeyDown(KeyCode.J))
        {
            var queryMeta = new QueryMeta
            {
                Query = $"Show highlight object {HelperFunctions.JoinListIntoString(_currentObjectIds)}",
                Programs = new[]
                {
                    $"FindObjectsWithIds {HelperFunctions.JoinListIntoString(_currentObjectIds)}",
                    $"HighlightHandler {State.Off} {highlightWidth} {HelperFunctions.ConvertColorToString(highlightColor)}"
                },
                Reply = "highlight off"
            };
            var result = FunctionalProgramsExecutor.Instance.Execute(queryMeta);
            print($"result: {result}");
        }
        
        // rotate
        if (Input.GetKeyDown(KeyCode.R))
        {
            const State.Axis axis = State.Axis.Y;
            
            var queryMeta = new QueryMeta
            {
                Query = $"Rotate {_currentFigureId} by {rotationAngle} degrees in {axis} axis",
                Programs = new[]
                {
                    $"FindFigureWithId {_currentFigureId}",
                    $"RotateHandler {rotationAngle} {axis.ToString()} {rotationDuration}"
                },
                Reply = "rotate"
            };
            var result = FunctionalProgramsExecutor.Instance.Execute(queryMeta);
            print($"result: {result}");
        }
        
        // scale up
        if (Input.GetKeyDown(KeyCode.S))
        {
            var queryMeta = new QueryMeta
            {
                Query = $"Scale up {_currentFigureId}",
                Programs = new[]
                {
                    $"FindFigureWithId {_currentFigureId}",
                    $"ScaleHandler {State.On} {scaleRatio} {scaleDuration}"
                },
                Reply = "scale up"
            };
            var result = FunctionalProgramsExecutor.Instance.Execute(queryMeta);
            print($"result: {result}");
        }
        
        // scale down
        if (Input.GetKeyDown(KeyCode.D))
        {
            var queryMeta = new QueryMeta
            {
                Query = $"Scale down {_currentFigureId}",
                Programs = new[]
                {
                    $"FindFigureWithId {_currentFigureId}",
                    $"ScaleHandler {State.Off} {scaleRatio} {scaleDuration}"
                },
                Reply = "scale down"
            };
            var result = FunctionalProgramsExecutor.Instance.Execute(queryMeta);
            print($"result: {result}");
        }
        
        // reset
        if (Input.GetKeyDown(KeyCode.P))
        {
            var queryMeta = new QueryMeta
            {
                Query = $"Reset {_currentFigureId}",
                Programs = new[]
                {
                    $"FindFigureWithId {_currentFigureId}",
                    $"ResetHandler {_currentFigureId} {resetDuration}"
                },
                Reply = "reset"
            };
            var result = FunctionalProgramsExecutor.Instance.Execute(queryMeta);
            print($"result: {result}");
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
}
