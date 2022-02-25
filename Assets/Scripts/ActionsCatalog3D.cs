using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActionsCatalog3DInterface
{
    List<GameObject> FindObjectsWithIds(string args);
    GameObject FindFigureWithId(string args);
    void ZoomHandler(string args);
    void ResetHandler(string args);
    void HighlightHandler(string args);
    void RotateHandler(string args);
    void ScaleHandler(string args);
    void ShowFigureSideHandler(string args);
    void CloseLookHandler(string args);
    void AnimateFigureHandler(string args);
    void VisibilityHandler(string args);
}

public class ActionsCatalog3D : MonoBehaviour, IActionsCatalog3DInterface
{
    private IEnumerator Sequence(List<IEnumerator> coroutines)
    {
        foreach (var c in coroutines) yield return StartCoroutine(c);
    }
    
    // find object with part of name
    public List<GameObject> FindObjectsWithIds(string args)
    {
        var allObjects = GameObject.FindGameObjectsWithTag("Object");
        var foundObs = new List<GameObject>();
        var ids = args.Split('#');

        foreach (var obj in allObjects)
        foreach (var id in ids)
            if (obj.name.Contains($"[{id}]"))
                foundObs.Add(obj);

        return foundObs;
    }

    public GameObject FindFigureWithId(string args)
    {
        var allFigures = GameObject.FindGameObjectsWithTag("Figure");
        var id = args.Split('#')[0];

        foreach (var fig in allFigures)
        {
            if (fig.name.Equals(id))
            {
                Context.Instance.CurrentFigureID = id;
                return fig;
            }
        }


        return null;
    }

    // close look
    public void ZoomHandler(string args)
    {
        var argsList = args.Split('#');

        const float changeFieldOfView = -5;
        var duration = float.Parse(argsList[0]);

        var finalFieldOfView = Context.Instance.Camera.fieldOfView + changeFieldOfView;
        var cameraTransform = Context.Instance.Camera.transform;

        var coroutines = new List<IEnumerator>();

        if (!Physics.Raycast(Context.Instance.Camera.transform.position,
                Context.Instance.Camera.transform.TransformDirection(Vector3.forward), Mathf.Infinity))
        {
            var finalRotation =
                Quaternion.LookRotation(((GameObject) Context.Instance.Prev[0]).transform.position -
                                        cameraTransform.position);

            coroutines.Add(RotateObject(Context.Instance.Camera, finalRotation, duration));
        }

        coroutines.Add(ChangeFieldOfViewByValue(Context.Instance.Camera, finalFieldOfView, duration));

        StartCoroutine(Sequence(coroutines));
    }

    // rotate
    public void RotateHandler(string args)
    {
        var argsList = args.Split('#');
        var degree = float.Parse(argsList[0]);
        var duration = float.Parse(argsList[2]);

        Enum.TryParse(argsList[1], out State.Axis axis);

        var obj = (GameObject) Context.Instance.Prev;

        if (obj == null) return;
        
        var rotation = obj.transform.rotation;
        var rotationX = axis == State.Axis.X ? degree : 0;
        var rotationY = axis == State.Axis.Y ? degree : 0;
        var rotationZ = axis == State.Axis.Z ? degree : 0;

        var newRotation = rotation * Quaternion.Euler(rotationX, rotationY, rotationZ);
        StartCoroutine(RotateObject(obj, newRotation, duration));
    }

    public void ScaleHandler(string args)
    {
        var argsList = args.Split('#');

        Debug.Log(args);
        
        var state = argsList[0];
        var scaleRatio = float.Parse(argsList[1]);
        var duration = float.Parse(argsList[2]);

        var obj = (GameObject) Context.Instance.Prev;
        if (obj == null) return;

        var currentLocalScale = obj.transform.localScale;
        var currentLocalScaleX = currentLocalScale.x;
        var currentLocalScaleY = currentLocalScale.y;
        var currentLocalScaleZ = currentLocalScale.z;
        var change = state == State.On ? 1 + scaleRatio : 1 - scaleRatio;
        
        var finalScale = new Vector3(currentLocalScaleX * change, currentLocalScaleY * change, currentLocalScaleZ * change);
        
        StartCoroutine(ScaleObject(obj, finalScale, duration));
    }
    
