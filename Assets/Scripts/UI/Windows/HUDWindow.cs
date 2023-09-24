using Engine;
using Engine.UI;
using Engine.UI.Windows;
using TestGame.Platformer;
using TestGame.Platformer.UI;
using UnityEngine;
using UnityEngine.UI;

namespace TestGame.UI.Windows
{
	public class HUDWindow : Window
	{
		[field:SerializeField]
		public CoinPanel CoinPanel { get; private set; }   

		[field:SerializeField]
		public TimerPanel TimerPanel { get; private set; }
		
		[field:SerializeField]
		public Transform CoinTransform { get; private set; }
		
		[field:SerializeField]
		public Button MenuButton { get; private set; }
		
		protected override void OnExpose()
		{
			MenuButton?.onClick.AddListener(OnMenuButton);
		}
		
		protected override void OnDispose()
		{
			MenuButton?.onClick.RemoveListener(OnMenuButton);
		}

		public async void OnMenuButton()
		{
			IoC.TryResolve<LevelLogic>(out var levelLogic);
			IoC.TryResolve<UiManager>(out var uiManager);
			var window = await uiManager.ActivateAsync("UI/GameMenu");
			levelLogic?.Pause();
			await window.WaitForDeactivateAsync();
			levelLogic?.Continue();
		}
	}
}