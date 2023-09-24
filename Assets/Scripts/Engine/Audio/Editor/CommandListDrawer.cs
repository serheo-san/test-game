using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Engine.Audio.Editor {
	[CustomPropertyDrawer( typeof(AudioBehaviour.CommandList), true )]
	public class CommandListDrawer : PropertyDrawer {
		private ReorderableList reorderableList = null;

		public override void OnGUI( Rect position, SerializedProperty property, GUIContent label ) {
			Init( property );
			if ( reorderableList != null )
				reorderableList.DoList( position );
		}

		public override float GetPropertyHeight( SerializedProperty property, GUIContent label ) {
			Init( property );
			if ( reorderableList == null )
				return 0f;
			return reorderableList.GetHeight();
		}

		private void Init( SerializedProperty theProperty ) {
			if ( reorderableList != null )
				return;
			SerializedProperty anArray = theProperty.FindPropertyRelative( "items" );
			if ( anArray == null )
				return;
			reorderableList = new ReorderableList( theProperty.serializedObject, anArray );
			reorderableList.drawElementCallback = DrawElement;
			reorderableList.drawHeaderCallback = ( theRect ) => DrawHeader( theRect, theProperty.name );
			reorderableList.elementHeightCallback = GetElementHeight;
			//reorderableList.elementHeight = EditorGUIUtility.singleLineHeight * 2 + 4;
		}

		private void DrawHeader( Rect theRect, string theName ) {
			GUI.Label( theRect, theName );
		}

		private void DrawElement( Rect theRect, int theIndex, bool theIsActive, bool theIsFocused ) {
			SerializedProperty anItemProperty = reorderableList.serializedProperty.GetArrayElementAtIndex( theIndex );
			if ( anItemProperty == null ) {
				return;
			}

			theRect.y += 2;
			theRect.x += 10;
			theRect.width -= 10;
			SerializedProperty aProperty = anItemProperty.FindPropertyRelative( "commandType" );
			var aCommandType = aProperty != null ? (AudioBehaviour.CommandType)aProperty.intValue : AudioBehaviour.CommandType.None;

			theRect.height = EditorGUIUtility.singleLineHeight;
			anItemProperty.isExpanded = EditorGUI.Foldout( theRect, anItemProperty.isExpanded, $"Command {theIndex}" );
			theRect.y += EditorGUIUtility.singleLineHeight;

			if ( anItemProperty.isExpanded ) {
				if ( aProperty != null ) {
					EditorGUI.PropertyField( theRect, aProperty, new GUIContent( "Command Type" ) );
					theRect.y += EditorGUIUtility.singleLineHeight;
				}

				aProperty = anItemProperty.FindPropertyRelative( "audioId" );
				if ( aProperty != null && aCommandType != AudioBehaviour.CommandType.None ) {
					EditorGUI.PropertyField( theRect, aProperty, new GUIContent( "Audio Id" ) );
					theRect.y += EditorGUIUtility.singleLineHeight;
				}
				
				aProperty = anItemProperty.FindPropertyRelative( "audioEvent" );
				if ( aProperty != null && aCommandType != AudioBehaviour.CommandType.None ) {
					EditorGUI.PropertyField( theRect, aProperty, new GUIContent( "Audio Event" ) );
					theRect.y += EditorGUIUtility.singleLineHeight;
				}
			}
		}

		private float GetElementHeight( int theIndex ) {
			SerializedProperty anItemProperty = reorderableList.serializedProperty.GetArrayElementAtIndex( theIndex );
			if ( anItemProperty != null && anItemProperty.isExpanded ) {
				SerializedProperty aProperty = anItemProperty?.FindPropertyRelative( "commandType" );
				if ( aProperty != null ) {
					switch ( (AudioBehaviour.CommandType)aProperty.intValue ) {
						case AudioBehaviour.CommandType.None:
							return EditorGUIUtility.singleLineHeight * 2 + 4;
						case AudioBehaviour.CommandType.Play:
						case AudioBehaviour.CommandType.FadeIn:
						case AudioBehaviour.CommandType.Stop:
						case AudioBehaviour.CommandType.FadeOut:
						case AudioBehaviour.CommandType.FadeOutAll:
							return EditorGUIUtility.singleLineHeight * 4 + 4;
					}
				}
			}
			return EditorGUIUtility.singleLineHeight + 4;
		}
	}
}
