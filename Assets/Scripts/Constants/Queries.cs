using System;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

namespace Constants
{
	public static class Queries
	{
		public static List<QueryMeta> GetAllQueries()
		{
			var figureName = Context.Instance.CurrentFigureID;
			
			var jsonDocumentName = figureName + "-queries";
			var jsonContent = Resources.Load<TextAsset>(jsonDocumentName);
			
			var unparsedQueries = JSON.Parse(jsonContent.ToString());

			var parsedQueries = new List<QueryMeta>();

			foreach (JSONNode query in unparsedQueries)
			{
				parsedQueries.Add(new QueryMeta
				{
					
					Query = query.HasKey("query") ? query["query"].Value : "",
					Knowledge = query.HasKey("knowledge") ? query["knowledge"].Value : "",
					Reply = query.HasKey("reply") ? query["reply"].Value : "",
					Programs = query.HasKey("programs") ? query["programs"].AsArray : Array.Empty<string>()
				});
			}

			return parsedQueries;
		}
	}
}