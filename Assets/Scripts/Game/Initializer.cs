using System.Threading.Tasks;
using Engine;
using Engine.Audio;
using Engine.Audio.Settings;
using Engine.Scenes;
using Engine.UI;
using Game.Statistics;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityEngine.EventSystems
{
	public class Initializer : MonoBehaviour
	{
		public async void Start()
		{
			await InitializeAsync();
			IoC.TryResolve<UiManager>(out var uiManager);
			IoC.TryResolve<SceneSwitcher>(out var sceneSwitcher);
			await sceneSwitcher.SwitchSceneAsync( "MainMenu", closedAction: () => { }, additionLoad: ()=>uiManager.ActivateAsync( "UI/MainMenu" ) );
		}

		public static async Task InitializeAsync()
		{
			if (IoC.TryResolve<UiManager>(out var uiManager))
			{
				return;
			}

			if (!IoC.TryResolve<UiManager>(out uiManager))
			{
				var objectOperationHandle = Addressables.LoadAssetAsync<GameObject>("UI/UiManager");
				if (objectOperationHandle.IsValid())
				{
					await objectOperationHandle.Task;
					if (objectOperationHandle.Status == AsyncOperationStatus.Succeeded)
					{
						var uiManagerObject = (GameObject) GameObject.Instantiate(objectOperationHandle.Result);
						uiManagerObject.name = "UiManager";
						Addressables.Release(objectOperationHandle);
						uiManager = uiManagerObject.GetComponent<UiManager>();
						if (uiManager)
						{
							DontDestroyOnLoad(uiManager);
							IoC.AddInstance(uiManager);
						}
					}
				}
			}
			var sceneLoader = new SceneLoader();
			IoC.AddInstance(sceneLoader);
			var sceneSwitcher = new SceneSwitcher(sceneLoader, uiManager);
			IoC.AddInstance(sceneSwitcher);
			var saveData = new SaveDataManager();
			saveData.Load();
			int sfxVolume = saveData.GetValue("SfxVolume");
			IoC.AddInstance(saveData);
			AudioManager audioManager = default;
			if (!IoC.TryResolve<AudioManager>(out audioManager))
			{
				var objectOperationHandle = Addressables.LoadAssetAsync<GameObject>("Audio/SfxManager");
				if (objectOperationHandle.IsValid())
				{
					await objectOperationHandle.Task;
					if (objectOperationHandle.Status == AsyncOperationStatus.Succeeded)
					{
						var audioManagerObject = (GameObject) GameObject.Instantiate(objectOperationHandle.Result);
						audioManagerObject.name = "SfxManager";
						Addressables.Release(objectOperationHandle);
						audioManager = audioManagerObject.GetComponent<AudioManager>();
						if (uiManager)
						{
							DontDestroyOnLoad(audioManager);
							IoC.AddInstance(audioManager);
						}
					}
				}
			}
			if (audioManager)
			{
				audioManager.Volume = sfxVolume*0.01f;
				var objectOperationHandle = Addressables.LoadAssetAsync<AudioBank>("Audio/SfxBank");
				if (objectOperationHandle.IsValid())
				{
					await objectOperationHandle.Task;
					if (objectOperationHandle.Status == AsyncOperationStatus.Succeeded)
					{
						var audioBank = (AudioBank) GameObject.Instantiate(objectOperationHandle.Result);
						audioManager.AddBank(audioBank);
					}
				}
			}


			
		}
	}
}