using System.Collections;
using UnityEngine;

public class Robot
{
	private float _moveDistance;
	private float _moveSpeed;
	private float _rotateDegree;
	private bool _isForceStopped;

	private float _moveThreshold = 0.3f;

	public Robot(float moveDistance = 1.0f, float moveSpeed = 1.0f, float rotateDegree = 3.0f)
	{
		_moveDistance = moveDistance;
		_moveSpeed = moveSpeed;
		_rotateDegree = rotateDegree;
	}

	public void SetMoveDistance(float distance)
	{
		_moveDistance = distance;
	}

	public IEnumerator SetMoveSpeed(float speed)
	{
		_moveSpeed = speed;
		yield return null;
	}

	public float GetMoveDistance()
	{
		return _moveDistance;
	}

	public float GetMoveSpeed()
	{
		return _moveSpeed;
	}

	public IEnumerator Move(GameObject sourceObject, Vector3 finalPosition)
	{
		var duration = GetMoveDistance() / GetMoveSpeed();
		var initialPosition = sourceObject.transform.position;

		var delta = finalPosition - initialPosition;

		var count = 0.0f;

		while (count < 1)
		{
			if (_isForceStopped) break;

			count += Time.deltaTime / duration;
			var temp = initialPosition + delta.normalized * GetMoveDistance();

			if (Vector3.Distance(temp, finalPosition) < _moveThreshold)
			{
				temp = finalPosition;
			}

			sourceObject.transform.position = Vector3.Lerp(initialPosition,
				temp, count);
			
			if (Vector3.Distance(sourceObject.transform.position, finalPosition) < 0.01)
			{
				break;
			}
			
			yield return null;
		}

		yield return null;
	}

	public IEnumerator MoveWithRotation(GameObject sourceObject, Vector3 finalPosition, Vector3 rotationAxis)
	{
		var duration = GetMoveDistance() / GetMoveSpeed();
		var initialPosition = sourceObject.transform.position;

		var delta = finalPosition - initialPosition;

		var count = 0.0f;

		while (count < 1)
		{
			if (_isForceStopped) break;

			count += Time.deltaTime / duration;
			var temp = initialPosition + delta.normalized * GetMoveDistance();

			if (Vector3.Distance(temp, finalPosition) < _moveThreshold)
			{
				temp = finalPosition;
			}

			sourceObject.transform.position = Vector3.Lerp(initialPosition,
				temp, count);
			sourceObject.transform.rotation = Quaternion.Euler(sourceObject.transform.rotation.eulerAngles + rotationAxis * _rotateDegree);
			
			if (Vector3.Distance(sourceObject.transform.position, finalPosition) < 0.01)
			{
				break;
			}
			
			yield return null;
		}

		yield return null;
	}

	public IEnumerator Stop(float duration = 0.0f)
	{
		if (duration == 0.0f)
		{
			_isForceStopped = true;
		}

		yield return new WaitForSeconds(duration);
	}
}