    // reset
    public void ResetHandler(string args)
    {
        var argsList = args.Split('#');

        var id = argsList[0];
        
        var obj = (GameObject) Context.Instance.Prev;
        if (obj == null) return;
        
        var attributes = Context.Instance.InitialAttributes[id];
        var duration = float.Parse(argsList[1]);

        var cameraAttributes = Context.Instance.InitialAttributes["camera"];

        var coroutines = new List<IEnumerator>();

        coroutines.Add(Reset(obj, attributes, duration));
        coroutines.Add(Reset(Context.Instance.Camera, cameraAttributes, duration));

        if (cameraAttributes.FoV != 0)
            coroutines.Add(ChangeFieldOfViewByValue(Context.Instance.Camera, cameraAttributes.FoV, duration));

        StartCoroutine(Sequence(coroutines));
    }
    
    // highlight
    public void HighlightHandler(string args)
    {
        var argsList = args.Split('#');
        
        var state = argsList[0];
        var highlightWidth = float.Parse(argsList[1]);
        var highlightColor = HelperFunctions.ConvertStringToColor(argsList[2]);

        var objs = (List<GameObject>) Context.Instance.Prev;
        if (objs.Count == 0) return;
        
        HighlightObject(objs, state, highlightWidth, highlightColor);
    }

    private void HighlightObject(List<GameObject> objs, string state = State.On, float highlightWidth = 1.0f, Color highlightColor = default)
    {
        foreach (var obj in objs)
        {
            var outlineComponent = obj.GetComponent<Outline>();
            if (outlineComponent == null) outlineComponent = obj.AddComponent<Outline>();

            switch (state)
            {
                case State.On:
                    outlineComponent.OutlineMode = Outline.Mode.OutlineAll;
                    outlineComponent.OutlineWidth = highlightWidth;
                    outlineComponent.OutlineColor = highlightColor;

                    outlineComponent.enabled = true;
                    break;
                case State.Off:
                    outlineComponent.enabled = false;
                    break;
            }
        }
    }

    public void ShowFigureSideHandler(string args)
    {
        var obj = (GameObject) Context.Instance.Prev;
        if (obj == null) return;

        var argsList = args.Split('#');
        
        Enum.TryParse(argsList[0], out State.FigureSide figureSide);
        var duration = float.Parse(argsList[1]);

        Quaternion sideRotation;

        switch (figureSide)
        {
            case State.FigureSide.Front:
                sideRotation = Quaternion.Euler(0, -90, -45);
                break;
            case State.FigureSide.Back:
                sideRotation = Quaternion.Euler(0, 90, 0);
                break;
            case State.FigureSide.Right:
                sideRotation =Quaternion.Euler(0, 0, 0);
                break;
            case State.FigureSide.Left:
                sideRotation = Quaternion.Euler(0, 180, 0);
                break;
            case State.FigureSide.Top:
                sideRotation =Quaternion.Euler(-45, 0, 0);
                break;
            case State.FigureSide.Bottom:
                sideRotation = Quaternion.Euler(135, 0, 0);
                break;
            default:
                sideRotation = Quaternion.Euler(0, 0, 0);
                break;
        }

        var coroutines = new List<IEnumerator>();

        coroutines.Add(RotateObject(obj, sideRotation, duration));

        var finalScale = new Vector3(0.7f, 0.7f, 0.7f);
        if (obj.transform.localScale != finalScale)
        {
            coroutines.Add(ScaleObject(obj, finalScale, 1.0f));
        }

        var finalFoV = 70;
        if (Context.Instance.Camera.fieldOfView != finalFoV)
        {
            coroutines.Add(ChangeFieldOfViewByValue(Context.Instance.Camera, finalFoV, 1.0f));
        }

        StartCoroutine(Sequence(coroutines));
    }

