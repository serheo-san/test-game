using Engine.Common;
using UnityEngine;

namespace Engine.Components
{
	//! ResettableBehaviour \author Serheo 
	public class ResettableBehaviour : MonoBehaviour, IResettable
	{
		#region Members

		protected bool mDisposed = true;

		#endregion // Members

		public bool Disposed => mDisposed;

		#region Component Functions

		//! Awake \author Serheo 
		protected virtual void Awake()
		{
			Expose();
		}

		//! OnDisable \author Serheo 
		protected virtual void OnDestroy()
		{
			Dispose();
		}

		#endregion // Component Functions

		#region Virtual Functions

		//! OnExpose \author Serheo 
		protected virtual void OnExpose()
		{
			
		}

		//! OnDispose \author Serheo 
		protected virtual void OnDispose()
		{
			
		}

		#endregion // Virtual Functions

		#region Functions

		//! Expose \author Serheo 
		public void Expose()
		{
			if (mDisposed)
			{
				mDisposed = false;
				OnExpose();
			}
				
		}

		//! Dispose \author Serheo 
		public void Dispose()
		{
			if (!mDisposed)
			{
				mDisposed = true;
				OnDispose();
			}
		}

		#endregion // Functions

	}
}
