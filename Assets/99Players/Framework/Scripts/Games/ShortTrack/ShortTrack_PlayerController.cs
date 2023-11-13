using System;
using System.Collections.Generic;
using UnityEngine;
public class ShortTrack_PlayerController : MonoBehaviour
{
	[Serializable]
	public struct AIStrong
	{
		[SerializeField]
		[Header("AIの加速変化[弱い]")]
		public List<float> WEAK_SPEED;
		[SerializeField]
		[Header("AIの加速変化[普通]")]
		public List<float> USUALLY_SPEED;
		[SerializeField]
		[Header("AIの加速変化[強い]")]
		public List<float> STRONG_SPEED;
		[SerializeField]
		[Header("スリップストリ\u30fcム中の速度変化[弱い]")]
		public List<float> SLIPSTREAM_WEAK_SPEED;
		[SerializeField]
		[Header("スリップストリ\u30fcム中の速度変化[弱い]")]
		public List<float> SLIPSTREAM_USUALLY_SPEED;
		[SerializeField]
		[Header("スリップストリ\u30fcム中の速度変化[弱い]")]
		public List<float> SLIPSTREAM_STRONG_SPEED;
	}
	[SerializeField]
	[Header("プレイヤ\u30fc番号")]
	private int PlayerNo;
	[SerializeField]
	[Header("最初の位置")]
	private Transform StartPos;
	public int CharacterRanking;
	private float CharacterSpeed;
	[SerializeField]
	[Header("触らなかった時の速さ")]
	private float DefaultSpeed;
	private Rigidbody rb;
	[NonSerialized]
	public bool LButton;
	[NonSerialized]
	public bool RButton;
	[NonSerialized]
	public bool IsSlipStream;
	private bool IsPlayer;
	private void Awake()
	{
		IsPlayer = true;
		switch (PlayerNo)
		{
		}
	}
	private void Update()
	{
		GameDuring();
	}
	private void FixedUpdate()
	{
	}
	private void GameDuring()
	{
		if (Time.timeScale != 0f && SHORTTRACK.MGM.IsDuringGame() && IsPlayer)
		{
			if (SHORTTRACK.CM.IsPushButton_A(PlayerNo))
			{
				CharacterSpeed += 1f;
			}
			if (SHORTTRACK.CM.IsPushButton_B(PlayerNo))
			{
				CharacterSpeed += 1f;
			}
			CharacterSpeed -= 0.01f;
			CharacterSpeed = Mathf.Clamp(CharacterSpeed, DefaultSpeed, 100f);
		}
	}
}
