using System;
using UnityEngine;
public class AlpineSkiing_CourseManager : SingletonCustom<AlpineSkiing_CourseManager> {
    public GameObject[][] arrayCheckPointAuto;
    private GameObject goalPoint;
    public int[] currentCheckPointLength;
    public void Init() {
        arrayCheckPointAuto = new GameObject[AlpineSkiing_Define.MEMBER_NUM][];
        currentCheckPointLength = new int[AlpineSkiing_Define.MEMBER_NUM];
        GetCheckPointAnchorAuto();
    }
    public Transform GetGoalPoint() {
        goalPoint = GameObject.Find("GoalChecker");
        return goalPoint.transform;
    }
    public void GetCheckPointAnchorAuto() {
        for (int i = 0; i < AlpineSkiing_Define.MEMBER_NUM; i++) {
            arrayCheckPointAuto[i] = GameObject.FindGameObjectsWithTag("CheckPoint_Player" + i.ToString());
            currentCheckPointLength[i] = arrayCheckPointAuto[i].Length;
            Array.Sort(arrayCheckPointAuto[i], YPositionComparison);
            Array.Reverse((Array)arrayCheckPointAuto[i]);
        }
    }
    public int YPositionComparison(GameObject a, GameObject b) {
        if (a == null) {
            if (!(b == null)) {
                return -1;
            }
            return 0;
        }
        if (b == null) {
            return 1;
        }
        float y = a.transform.position.y;
        float y2 = b.transform.position.y;
        return y.CompareTo(y2);
    }
    public Transform GetCheckPointAnchor(int player, int idx) {
        return arrayCheckPointAuto[player][idx].transform;
    }
}
