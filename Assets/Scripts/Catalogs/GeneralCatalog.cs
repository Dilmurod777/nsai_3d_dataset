using System.Collections.Generic;
using System.Text.RegularExpressions;
using Constants;

namespace Catalogs
{
	public interface IGeneralCatalogInterface
	{
		object SaveVal2Var(string args);
		int Count(object[] objects);
		bool Exist(object[] objects);
		object Unique(string args);
		string[] ExtractNumbers(string args);
		string ExtractID(string args);
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
			return objects.Length;
		}

		public bool Exist(object[] objects)
		{
			return (objects.Length > 0);
		}

		public object Unique(string args)
		{
			var argsList = args.Split(GeneralConstants.ArgsSeparator);
			var prev = Context.GetAttribute(argsList[0]);
			return prev.Count > 0 ? prev[0] : null;
		}

		public string[] ExtractNumbers(string args)
		{
			var argsList = args.Split(GeneralConstants.ArgsSeparator);
			var query = Context.GetAttribute(argsList[0]);
		
			var numbers = Regex.Matches(query, @"((\d|-)+[A-Z]+)|(([0-9]*[.])?[0-9]+)|(\[\d+\])");
			var result = new string[numbers.Count];
			for (var i = 0; i < numbers.Count; i++)
			{
				result[i] = numbers[i].ToString();
			}
			return result;
		}

		public string ExtractID(string args)
		{
			var argsList = args.Split(GeneralConstants.ArgsSeparator);
			var attrId = argsList[0];
			var source = Context.GetAttribute(argsList[1]);
			
			switch (attrId)
			{
				case "subtask_id":
				case "task_id":
					return Regex.Match(source, @"[\d-]+$").Value;
				case "figure":
					return Regex.Match(source, @"((\d|-)+[A-Z]+)").Value;
				case "object":
					return Regex.Match(source, @"(\[\d+\])").Value;
					default:
					return Regex.Match(source, @"\d+$").Value;
			}
		}
	}
}