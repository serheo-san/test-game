using System;
using Engine.Components;
using UnityEngine;
using UnityEngine.Audio;

namespace Engine.Audio {
	[RequireComponent( typeof(AudioSource) )]
	public class AudioPlayer : ResettableBehaviour {
		#region Types

		protected enum Mode {
			Normal = 0,
			FadeIn,
			FadeOut,
		}

		public delegate void EventHandler( AudioPlayer thePlayer );

		#endregion //Types

		#region Members

		protected AudioManager manager = null;
		protected new AudioSource audio = null;
		protected float volume = 1f;
		protected float fadeVolumeScale = 1f;
		protected bool isActive = false;
		protected bool isPaused = false;
		protected bool isAppPause = false;
		protected bool isDelaying = false;
		protected Mode mode = Mode.Normal;
		protected object data = null;
		protected float fadeTime = 0f;
		protected float fadeDuration = 0f;
		protected float delayTime = 0f;
		protected float delayDuration = 0f;
		protected EventHandler activateEvent = null;
		protected EventHandler deactivateEvent = null;

		#endregion // Members

		#region Properties

		public float Volume {
			get => volume;
			set {
				var aVolume = Math.Max( Math.Min( 1f, value ), 0f );
				if ( !volume.Equals( aVolume ) ) {
					volume = aVolume;
					UpdateVolume();
				}
			}
		}

		public float Duration {
			get {
				var anAudioClip = AudioClip;
				return anAudioClip != null ? anAudioClip.length : 0f;
			}
		}

		public float FadeDuration => fadeDuration;

		public event EventHandler ActivateEvent {
			add {
				activateEvent -= value;
				activateEvent += value;
			}
			remove {
				activateEvent -= value;
			}
		}

		public event EventHandler DeactivateEvent {
			add {
				deactivateEvent -= value;
				deactivateEvent += value;
			}
			remove {
				deactivateEvent -= value;
			}
		}

		public AudioClip AudioClip => audio != null ? audio.clip : null;

		public bool IsActive => IsPlaying || IsDelaying || IsPaused;

		public bool IsPlaying => audio != null && audio.isPlaying;

		public bool IsDelaying => isDelaying;

		public bool IsPaused => isPaused;

		public AudioManager Manager => manager;

		public object Data {
			get => data;
			set => data = value;
		}

		#endregion // Properties

		#region ResettableComponent Funcions

		protected override void OnExpose() {
			base.OnExpose();
			volume = 1f;
			fadeVolumeScale = 1f;
			isActive = false;
			isPaused = false;
			isAppPause = false;
			isDelaying = false;
			mode = Mode.Normal;
			data = null;
			fadeTime = 0f;
			fadeDuration = 0f;
			delayTime = 0f;
			delayDuration = 0f;
			audio = GetComponent<AudioSource>();
			if ( audio != null ) {
				audio.playOnAwake = false;
			}
			transform.position = Vector3.zero;
		}

		protected override void OnDispose() {
			if ( audio != null ) {
				audio.clip = null;
				audio = null;
			}
			activateEvent = null;
			deactivateEvent = null;
			base.OnDispose();
		}

		#endregion

		#region MonoBehaviour Event Handlers

		protected override void OnDestroy() {
			if ( IsActive ) {
				deactivateEvent?.Invoke( this );
				if ( manager ) {
					manager.OnPlayerDeactivate(this);
				}
			}
			if ( manager ) {
				manager.OnPlayerDestroy(this);
			}
			base.OnDestroy();
		}

		private void Update() {
			if ( audio ) {
				if (  isActive && !IsActive ) {
					OnDeactivate();
				}
				else if ( IsActive && !isActive ) {
					OnActivate();
				}
				if ( isActive && !isPaused && !isDelaying ) {
					switch ( mode ) {
						case Mode.FadeIn:
							fadeTime += Time.deltaTime;
							fadeVolumeScale = fadeTime / fadeDuration;
							if ( fadeVolumeScale >= 1f ) {
								fadeVolumeScale = 1f;
								mode = Mode.Normal;
							}
							UpdateVolume();
							break;
						case Mode.FadeOut:
							fadeTime += Time.deltaTime;
							fadeVolumeScale = 1f - fadeTime / fadeDuration;
							if ( fadeVolumeScale <= 0f ) {
								fadeVolumeScale = 0f;
								mode = Mode.Normal;
								Stop();
							}
							//Debug.LogFormat("Fade Out '{0}[{1}]' isPlaying={2} Fade={3}", name, transform.GetSiblingIndex(), mAudio != null && mAudio.isPlaying, mFadeVolumeScale);
							UpdateVolume();
							break;
					}
				}
				if ( isDelaying && delayTime < delayDuration && !isPaused && !isAppPause ) {
					delayTime += Time.deltaTime;
					if ( delayTime >= delayDuration ) {
						isDelaying = false;
						if ( !IsPlaying ) {
							audio.Play();							
						}
					}
				}
			}
		}

		private void OnApplicationPause( bool thePause ) {
			isAppPause = thePause;
		}

		#endregion // MonoBehaviour Event Handlers

		#region Functions

		public void Initialize( AudioManager theManager ) {
			manager = theManager;
		}

		public bool Set( AudioClip theClip, AudioMixerGroup theMixerGroup ) {
			if ( !audio || audio.isPlaying ) {
				return false;
			}
			audio.clip = theClip;
			audio.outputAudioMixerGroup = theMixerGroup;
			return true;
		}
		
