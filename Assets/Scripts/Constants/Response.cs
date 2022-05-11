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
}