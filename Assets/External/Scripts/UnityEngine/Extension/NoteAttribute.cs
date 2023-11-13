namespace UnityEngine.Extension
{
	public class NoteAttribute : InspectorAttribute
	{
		public string Text
		{
			get;
		}

		public DrawPosition DrawPosition
		{
			get;
		}

		public NoteAttribute(string text, DrawPosition drawPosition = DrawPosition.After)
		{
			Text = text;
			DrawPosition = drawPosition;
		}
	}
}
