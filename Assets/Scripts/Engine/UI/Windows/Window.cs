using System;
using System.Threading.Tasks;
using Engine.Components;
using UnityEngine;

namespace Engine.UI.Windows
{
	//! WindowType \author Serheo \date 2018/11/14 17:21:27
	public enum WindowType
	{
		Undefined = 0,
		Basic,
		Dialog,
		Curtain,
	}

	[AddComponentMenu("Engine/UI/Window")]
	public class Window : StateBehaviour
	{
		public delegate void EventHandler(Window theWindow);

		#region Members

		[SerializeField]
		protected WindowType windowType = WindowType.Undefined;

		[SerializeField]
		protected bool cacheWindow = false; //!< Window hidden window stored in the cache.

		protected UiManager manager = null;
		protected EventHandler activateEvent = null;
		protected EventHandler deactivateEvent = null;

		#endregion // Members

		public Window() : base()
		{
		}


		#region Properties

		public UiManager Manager
		{
			get { return manager; }
		}

		public RectTransform RectTransform
		{
			get { return transform as RectTransform; }
		}

		public bool Activated
		{
			get { return manager != null; }
		}

		public bool CacheWindow => cacheWindow;

		public WindowType WindowType => windowType;

		public bool HasFocus
		{
			get
			{
				if (manager == null)
				{
					return false;
				}

				return ReferenceEquals(manager.FocusWindow, this);
			}
		}

		public bool HasTop
		{
			get
			{
				if (manager == null)
				{
					return false;
				}

				return ReferenceEquals(manager.TopWindow, this);
			}
		}


		public event EventHandler DeactivateEvent
		{
			add
			{
				deactivateEvent -= value;
				deactivateEvent += value;
			}
			remove { deactivateEvent -= value; }
		}

		public event EventHandler ActivateEvent
		{
			add
			{
				activateEvent -= value;
				activateEvent += value;
			}
			remove { activateEvent -= value; }
		}

		#endregion // Properties

		#region Virtual Functions

		protected override void OnExpose()
		{
			mDisposed = false;
		}

		protected override void OnDispose()
		{
			mDisposed = true;
			if (manager != null)
			{
				manager.Deactivate(gameObject);
			}

			manager = null;
			deactivateEvent = null;
			activateEvent = null;
		}

		protected virtual void OnActivate()
		{
			if (activateEvent != null)
			{
				activateEvent(this);
				activateEvent = null;
			}
		}

		protected virtual void OnDeactivate()
		{
			if (deactivateEvent != null)
			{
				deactivateEvent(this);
				deactivateEvent = null;
			}
		}

		public virtual void GotTop()
		{
		}

		public virtual void LostTop()
		{
		}

		public virtual void DeactivateSelf()
		{
			if (manager != null)
			{
				manager.Deactivate(gameObject);
			}
		}

		#endregion // Virtual Functions

		#region Functions

		public bool Activate(UiManager theManager)
		{
			if (Activated)
			{
				return false;
			}

			manager = theManager;
			OnActivate();
			return true;
		}

		public bool Deactivate(UiManager theManager)
		{
			if (!Activated || manager != theManager)
			{
				return false;
			}

			OnDeactivate();
			manager = null;
			return true;
		}

		public Window OnDeactivateAction(Action theAction)
		{
			if (theAction != null)
			{
				EventHandler aFunction =
					(Window theWindow) => { theAction(); };
				DeactivateEvent += aFunction;
			}

			return this;
		}
		
		public async Task WaitForDeactivateAsync()
		{
			while (Activated)
			{
				await Task.Yield();
			}
		}

		#endregion // Functions
	}
}