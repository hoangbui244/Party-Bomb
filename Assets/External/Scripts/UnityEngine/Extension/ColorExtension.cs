namespace UnityEngine.Extension
{
	public static class ColorExtension
	{
		public delegate T Evaluate1<T>(T origin);

		public delegate T Evaluate2<T>(T origin, T value);

		public static Color R(this Color color, float r)
		{
			color.r = r;
			return color;
		}

		public static Color G(this Color color, float g)
		{
			color.g = g;
			return color;
		}

		public static Color B(this Color color, float b)
		{
			color.b = b;
			return color;
		}

		public static Color A(this Color color, float a)
		{
			color.a = a;
			return color;
		}

		public static Color R(this Color color, Evaluate1<float> func)
		{
			color.r = func(color.r);
			return color;
		}

		public static Color G(this Color color, Evaluate1<float> func)
		{
			color.g = func(color.g);
			return color;
		}

		public static Color B(this Color color, Evaluate1<float> func)
		{
			color.b = func(color.b);
			return color;
		}

		public static Color A(this Color color, Evaluate1<float> func)
		{
			color.a = func(color.a);
			return color;
		}

		public static Color R(this Color color, float r, Evaluate2<float> func)
		{
			color.r = func(color.r, r);
			return color;
		}

		public static Color G(this Color color, float g, Evaluate2<float> func)
		{
			color.g = func(color.g, g);
			return color;
		}

		public static Color B(this Color color, float b, Evaluate2<float> func)
		{
			color.b = func(color.b, b);
			return color;
		}

		public static Color A(this Color color, float a, Evaluate2<float> func)
		{
			color.a = func(color.a, a);
			return color;
		}

		public static ref Color RefR(ref Color color, float r)
		{
			color.r = r;
			return ref color;
		}

		public static ref Color RefG(ref Color color, float g)
		{
			color.g = g;
			return ref color;
		}

		public static ref Color RefB(ref Color color, float b)
		{
			color.b = b;
			return ref color;
		}

		public static ref Color RefA(ref Color color, float a)
		{
			color.a = a;
			return ref color;
		}

		public static ref Color RefR(ref Color color, Evaluate1<float> func)
		{
			color.r = func(color.r);
			return ref color;
		}

		public static ref Color RefG(ref Color color, Evaluate1<float> func)
		{
			color.g = func(color.g);
			return ref color;
		}

		public static ref Color RefB(ref Color color, Evaluate1<float> func)
		{
			color.b = func(color.b);
			return ref color;
		}

		public static ref Color RefA(ref Color color, Evaluate1<float> func)
		{
			color.a = func(color.a);
			return ref color;
		}

		public static ref Color RefR(ref Color color, float r, Evaluate2<float> func)
		{
			color.r = func(color.r, r);
			return ref color;
		}

		public static ref Color RefG(ref Color color, float g, Evaluate2<float> func)
		{
			color.g = func(color.g, g);
			return ref color;
		}

		public static ref Color RefB(ref Color color, float b, Evaluate2<float> func)
		{
			color.b = func(color.b, b);
			return ref color;
		}

		public static ref Color RefA(ref Color color, float a, Evaluate2<float> func)
		{
			color.a = func(color.a, a);
			return ref color;
		}

		public static ref float GetRefR(ref Color color)
		{
			return ref color.r;
		}

		public static ref float GetRefG(ref Color color)
		{
			return ref color.g;
		}

		public static ref float GetRefB(ref Color color)
		{
			return ref color.b;
		}

		public static ref float GetRefA(ref Color color)
		{
			return ref color.a;
		}
	}
}
