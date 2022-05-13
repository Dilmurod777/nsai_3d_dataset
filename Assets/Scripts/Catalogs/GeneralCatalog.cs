using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Constants;
using UnityEngine;

namespace Catalogs
{
	public interface IGeneralCatalogInterface
	{
		object SaveVal2Var(string args);
		int Count(object[] objects);
		bool Exist(object[] objects);
		object Unique(string args);
		List<string> ExtractNumbers(string args);
		List<string> ExtractID(string args);
		string Same(string args);
	}

	public class GeneralCatalog : IGeneralCatalogInterface
	{
		public object SaveVal2Var(string args)
		{
			var argsList = args.Split(GeneralConstants.ArgsSeparator);
			var source = Context.GetAttribute(argsList[0]);
			var varName = argsList[1];

			switch (varName)
			{
				case "var1":
					Context.Instance.Var1 = source;
					break;
				case "var2":
					Context.Instance.Var2 = source;
					break;
			}

			return source;
		}

		public int Count(object[] objects)
		{
			Debug.Log("Count");
			return objects.Length;
		}

		public bool Exist(object[] objects)
		{
			Debug.Log("Exist");
			return objects.Length > 0;
		}

		public object Unique(string args)
		{
			var argsList = args.Split(GeneralConstants.ArgsSeparator);
			var prev = Context.GetAttribute(argsList[0]);
			return prev.Count > 0 ? prev[0] : null;
		}

		public List<string> ExtractNumbers(string args)
		{
			var argsList = args.Split(GeneralConstants.ArgsSeparator);
			string query = Context.GetAttribute(argsList[0]);
			
			
			var figureIds = Regex.Matches(query, GeneralConstants.FigureRegex);
			for (var i = 0; i < figureIds.Count; i++)
			{
				query = query.Replace(figureIds[i].Value, "");
			}
			
			var objectIds = Regex.Matches(query, GeneralConstants.ObjectRegex);
			for (var i = 0; i < objectIds.Count; i++)
			{
				query = query.Replace(objectIds[i].Value, "");
			}
			
			var numbers = Regex.Matches(query, GeneralConstants.NumberRegex);
			var result = new List<string>();
			for (var i = 0; i < numbers.Count; i++)
			{
				result.Add(numbers[i].Value);
			}

			return result;
		}

		public List<string> ExtractID(string args)
		{
			var argsList = args.Split(GeneralConstants.ArgsSeparator);
			var attrId = argsList[0];
			var source = Context.GetAttribute(argsList[1]);

			var result = new List<string>();
			switch (attrId)
			{
				case "subtask_id":
				case "task_id":
					foreach (Match match in Regex.Matches(source, @"[\d-]+$"))
					{
						result.Add(match.Value);
					}
					break;
				case "figure":
					foreach (Match match in Regex.Matches(source, GeneralConstants.FigureRegex))
					{
						result.Add(match.Value);
					}
					break;
				case "object":
					foreach (Match match in Regex.Matches(source, GeneralConstants.ObjectRegex))
					{
						result.Add(match.Value);
					}
					break;
				default:
					foreach (Match match in Regex.Matches(source, @"\d+$"))
					{
						result.Add(match.Value);
					}
					break;
			}
			
			return result;
		}

		public string Same(string args)
		{
			var argsList = args.Split(GeneralConstants.ArgsSeparator);
			var var1 = Context.GetAttribute(argsList[0]);
			var var2 = Context.GetAttribute(argsList[1]);

			return var1 == "402-32-11-61-990-802-A" ? var1 : "402-32-11-61-990-802-A";
		}
	}
}