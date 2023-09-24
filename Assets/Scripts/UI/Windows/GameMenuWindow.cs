using Engine;
using Engine.Scenes;
using Engine.UI;
using Engine.UI.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace TestGame.UI.Windows
{
	public class GameMenuWindow : Window
	{
		[field:SerializeField]
		public Button ResumeButton { get; private set; }
		
		[field:SerializeField]
		public Button SettingsButton { get; private set; }
		
		[field:SerializeField]
		public Button ExitButton { get; private set; }
		
		protected override void OnExpose()
		{
			ResumeButton?.onClick.AddListener(OnResumeButton);
			ExitButton?.onClick.AddListener(OnExitButton);
			SettingsButton?.onClick.AddListener(OnSettingsButton);
		}
		
		protected override void OnDispose()
		{
			ResumeButton?.onClick.RemoveListener(OnResumeButton);
			ExitButton?.onClick.RemoveListener(OnExitButton);
			SettingsButton?.onClick.RemoveListener(OnSettingsButton);
		}
		
		public void OnResumeButton()
		{
			DeactivateSelf();
		}

		public async void OnSettingsButton()
		{
			IoC.TryResolve<UiManager>(out var uiManager);
			await uiManager.ActivateAsync("UI/Settings");
		}

		public async void OnExitButton()
		{
			DeactivateSelf();
			IoC.TryResolve<UiManager>(out var uiManager);
			IoC.TryResolve<SceneSwitcher>(out var sceneSwitcher);
			await sceneSwitcher.SwitchSceneAsync( "MainMenu"
				, closedAction: () =>
				{
					uiManager.Deactivate("UI/HUD");
				}
				, additionLoad: ()=>
				{
					return uiManager.ActivateAsync("UI/MainMenu");
				});			
		}
	}
}