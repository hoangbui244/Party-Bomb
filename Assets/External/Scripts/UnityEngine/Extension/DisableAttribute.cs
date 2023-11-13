namespace UnityEngine.Extension
{
	public class DisableAttribute : InspectorAttribute
	{
		public bool EnableOnPlaying
		{
			get;
		}

		public DisableAttribute(bool enableOnPlaying = false)
		{
			EnableOnPlaying = enableOnPlaying;
		}
	}
}
