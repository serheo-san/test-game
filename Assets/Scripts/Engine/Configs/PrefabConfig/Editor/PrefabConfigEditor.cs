using Engine.ScriptableObjects.Prefabs;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Engine.Configs.Editor {
	[CustomEditor( typeof( PrefabContainer ) )]
	public class PrefabConfigEditor : ConfigEditor {
		#region Constants

		private const int PropertyCount = 3;

		private const string PrefabPropertyName = "prefab";
		private const string HeaderName = "Prefabs";
		private const string IconObjectName = "Icon";
		private const string SortButtonName = "Sort";

		#endregion // Constants

		PrefabConfigEditor() {
			propertyCount = PropertyCount;
			headerName = HeaderName;
		}
		
		#region Virtual methods

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();
			if( GUILayout.Button( SortButtonName ) ) {
				var prefabs = serializedObject.targetObject as PrefabContainer;
				if( prefabs ) {
					prefabs.Sort();
					EditorUtility.SetDirty( prefabs );
				}
			}
		}

		protected override Texture2D GetItemIconTexture( SerializedProperty itemProperty ) {
			var prefabProperty = itemProperty.FindPropertyRelative( PrefabPropertyName );
			if( prefabProperty == null ) {
				return null;
			}
			var gameObject = prefabProperty.objectReferenceValue as GameObject;
			if( !gameObject ) {
				return null;
			}
			Texture2D texture = null;
			var transform = gameObject.transform.Find( IconObjectName );
			if( transform != null ) {
				if( !texture ) {
					var image = transform.GetComponentInChildren<Image>();
					var sprite = image ? image.sprite : null;
					texture = sprite ? sprite.texture : null;
				}
				if( !texture ) {
					var spriteRenderer = transform.GetComponentInChildren<SpriteRenderer>();
					var sprite = spriteRenderer ? spriteRenderer.sprite : null;
					texture = sprite ? sprite.texture : null;
				}
			}
			if( !texture ) {
				var image = gameObject ? gameObject.GetComponentInChildren<Image>() : null;
				var sprite = image ? image.sprite : null;
				texture = sprite ? sprite.texture : null;
			}
			if( !texture ) {
				var spriteRenderer = gameObject ? gameObject.GetComponentInChildren<SpriteRenderer>() : null;
				var sprite = spriteRenderer ? spriteRenderer.sprite : null;
				texture = sprite ? sprite.texture : null;
			}
			return texture;
		}

		protected override void DrawItemProperties( Rect rect, SerializedProperty itemProperty, SerializedProperty idProperty ) {
			var prefabProperty = itemProperty.FindPropertyRelative( PrefabPropertyName );
			DrawProperties(rect, idProperty, prefabProperty);
		}

		#endregion // Virtual methods
		
	}

}