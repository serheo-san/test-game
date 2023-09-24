using Engine.Audio.Settings;
using UnityEngine;
using UnityEngine.Audio;
using PlayerList = System.Collections.Generic.LinkedList<Engine.Audio.AudioPlayer>;
using SoundDictionary = System.Collections.Generic.Dictionary<UnityEngine.AudioClip, int>;
using AudioBankList = System.Collections.Generic.List<Engine.Audio.Settings.AudioBank>;

namespace Engine.Audio {
	public class AudioManager : MonoBehaviour {
		#region Members

		[SerializeField]
		private AudioPlayer etalon = null;
		[SerializeField]
		private AudioMixerGroup mixerGroup = null;

		private PlayerList players = new PlayerList();
		private PlayerList pool = new PlayerList();
		private SoundDictionary soundStatistic = new SoundDictionary();
		private AudioBankList audioBanks = new AudioBankList();
		private float muteVolume = 0f;
		private bool isMute = false;
		private bool isPause = false;

		#endregion // Members

		#region Constants

		public const string VolumeParameterName = "Volume";
		public const float LowerVolumeBound = -80.0f;
		public const float UpperVolumeBound = 0.0f;

		#endregion // Constants

		#region Constructor

		public AudioManager() {
		}

		#endregion // Constructor

		#region Properties

		protected float BaseVolume {
			get {
				float aValue = 0f;
				if ( mixerGroup ) {
					mixerGroup.audioMixer.GetFloat( VolumeParameterName, out aValue );
				}
				return  Mathf.InverseLerp( LowerVolumeBound, UpperVolumeBound, aValue );
			}
			set {
				if ( mixerGroup ) {
					mixerGroup.audioMixer.SetFloat( VolumeParameterName, Mathf.Lerp(LowerVolumeBound, UpperVolumeBound, value ) );
				}
			}
		}

		public float Volume {
			get => isMute ? muteVolume : BaseVolume;
			set {
				if ( isMute ) {
					muteVolume = value;
				}
				else {
					BaseVolume = value;
				}
			}
		}

		public bool Mute {
			get => isMute;
			set {
				if ( isMute && !value ) {
					isMute = false;
					BaseVolume = muteVolume;
				}
				else if ( !isMute && value ) {
					isMute = true;
					muteVolume = BaseVolume;
					UpdateMuteState();
				}
			}
		}

		public bool IsPause => isPause;

		#endregion // Properties

		protected void Update() {
			UpdateMuteState();
		}

		#region Functions
		
		private void UpdateMuteState() {
			if ( isMute && BaseVolume > 0 ) {
				BaseVolume = 0f;
			}
		}

		private AudioPlayer Allocate() {
			AudioPlayer aPlayer;
			if ( pool.Count > 0 ) {
				aPlayer = pool.Last.Value;
				pool.RemoveLast();
				aPlayer.Expose();
			}
			else {
				var aGameObject = etalon ? Instantiate( etalon.gameObject ) : new GameObject( "sound", typeof(AudioPlayer) );
				aGameObject.transform.parent = transform;
				aPlayer = aGameObject.GetComponent<AudioPlayer>();
				aPlayer.Initialize( this );
			}
			aPlayer.gameObject.SetActive( true );
			return aPlayer;
		}

		private bool Free( AudioPlayer thePlayer ) {
			if ( !ReferenceEquals( thePlayer.Manager, this ) ) {
				Debug.LogWarningFormat( "Invalid Player Manager '{0}'", thePlayer.name );
				return false;
			}
			thePlayer.Dispose();
			thePlayer.gameObject.SetActive( false );
			pool.AddLast( thePlayer );
			return true;
		}

		public void DestroyUnused() {
			while ( pool.Count > 0 ) {
				var aPlayer = pool.Last.Value;
				pool.RemoveLast();
				if ( aPlayer != null && aPlayer.gameObject != null ) {
					Destroy( aPlayer.gameObject );
				}
			}
		}

		private AudioPlayer Construct( AudioClip theAudioClip ) {
			var aPlayer = Allocate();
			aPlayer.Set( theAudioClip, mixerGroup );
			aPlayer.name = theAudioClip.name;
			return aPlayer;
		}

		public bool AddBank( AudioBank theAudioBank ) {
			if ( audioBanks.Contains( theAudioBank ) ) {
				return false;
			}
			audioBanks.Add( theAudioBank );
			return true;
		}

		public bool RemoveBank( AudioBank theAudioBank ) {
			return audioBanks.Remove( theAudioBank );
		}

