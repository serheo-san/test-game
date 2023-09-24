using System;
using System.Collections;
using System.Collections.Generic;
using Engine.Audio.Settings;
using Engine.Components;
using UnityEngine;

namespace Engine.Audio {
	public class AudioBehaviour : ResettableBehaviour {
		#region Types

		public enum CommandType {
			None = 0,
			Play,
			Stop,
			FadeIn,
			FadeOut,
			FadeOutAll,
		}

		[Serializable]
		public class Command {
			public CommandType commandType = CommandType.None;
			public string audioId = string.Empty;
			public AudioEvent audioEvent = default;
		}

		[Serializable]
		public class CommandList : IReadOnlyList<Command> {
			public List<Command> items = null;
			IEnumerator<Command> IEnumerable<Command>.GetEnumerator() {
				return items.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator() {
				return ((IEnumerable)items).GetEnumerator();
			}

			public int Count => items.Count;

			public Command this[ int theIndex ] {
				get {
					if ( theIndex < 0 || theIndex >= items.Count ) {
						return null;
					}
					return items[theIndex];
				}
			}
		}

		#endregion // Types

		#region Fields

		[SerializeField]
		private string audioManagerType = string.Empty;
		[SerializeField]
		private CommandList awakeCommands = default;
		[SerializeField]
		private CommandList destroyCommands = default;
		[SerializeField]
		private CommandList enableCommands = default;
		[SerializeField]
		private CommandList disableCommands = default;
		[SerializeField]
		private CommandList audioCommands = default;


		private AudioManager audioManager = default;

		#endregion // Fields

		#region MonoBehaviour Event Handlers

		protected override void OnExpose()
		{
			base.OnExpose();
			IoC.TryResolve(out audioManager, audioManagerType);
			if ( awakeCommands != null ) {
				foreach ( var aCommand in awakeCommands ) {
					DoCommand( aCommand );	
				}
			}
		}

		protected override void OnDispose() {
			if ( destroyCommands != null ) {
				foreach ( var aCommand in destroyCommands ) {
					DoCommand( aCommand );	
				}
			}
			audioManager = null;
		}

		protected void OnEnable() {
			if ( enableCommands != null ) {
				foreach ( var aCommand in enableCommands ) {
					DoCommand( aCommand );
				}
			}
		}

		protected void OnDisable() {
			if ( disableCommands != null ) {
				foreach ( var aCommand in disableCommands ) {
					DoCommand( aCommand );
				}
			}
		}

		private bool DoCommand( Command theCommand ) {
			if ( theCommand == null ) {
				return false;
			}
			switch ( theCommand.commandType ) {
				case CommandType.Play:
					Play( theCommand );
					return true;
				case CommandType.Stop:
					Stop( theCommand );
					return true;
				case CommandType.FadeIn:
					FadeIn( theCommand );
					return true;
				case CommandType.FadeOut:
					FadeOut( theCommand );
					return true;
				case CommandType.FadeOutAll:
					FadeOutAll( theCommand );
					return true;
			}
			return false;
		}

		#endregion // MonoBehaviour Event Handlers

		#region Methods
		
		public void Play( Command theCommand) {
			if ( theCommand.audioEvent ) {
				audioManager?.Play( theCommand.audioEvent );
			}
			else {
				audioManager?.Play( theCommand.audioId );	
			}
		}


		public void Stop( Command theCommand ) {
			if ( theCommand.audioEvent ) {
				audioManager?.Stop( theCommand.audioEvent );
			}
			else {
				audioManager?.Stop( theCommand.audioId );	
			}
		}

		public void FadeIn( Command theCommand ) {
			if ( theCommand.audioEvent ) {
				audioManager?.FadeIn( theCommand.audioEvent );
			}
			else {
				audioManager?.FadeIn( theCommand.audioId );	
			}
		}
		
		public void FadeOut( Command theCommand ) {
			if ( theCommand.audioEvent ) {
				audioManager?.FadeOut( theCommand.audioEvent );
			}
			else {
				audioManager?.FadeOut( theCommand.audioId );	
			}
		}
		
		public void FadeOutAll( Command theCommand ) {
			if ( theCommand.audioEvent ) {
				audioManager?.FadeOutAll( theCommand.audioEvent );
			}
			else {
				audioManager?.FadeOutAll( theCommand.audioId );	
			}
		}

		public void Play( string theAudioId) {
			audioManager?.Play( theAudioId );
		}


		public void Stop( string theAudioId ) {
			audioManager?.Stop( theAudioId );
		}

		public void FadeIn( string theAudioId) {
			audioManager?.FadeIn( theAudioId);
		}
		
		public void FadeOut( string theAudioId) {
			audioManager?.FadeOut( theAudioId);
		}
		
		public void FadeOutAll( string theExcludeAudioId=null ) {
			audioManager?.FadeOutAll( theExcludeAudioId );
		}

		public void DoAudioCommand( int theIndex ) {
			if ( audioCommands == null || theIndex < 0 || theIndex >= audioCommands.Count ) {
				return;
			}
			var aCommand = audioCommands[theIndex];
			if ( aCommand == null ) {
				return;
			}
			DoCommand( aCommand );
		}

		public void DoFirstAudioCommand() {
			DoAudioCommand(0);
		}
		
		public void DoSecondAudioCommand() {
			DoAudioCommand(1);
		}
		
		public void DoThirdAudioCommand() {
			DoAudioCommand(2);
		}
		
		#endregion // Methods
	}
}
