using System;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

namespace Constants
{
	public static class Queries
	{
		public static List<QueryMeta> GetAllKnowledgeQueries()
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
					Reply = query.HasKey("reply") ? query["reply"].Value : "",
					Programs = query.HasKey("programs") ? query["programs"].AsArray : Array.Empty<string>()
				});
			}

			return parsedQueries;
		}
		
		public static List<QueryMeta> GetAllGeneralQueries()
		{
			var jsonDocumentName = "general-queries";
			var jsonContent = Resources.Load<TextAsset>(jsonDocumentName);
			
			var unparsedQueries = JSON.Parse(jsonContent.ToString());

			var parsedQueries = new List<QueryMeta>();

			foreach (JSONNode query in unparsedQueries)
			{
				parsedQueries.Add(new QueryMeta
				{
					Query = query.HasKey("query") ? query["query"].Value : "",
					Reply = query.HasKey("reply") ? query["reply"].Value : "",
					Programs = query.HasKey("programs") ? query["programs"].AsArray : Array.Empty<string>()
				});
			}

			return parsedQueries;
		}
	}
}