		public AudioEvent FindAudioEvent( string theAudioId ) {
			if ( string.IsNullOrEmpty( theAudioId ) )
				return null;
			foreach ( var audioBank in audioBanks ) {
				var audioEvent = audioBank.FindById( theAudioId );
				if ( audioEvent )
					return audioEvent;
			}
			return null;
		}

		public AudioPlayer FindAudioPlayer( AudioClip theAudioClip ) {
			if ( !theAudioClip ) {
				return null;
			}
			foreach ( var anPlayer in players ) {
				if ( !object.ReferenceEquals( anPlayer.AudioClip, theAudioClip ) ) {
					continue;
				}
				return anPlayer;
			}
			return null;
		}

		public void Pause() {
			if ( isPause ) {
				return;
			}
			isPause = true;
			foreach ( var aPlayer in players ) {
				aPlayer.Pause();
			}
		}

		public void Continue() {
			if ( !isPause ) {
				return;
			}
			isPause = false;
			foreach ( var aPlayer in players ) {
				aPlayer.Continue();
			}
		}

		public void StopAll() {
			var aNode = players.First;
			while(aNode!=null) {
				var aPlayer = aNode.Value;
				aNode = aNode.Next;
				aPlayer.RemoveAllListeners();
				aPlayer.Stop();
			}
		}

		public void FadeOutAll( float theFadeDuration = 2f, AudioClip theExcludeAudioClip = null ) {
			foreach ( var aPlayer in players ) {
				if ( theExcludeAudioClip == aPlayer.AudioClip )
					continue;
				aPlayer.RemoveAllListeners();
				aPlayer.FadeOut( theFadeDuration );
			}
		}
		
		public AudioPlayer Play( AudioClip theAudioClip, bool theLoop = false, float theVolume = 1f, bool theSingle = false, float theDelay = 0f ) {
			if ( !theAudioClip ) {
				return null;
			}
			if ( theSingle && IsSoundPlayed( theAudioClip ) ) {
				return null;
			}
			var aPlayer = Construct( theAudioClip );
			if ( !aPlayer ) {
				return null;
			}
			if ( !aPlayer.Play( theLoop, theVolume, theDelay ) ) {
				Free( aPlayer );
				return null;
			}
			if ( isPause ) {
				aPlayer.Pause();
			}
			return aPlayer;
		}

		public bool Stop( AudioClip theAudioClip ) {
			bool aResult = false;
			var aNode = players.First;
			while(aNode!=null) {
				var aPlayer = aNode.Value;
				aNode = aNode.Next;
				if ( aPlayer.AudioClip == theAudioClip ) {
					aPlayer.Stop();
					aResult = true;
				}
			}
			return aResult;
		}

		public AudioPlayer FadeIn( AudioClip theAudioClip, float theFadeDuration = 2f, bool theLoop = false, float theVolume = 1f, bool theSingle = false, float theDelay = 0f ) {
			if ( !theAudioClip ) {
				return null;
			}
			if ( theSingle && IsSoundPlayed( theAudioClip ) ) {
				var aPrevPlayer = FindAudioPlayer( theAudioClip );
				if ( aPrevPlayer && aPrevPlayer.FadeIn(theFadeDuration, theLoop, theVolume, theDelay) ) {
					return aPrevPlayer;
				}
				return null;
			}
			var aPlayer = Construct( theAudioClip );
			if ( !aPlayer ) {
				return null;
			}
			if ( !aPlayer.FadeIn( theFadeDuration, theLoop, theVolume, theDelay ) ) {
				Free( aPlayer );
				return null;
			}
			if ( isPause ) {
				aPlayer.Pause();
			}
			return aPlayer;
		}

		public bool FadeOut( AudioClip theAudioClip, float theFadeDuration = 2f ) {
			bool aResult = false;
			foreach ( var aPlayer in players ) {
				if ( aPlayer.AudioClip == theAudioClip ) {
					aPlayer.FadeOut( theFadeDuration );
					aResult = true;
				}
			}
			return aResult;
		}
		
		public AudioPlayer Play( AudioEvent theAudioEvent ) {
			if ( !theAudioEvent || !theAudioEvent.AudioClip ) {
				return null;
			}
			var aPlayer = Play( theAudioEvent.AudioClip, theAudioEvent.Loop, theAudioEvent.VolumeScale, theAudioEvent.Single, theAudioEvent.Delay );
			if ( aPlayer )
				aPlayer.Data = theAudioEvent;
			return aPlayer;
		}

