using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class ScriptExecutor : MonoBehaviour
{
    public static bool IsInAction = false;
    private enum Axis
    {
        X,
        Y,
        Z
    }

    private string _currentObjectId;
    
    public float cameraSpeed = 0.01f;

    public Color highlightColor = Color.magenta;
    public int highlightWidth = 4;

    public float minRotationAngle = -15;
    public float maxRotationAngle = 15;
    public float rotationDuration = 1.0f;

    public float minScale = 0.001f;
    public float maxScale = 1.5f;
    public float scaleDuration = 1.0f;

    public float zoomDuration = 1.0f;
    
    private bool _isModifying;

    private void Start()
    {
        Context.Instance.Camera = Camera.main;

        _currentObjectId = "41";

        Context.Instance.CurrentTaskID = "1";
        Context.Instance.CurrentSubtaskID = "2";
        Context.Instance.CurrentInstructionOrder = "3";
    }

    private void Update()
    {
        CameraMovements();

        if (Input.GetKeyDown(KeyCode.H))
        {
            var queryMeta = new QueryMeta
            {
                Query = $"Show highlight object {_currentObjectId}",
                Programs = new[]
                {
                    $"FindObjectWithPartOfName {_currentObjectId}",
                    $"HighlightHandler {State.On} {highlightWidth} {HelperFunctions.ConvertColorToString(highlightColor)}"
                },
                Reply = "highlight on"
            };
            var result = FunctionalProgramsExecutor.Instance.Execute(queryMeta);
            print($"result: {result}");
        }

        if (Input.GetKeyUp(KeyCode.H))
        {
            var queryMeta = new QueryMeta
            {
                Query = $"Show highlight object {_currentObjectId}",
                Programs = new[]
                {
                    $"FindObjectWithPartOfName {_currentObjectId}",
                    $"HighlightHandler {State.Off} {highlightWidth} {HelperFunctions.ConvertColorToString(highlightColor)}"
                },
                Reply = "highlight off"
            };
            var result = FunctionalProgramsExecutor.Instance.Execute(queryMeta);
            print($"result: {result}");
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            RotateHandler();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            ScaleHandler();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            // Catalog.Instance.ResetHandler(_currentObject, _initialAttributes[_currentObjectId], resetDuration);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            var queryMeta = new QueryMeta
            {
                Query = $"Zoom object {_currentObjectId}",
                Programs = new[]
                {
                    $"FindObjectWithPartOfName {_currentObjectId}",
                    $"ZoomHandler {zoomDuration}"
                },
                Reply = "zoom"
            };
            var result = FunctionalProgramsExecutor.Instance.Execute(queryMeta);
            print($"result: {result}");
        }
    }

    // camera movements
    private void CameraMovements()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            Context.Instance.Camera.transform.position += cameraSpeed*Vector3.forward;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            Context.Instance.Camera.transform.position += cameraSpeed*Vector3.back;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Context.Instance.Camera.transform.position += cameraSpeed*Vector3.left;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Context.Instance.Camera.transform.position += cameraSpeed*Vector3.right;
        }
    }
    
    // side
    
    // rotate
    private void RotateHandler()
    {
        var degree = Random.Range(minRotationAngle, maxRotationAngle);
        var axis = (Axis)Random.Range(0, 3); // x, y, z

        var rotation = transform.rotation;
        var rotationX = rotation.x + (axis == Axis.X ? degree : 0);
        var rotationY = rotation.y + (axis == Axis.Y ? degree : 0);
        var rotationZ = rotation.z + (axis == Axis.Z ? degree : 0);
        
        // HighlightObject(_currentObject);
        var newRotation = Quaternion.Euler(rotationX, rotationY, rotationZ);
        // StartCoroutine(RotateObject(_currentObject, newRotation));

        var text = $"rotate object {_currentObjectId} by {degree} in {axis} axis";
        print(text);
    }

    private IEnumerator RotateObject(GameObject obj, Quaternion newRotation)
    {
        if (_isModifying)
        {
            yield break;
        }
        _isModifying = true;

        var currentRot = obj.transform.rotation;

        float counter = 0;
        while (counter < rotationDuration)
        {
            counter += Time.deltaTime;
            obj.transform.rotation = Quaternion.Lerp(currentRot, newRotation, counter / rotationDuration);
            yield return null;
        }
        _isModifying = false;
    }
    
    // annotate
    
    // scale
    private void ScaleHandler()
    {
        var value = Random.Range(minScale, maxScale);

        // _currentObject.transform.localScale *= value;
        // HighlightObject(_currentObject);
        // StartCoroutine(ScaleObject(_currentObject, _currentObject.transform.localScale * value));
        
        var text = $"scale object {_currentObjectId} by {value}";
        print(text);
    }
    
    private IEnumerator ScaleObject(GameObject obj, Vector3 newScale)
    {
        if (_isModifying)
        {
            yield break;
        }
        _isModifying = true;

        float counter = 0;
        while (counter < scaleDuration)
        {
            counter += Time.deltaTime;
            
            obj.transform.localScale = Vector3.Lerp(obj.transform.localScale, newScale, counter / scaleDuration);;
            yield return null;
        }
        _isModifying = false;
    }
}
