using UnityEngine;
public class FindMask_Define : MonoBehaviour
{
	public enum OperationType
	{
		Player0,
		Player1,
		Player2,
		Player3,
		Cpu0,
		Cpu1,
		Cpu2,
		Cpu3,
		Cpu4
	}
	public static int CHARA_NUM = 4;
	public static int TOTAL_MASK_NUM = 40;
	public static int CPU_SELECT_PROBABILITY;
	public static int WEAK_SELECT_PROBABILITY = 70;
	public static int NORMAL_SELECT_PROBABILITY = 80;
	public static int STRONG_SELECT_PROBABILITY = 90;
	public static int CPU_MEMORY_CAPACITY;
	public static int WEAK_MEMORY_CAPACITY = 7;
	public static int NORMAL_MEMORY_CAPACITY = 9;
	public static int STRONG_MEMORY_CAPACITY = 11;
}
