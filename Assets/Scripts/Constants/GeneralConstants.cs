namespace Constants
{
	public static class GeneralConstants
	{
		public const char ArgsSeparator = '#';
		public const string FigureRegex = @"((\d|-)+[A-Z]+)";
		public const string ObjectRegex = @"(\[\d+\])";
		public const string NumberRegex = @"(([0-9]*[.])?[0-9]+)";
		
		public enum AttachTypes {SmoothInstall, Align, StepScrew, SmoothScrew, StepInstall}
	}
}