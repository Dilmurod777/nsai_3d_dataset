using System.Collections;
using System.Collections.Generic;
using Constants;
using UnityEngine;

public class Robot
{
	private float _moveDistance;
	private float _moveSpeed;
	private float _rotateDegree;
	private float _rotateSpeed;
	private bool _isForceStopped;

	private const float MoveThreshold = 0.05f;
	private const float RotateThreshold = 0.05f;

	public Robot(float moveDistance = 1.0f, float moveSpeed = 1.0f, float rotateDegree = 1.0f, float rotateSpeed = 1.0f)
	{
		_moveDistance = moveDistance;
		_moveSpeed = moveSpeed;
		_rotateDegree = rotateDegree;
		_rotateSpeed = rotateSpeed;
	}

	public void SetMoveDistance(float distance)
	{
		_moveDistance = distance;
	}

	public IEnumerator SetMoveSpeed(float speed)
	{
		var previousMoveSpeed = _moveSpeed;
		_moveSpeed = speed;
		yield return new TestingResponse(TestingResponse.TestingResponseTypes.SetMoveSpeed, prevms: previousMoveSpeed,
			curms: speed);
	}

	public IEnumerator SetRotateSpeed(float speed)
	{
		var previousRotateSpeed = speed;
		_rotateSpeed = speed;
		yield return new TestingResponse(TestingResponse.TestingResponseTypes.SetRotateSpeed, prevms: previousRotateSpeed,
			curms: speed);
	}

	public float GetMoveDistance()
	{
		return _moveDistance;
	}

	public float GetMoveSpeed()
	{
		return _moveSpeed;
	}

	public float GetRotateDegree()
	{
		return _rotateDegree;
	}

	public float GetRotateSpeed()
	{
		return _rotateSpeed;
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

			if (Vector3.Distance(temp, finalPosition) < MoveThreshold)
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

		yield return new TestingResponse(TestingResponse.TestingResponseTypes.Move, sourceObject.name,
			cp: sourceObject.transform.position, fp: finalPosition);
	}

	public IEnumerator Rotate(GameObject sourceObject, Vector3 finalRotation, Vector3 rotationAxis)
	{
		var duration = GetRotateDegree() / GetRotateSpeed();
		var initialRotation = sourceObject.transform.rotation;
		var delta = finalRotation - initialRotation.eulerAngles;

		var count = 0.0f;
		while (count < 1)
		{
			if (_isForceStopped) break;

			count += Time.deltaTime / duration;
			// var temp = Vector3.MoveTowards(initialRotation.eulerAngles, finalRotation, _rotateDegree);
			var temp = initialRotation.eulerAngles + new Vector3(delta.normalized.x * rotationAxis.x,
				delta.normalized.y * rotationAxis.y, delta.normalized.z * rotationAxis.z) * GetRotateDegree();

			if (Vector3.Angle(temp, finalRotation) < RotateThreshold)
			{
				temp = finalRotation;
			}

			sourceObject.transform.rotation = Quaternion.Slerp(initialRotation, Quaternion.Euler(temp), count);

			if (Vector3.Angle(sourceObject.transform.rotation.eulerAngles, finalRotation) < 0.01)
			{
				break;
			}

			yield return null;
		}

		yield return new TestingResponse(TestingResponse.TestingResponseTypes.Rotate, sourceObject.name,
			cr: sourceObject.transform.rotation.eulerAngles, fp: finalRotation);
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

			if (Vector3.Distance(temp, finalPosition) < MoveThreshold)
			{
				temp = finalPosition;
			}

			sourceObject.transform.position = Vector3.Lerp(initialPosition,
				temp, count);
			sourceObject.transform.rotation =
				Quaternion.Euler(sourceObject.transform.rotation.eulerAngles + rotationAxis * _rotateDegree);

			if (Vector3.Distance(sourceObject.transform.position, finalPosition) < 0.01)
			{
				break;
			}

			yield return null;
		}

		yield return new TestingResponse(TestingResponse.TestingResponseTypes.MoveWithRotation, sourceObject.name,
			cp: sourceObject.transform.position, cr: sourceObject.transform.rotation.eulerAngles, fp: finalPosition);
	}

	public IEnumerator Stop(float duration = 0.0f)
	{
		if (duration == 0.0f)
		{
			_isForceStopped = true;
		}

		yield return new WaitForSeconds(duration);

		yield return new TestingResponse(TestingResponse.TestingResponseTypes.Stop);
	}
}