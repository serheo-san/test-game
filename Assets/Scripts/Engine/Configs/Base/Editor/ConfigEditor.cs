using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Engine.Configs.Editor {
	public class ConfigEditor : UnityEditor.Editor {
		#region Fields

		protected ReorderableList reorderableList = default;

		protected string collectionPropertyName = "items";
		protected string idPropertyName = "id";
		protected string headerName = "Prefabs";
		protected string defaultItemName = "Index {0}";
		protected int propertyCount = 0;
		protected bool stretchIcon = false;

		#endregion // Fields

		#region Constants

		public const int TopPadding = 2;
		public const int BottomPadding = 2;
		public const int LeftPadding = 10;
		public const int RightPadding = 0;

		#endregion // Constants

		#region Virtual methods
		
		protected virtual void OnEnable() {
			UpdateReordableList();
		}

		protected virtual void OnDisable() {
			reorderableList = null;
		}

		public override void OnInspectorGUI() {
			EditorGUI.BeginChangeCheck();
			serializedObject.UpdateIfRequiredOrScript();
			reorderableList?.DoLayoutList();
			serializedObject.ApplyModifiedProperties();
			EditorGUI.EndChangeCheck();
		}

		protected virtual void DrawItemProperties( Rect rect, SerializedProperty itemProperty, SerializedProperty idProperty ) {
		}

		protected virtual Texture2D GetItemIconTexture( SerializedProperty itemProperty ) {
			return null;
		}

		#endregion // Virtual methods

		#region Methods
		
		private void UpdateReordableList() {
			if( reorderableList != null )
				return;
			var property = serializedObject.FindProperty( collectionPropertyName );
			if( property == null )
				return;
			reorderableList = new ReorderableList( serializedObject, property, false, true, true, true );
			reorderableList.drawHeaderCallback = DrawHeader;
			reorderableList.drawElementCallback = DrawElement;
			reorderableList.elementHeightCallback = GetElementHeight;
		}

		private void DrawHeader( Rect rect ) {
			GUI.Label( rect, headerName );
		}

		protected void DrawElement( Rect rect, int index, bool isActive, bool isFocused ) {
			var itemProperty = reorderableList.serializedProperty.GetArrayElementAtIndex( index );
			if( itemProperty == null ) {
				return;
			}

			var rowRect = rect;
			rowRect.y += TopPadding;
			rowRect.x += LeftPadding;
			rowRect.width -= LeftPadding + RightPadding;
			rowRect.height = EditorGUIUtility.singleLineHeight;

			var texture = GetItemIconTexture( itemProperty );
			var iconRect = rowRect;
			iconRect.height = itemProperty.isExpanded ? rect.height - TopPadding - BottomPadding : iconRect.height;
			iconRect.width = iconRect.height;
			iconRect.x = rowRect.xMax - iconRect.width;
			
			if( stretchIcon ) {
				var prevTexture = GUI.skin.box.normal.background;
				GUI.skin.box.normal.background = texture;
				GUI.Box( iconRect, GUIContent.none);
				GUI.skin.box.normal.background = prevTexture;
			}
			else {
				GUI.Box( iconRect, texture );	
			}

			rowRect.width -= iconRect.width + EditorGUIUtility.singleLineHeight;
			var idProperty = itemProperty.FindPropertyRelative( idPropertyName );
			string itemName = ( idProperty != null && !string.IsNullOrEmpty( idProperty.stringValue ) ) ? idProperty.stringValue : string.Format( defaultItemName, index );
			var foldRect = rowRect;
			itemProperty.isExpanded = EditorGUI.Foldout( foldRect, itemProperty.isExpanded, itemName );
			rowRect.y += EditorGUIUtility.singleLineHeight;

			if( itemProperty.isExpanded ) {
				DrawItemProperties( rowRect, itemProperty, idProperty );
			}
		}

		protected void DrawProperties( Rect rect, params SerializedProperty[] properties ) {
			foreach( var property in properties ) {
				if( property != null ) {
					EditorGUI.PropertyField( rect, property, new GUIContent( property.displayName ) );
					rect.y += EditorGUIUtility.singleLineHeight;
				}
			}
		}

		private float GetElementHeight( int index ) {
			SerializedProperty anItemProperty = reorderableList.serializedProperty.GetArrayElementAtIndex( index );
			if( anItemProperty == null || anItemProperty.isExpanded ) {
				return EditorGUIUtility.singleLineHeight * propertyCount + TopPadding + BottomPadding;
			}
			return EditorGUIUtility.singleLineHeight + TopPadding + BottomPadding;
		}
	}

	#endregion // Methods
}