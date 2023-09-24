using System;
using Engine.Common;
using UnityEngine;

namespace Engine.ScriptableObjects.Prefabs {
	[Serializable]
	public class PrefabData : IOwnerId<string>, IComparable<PrefabData> {
		#region Fields

		[SerializeField]
		private string id = string.Empty;
		[SerializeField]
		private GameObject prefab = null;

		#endregion // Fields

		#region Properties

		string IOwnerId<string>.Id {
			get => Id;
			set => id = value;
		}

		public string Id => id;

		public GameObject Prefab => prefab;

		#endregion // Properties

		#region Methods

		public int CompareTo( PrefabData theOther ) {
			if( theOther == null ) {
				return 1;
			}

			return id.CompareTo( theOther.id );
		}

		#endregion // Methods
	}
}