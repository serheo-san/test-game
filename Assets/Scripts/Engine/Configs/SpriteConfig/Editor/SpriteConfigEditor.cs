using UnityEditor;
using UnityEngine;

namespace Engine.Configs.Editor {
	[CustomEditor( typeof( SpriteConfig ) )]
	public class SpriteConfigEditor : ConfigEditor {
		#region Constants

		private const string PrefabSpriteName = "sprite";
		private const string HeaderName = "Sprites";
		private const string SortButtonName = "Sort";

		private const int PropertyCount = 3;

		#endregion // Constants

		SpriteConfigEditor() {
			propertyCount = PropertyCount;
			headerName = HeaderName;
		}

		#region Virtual methods

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();
			if( GUILayout.Button( SortButtonName ) ) {
				var sprites = serializedObject.targetObject as SpriteConfig;
				if( sprites ) {
					sprites.Sort();
					EditorUtility.SetDirty( sprites );
				}
			}
		}

		protected override Texture2D GetItemIconTexture( SerializedProperty itemProperty ) {
			SerializedProperty spriteProperty = itemProperty.FindPropertyRelative( PrefabSpriteName );
			Texture2D texture = null;
			if( spriteProperty != null ) {
				var sprite = spriteProperty.objectReferenceValue as Sprite;
				texture = sprite ? sprite.texture : null;
			}
			return texture;
		}

		protected override void DrawItemProperties( Rect rect, SerializedProperty itemProperty, SerializedProperty idProperty ) {
			var spriteProperty = itemProperty.FindPropertyRelative( PrefabSpriteName );
			DrawProperties( rect, idProperty, spriteProperty );
		}

		#endregion // Virtual methods
	}
}