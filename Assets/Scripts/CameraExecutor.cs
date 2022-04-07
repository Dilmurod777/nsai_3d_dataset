using UnityEngine;

public class CameraExecutor : MonoBehaviour
{
	public float cameraSpeed = 0.5f;
	private void Update()
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
}
