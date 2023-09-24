using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Engine.UI.Windows;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using WindowContainer = System.Collections.Generic.LinkedList<UnityEngine.GameObject>;
using ObjectContainer = System.Collections.Generic.Dictionary<string, UnityEngine.GameObject>;

namespace Engine.UI
{
	[AddComponentMenu("Engine/UI/UiManager")]
	public class UiManager : MonoBehaviour
	{
		public delegate void WindowEventHandler(UiManager theManager, Window theWindow);

		#region Members

		[SerializeField]
		private EventSystem eventSystem = default;

		[SerializeField]
		private Camera uiCamera = default;

		private readonly WindowContainer activeWindows = new WindowContainer();
		private readonly ObjectContainer cacheWindows = new ObjectContainer();

		private int[] layerIds = default;
		private int[] layerOffsets = default;
		private int[] layerCurrentOrders = default;
		private readonly Stack<Transform> stack = new Stack<Transform>();


		private WindowEventHandler activateWindowEvent = null;
		private WindowEventHandler deactivateWindowEvent = null;

		#endregion // Members

		#region Properties

		public GameObject TopWindowObject => activeWindows.Last?.Value;
		
		public Camera Camera => uiCamera;

		public Window TopWindow
		{
			get
			{
				var aWindowObject = TopWindowObject;
				return aWindowObject != null ? aWindowObject.GetComponent<Window>() : null;
			}
		}

		public GameObject TopNormalObject
		{
			get
			{
				return FindWindow(theGameObject =>
				{
					var aWindow = theGameObject.GetComponent<Window>();
					return aWindow != null && (aWindow.WindowType == WindowType.Dialog || aWindow.WindowType == WindowType.Basic);
				});
			}
		}

		public Window TopNormalWindow
		{
			get
			{
				var aWindowObject = TopNormalObject;
				return aWindowObject != null ? aWindowObject.GetComponent<Window>() : null;
			}
		}

		public GameObject TopDialogObject
		{
			get
			{
				return FindWindow(theGameObject =>
				{
					var aWindow = theGameObject.GetComponent<Window>();
					return aWindow != null && aWindow.WindowType == WindowType.Dialog;
				});
			}
		}

		public Window TopDialog
		{
			get
			{
				var aWindowObject = TopDialogObject;
				return aWindowObject != null ? aWindowObject.GetComponent<Window>() : null;
			}
		}

		public EventSystem EventSystem
		{
			get { return eventSystem; }
		}

		public Window FocusWindow
		{
			get
			{
				var aFocusObject = FocusObject;
				if (aFocusObject == null)
				{
					return null;
				}

				return aFocusObject.GetComponentInParent<Window>();
			}
		}

		public GameObject FocusObject
		{
			get { return eventSystem.currentSelectedGameObject; }
			set
			{
				var aFocus = (value != null && value.activeInHierarchy) ? value : null;
				eventSystem.SetSelectedGameObject(aFocus);
			}
		}

		public event WindowEventHandler ActivateEvent
		{
			add
			{
				activateWindowEvent -= value;
				activateWindowEvent += value;
			}
			remove { activateWindowEvent -= value; }
		}

		public event WindowEventHandler DeactivateEvent
		{
			add
			{
				deactivateWindowEvent -= value;
				deactivateWindowEvent += value;
			}
			remove { deactivateWindowEvent -= value; }
		}

		#endregion // Properties

		protected void Awake()
		{
			var layerCount = SortingLayer.layers.Length;
			layerIds = new int[layerCount];
			layerOffsets = new int[layerCount];
			layerCurrentOrders = new int[layerCount];
			ResetLayerOrders();
		}


		#region Functions

		private void ResetLayerOrders()
		{
			var layerCount = layerIds.Length;
			for (var index = 0; index < layerCount; ++index)
			{
				layerIds[index] = SortingLayer.layers[index].id;
				layerOffsets[index] = 0;
				layerCurrentOrders[index] = -1;
			}
		}

		private async Task<GameObject> AllocateWindowObject<TWindow>(string windowName) where TWindow : Window
		{
			GameObject windowObject = default;
			TWindow window = null;
			if (cacheWindows.TryGetValue(windowName, out windowObject))
			{
				cacheWindows.Remove(windowName);
				if (windowObject != null)
				{
					window = windowObject.GetComponent<TWindow>();
					if (window == null)
					{
						Destroy(windowObject);
						return null;
					}
					windowObject.SetActive(true);
					window.Expose();
					return windowObject;
				}
			}
			var asyncOperation = Addressables.InstantiateAsync(windowName);
			await asyncOperation.Task;
			return asyncOperation.Result;
		}

