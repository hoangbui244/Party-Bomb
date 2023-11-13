namespace UnityEngine.Extension
{
	public static class Vector3Extension
	{
		public delegate T Evaluate1<T>(T origin);

		public delegate T Evaluate2<T>(T origin, T value);

		public static Vector3 X(this Vector3 vector, float x)
		{
			vector.x = x;
			return vector;
		}

		public static Vector3 X(this Vector3 vector, Evaluate1<float> func)
		{
			vector.x = func(vector.x);
			return vector;
		}

		public static Vector3 X(this Vector3 vector, float x, Evaluate2<float> func)
		{
			vector.x = func(vector.x, x);
			return vector;
		}

		public static Vector3 Y(this Vector3 vector, float y)
		{
			vector.y = y;
			return vector;
		}

		public static Vector3 Y(this Vector3 vector, Evaluate1<float> func)
		{
			vector.y = func(vector.y);
			return vector;
		}

		public static Vector3 Y(this Vector3 vector, float y, Evaluate2<float> func)
		{
			vector.y = func(vector.y, y);
			return vector;
		}

		public static Vector3 Z(this Vector3 vector, float z)
		{
			vector.z = z;
			return vector;
		}

		public static Vector3 Z(this Vector3 vector, Evaluate1<float> func)
		{
			vector.z = func(vector.z);
			return vector;
		}

		public static Vector3 Z(this Vector3 vector, float z, Evaluate2<float> func)
		{
			vector.z = func(vector.z, z);
			return vector;
		}

		public static ref Vector3 RefX(ref Vector3 vector, float x)
		{
			vector.x = x;
			return ref vector;
		}

		public static ref Vector3 RefX(ref Vector3 vector, Evaluate1<float> func)
		{
			vector.x = func(vector.x);
			return ref vector;
		}

		public static ref Vector3 RefX(ref Vector3 vector, float x, Evaluate2<float> func)
		{
			vector.x = func(vector.x, x);
			return ref vector;
		}

		public static ref Vector3 RefY(ref Vector3 vector, float y)
		{
			vector.y = y;
			return ref vector;
		}

		public static ref Vector3 RefY(ref Vector3 vector, Evaluate1<float> func)
		{
			vector.y = func(vector.y);
			return ref vector;
		}

		public static ref Vector3 RefY(ref Vector3 vector, float y, Evaluate2<float> func)
		{
			vector.y = func(vector.y, y);
			return ref vector;
		}

		public static ref Vector3 RefZ(ref Vector3 vector, float z)
		{
			vector.z = z;
			return ref vector;
		}

		public static ref Vector3 RefZ(ref Vector3 vector, Evaluate1<float> func)
		{
			vector.z = func(vector.z);
			return ref vector;
		}

		public static ref Vector3 RefZ(ref Vector3 vector, float z, Evaluate2<float> func)
		{
			vector.z = func(vector.z, z);
			return ref vector;
		}
	}
}
