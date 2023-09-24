using System.IO;
using Engine.Common;
using Engine.IO;
using Engine.Storage.Collections;
using UnityEngine;

namespace Game.Statistics
{
	public class SaveDataManager : SortedList<string, SaveDataItem>
	{
		public bool IsMiniGameWasRun = false;

		public const string FileName = "SaveData.bytes";


		#region Virtual Functions

		public override string GetUniqueItemId()
		{
			return SaveDataManager.GetUniqueItemId(this, "item{0:00}");
		}

		public override string GetInvalidItemId()
		{
			return SaveDataItem.InvalidId;
		}

		#endregion // Virtual Functions

		#region Functions
		public bool Load()
		{
			Clear();
			var aPath = PathEx.Combine(Application.persistentDataPath, FileName);
			var aStream = FileEx.OpenRead(aPath);
			if (aStream == null)
			{
				return false;
			}
			var aReader = new StreamReader(aStream);
			Load(aReader);
			aReader.Dispose();
			return true;
		}

		public bool Save()
		{
			var aPath = PathEx.Combine(Application.persistentDataPath, FileName);
			var aStream = FileEx.OpenWrite(aPath);
			if (aStream == null)
			{
				return false;
			}
			var aWriter = new StreamWriter(aStream);
			Save(aWriter);
			aWriter.Dispose();
			return true;
		}

		public int GetValue(string theId)
		{
			var anItem = Get(theId);
			return anItem != null ? anItem.Value : 0;
		}

		public bool SetValue(string theId, int theValue)
		{
			var anItem = Get(theId);
			if (anItem == null)
			{
				anItem = new SaveDataItem();
				var anIdOwner = (IOwnerId<string>)anItem;
				anIdOwner.Id = theId;
				Add(anItem);
				Sort();
			}
			anItem.Value = theValue;
			return true;
		}

		#endregion // Functions

	}
}