    public void CloseLookHandler(string args)
    {
        var argsList = args.Split('#');
        var currentFigure = FindFigureWithId(argsList[0]);
        var duration = float.Parse(argsList[1]);

        var objs = (List<GameObject>) Context.Instance.Prev;
        if (objs.Count == 0) return;

        var coroutines = new List<IEnumerator>();
        var waitTime = 0.0f;
        
        var finalPosition = currentFigure.transform.position + new Vector3(0, 0, 10.0f);
        coroutines.Add(MoveObject(currentFigure, finalPosition, 1.0f));
        waitTime += 1.0f;
        
        var finalScale = new Vector3(0.5f, 0.5f, 0.5f);
        if (currentFigure.transform.localScale != finalScale)
        {
            coroutines.Add(ScaleObject(currentFigure, finalScale, 0.5f));
            waitTime += 0.5f;
        }
        
        float finalFoV = Context.Instance.Camera.fieldOfView + 5.0f;
        if (Context.Instance.Camera.fieldOfView != finalFoV)
        {
            coroutines.Add(ChangeFieldOfViewByValue(Context.Instance.Camera, finalFoV, 0.5f));
            waitTime += 0.5f;
        }

        var cloneSpawnPosition = currentFigure.transform.position + new Vector3(-5.0f, 0, -3.0f);
        
        void ChangeCloneObjectsPosition()
        {
            for (var i = 0; i < objs.Count; i++)
            {
                var cloneObj = Instantiate(objs[i]);
                cloneObj.transform.position = cloneSpawnPosition + new Vector3(5.0f * i, 0, 0);
                cloneObj.tag = "CloneObject";
            }
        }
        
        StartCoroutine(Sequence(coroutines));
        StartCoroutine(Delay(waitTime, ChangeCloneObjectsPosition));
    }

    public void AnimateFigureHandler(string args)
    {
        var fig = (GameObject) Context.Instance.Prev;
        if (fig == null) return;

        var currentFigureID = Context.Instance.CurrentFigureID;
        var attributes = Context.Instance.InitialAttributes[currentFigureID];

        var argsList = args.Split('#');
        var state = argsList[0];
        var animationSpeed = float.Parse(argsList[1]);
        
        void StartAnimatingFigure()
        {
            fig.AddComponent<InfiniteRotation>();
                
            var infiniteRotationComponent = fig.GetComponent<InfiniteRotation>();
            infiniteRotationComponent.SetSpeed(animationSpeed);
        }

        void StopAnimatingFigure()
        {
            var infiniteRotationComponent = fig.GetComponent<InfiniteRotation>();
            if (infiniteRotationComponent != null)
            {
                Destroy(infiniteRotationComponent);
            }
        }

        StartCoroutine(Reset(fig, attributes, 1.0f));
        
        var infiniteRotationComponent = fig.GetComponent<InfiniteRotation>();
        StartCoroutine(infiniteRotationComponent == null
            ? Delay(1.0f, StartAnimatingFigure)
            : Delay(1.0f, StopAnimatingFigure));
    }

    public void VisibilityHandler(string args)
    {
        var objs = (List<GameObject>) Context.Instance.Prev;
        if (objs.Count == 0) return;

        var argsList = args.Split('#');
        var state = argsList[0];
        

        foreach (var obj in objs)
        {
            obj.GetComponent<MeshRenderer>().enabled = state == State.On;
        }
    }

    private static IEnumerator Delay(float duration, State.VoidFunction method = null)
    {
        yield return new WaitForSeconds(duration);
        method?.Invoke();
    }
    
    private static IEnumerator Reset(dynamic obj, Attributes attributes, float duration)
    {
        if (ScriptExecutor.IsInAction) yield break;
        ScriptExecutor.IsInAction = true;

        var currentRot = obj.transform.rotation;

        var infiniteRotationComponent = obj.GetComponent<InfiniteRotation>();
        if (infiniteRotationComponent != null)
        {
            Destroy(infiniteRotationComponent);
        }

        var cloneObjects = GameObject.FindGameObjectsWithTag("CloneObject");

        foreach (var cloneObj in cloneObjects)
        {
            Destroy(cloneObj);
        }

        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;

            obj.transform.rotation = Quaternion.Lerp(currentRot, attributes.Rotation, counter / duration);
            obj.transform.localScale = Vector3.Lerp(obj.transform.localScale, attributes.Scale, counter / duration);
            obj.transform.position = Vector3.Lerp(obj.transform.position, attributes.Position, counter / duration);

            yield return null;
        }

        ScriptExecutor.IsInAction = false;
    }
    
    // smoothly move object
    private static IEnumerator MoveObject(dynamic obj, Vector3 finalPosition, float duration)
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
    private static IEnumerator RotateObject(dynamic obj, Quaternion finalRotation, float duration)
    {
        if (ScriptExecutor.IsInAction) yield break;
        ScriptExecutor.IsInAction = true;

        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;

            obj.transform.rotation = Quaternion.Lerp(obj.transform.rotation, finalRotation, counter / duration);
            yield return null;
        }

        ScriptExecutor.IsInAction = false;
    }
    
    private static IEnumerator ScaleObject(dynamic obj, Vector3 finalScale, float duration)
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
    private static IEnumerator ChangeFieldOfViewByValue(Camera camera, float finalFoV, float duration)
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