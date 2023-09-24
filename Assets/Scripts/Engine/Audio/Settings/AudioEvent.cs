using System;
using Engine.Common;
using UnityEngine;

namespace Engine.Audio.Settings {
	[CreateAssetMenu( fileName = "AudioEvent", menuName = "Engine/Audio/Audio Event" )]
	public class AudioEvent : ScriptableObject, IComparable<AudioEvent>, IOwnerId<string>, ISerializationCallbackReceiver {
		#region Fields

		[SerializeField, HideInInspector]
		private string id = string.Empty;

		[SerializeField]
		private AudioClip audioClip = null;

		[SerializeField, Range( 0f, 1f )]
		private float volumeScale = 1f;
		
		[SerializeField]
		private bool loop = false;

		[SerializeField]
		private bool single = false;
		
		[SerializeField]
		private float fadeDuration = 2f;

		[SerializeField]
		private float delay = 0f;


		#endregion // Fields

		#region Properties

		public string Id => id;
		
		public AudioClip AudioClip => audioClip;

		public float VolumeScale => volumeScale;

		public bool Loop => loop;

		public bool Single => single;
		
		public float FadeDuration => fadeDuration;

		public float Delay => delay;
		

		string IOwnerId<string>.Id {
			get => id;
			set => id = value;
		}

		#endregion // Properties

		#region Methods

		int IComparable<AudioEvent>.CompareTo( AudioEvent theOther ) {
			if ( theOther == null ) {
				return 1;
			}
			return String.Compare( Id, theOther.Id, StringComparison.InvariantCulture );
		}

		public void OnBeforeSerialize() {
			id = name;
		}

		public void OnAfterDeserialize() {

		}

		#endregion // Methods
	}
}
