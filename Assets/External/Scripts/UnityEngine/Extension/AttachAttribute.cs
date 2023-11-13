namespace UnityEngine.Extension
{
	public class AttachAttribute : InspectorAttribute
	{
		public AttachMode AttachMode
		{
			get;
		}

		public AttachAttribute(AttachMode mode = AttachMode.FindCurrent)
		{
			AttachMode = mode;
		}
	}
}
