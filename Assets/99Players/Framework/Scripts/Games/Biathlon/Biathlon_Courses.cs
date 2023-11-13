using UnityEngine;
public class Biathlon_Courses : SingletonCustom<Biathlon_Courses>
{
	[SerializeField]
	private Biathlon_Course[] courses;
	[SerializeField]
	private Transform anchor;
	public Biathlon_Course Current
	{
		get;
		private set;
	}
	public void Init()
	{
		GenerateCourse();
	}
	private void GenerateCourse()
	{
		Biathlon_Course biathlon_Course = Object.Instantiate(courses[Random.Range(0, courses.Length)], anchor);
		biathlon_Course.Init();
		Current = biathlon_Course;
	}
}
