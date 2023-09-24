using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Engine.Scenes
{
	//! SceneLoader \author Serheo \date 2018/11/14 17:19:15
	public class SceneLoader
	{
		public delegate void EventHandler(SceneLoader theSceneLoader);

		#region Members
		private string prevSceneName = string.Empty;
		private AsyncOperationHandle<SceneInstance> asyncOperation;
		private bool busy = false;

		private EventHandler loadBeginEvent = null;
		private EventHandler loadEndEvent = null;

		#endregion // Members

		#region Properties
		public bool IsBusy => busy;

		public string SceneName => Scene.name;

		public string PrevSceneName => prevSceneName;

		public Scene Scene => SceneManager.GetSceneAt( SceneManager.sceneCount - 1 );

		public event EventHandler LoadBeginEvent
		{
			add
			{
				loadBeginEvent -= value;
				loadBeginEvent += value;
			}
			remove
			{
				loadBeginEvent -= value;
			}
		}

		public event EventHandler LoadEndEvent
		{
			add
			{
				loadEndEvent -= value;
				loadEndEvent += value;
			}
			remove
			{
				loadEndEvent -= value;
			}
		}

		#endregion // Properties

		#region Functions

		public async Task<bool> LoadSceneAsync(string newSceneName)
		{
			if( busy ) {
				Debug.Log("[SceneLoader][LoadSceneAsync] Other scene already loading" );
			}
			busy = true;
			var sceneName = SceneName;
			loadBeginEvent?.Invoke( this );
			asyncOperation = Addressables.LoadSceneAsync( newSceneName );
			await asyncOperation.Task;
			prevSceneName = sceneName;
			busy = false;
			loadEndEvent?.Invoke( this );
			//Addressables.Release(asyncOperation);
			return true;
		}

		public float GetLoadingProgress() {
			if( !asyncOperation.IsValid() ) {
				return 1f;
			}
			return asyncOperation.PercentComplete;
		}

		#endregion // Functions
	}
}