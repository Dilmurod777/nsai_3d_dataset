public static class State
{
	public const string On = "on";
	public const string Off = "off";
	
	public enum Axis
	{
		X,
		Y,
		Z
	}

	public enum FigureSide
	{
		Right, Left, Front, Back, Top, Bottom
	}
	
	public delegate void VoidFunction(); 
}