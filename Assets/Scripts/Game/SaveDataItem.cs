using System;
using Engine.Common;
using Engine.Storage;

namespace Game.Statistics
{
	public class SaveDataItem : DataBlock, IOwnerId<string>, IComparable<SaveDataItem>, IChild
	{
		#region Fields
		private string id = InvalidId;
		private int value = 0;

		private SaveDataManager parent = null;

		#endregion // Fields

		public const string InvalidId = "";

		#region Properties
		string IOwnerId<string>.Id
		{
			get
			{
				return id;
			}
			set
			{
				id = value;
			}
		}

		object IChild.Parent
		{
			get
			{
				return parent;
			}
			set
			{
				parent = value as SaveDataManager;
			}
		}

		public string Id
		{
			get
			{
				return id;
			}
		}

		public SaveDataManager Parent
		{
			get
			{
				return parent;
			}
		}

		public int Value
		{
			get
			{
				return value;
			}
			set
			{
				this.value = value;
			}
		}

		#endregion // Properties

		public override void ReadData(BlockReader theReader)
		{
			base.ReadData(theReader);
			theReader.TryRead(out id);
			theReader.TryRead(out value);
		}

		public override void WriteData(BlockWriter theWriter)
		{
			base.WriteData(theWriter);
			theWriter.Write(id);
			theWriter.Write(value);
		}

		public int CompareTo(SaveDataItem theOther)
		{
			if (theOther == null)
			{
				return -1;
			}
			return id.CompareTo(theOther.id);
		}

	}
}