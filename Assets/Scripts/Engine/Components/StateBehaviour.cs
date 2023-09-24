using UnityEngine;

namespace Engine.Components
{
	//! ResettableBehaviour \author Serheo 
	public class StateBehaviour : ResettableBehaviour
	{
		#region Members

		protected bool mInvalidState = false;

		#endregion // Members


		#region Component Functions

		//! UpdateState \author Serheo 
		protected virtual void Update()
		{
			UpdateState();
		}

		//! OnEnable \author Serheo 
		protected virtual void OnEnable()
		{
			UpdateState();
		}

		//! OnDisable \author Serheo 
		protected virtual void OnDisable()
		{
			
		}

		#endregion // Component Functions

		#region Virtual Functions

		//! OnExpose \author Serheo 
		protected override void OnExpose()
		{
			mInvalidState = false;
		}

		//! OnDispose \author Serheo 
		protected override void OnDispose()
		{
			
		}

		//! UpdateState \author Serheo 
		public virtual void UpdateState()
		{
			if (mInvalidState && !mDisposed)
			{
				mInvalidState = false;
				OnUpdateState();
			}
		}

		//! OnUpdateState \author Serheo 
		[ContextMenu("Update State")]
		protected virtual void OnUpdateState()
		{
			
		}

		#endregion // Virtual Functions

		#region Functions

		//! InvalidateState \author Serheo 
		public void InvalidateState()
		{
			mInvalidState = true;
		}

		#endregion // Functions

	}
}
