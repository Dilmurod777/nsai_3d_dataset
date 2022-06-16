using System.Collections;
using System.Collections.Generic;
using Constants;
using UnityEngine;

public class Robot
{
	private float _moveDistance;
	private float _rotateDegree;
	private float _moveDuration;
	private float _rotateDuration;
	private bool _isForceStopped;

	private const float MoveThreshold = 1.5f;
	private const float RotateThreshold = 0.2f;

	public Robot(float moveDistance = 0.001f, float moveDuration = 1.5f, float rotateDegree = 35.0f, float rotateDuration = 1.5f)
	{
		_moveDistance = moveDistance;
		_rotateDegree = rotateDegree;
		_moveDuration = moveDuration;
		_rotateDuration = rotateDuration;
	}

	public float GetMoveDistance()
	{
		return _moveDistance;
	}

	public float GetRotateDegree()
	{
		return _rotateDegree;
	}
	
	public float GetMoveDuration()
	{
		return _moveDuration;
	}
	
	public float GetRotateDuration()
	{
		return _rotateDuration;
	}

	public IEnumerator SetMoveDistance(float distance)
	{
		_moveDistance = distance;
		yield return null;
	}

	public IEnumerator SetRotateDegree(float degree)
	{
		_rotateDegree = degree;
		yield return null;
	}
	
	public IEnumerator SetRotateDuration(float duration)
	{
		_rotateDuration = duration;
		yield return null;
	}


	public IEnumerator Move(GameObject sourceObject, Vector3 finalPosition)
	{
		var initialPosition = sourceObject.transform.position;

		var delta = finalPosition - initialPosition;
		var temp = initialPosition + delta.normalized * GetMoveDistance();

		if (Vector3.Distance(temp, finalPosition) < GetMoveDistance() * MoveThreshold)
		{
			temp = finalPosition;
		}

		sourceObject.transform.position = temp;

		yield return new TestingResponse(
			TestingResponse.TestingResponseTypes.Move,
			sourceObject.name,
			ip: initialPosition,
			cp: sourceObject.transform.position,
			fp: finalPosition
		);
	}

	public IEnumerator Rotate(GameObject sourceObject, Vector3 finalRotation, Vector3 rotationAxis = default)
	{
		var initialRotation = sourceObject.transform.rotation;
		
		// sourceObject.transform.Rotate(rotationAxis, GetRotateDegree()); 
		
		sourceObject.transform.rotation = Quaternion.RotateTowards(initialRotation, Quaternion.Euler(finalRotation), GetRotateDegree());

		yield return new TestingResponse(
			TestingResponse.TestingResponseTypes.Rotate,
			sourceObject.name,
			ir: initialRotation.eulerAngles,
			cr: sourceObject.transform.rotation.eulerAngles,
			fp: finalRotation
		);
	}

	public IEnumerator MoveWithRotation(GameObject sourceObject, Vector3 finalPosition, Vector3 rotationAxis)
	{
		var initialPosition = sourceObject.transform.position;
		var initialRotation = sourceObject.transform.eulerAngles;

		var delta = finalPosition - initialPosition;

		var temp = initialPosition + delta.normalized * GetMoveDistance();

		if (Vector3.Distance(temp, finalPosition) < GetMoveDistance() * MoveThreshold)
		{
			temp = finalPosition;
		}

		sourceObject.transform.position = temp;
		sourceObject.transform.Rotate(rotationAxis, GetRotateDegree());

		yield return new TestingResponse(
			TestingResponse.TestingResponseTypes.MoveWithRotation,
			sourceObject.name,
			ip: initialPosition,
			ir: initialRotation,
			cp: sourceObject.transform.position,
			cr: sourceObject.transform.rotation.eulerAngles,
			fp: finalPosition
		);
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