		private bool FreeWindowObject(GameObject windowObject, bool cacheWindow = false)
		{
			if (!cacheWindow)
			{
				Addressables.ReleaseInstance(windowObject);
				return true;
			}

			if (cacheWindows.ContainsKey(windowObject.name))
			{
				Debug.LogWarning("[WindowManager].[FreeObject] Object have been already in cache!");
				Destroy(windowObject);
				return false;
			}

			windowObject.SetActive(false);
			var aWindow = windowObject.GetComponent<Window>();
			if (aWindow == null)
			{
				Debug.LogWarning("[WindowManager].[FreeObject] Object have been already in cache!");
				Destroy(windowObject);
				return false;
			}

			aWindow.Dispose();
			cacheWindows.Add(windowObject.name, windowObject);
			return true;
		}

		public void ClearCache()
		{
			foreach (var windowObject in cacheWindows.Values)
			{
				FreeWindowObject(windowObject);
			}

			cacheWindows.Clear();
		}

		public bool HasActiveWindow(string windowName)
		{
			foreach (var windowObject in activeWindows)
			{
				if (windowObject.name == windowName)
				{
					return true;
				}
			}

			return false;
		}

		public GameObject GetActiveWindow(string windowName)
		{
			foreach (var windowObject in activeWindows)
			{
				if (windowObject.name == windowName)
				{
					return windowObject;
				}
			}

			return null;
		}

		public TWindow GetActiveWindow<TWindow>(string windowName) where TWindow : Window
		{
			return GetActiveWindow(windowName).GetComponent<TWindow>();
		}

		public TWindow GetActiveWindow<TWindow>() where TWindow : Window
		{
			foreach (var windowObject in activeWindows)
			{
				TWindow aWindow = windowObject.GetComponent<TWindow>();
				if (aWindow != null)
				{
					return aWindow;
				}
			}

			return null;
		}

		public GameObject FindWindow(Predicate<GameObject> predicate)
		{
			var aNode = activeWindows.Last;
			while (aNode != null)
			{
				var aGameObject = aNode.Value;
				aNode = aNode.Previous;
				if (predicate(aGameObject))
				{
					return aGameObject;
				}
			}

			return null;
		}

		public async Task<Window> ActivateAsync(string windowName, Action<Window> theActivate = null,
			Action<Window> theDeactivate = null)
		{
			return await ActivateAsync<Window>(windowName, theActivate, theDeactivate);
		}

		public async Task<TWindow> ActivateAsync<TWindow>(string windowName, Action<TWindow> theActivate = null,
			Action<TWindow> theDeactivate = null) where TWindow : Window
		{
			if (HasActiveWindow(windowName))
			{
				return null;
			}

			if (!gameObject)
			{
				return null;
			}

			var windowObject = await AllocateWindowObject<TWindow>(windowName);
			if (windowObject == null)
			{
				return null;
			}

			var window = windowObject.GetComponent<TWindow>();
			if (window == null)
			{
				return null;
			}

			void OnBeforeActivate(Window theWindow)
			{
				theActivate?.Invoke(theWindow as TWindow);
			}

			void OnAfterDeactivate(Window theWindow)
			{
				theDeactivate?.Invoke(theWindow as TWindow);
			}

			var aPrevTopWindow = TopWindow;
			windowObject.transform.SetParent(transform, true);
			windowObject.name = windowName;
			windowObject.SetActive(true);
			activeWindows.AddLast(windowObject);
			SetCameraForCanvases(windowObject);
			ReorderWindowLayers(windowObject);
			window.ActivateEvent += OnBeforeActivate;
			window.DeactivateEvent += OnAfterDeactivate;
			window.Activate(this);
			if (aPrevTopWindow != null)
				aPrevTopWindow.LostTop();
			window.GotTop();
			activateWindowEvent?.Invoke(this, window);

			return window;
		}

		public bool Deactivate(string windowName)
		{
			var aWindowObject = GetActiveWindow(windowName);
			if (aWindowObject == null)
			{
				return false;
			}

			return Deactivate(aWindowObject);
		}

