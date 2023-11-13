namespace UnityEngine.Extension
{
	public class DisplayNameAttribute : InspectorAttribute
	{
		public string DisplayName
		{
			get;
		}

		public DisplayNameAttribute(string label)
		{
			DisplayName = label;
		}
	}
}
