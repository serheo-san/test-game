using UnityEditor;
using UnityEngine;

namespace Engine.Configs.Editor {
	[CustomEditor( typeof( ColorConfig) )]
	public class ColorConfigEditor : ConfigEditor {
		#region Fields

		private Texture2D iconTexture = null;

		#endregion // Fields

		#region Constants

		private const int PropertyCount = 3;

		private const string ColorPropertyName = "color";
		private const string HeaderName = "Colors";
		private const string SortButtonName = "Sort";

		#endregion // Constants
		
		ColorConfigEditor() {
			propertyCount = PropertyCount;
			headerName = HeaderName;
			stretchIcon = true;
		}


		#region Virtual methods

		protected override void OnEnable() {
			base.OnEnable();
			iconTexture = new Texture2D( 1, 1 );
		}

		protected override void OnDisable() {
			base.OnDisable();
			DestroyImmediate( iconTexture );
		}

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
			SerializedProperty colorProperty = itemProperty.FindPropertyRelative( ColorPropertyName );
			Color color = Color.black;
			if( colorProperty != null ) {
				color = colorProperty.colorValue;
			}
			if( iconTexture ) {
				iconTexture.SetPixel( 0, 0, color );
				iconTexture.Apply();
			}
			return iconTexture;
		}

		protected override void DrawItemProperties( Rect rect, SerializedProperty itemProperty, SerializedProperty idProperty ) {
			var colorProperty = itemProperty.FindPropertyRelative( ColorPropertyName );
			DrawProperties( rect, idProperty, colorProperty );
		}

		#endregion // Virtual methods
	}
}