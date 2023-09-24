using System.Threading.Tasks;
using UnityEngine;

namespace Engine.UI.Windows
{
	public class AnimationWindow : Window
	{
		// Fields
		[SerializeField]
		protected Animator animator = default;

		[SerializeField]
		protected string appearanceStateName = "Appearance";

		[SerializeField]
		protected string disappearanceStateName = "Disappearance";

		[SerializeField]
		protected float transitionDuration = .2f;


		// Properties
		public bool Appeared { get; private set; } = false;


		// Methods
		public async Task<bool> AppearAsync()
		{
			if (Appeared || !animator)
			{
				return false;
			}
			animator.CrossFadeInFixedTime(appearanceStateName, transitionDuration);
			animator.Update(0);
			while (!Appeared)
			{
				await Task.Yield();
			}

			return true;
		}

		public async Task<bool> DisappearAsync()
		{
			if (!Appeared || !animator)
			{
				return false;
			}
			animator.CrossFadeInFixedTime(disappearanceStateName, transitionDuration);
			while (Appeared)
			{
				await Task.Yield();
			}
			return true;
		}

		public virtual void OnDisappeared()
		{
			Appeared = false;
		}

		public virtual void OnAppeared()
		{
			Appeared = true;
		}
	}
}