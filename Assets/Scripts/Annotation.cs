
using TMPro;
using UnityEngine;

public class Annotation : MonoBehaviour
{
	public TextMeshPro annotationText;
	public LineRenderer annotationLine;

	private MeshRenderer _annotationTextMeshRenderer, _meshRenderer;
	private Vector3 _annotationWrapperOffset;
	private Vector3 _annotationLineOffset;

	private void Start()
	{
		_annotationTextMeshRenderer = annotationText.GetComponent<MeshRenderer>();
		_meshRenderer = GetComponent<MeshRenderer>();
		_annotationWrapperOffset = new Vector3(0.5f, 1.0f, 0);
		_annotationLineOffset = new Vector3(0.1f, 0.35f, 0);

		SetAnnotationVisibility(false);
	}

	// Update is called once per frame
	private void Update()
	{
		var currentObj = TestingScriptExecutor.CurrentAttachingObj;
		if (currentObj != null)
		{
			SetAnnotationVisibility(true);

			var annotationWrapperTransform = transform;
			var currentAttachingObjPosition = currentObj.transform.position;
			annotationWrapperTransform.position = currentAttachingObjPosition + _annotationWrapperOffset;
			annotationLine.SetPosition(0, annotationLine.transform.InverseTransformPoint(currentAttachingObjPosition) + _annotationLineOffset);
			if (Camera.main != null)
			{
				annotationWrapperTransform.rotation = Camera.main.transform.rotation;
			}

			annotationText.text = currentObj.name;
		}
		else
		{
			SetAnnotationVisibility(false);
		}
	}

	private void SetAnnotationVisibility(bool isVisible)
	{
		_meshRenderer.enabled = isVisible;
		_annotationTextMeshRenderer.enabled = isVisible;
		annotationLine.enabled = isVisible;
	}
}