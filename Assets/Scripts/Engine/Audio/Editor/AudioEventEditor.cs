using System;
using System.Reflection;
using Engine.Audio.Settings;
using UnityEditor;
using UnityEngine;

namespace Engine.Audio.Editor {
	[CustomEditor( typeof(AudioEvent) ), CanEditMultipleObjects]
	public class AudioEventEditor : UnityEditor.Editor {
		
		public override void OnInspectorGUI() {
			base.OnInspectorGUI();
			var anAudioEvent = serializedObject.targetObject as AudioEvent;
			GUILayout.BeginHorizontal();
			if ( anAudioEvent!=null && anAudioEvent.AudioClip && GUILayout.Button( "Play" ) ) {
				StopAllClips();
				PlayClip( anAudioEvent.AudioClip );
			}
			if ( GUILayout.Button( "Stop" ) ) {
				StopAllClips();
			}
			GUILayout.EndHorizontal();
		}

		public static void PlayClip( AudioClip theAudioClip, float theStartTime = 0, bool theLoop = false ) {
			if ( theAudioClip == null )
				return;
			int aStartSample = (int)(theStartTime * theAudioClip.frequency);

			Assembly anAssembly = typeof(AudioImporter).Assembly;
			Type anAudioUtilType = anAssembly.GetType( "UnityEditor.AudioUtil" );

			Type[] aTypeParams = {typeof(AudioClip), typeof(int), typeof(bool)};
			object[] aValueParams = {theAudioClip, aStartSample, theLoop};

			MethodInfo aMethod = anAudioUtilType.GetMethod( "PlayPreviewClip", BindingFlags.Static | BindingFlags.Public,
				null, aTypeParams, null );
			aMethod?.Invoke( null, aValueParams );
		}

		public static void StopAllClips() {
			Assembly anAssembly = typeof(AudioImporter).Assembly;
			Type anAudioUtilType = anAssembly.GetType( "UnityEditor.AudioUtil" );

			Type[] aTypeParams = {};
			object[] aValueParams = {};
			
			var aMethod = anAudioUtilType.GetMethod("StopAllPreviewClips", BindingFlags.Static | BindingFlags.Public,
				null, aTypeParams, null );
			aMethod?.Invoke( null, aValueParams );
		}
	}
}
