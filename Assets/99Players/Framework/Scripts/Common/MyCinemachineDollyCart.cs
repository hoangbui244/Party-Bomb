using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;
[ExecuteInEditMode]
public class MyCinemachineDollyCart : MonoBehaviour {
    public CinemachinePathBase m_Path;
    public CinemachinePathBase.PositionUnits m_PositionUnits = CinemachinePathBase.PositionUnits.Distance;
    [FormerlySerializedAs("m_CurrentDistance")]
    public float m_Position;
    [FormerlySerializedAs("m_Speed")]
    public float m_Speed;
    private void Update() {
        SetCartPosition(m_Position + m_Speed * Time.deltaTime);
    }
    private void SetCartPosition(float distanceAlongPath) {
        if (m_Path != null) {
            m_Position = m_Path.StandardizeUnit(distanceAlongPath, m_PositionUnits);
            base.transform.position = m_Path.EvaluatePositionAtUnit(m_Position, m_PositionUnits);
            Vector3 vector = m_Path.EvaluateTangentAtUnit(m_Position, m_PositionUnits);
            vector.y = 0f;
            if (vector == Vector3.zero) {
                base.transform.rotation = Quaternion.identity;
            } else {
                base.transform.rotation = Quaternion.LookRotation(vector);
            }
        }
    }
}
