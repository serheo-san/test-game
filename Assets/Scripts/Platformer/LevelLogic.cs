using System.Threading.Tasks;
using Engine;
using Engine.Audio;
using Engine.UI;
using Game.Statistics;
using TestGame.Platformer.Actors;
using TestGame.Platformer.Carriers;
using TestGame.Platformer.Loots;
using TestGame.UI.Windows;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TestGame.Platformer
{
	public class LevelLogic : MonoBehaviour
	{
		[SerializeField]
		private LootManager lootManager = default;
		[SerializeField]
		private Player player = default;
		[SerializeField]
		private Bot bot = default;
		[SerializeField]
		private Transform playerRespawnPoint = default;
		[SerializeField]
		private Transform botRespawnPoint = default;
		[SerializeField]
		private int levelDuration = 100;

		private UiManager uiManager = default;
		private HUDWindow hudWindow;

		private readonly Timer timer = new Timer();
		private int coins = 0;

		private bool isPause = false;

		
		protected void Awake()
		{
			timer.StopEvent += OnTimerStop;
			IoC.AddInstance(this);
		}

		protected void OnDestroy()
		{
			if (lootManager)
			{
				lootManager.LootTakenEvent -= OnLootTaken;
			}
			timer.StopEvent -= OnTimerStop;
			IoC.RemoveInstance(this);
		}
		
		protected async void Start()
		{
			if (lootManager)
			{
				lootManager.LootTakenEvent += OnLootTaken;
			}
			if (hudWindow && hudWindow.TimerPanel)
			{
				hudWindow.TimerPanel.SetTimer(timer);
			}
			StartLevel();
			await WaitHUDAsync();
		}

		protected void Update()
		{
			if (isPause)
			{
				return;
			}
			timer.Update(Time.deltaTime);
		}

		private async Task WaitHUDAsync()
		{
			while (!IoC.TryResolve(out uiManager))
			{
				await Task.Yield();	
			}

			while (hudWindow==null && uiManager)
			{
				hudWindow = uiManager.GetActiveWindow<HUDWindow>();
				await Task.Yield();
			}
			if (hudWindow == null)
			{
				return;
			}
			if (hudWindow.TimerPanel)
			{
				hudWindow.TimerPanel.SetTimer(timer);
			}
			if (hudWindow.CoinPanel)
			{
				hudWindow.CoinPanel.SetCoins(coins);
			}

		}

		public void StartLevel()
		{
			timer.Start(levelDuration);
			if (lootManager)
			{
				lootManager.PlaceFirstCoins();
				lootManager.PlaceChest();
			}
			if (player)
			{
				player.gameObject.SetActive(false);
				player.transform.position = playerRespawnPoint.transform.position;
				player.gameObject.SetActive(true);
				player.MoveRight();
			}
			if (bot)
			{
				bot.gameObject.SetActive(false);
				bot.transform.position = botRespawnPoint.transform.position;
				bot.gameObject.SetActive(true);
				bot.MoveRight();
			}
			coins = 0;
			if (hudWindow && hudWindow.CoinPanel)
			{
				hudWindow.CoinPanel.SetCoins(coins);
			}
		}

		public void Pause()
		{
			if (isPause)
			{
				return;
			}

			isPause = true;
			if(player)
				player.Pause();
			if(bot)
				bot.Pause();
		}

		public void Continue()
		{
			if (!isPause)
			{
				return;
			}
			isPause = false;
			if(player)
				player.Continue();
			if(bot)
				bot.Continue();
		}
		
		private void OnTimerStop(Timer timer)
		{
			if(player)
				player.Freeze();
			int prevBestScore = 0;
			var sceneName = SceneManager.GetActiveScene().name;
			IoC.TryResolve<SaveDataManager>(out var saveDataManager);
			if (saveDataManager != null)
			{
				prevBestScore = saveDataManager.GetValue(sceneName);
				if (prevBestScore<coins)
				{
					saveDataManager.SetValue(sceneName, coins);
					saveDataManager.Save();
				}
			}

			if (uiManager)
			{
				uiManager.ActivateAsync<LevelFinishWindow>("UI/LevelFinish", window =>
				{
					window.LevelName = sceneName;
					window.PlayerScore = coins;
					window.BestScore = prevBestScore;
				}).ContinueWith(task =>
				{
					if (task.Exception != null)
						Debug.LogException(task.Exception);
				}, TaskScheduler.FromCurrentSynchronizationContext() );	
			}
		}
		
		private void OnLootTaken(Loot loot)
		{
			coins += loot.Count;
			var newCoins = coins;
			IoC.TryResolve<CarrierManager>(out var carrierManager);
			if (carrierManager && hudWindow && hudWindow.CoinPanel)
			{
				carrierManager.Deliver(loot.Sprite, loot.transform.localScale, loot.transform, hudWindow.CoinTransform, uiManager.Camera, carrier =>
				{
					if (hudWindow && hudWindow.CoinPanel)
					{
						hudWindow.CoinPanel.AnimateCoins(newCoins);
					}
				});
			}
			else
			{
				if (hudWindow && hudWindow.CoinPanel)
				{
					hudWindow.CoinPanel.AnimateCoins(newCoins);
				}
			}

			IoC.TryResolve<AudioManager>(out var sfxManager);
			if(sfxManager)
			{
				sfxManager.Play("CoinEvent");
			}
		}
	}
}