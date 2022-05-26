using System.Collections.Generic;
using UnityEngine;

namespace Constants
{
	public class Response
	{
		public readonly Dictionary<string, dynamic> Operation;
		public readonly List<string> Objects;
		public readonly Dictionary<string, dynamic> Extra;

		public Response(Dictionary<string, dynamic> op, List<string> objs, Dictionary<string, dynamic> ex)
		{
			Operation = op;
			Objects = objs;
			Extra = ex;
		}
	}

	public class TestingResponse
	{
		public enum TestingResponseTypes
		{
			Move,
			Rotate,
			Stop,
			MoveWithRotation,
			SetMoveSpeed,
			SetRotateSpeed
		}

		public readonly TestingResponseTypes Type;
		public readonly string ObjectName;
		public readonly Vector3 InitialPosition;
		public readonly Vector3 InitialRotation;
		public readonly Vector3 CurrentPosition;
		public readonly Vector3 CurrentRotation;
		public readonly Vector3 FinalPosition;
		public readonly Vector3 FinalRotation;
		public readonly float PreviousMoveSpeed;
		public readonly float CurrentMoveSpeed;

		public TestingResponse(TestingResponseTypes t, string name = default, Vector3 cp = default, Vector3 cr = default,
			Vector3 fp = default, Vector3 ip = default, Vector3 ir = default,
			Vector3 fr = default, float prevms = default, float curms = default)
		{
			Type = t;
			ObjectName = name;
			InitialPosition = ip;
			InitialRotation = ir;
			CurrentPosition = cp;
			CurrentRotation = cr;
			FinalPosition = fp;
			FinalRotation = fr;
			CurrentMoveSpeed = curms;
			PreviousMoveSpeed = prevms;
		}
	}
}