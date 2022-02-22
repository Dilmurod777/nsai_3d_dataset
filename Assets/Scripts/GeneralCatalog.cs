using System.Text.RegularExpressions;
public interface IGeneralCatalogInterface
{
	object SaveVal2Var(object value, string varName);
	int Count(object[] objects);
	bool Exist(object[] objects);
	object Unique(object[] objects);
	string[] ExtractNumbers(string value);
	string ExtractID(string attrId, string query);
}

public class GeneralCatalog : IGeneralCatalogInterface
{
	public object SaveVal2Var(object value, string varName)
	{
		switch (varName)
		{
			case "v_1":
				Context.Instance.Var1 = value;
				break;
			case "v_2":
				Context.Instance.Var2 = value;
				break;
		}
		return value;
	}

	public int Count(object[] objects)
	{
		return objects.Length;
	}

	public bool Exist(object[] objects)
	{
		return (objects.Length > 0);
	}

	public object Unique(object[] objects)
	{
		return objects.Length > 0 ? objects[0] : null;
	}

	public string[] ExtractNumbers(string value)
	{
		MatchCollection numbers = Regex.Matches(value, @"\d+");
		string[] result = new string[numbers.Count];
		for (int i = 0; i < numbers.Count; i++)
		{
			result[i] = numbers[i].ToString();
		}
		return result;
	}

	public string ExtractID(string attrId, string query)
	{
		string result = "";
		switch (attrId)
		{
			case "subtask_id":
			case  "task_id":
				result = Regex.Match(query, @"[\d-]+$").Value;
				break;
			case "title":
				result = Regex.Match(query, @"[\d-]+[A-Z]$").Value;
				break;
			default:
				result = Regex.Match(query, @"\d+$").Value;
				break;
		}
		return result;
	}
}