using Engine;
using Engine.Scenes;
using Engine.UI;
using Engine.UI.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace TestGame.UI.Windows
{
	public class MainMenuWindow : Window
	{
		[SerializeField]
		private Button playButton = default;
		[SerializeField]
		private Button settingsButton = default;
		
		protected override void OnExpose()
		{
			playButton.onClick.AddListener(OnPlayButton);
			settingsButton.onClick.AddListener(OnSettingsButton);
		}
		
		protected override void OnDispose()
		{
			playButton.onClick.RemoveListener(OnPlayButton);
			settingsButton.onClick.RemoveListener(OnSettingsButton);
		}
		
		public async void OnPlayButton()
		{
			IoC.TryResolve<UiManager>(out var uiManager);
			IoC.TryResolve<SceneSwitcher>(out var sceneSwitcher);
			await sceneSwitcher.SwitchSceneAsync( "Level01"
				, closedAction: () =>
				{
					uiManager.Deactivate("UI/MainMenu");
				}
				, additionLoad: ()=>
				{
					return uiManager.ActivateAsync("UI/HUD");
				});
		}

		public async void OnSettingsButton()
		{
			IoC.TryResolve<UiManager>(out var uiManager);
			await uiManager.ActivateAsync("UI/Settings");
		}

	}
}