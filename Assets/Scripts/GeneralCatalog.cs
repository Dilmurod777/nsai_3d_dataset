public interface IGeneralCatalogInterface
{
	object SaveVal2Var(object value, string varName);
}

public class GeneralCatalog
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
}