		public void RemoveAllListeners() {
			activateEvent = null;
			deactivateEvent = null;
		}

		private bool Activate(bool theDelayed=false) {
			if ( IsActive ) {
				return false;
			}
			isDelaying = theDelayed;
			if ( !isDelaying && !IsPlaying) {
				audio.Play();
			}
			if ( manager && manager.IsPause ) {
				Pause();
			}
			OnActivate();
			return IsActive;
		}

		private bool Deactivate() {
			if ( !IsActive ) {
				return false;
			}
			if ( isDelaying ) {
				isDelaying = false;
			}
			if ( !isPaused) {
				audio.UnPause();
			}
			if ( IsPlaying ) {
				audio.Stop();
			}		
			OnDeactivate();
			return !IsActive;
		}

		public bool Play( bool theLoop = false, float theVolume = 1f, float theDelay = 0f ) {
			//Debug.LogFormat("Play Player '{0}[{1}]'", name, transform.GetSiblingIndex());
			if ( !audio || IsActive ) {
				return false;
			}
			//Debug.Log("AudioPlayer.Play " + name);
			mode = Mode.Normal;
			volume = theVolume;
			fadeVolumeScale = 1f;
			delayDuration = theDelay;
			UpdateVolume();
			audio.loop = theLoop;
			return Activate(!delayDuration.Equals( 0f ));
		}
		
		public bool Stop() {
			//Debug.LogFormat( "Stop Player '{0}[{1}]' isPlaying={2}", name, transform.GetSiblingIndex(), audio != null && audio.isPlaying );
			return Deactivate();
		}

		public bool FadeIn( float theFadeDuration = 2f, bool theLoop = false, float theVolume = 1f, float theDelay = 0f ) {
			//Debug.LogFormat("FadeIn Player '{0}[{1}]'", name, transform.GetSiblingIndex());
			if ( !audio ) {
				return false;
			}
			if ( IsActive && mode != Mode.FadeOut ) {
				return false;
			}
			if ( IsActive && mode == Mode.FadeOut ) {
				fadeDuration = theFadeDuration.Equals( 0f ) ? 1f : theFadeDuration;
				fadeTime = fadeDuration * fadeVolumeScale;
			}
			else {
				fadeDuration = theFadeDuration.Equals( 0f ) ? 1f : theFadeDuration;
				fadeTime = 0f;
				fadeVolumeScale = 0f;
			}
			volume = theVolume;
			mode = Mode.FadeIn;
			UpdateVolume();
			audio.loop = theLoop;
			delayDuration = theDelay;
			Activate( !delayDuration.Equals( 0f ) );
			return true;
		}

		public bool FadeOut( float theFadeDuration = 2f ) {
			//Debug.LogFormat( "FadeOut Player '{0}[{1}]' isPlaying={2}", name, transform.GetSiblingIndex(), audio != null && audio.isPlaying );
			if ( !audio ) {
				return false;
			}
			if ( IsPlaying && mode == Mode.FadeOut ) {
				fadeDuration = theFadeDuration.Equals( 0f ) ? 1f : theFadeDuration;
				fadeTime = fadeDuration * (1f - fadeVolumeScale);
			}
			else {
				fadeTime = 0f;
				fadeDuration = theFadeDuration.Equals( 0f ) ? 1f : theFadeDuration;
				fadeVolumeScale = 1f;
			}
			mode = Mode.FadeOut;
			UpdateVolume();
			return true;
		}

		public bool Pause() {
			if ( !audio || isPaused || !IsActive ) {
				return false;
			}
			if ( audio.isPlaying ) {
				//Debug.Log("AudioPlayer.Pause " + name);
				audio.Pause();
			}
			isPaused = true;
			return true;
		}

		public bool Continue() {
			if ( !audio || !isPaused || !IsActive) {
				return false;
			}
			if ( !audio.isPlaying ) {
				audio.UnPause();
			}
			isPaused = false;
			return true;
		}

		private void UpdateVolume() {
			if ( audio ) {
				audio.volume = volume * fadeVolumeScale;
			}
		}

		private void OnActivate() {
			isActive = IsActive;
			if(isActive) {
				activateEvent?.Invoke( this );
				if ( manager ) {
					manager.OnPlayerActivate(this);
				}
			}
		}

		private void OnDeactivate() {
			isActive = IsActive;
			if(!isActive) {
				deactivateEvent?.Invoke( this );
				if ( manager ) {
					manager.OnPlayerDeactivate(this);
				}
			}
		}

		public AudioPlayer OnStart( Action theAction ) {
			DeactivateEvent += OnStopEvent;

			void OnStopEvent( AudioPlayer thePlayer ) {
				thePlayer.DeactivateEvent -= OnStopEvent;
				theAction();
			}

			return this;
		}

		public AudioPlayer OnStopPlay( string theAudioId ) {
			DeactivateEvent += OnStopEvent;

			void OnStopEvent( AudioPlayer thePlayer ) {
				thePlayer.DeactivateEvent -= OnStopEvent;
				thePlayer.Manager.Play( theAudioId );
			}

			return this;
		}

		public AudioPlayer OnStop( Action theAction ) {
			DeactivateEvent += OnStopEvent;

			void OnStopEvent( AudioPlayer thePlayer ) {
				thePlayer.DeactivateEvent -= OnStopEvent;
				theAction();
			}

			return this;
		}

		#endregion // Functions
	}
}
