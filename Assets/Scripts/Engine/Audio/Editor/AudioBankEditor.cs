using System;
using System.Reflection;
using Engine.Audio.Settings;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Engine.Audio.Editor {
	//[CustomEditor( typeof(AudioBank) )]
	public class AudioBankEditor : UnityEditor.Editor {
		private ReorderableList reorderableList = null;
		
		private void OnEnable() {
			Init();
		}

		private void OnDisable() {
			reorderableList = null;
		}

		public override void OnInspectorGUI() {
			EditorGUI.BeginChangeCheck();
			serializedObject.UpdateIfRequiredOrScript();
			reorderableList.DoLayoutList();
			serializedObject.ApplyModifiedProperties();
			EditorGUI.EndChangeCheck();
			if ( GUILayout.Button( "Update" ) ) {
				var anAudioBank = serializedObject.targetObject as AudioBank;
				if ( anAudioBank )
					anAudioBank.UpdateDictionary();
			}
			if ( GUILayout.Button( "Sort" ) ) {
				var anAudioBank = serializedObject.targetObject as AudioBank;
				if ( anAudioBank ) {
					anAudioBank.Sort();
					EditorUtility.SetDirty( anAudioBank );
				}
			}
		}

		private void Init() {
			if ( reorderableList != null )
				return;
			SerializedProperty aProperty = serializedObject.FindProperty( "items" );
			if ( aProperty == null )
				return;
			reorderableList = new ReorderableList( serializedObject, aProperty, true, true, true, true );
			reorderableList.drawHeaderCallback = rect => DrawHeader( rect, aProperty );
			reorderableList.drawElementCallback = DrawElement;
			reorderableList.elementHeightCallback = GetFieldElementHeight;
		}

		private void DrawHeader( Rect theRect, SerializedProperty theProperty ) {
			GUI.Label( theRect, "Audio Items" );
		}

		private void DrawElement( Rect theRect, int theIndex, bool theIsActive, bool theIsFocused ) {
			SerializedProperty anItemProperty = reorderableList.serializedProperty.GetArrayElementAtIndex( theIndex );
			if ( anItemProperty == null ) {
				return;
			}

			theRect.y += 2;
			theRect.x += 10;
			theRect.width -= 10;
			theRect.height = EditorGUIUtility.singleLineHeight;
			var anAudioEvent = anItemProperty.objectReferenceValue as AudioEvent;
			SerializedProperty aProperty = anItemProperty.FindPropertyRelative( "id" );
			string aText = (anAudioEvent != null && !string.IsNullOrEmpty( anAudioEvent.Id )) ? anAudioEvent.Id : $"Index {theIndex}";
			anItemProperty.isExpanded = EditorGUI.Foldout( theRect, anItemProperty.isExpanded, aText );
			theRect.y += EditorGUIUtility.singleLineHeight;

			if ( anItemProperty.isExpanded ) {
				if ( anItemProperty != null ) {
					EditorGUI.PropertyField( theRect, anItemProperty, new GUIContent( "Audio Event" ) );
					theRect.y += EditorGUIUtility.singleLineHeight;
				}
				var aRect = theRect;
				aRect.width = theRect.width / 4;
				aRect.x = theRect.x + theRect.width - aRect.width;
				if ( GUI.Button( aRect, "Stop" ) ) {
					StopAllClips();
				}
				aRect.x -= aRect.width;
				if (anAudioEvent!=null && GUI.Button( aRect, "Play" ) ) {
					StopAllClips();
					PlayClip( anAudioEvent.AudioClip );
				}
				aRect.x -= aRect.width;
			}
		}
		
		private float GetFieldElementHeight( int theIndex ) {
			SerializedProperty anItemProperty = reorderableList.serializedProperty.GetArrayElementAtIndex( theIndex );
			if ( anItemProperty == null || anItemProperty.isExpanded ) {
				return EditorGUIUtility.singleLineHeight * 3 + 6;
			}
			return EditorGUIUtility.singleLineHeight + 6;
		}

		public static void PlayClip( AudioClip theAudioClip, float theStartTime = 0, bool theLoop = false ) {
			if ( theAudioClip == null )
				return;
			int aStartSample = (int)(theStartTime * theAudioClip.frequency);

			Assembly anAssembly = typeof(AudioImporter).Assembly;
			Type anAudioUtilType = anAssembly.GetType( "UnityEditor.AudioUtil" );

			Type[] aTypeParams = {typeof(AudioClip), typeof(int), typeof(bool)};
			object[] aValueParams = {theAudioClip, aStartSample, theLoop};

			MethodInfo aMethod = anAudioUtilType.GetMethod( "PlayClip", BindingFlags.Static | BindingFlags.Public,
				null, aTypeParams, null );
			aMethod.Invoke( null, aValueParams );
		}

		public static void StopAllClips() {
			Assembly anAssembly = typeof(AudioImporter).Assembly;
			Type anAudioUtilType = anAssembly.GetType( "UnityEditor.AudioUtil" );

			Type[] aTypeParams = {};
			object[] aValueParams = {};
			
			var aMethod = anAudioUtilType.GetMethod("StopAllClips", BindingFlags.Static | BindingFlags.Public,
				null, aTypeParams, null );
			aMethod.Invoke( null, aValueParams );
		}
	}
}
