using System;
using UnityEngine;
using UnityEngine.UI;

namespace Engine.UI.Windows
{
	public class CurtainWindow : AnimationWindow
	{
		// Fields
		[SerializeField]
		private Slider progressSlider = default;

		private Func<float> progressFunc = default;

		protected override void Update()
		{
			base.Update();
			if (progressFunc != null && progressSlider)
			{
				progressSlider.value = progressFunc();
			}
		}

		public void StartProgress(Func<float> progressFunc)
		{
			this.progressFunc = progressFunc;
		}
	}
}