		public bool Deactivate(GameObject windowObject)
		{
			if (windowObject == null)
			{
				return false;
			}

			var window = windowObject.GetComponent<Window>();
			if (window == null)
			{
				return false;
			}

			if (!activeWindows.Remove(windowObject))
			{
				return false;
			}

			window.LostTop();
			var topWindow = TopWindow;
			if (topWindow != null)
			{
				topWindow.GotTop();
			}

			deactivateWindowEvent?.Invoke(this, window);
			bool hasFocus = window.HasFocus;
			window.gameObject.SetActive(false);
			window.Deactivate(this);
			if (hasFocus)
			{
				FocusObject = null;
				Canvas.ForceUpdateCanvases();
			}

			if (!window.Disposed)
			{
				FreeWindowObject(windowObject, window.CacheWindow);
			}

			if (activeWindows.Count == 0)
			{
				ResetLayerOrders();
			}

			return true;
		}

		public void DeactivateAll()
		{
			while (TopWindowObject != null)
			{
				Deactivate(TopWindowObject);
			}
		}

		public bool IsWindowInCache(string theWindowName)
		{
			return cacheWindows.ContainsKey(theWindowName);
		}

		private void SetCameraForCanvases(GameObject windowObject)
		{
			var canvases = windowObject.GetComponentsInChildren<Canvas>();
			foreach (var canvas in canvases)
			{
				if (canvas.renderMode != RenderMode.WorldSpace)
				{
					continue;
				}

				canvas.worldCamera = uiCamera;
			}
		}

		private void ReorderWindowLayers(GameObject windowObject)
		{
			stack.Push(windowObject.transform);

			while (stack.Count > 0)
			{
				var currentTransform = stack.Pop();
				if (currentTransform.TryGetComponent<Renderer>(out var renderer))
				{
					var layerIndex = Array.IndexOf(layerIds, renderer.sortingLayerID);
					var sortingOrder = renderer.sortingOrder + layerOffsets[layerIndex];
					renderer.sortingOrder = sortingOrder;
					layerCurrentOrders[layerIndex] = Mathf.Max(layerCurrentOrders[layerIndex], sortingOrder);
				}

				if (currentTransform.TryGetComponent<Canvas>(out var canvas))
				{
					var layerIndex = Array.IndexOf(layerIds, canvas.sortingLayerID);
					var sortingOrder = canvas.sortingOrder + layerOffsets[layerIndex];
					canvas.sortingOrder = sortingOrder;
					layerCurrentOrders[layerIndex] = Mathf.Max(layerCurrentOrders[layerIndex], sortingOrder);
				}

				if (currentTransform.TryGetComponent<SortingGroup>(out var sortingGroup))
				{
					var layerIndex = Array.IndexOf(layerIds, sortingGroup.sortingLayerID);
					var sortingOrder = sortingGroup.sortingOrder + layerOffsets[layerIndex];
					sortingGroup.sortingOrder = sortingOrder;
					layerCurrentOrders[layerIndex] = Mathf.Max(layerCurrentOrders[layerIndex], sortingOrder);
				}

				if (currentTransform.TryGetComponent<SpriteMask>(out var spriteMask))
				{
					if (spriteMask.isCustomRangeActive)
					{
						var layerIndex = Array.IndexOf(layerIds, spriteMask.frontSortingLayerID);
						var sortingOrder = spriteMask.frontSortingOrder + layerOffsets[layerIndex];
						spriteMask.frontSortingOrder = sortingOrder;
						layerCurrentOrders[layerIndex] = Mathf.Max(layerCurrentOrders[layerIndex], sortingOrder);

						layerIndex = Array.IndexOf(layerIds, spriteMask.backSortingLayerID);
						sortingOrder = spriteMask.backSortingOrder + layerOffsets[layerIndex];
						spriteMask.backSortingOrder = sortingOrder;
						layerCurrentOrders[layerIndex] = Mathf.Max(layerCurrentOrders[layerIndex], sortingOrder);
					}
				}

				if (currentTransform.TryGetComponent<LayerOrderReserve>(out var reserve))
				{
					var layerIndex = Array.IndexOf(layerIds, reserve.SortingLayer);
					var sortingOrder = reserve.SortingOrder + layerOffsets[layerIndex];
					reserve.SortingOrder = sortingOrder;
					layerCurrentOrders[layerIndex] = Mathf.Max(layerCurrentOrders[layerIndex], sortingOrder);
				}

				var numChildren = currentTransform.childCount;
				for (var index = 0; index < numChildren; ++index)
				{
					stack.Push(currentTransform.GetChild(index));
				}
			}

			var layerCount = layerOffsets.Length;
			for (var index = 0; index < layerCount; ++index)
			{
				layerOffsets[index] = Mathf.Max(layerOffsets[index], layerCurrentOrders[index] + 1);
				layerCurrentOrders[index] = -1;
			}
		}

		#endregion // Functions
	}
}