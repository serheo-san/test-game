using System;
using System.Threading.Tasks;
using Engine.UI;
using Engine.UI.Windows;

namespace Engine.Scenes {
	public class SceneSwitcher {

		private SceneLoader sceneLoader = default;
		private UiManager uiManager = default;

		private bool busy = false;

		public SceneSwitcher( SceneLoader sceneLoader, UiManager uiManager ) {
			this.sceneLoader = sceneLoader;
			this.uiManager = uiManager;
		}

		public async Task SwitchSceneAsync( string sceneName, string curtainFormName="UI/VerticalCurtain", Func<Task> additionLoad = null, Action closedAction=null, Action openingAction=null ) {
			if( busy ) {
				return;
			}
			busy = true;
			var window = await uiManager.ActivateAsync<CurtainWindow>( curtainFormName );
			if( !window ) {
				busy = false;
				return;
			}
			await window.AppearAsync();
			closedAction?.Invoke();
			window.StartProgress(  sceneLoader.GetLoadingProgress );
			await sceneLoader.LoadSceneAsync( sceneName );
			await Task.Yield();
			if (additionLoad != null)
			{
				await additionLoad.Invoke();
			}
			openingAction?.Invoke();
			window.StartProgress( null );
			await window.DisappearAsync();
			window.DeactivateSelf();
			busy = false;
		}
	}
}