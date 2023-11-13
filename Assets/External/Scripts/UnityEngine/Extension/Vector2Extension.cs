namespace UnityEngine.Extension
{
	public static class Vector2Extension
	{
		public delegate T Evaluate1<T>(T origin);

		public delegate T Evaluate2<T>(T origin, T value);

		public static Vector2 X(this Vector2 vector, float x)
		{
			vector.x = x;
			return vector;
		}

		public static Vector2 X(this Vector2 vector, Evaluate1<float> func)
		{
			vector.x = func(vector.x);
			return vector;
		}

		public static Vector2 X(this Vector2 vector, float x, Evaluate2<float> func)
		{
			vector.x = func(vector.x, x);
			return vector;
		}

		public static Vector2 Y(this Vector2 vector, float y)
		{
			vector.y = y;
			return vector;
		}

		public static Vector2 Y(this Vector2 vector, Evaluate1<float> func)
		{
			vector.y = func(vector.y);
			return vector;
		}

		public static Vector2 Y(this Vector2 vector, float y, Evaluate2<float> func)
		{
			vector.y = func(vector.y, y);
			return vector;
		}

		public static ref Vector2 RefX(ref Vector2 vector, float x)
		{
			vector.x = x;
			return ref vector;
		}

		public static ref Vector2 RefX(ref Vector2 vector, Evaluate1<float> func)
		{
			vector.x = func(vector.x);
			return ref vector;
		}

		public static ref Vector2 RefX(ref Vector2 vector, float x, Evaluate2<float> func)
		{
			vector.x = func(vector.x, x);
			return ref vector;
		}

		public static ref Vector2 RefY(ref Vector2 vector, float y)
		{
			vector.y = y;
			return ref vector;
		}

		public static ref Vector2 RefY(ref Vector2 vector, Evaluate1<float> func)
		{
			vector.y = func(vector.y);
			return ref vector;
		}

		public static ref Vector2 RefY(ref Vector2 vector, float y, Evaluate2<float> func)
		{
			vector.y = func(vector.y, y);
			return ref vector;
		}

		public static Vector2 Clamp(this Vector2 vec, Vector2 min, Vector2 max)
		{
			Vector2 vector = new Vector2(vec.x, vec.y);
			if (vector.x < min.x)
			{
				vector.x = min.x;
			}
			if (vector.x > max.x)
			{
				vector.x = max.x;
			}
			if (vector.y < min.y)
			{
				vector.y = min.y;
			}
			if (vector.y > max.y)
			{
				vector.y = max.y;
			}
			return vector;
		}
	}
}
