using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EpicToonFX
{
	public class ETFXEffectControllerPooled : MonoBehaviour
	{
		public GameObject[] effects;

		private List<GameObject> effectsPool;

		private int effectIndex;

		[Space(10f)]
		[Header("Spawn Settings")]
		public bool disableLights = true;

		public bool disableSound = true;

		public float startDelay = 0.2f;

		public float respawnDelay = 0.5f;

		public bool slideshowMode;

		public bool autoRotation;

		[Range(0.001f, 0.5f)]
		public float autoRotationSpeed = 0.1f;

		private GameObject currentEffect;

		private Text effectNameText;

		private Text effectIndexText;

		private ETFXMouseOrbit etfxMouseOrbit;

		private void Awake()
		{
			effectNameText = GameObject.Find("EffectName").GetComponent<Text>();
			effectIndexText = GameObject.Find("EffectIndex").GetComponent<Text>();
			etfxMouseOrbit = Camera.main.GetComponent<ETFXMouseOrbit>();
			etfxMouseOrbit.etfxEffectControllerPooled = this;
			effectsPool = new List<GameObject>();
			for (int i = 0; i < effects.Length; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(effects[i], base.transform.position, Quaternion.identity);
				gameObject.transform.parent = base.transform;
				effectsPool.Add(gameObject);
				gameObject.SetActive(value: false);
			}
		}

		private void Start()
		{
			Invoke("InitializeLoop", startDelay);
		}

		private void Update()
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.RightArrow) || UnityEngine.Input.GetKeyDown(KeyCode.D))
			{
				NextEffect();
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.A) || UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow))
			{
				PreviousEffect();
			}
		}

		private void FixedUpdate()
		{
			if (autoRotation)
			{
				etfxMouseOrbit.SetAutoRotationSpeed(autoRotationSpeed);
				if (!etfxMouseOrbit.isAutoRotating)
				{
					etfxMouseOrbit.InitializeAutoRotation();
				}
			}
		}

		public void InitializeLoop()
		{
			StartCoroutine(EffectLoop());
		}

		public void NextEffect()
		{
			if (effectIndex < effects.Length - 1)
			{
				effectIndex++;
			}
			else
			{
				effectIndex = 0;
			}
			CleanCurrentEffect();
		}

		public void PreviousEffect()
		{
			if (effectIndex > 0)
			{
				effectIndex--;
			}
			else
			{
				effectIndex = effects.Length - 1;
			}
			CleanCurrentEffect();
		}

		private void CleanCurrentEffect()
		{
			StopAllCoroutines();
			if (currentEffect != null)
			{
				currentEffect.SetActive(value: false);
			}
			StartCoroutine(EffectLoop());
		}

		private IEnumerator EffectLoop()
		{
			currentEffect = effectsPool[effectIndex];
			currentEffect.SetActive(value: true);
			if (disableLights && (bool)currentEffect.GetComponent<Light>())
			{
				currentEffect.GetComponent<Light>().enabled = false;
			}
			if (disableSound && (bool)currentEffect.GetComponent<AudioSource>())
			{
				currentEffect.GetComponent<AudioSource>().enabled = false;
			}
			effectNameText.text = effects[effectIndex].name;
			effectIndexText.text = (effectIndex + 1).ToString() + " of " + effects.Length.ToString();
			ParticleSystem particleSystem = currentEffect.GetComponent<ParticleSystem>();
			while (true)
			{
				yield return new WaitForSeconds(particleSystem.main.duration + respawnDelay);
				if (!slideshowMode)
				{
					if (!particleSystem.main.loop)
					{
						currentEffect.SetActive(value: false);
						currentEffect.SetActive(value: true);
					}
				}
				else
				{
					if (particleSystem.main.loop)
					{
						yield return new WaitForSeconds(respawnDelay);
					}
					NextEffect();
				}
			}
		}
	}
}
