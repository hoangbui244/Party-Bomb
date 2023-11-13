using System.Collections.Generic;

namespace UnityEngine.Extension
{
	public static class TransformExtension
	{
		public delegate T Evaluate1<T>(T origin);

		public delegate T Evaluate2<T>(T origin, T value);

		public static void Identity(this Transform transform)
		{
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
		}

		public static void LocalPosition(this Transform transform, Evaluate1<Vector3> func)
		{
			transform.localPosition = func(transform.localPosition);
		}

		public static void LocalPosition(this Transform transform, Vector3 v, Evaluate2<Vector3> func)
		{
			transform.localPosition = func(transform.localPosition, v);
		}

		public static void Position(this Transform transform, Evaluate1<Vector3> func)
		{
			transform.position = func(transform.position);
		}

		public static void Position(this Transform transform, Vector3 v, Evaluate2<Vector3> func)
		{
			transform.position = func(transform.position, v);
		}

		public static void LocalRotation(this Transform transform, Evaluate1<Quaternion> func)
		{
			transform.localRotation = func(transform.localRotation);
		}

		public static void LocalRotation(this Transform transform, Quaternion q, Evaluate2<Quaternion> func)
		{
			transform.localRotation = func(transform.localRotation, q);
		}

		public static void Rotation(this Transform transform, Evaluate1<Quaternion> func)
		{
			transform.rotation = func(transform.rotation);
		}

		public static void Rotation(this Transform transform, Quaternion q, Evaluate2<Quaternion> func)
		{
			transform.rotation = func(transform.rotation, q);
		}

		public static void LocalEulerAngles(this Transform transform, Evaluate1<Vector3> func)
		{
			transform.localEulerAngles = func(transform.localEulerAngles);
		}

		public static void LocalEulerAngles(this Transform transform, Vector3 v, Evaluate2<Vector3> func)
		{
			transform.localEulerAngles = func(transform.localEulerAngles, v);
		}

		public static void EulerAngles(this Transform transform, Evaluate1<Vector3> func)
		{
			transform.eulerAngles = func(transform.eulerAngles);
		}

		public static void EulerAngles(this Transform transform, Vector3 v, Evaluate2<Vector3> func)
		{
			transform.eulerAngles = func(transform.eulerAngles, v);
		}

		public static void LocalScale(this Transform transform, Evaluate1<Vector3> func)
		{
			transform.localScale = func(transform.localScale);
		}

		public static void LocalScale(this Transform transform, Vector3 v, Evaluate2<Vector3> func)
		{
			transform.localScale = func(transform.localScale, v);
		}

		public static void GetAllChildren(this Transform transform, List<Transform> results, bool recursive = false)
		{
			int childCount = transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform child = transform.GetChild(i);
				results.Add(child);
				if (recursive)
				{
					child.GetAllChildren(results, recursive: true);
				}
			}
		}
	}
}
