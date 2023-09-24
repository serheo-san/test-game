using System.Threading.Tasks;
using Engine;
using Engine.Scenes;
using Engine.UI;
using Engine.UI.Windows;
using TestGame.Platformer;
using TMPro;
using UnityEngine;

namespace TestGame.UI.Windows
{
	public class LevelFinishWindow : AnimationWindow
	{
		[SerializeField]
		private TMP_Text levelNameView = default;

		[SerializeField] 
		private TMP_Text playerScoreView = default;

		[SerializeField]
		private TMP_Text bestScoreView = default;
		
		
		private int playerScore = 0;
		private int bestScore = 0;
		private string levelName = string.Empty;

		public string LevelName
		{
			get => levelName;
			set
			{
				if (levelName == value)
				{
					return;
				}
				levelName = value;
				InvalidateState();
			}
		}

		public int BestScore
		{
			get => bestScore;
			set
			{
				if (bestScore == value)
				{
					return;
				}
				bestScore = value;
				InvalidateState();
			}
		}

		public int PlayerScore
		{
			get => playerScore;
			set
			{
				if (playerScore == value)
				{
					return;
				}
				playerScore = value;
				InvalidateState();
			}
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			AppearAsync().ContinueWith(task =>
			{
				if (task.Exception != null)
				{
					Debug.LogError($"{task.Exception.Message}\n{task.Exception.StackTrace}");
				}
			}, TaskScheduler.FromCurrentSynchronizationContext());
		}

		protected override void OnUpdateState()
		{
			base.OnUpdateState();
			if (levelNameView)
			{
				levelNameView.text = levelName;
			}
			if (bestScoreView)
			{
				bestScoreView.text = bestScore.ToString();
			}
			if (playerScoreView)
			{
				playerScoreView.text = playerScore.ToString();
			}
		}

		public async void OnButtonExit()
		{
			await DisappearAsync();
			DeactivateSelf();
			IoC.TryResolve<UiManager>(out var uiManager);
            IoC.TryResolve<SceneSwitcher>(out var sceneSwitcher);
            await sceneSwitcher.SwitchSceneAsync( "MainMenu", closedAction: () =>
            {
	            uiManager.Deactivate("UI/HUD");
            }, additionLoad: ()=>
            {
	            return uiManager.ActivateAsync("UI/MainMenu");
            });

		}

		public async void OnButtonNext()
		{
			await DisappearAsync();
			DeactivateSelf();
			IoC.TryResolve<UiManager>(out var uiManager);
			IoC.TryResolve<SceneSwitcher>(out var sceneSwitcher);
			await sceneSwitcher.SwitchSceneAsync( "Level02" );
		}

		public async void OnButtonRestart()
		{
			await DisappearAsync();
			DeactivateSelf();
			IoC.TryResolve<LevelLogic>(out var levelLogic);
			if (levelLogic)
			{
				levelLogic.StartLevel();
			}
		}
	}
}