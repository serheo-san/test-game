using System;
using Engine.Common;
using UnityEngine;

namespace Engine.Configs {
	[Serializable]
	public class SpriteData : IOwnerId<string>, IComparable<SpriteData> {
		#region Fields

		[SerializeField]
		private string id = string.Empty;
		[SerializeField] 
		private Sprite sprite = null;

		#endregion // Fields

		#region Properties

		string IOwnerId<string>.Id {
			get => Id;
			set => id = value;
		}

		public string Id => id;

		public Sprite Sprite => sprite;

		#endregion // Properties

		#region Methods

		public int CompareTo( SpriteData theOther ) {
			if ( theOther == null ) {
				return 1;
			}

			return id.CompareTo( theOther.id );
		}

		#endregion // Methods
	}
}
