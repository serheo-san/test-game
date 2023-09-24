using System;
using Engine.Common;
using UnityEngine;

namespace Engine.Configs {
	[Serializable]
	public class ColorData : IOwnerId<string>, IComparable<ColorData> {
		#region Fields

		[SerializeField]
		private string id = string.Empty;

		[SerializeField]
		private Color color = Color.white;

		#endregion // Fields

		#region Properties

		string IOwnerId<string>.Id {
			get => Id;
			set => id = value;
		}

		public string Id => id;

		public Color Color => color;

		#endregion // Properties

		#region Methods

		public int CompareTo( ColorData theOther ) {
			if ( theOther == null ) {
				return 1;
			}

			return id.CompareTo( theOther.id );
		}

		#endregion // Methods
	}
}
