using System;
using UnityEngine;
namespace io.ninenine.players.party2d.games.bomberman {
    /// <summary>
    /// 
    /// </summary>
    public class BombermanCameraController: SingletonCustom<BombermanCameraController> {
        /// <summary>
        /// 
        /// </summary>
        public float damping = 1;
        /// <summary>
        /// 
        /// </summary>
        public float lookAheadFactor = 3;
        /// <summary>
        /// 
        /// </summary>
        public float lookAheadReturnSpeed = 0.5f;
        /// <summary>
        /// 
        /// </summary>
        public float lookAheadMoveThreshold = 0.1f;
        /// <summary>
        /// 
        /// </summary>
        private float m_OffsetZ;
        /// <summary>
        /// 
        /// </summary>
        private Vector3 m_LastTargetPosition;
        /// <summary>
        /// 
        /// </summary>
        private Vector3 m_CurrentVelocity;
        /// <summary>
        /// 
        /// </summary>
        private Vector3 m_LookAheadPos;
        /// <summary>
        /// 
        /// </summary>
        private Vector3 m_CentralView = Vector3.zero;
        //private void Start() {
        //    CalculateCentral();
        //    m_LastTargetPosition = m_CentralView;
        //    //transform.parent = null;
        //}
        //public void CalculateCentral() {
        //    m_CentralView = Vector3.zero;
        //    int Count = 0;
        //    foreach (GameObject GO in BombermanGameManager.Instance.PlayerList) {
        //        m_CentralView += GO.transform.position;
        //        Count++;
        //    }
        //    m_CentralView /= Count;
        //    m_OffsetZ = (transform.position - m_CentralView).z;
        //}
        //private void Update() {
        //    CalculateCentral();
        //    {
        //        // only update lookahead pos if accelerating or changed direction
        //        float xMoveDelta = (m_CentralView - m_LastTargetPosition).x;
        //        bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;
        //        if (updateLookAheadTarget) {
        //            m_LookAheadPos = lookAheadFactor * Vector3.right * Mathf.Sign(xMoveDelta);
        //        }
        //        else {
        //            m_LookAheadPos = Vector3.MoveTowards(m_LookAheadPos, Vector3.zero, Time.deltaTime * lookAheadReturnSpeed);
        //        }
        //        Vector3 aheadTargetPos = m_CentralView + m_LookAheadPos + Vector3.forward * m_OffsetZ;
        //        Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref m_CurrentVelocity, damping);
        //        if (newPos.x < -2.924f) {
        //            newPos.x = -2.92f;
        //        }
        //        else if (newPos.x > 2.92f) {
        //            newPos.x = 2.92f;
        //        }
        //        if (newPos.y < -0.89) {
        //            newPos.y = -0.89f;
        //        }
        //        else if (newPos.y > 105.76f) {
        //            newPos.y = 105.76f;
        //        }
        //        transform.position = newPos;
        //        m_LastTargetPosition = m_CentralView;
        //    }
        //}
    }
}