		public bool Stop( AudioEvent theAudioEvent ) {
			if ( !theAudioEvent || !theAudioEvent.AudioClip ) {
				return false;
			}
			return Stop( theAudioEvent.AudioClip );
		}

		public AudioPlayer FadeIn( AudioEvent theAudioEvent ) {
			if ( !theAudioEvent || !theAudioEvent.AudioClip ) {
				return null;
			}
			var aPlayer = FadeIn( theAudioEvent.AudioClip, theAudioEvent.FadeDuration, theAudioEvent.Loop, theAudioEvent.VolumeScale, theAudioEvent.Single, theAudioEvent.Delay );
			if ( aPlayer )
				aPlayer.Data = theAudioEvent;
			return aPlayer;
		}

		public bool FadeOut( AudioEvent theAudioEvent ) {
			if ( !theAudioEvent || !theAudioEvent.AudioClip ) {
				return false;
			}
			return FadeOut( theAudioEvent.AudioClip, theAudioEvent.FadeDuration );
		}

		public void FadeOutAll( AudioEvent theExcludeAudioEvent = null, float theFadeDuration = 2f ) {
			foreach ( var aPlayer in players ) {
				if ( object.ReferenceEquals( theExcludeAudioEvent, aPlayer.Data ) )
					continue;
				aPlayer.RemoveAllListeners();
				var anAudioEvent = aPlayer.Data as AudioEvent;
				aPlayer.FadeOut( anAudioEvent?.FadeDuration ?? theFadeDuration );
			}
		}
		
		public AudioPlayer Play( string theAudioId ) {
			if ( string.IsNullOrEmpty( theAudioId ) ) {
				return null;
			}
			return Play(FindAudioEvent( theAudioId ));
		}

		public bool Stop( string theAudioId ) {
			if ( string.IsNullOrEmpty( theAudioId ) ) {
				return false;
			}
			return Stop( FindAudioEvent( theAudioId ) );
		}

		public AudioPlayer FadeIn( string theAudioId ) {
			if ( string.IsNullOrEmpty( theAudioId ) ) {
				return null;
			}
			return FadeIn( FindAudioEvent( theAudioId ) );
		}

		public bool FadeOut( string theAudioId ) {
			if ( string.IsNullOrEmpty( theAudioId ) ) {
				return false;
			}
			return FadeOut( FindAudioEvent( theAudioId ) );
		}

		public void FadeOutAll( string theExcludeAudioId = null, float theFadeDuration = 2f ) {
			FadeOutAll( FindAudioEvent( theExcludeAudioId ), theFadeDuration );
		}

		public void OnPlayerActivate( AudioPlayer thePlayer ) {
			players.AddLast( thePlayer );
			IncrementSoundCounter( thePlayer.AudioClip );
			//Debug.LogFormat("Player Started '{0}[{1}]' Players Count={2}", thePlayer.name, thePlayer.transform.GetSiblingIndex(), players.Count );
		}

		public void OnPlayerDeactivate( AudioPlayer thePlayer ) {
			DecrementSoundCounter( thePlayer.AudioClip );
			players.Remove( thePlayer );
			Free( thePlayer );
			//Debug.LogFormat("Player Stopped '{0}[{1}]' Players Count={2}", thePlayer.name, thePlayer.transform.GetSiblingIndex(), players.Count);
		}

		public void OnPlayerDestroy( AudioPlayer thePlayer ) {
			//Debug.LogFormat("Player Destroyed '{0}[{1}]' Players Count={2}", thePlayer.name, thePlayer.transform.GetSiblingIndex(), mPlayers.Count);
			pool.Remove( thePlayer );
		}

		private void IncrementSoundCounter( AudioClip theAudioClip ) {
			if ( !theAudioClip ) {
				return;
			}
			if ( !soundStatistic.TryGetValue( theAudioClip, out var aCount ) ) {
				soundStatistic.Add( theAudioClip, aCount );
			}
			soundStatistic[theAudioClip] = ++aCount;
		}

		private void DecrementSoundCounter( AudioClip theAudioClip ) {
			if ( !theAudioClip ) {
				return;
			}
			if ( soundStatistic.TryGetValue( theAudioClip, out var aCount ) ) {
				if ( aCount > 0 ) {
					soundStatistic[theAudioClip] = --aCount;
				}
				if ( aCount == 0 ) {
					soundStatistic.Remove( theAudioClip );
				}
			}
		}

		public bool IsSoundPlayed( AudioClip theAudioClip ) {
			soundStatistic.TryGetValue( theAudioClip, out var aCount );
			return aCount > 0;
		}

		#endregion // Functions
	}
}
