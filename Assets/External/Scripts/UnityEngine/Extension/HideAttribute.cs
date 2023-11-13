namespace UnityEngine.Extension
{
	public class HideAttribute : InspectorAttribute
	{
		public string TargetProperty
		{
			get;
		}

		public bool Reverse
		{
			get;
		}

		public HideAttribute(string propertyName, bool reverse = false)
		{
			TargetProperty = propertyName;
			Reverse = reverse;
		}
	}
}
