using System.Collections.Generic;
using UnityEngine;

namespace Constants
{
	public class Response
	{
		public Dictionary<string, dynamic> operation;
		public List<GameObject> objects;
		public Dictionary<string, dynamic> extra;

		public Response(Dictionary<string, dynamic> op, List<GameObject> objs, Dictionary<string, dynamic> ex)
		{
			operation = op;
			objects = objs;
			extra = ex;
		}
	}
}