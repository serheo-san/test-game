using System;
using System.Linq;
using Engine.Attributes;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SortingLayerAttribute))]
public class SortingLayerDrawer : PropertyDrawer
{
	private static string[] layers = SortingLayer.layers.Select( layer => layer.name ).ToArray();
	
	public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
	{
		SortingLayerAttribute anAttribute = attribute as SortingLayerAttribute;
		if (anAttribute == null || property.propertyType != SerializedPropertyType.Integer ) {
			EditorGUI.PropertyField( rect, property );
			return;
		}
		EditorGUI.BeginProperty( rect, label, property );
		var name = SortingLayer.IDToName( property.intValue );
		var prevIndex = Array.IndexOf( layers, name );
		var index = EditorGUI.Popup( rect, label.text, prevIndex, layers );
		if ( index != prevIndex ) {
			name = layers[index];
			property.intValue = SortingLayer.NameToID( name );
		}
		EditorGUI.EndProperty();
	}
	
}
