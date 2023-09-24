using System;
using System.Collections;
using System.Collections.Generic;
using Engine.Common;
using UnityEngine;

namespace Engine.Configs {

	public class Config<TItemId, TItem> : ScriptableObject, IReadOnlyList<TItem>, ISerializationCallbackReceiver
		where TItem : class, IComparable<TItem>, IOwnerId<TItemId>, new() {

		#region Fields

		[SerializeField]
		protected List<TItem> items = new List<TItem>();

		protected readonly Dictionary<TItemId, TItem> keyItems = new Dictionary<TItemId, TItem>();

		#endregion // Fields
		
		#region Properties

		TItem IReadOnlyList<TItem>.this[ int theIndex ] => items[theIndex];

		public int Count => items.Count;

		#endregion // Properties

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		
		void ISerializationCallbackReceiver.OnBeforeSerialize() {
			OnBeforeSerialize();
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize() {
			OnAfterDeserialize();
		}

		#region  Virtual Methods
		protected virtual void OnBeforeSerialize() {
		}

		protected virtual void OnAfterDeserialize() {
			UpdateDictionary();
		}
		
		#endregion // Virtual Methods

		#region  Methods
		protected void OnEnable() {
			UpdateDictionary();
		}

		public IEnumerator<TItem> GetEnumerator() {
			return items.GetEnumerator();
		}

		public bool Contains( TItem theItem ) {
			return items.Contains( theItem );
		}

		public int IndexOf( TItem theItem ) {
			return items.IndexOf( theItem );
		}

		public TItem FindByIndex( int theIndex ) {
			if ( theIndex < 0 || theIndex >= Count )
				return default(TItem);
			return items[theIndex];
		}

		public TItem FindById( TItemId theItemId ) {
			UpdateDictionary();
			keyItems.TryGetValue( theItemId, out var item );
			return item;
		}
		
		public bool TryGetItem( TItemId theItemId, out TItem theItem ) {
			UpdateDictionary();
			return keyItems.TryGetValue( theItemId, out theItem );
		}

		public bool ContainsId( TItemId theItemId ) {
			UpdateDictionary();
			return keyItems.ContainsKey(theItemId);
		}

		public void UpdateDictionary() {
			if ( keyItems.Count == items.Count ) {
				return;
			}
			keyItems.Clear();
			foreach ( var item in items ) {
				if ( item == null ) {
					continue;
				}
				if ( keyItems.ContainsKey( item.Id ) ) {
					continue;
				}
				keyItems.Add( item.Id, item );
			}
		}
		
		public void Sort() {
			items.Sort();
		}

		#endregion // Methods
	}
}
