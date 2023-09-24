using Engine.Audio.Settings;
using Engine.Components;
using UnityEngine;
using UnityEngine.UI;

namespace Engine.Audio {
	[RequireComponent( typeof(Button) )]
	public class AudioButton : ResettableBehaviour {
		[SerializeField]
		private string audioManagerType = string.Empty;
		[SerializeField]
		private AudioEvent audioEvent = default;
		private AudioManager audioManager = default;

		protected override void OnExpose()
		{
			base.OnExpose();
			IoC.TryResolve(out audioManager, audioManagerType);
			var aButton = GetComponent<Button>();
			if ( aButton )
				aButton.onClick.AddListener( OnClick );
		}

		protected override void OnDispose()
		{
			base.OnDispose();
			var aButton = GetComponent<Button>();
			if ( aButton )
				aButton.onClick.RemoveListener( OnClick );
			audioManager = null;
		}
		
		private void OnClick() {
			if ( audioEvent && audioManager!=null ) {
				audioManager?.Play( audioEvent );
			}
		}
	}
}
