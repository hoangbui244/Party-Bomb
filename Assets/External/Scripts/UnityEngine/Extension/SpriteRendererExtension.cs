namespace UnityEngine.Extension
{
	public static class SpriteRendererExtension
	{
		public delegate T Evaluate1<T>(T origin);

		public delegate T Evaluate2<T>(T origin, T value);

		public static void Color(this SpriteRenderer renderer, Evaluate1<Color> func)
		{
			renderer.color = func(renderer.color);
		}

		public static void Color(this SpriteRenderer renderer, Color color, Evaluate2<Color> func)
		{
			renderer.color = func(renderer.color, color);
		}

		public static void Size(this SpriteRenderer renderer, Evaluate1<Vector2> func)
		{
			if (renderer.drawMode == SpriteDrawMode.Sliced)
			{
				renderer.size = func(renderer.size);
			}
		}

		public static void Size(this SpriteRenderer renderer, Vector2 size, Evaluate2<Vector2> func)
		{
			if (renderer.drawMode == SpriteDrawMode.Sliced)
			{
				renderer.size = func(renderer.size, size);
			}
		}
	}
}
