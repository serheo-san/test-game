using Engine;
using Engine.Audio;
using Engine.UI.Windows;
using Game.Statistics;
using UnityEngine;
using UnityEngine.UI;

namespace TestGame.UI.Windows
{
	public class SettingsWindow : Window
	{
		[field:SerializeField]
		public Button ExitButton { get; private set; }
		[field:SerializeField]
		public Slider SfxSlider { get; private set; }
		
		private AudioManager sfxManager = default;
		private SaveDataManager saveDataManager = default;
		private float sfxTimer = 0f;
		
		protected override void OnExpose()
		{
			ExitButton.onClick.AddListener(OnExitButton);
			SfxSlider.onValueChanged.AddListener(OnSfxSlider);
			IoC.TryResolve<AudioManager>(out sfxManager);
			IoC.TryResolve<SaveDataManager>(out saveDataManager);
		}

		protected override void OnDispose()
		{
			ExitButton.onClick.RemoveListener(OnExitButton);
			SfxSlider.onValueChanged.RemoveListener(OnSfxSlider);
			sfxManager = default;
			saveDataManager = default;
		}
		

		protected override void OnActivate()
		{
			base.OnActivate();
			SfxSlider.value = sfxManager.Volume;
		}

		protected override void Update()
		{
			if (sfxTimer > 0f)
			{
				sfxTimer -= Time.deltaTime;
				if (sfxTimer <= 0f)
				{
					sfxManager.Play("CoinEvent");
				}
			}
		}

		public void OnExitButton()
		{
			DeactivateSelf();
			saveDataManager.Save();
		}
		
		private void OnSfxSlider(float newValue)
		{
			if(sfxManager && !sfxManager.Volume.Equals(newValue))
			{
				sfxManager.Volume = newValue;
				saveDataManager.SetValue("SfxVolume", (int)(newValue * 100f));
				sfxTimer = 0.3f;
			}
		}

	}
}