using System;
using System.IO;
using Engine.Common;

namespace Engine.Storage.Collections
{
	//! SortedList \author Serheo \date 2017/04/06 17:25:43
	public abstract class SortedList<TItemId, TItem> : List<TItem> 
		where TItem : DataBlock, IComparable<TItem>, IOwnerId<TItemId>, new()
	{
		protected readonly TItem mEtalon = new TItem();


		public SortedList(bool theFlatFormat=false) : base(theFlatFormat)
		{
			
		}

		public TItem this[TItemId theItemId]
		{
			get
			{
				return Get(theItemId);
			}
		}
		
		//! GetUniqueId \author Serheo \date 2017/05/22 16:13:52
		public abstract TItemId GetUniqueItemId();

		//! GetInvalidId \author Serheo \date 2017/05/22 16:26:13
		public abstract TItemId GetInvalidItemId();
	

		//! Get \author Serheo \date 2017/03/10 17:00:11
		public virtual TItem Get(TItemId theItemId)
		{
			mEtalon.Id = theItemId;
			int anIndex = mItems.BinarySearch(mEtalon);
			if (anIndex >= 0)
				return mItems[anIndex];
			return null;
		}

		//! Add \author Serheo \date 2018/01/23 11:36:24
		public override bool Add(TItem theItem)
		{
			var aChild = theItem as IChild;
			if (aChild != null && !ReferenceEquals(aChild.Parent, null))
			{
				return false;
			}
			if (!CorrectId(theItem))
			{
				return false;
			}
			return base.Add(theItem);
		}

		//! ProcessId \author Serheo \date 2017/11/03 16:46:25
		public virtual bool CorrectId(TItem theItem)
		{
			var anOwnerId = (IOwnerId<TItemId>)theItem;
			if (anOwnerId.Id.Equals(GetInvalidItemId()) || ContainsId(anOwnerId.Id))
			{
				anOwnerId.Id = GetUniqueItemId();
				if (anOwnerId.Id.Equals(GetInvalidItemId()))
				{
					return false;
				}
			}
			return true;
		}

		//! Load \author Serheo \date 2017/03/10 16:45:56
		public override void Load(StreamReader theReader)
		{
			base.Load(theReader);
			mItems.Sort();
		}		

		//! Contains \author Serheo \date 2017/05/17 13:46:47
		public bool ContainsId(TItemId theItemId)
		{
			mEtalon.Id = theItemId;
			return mItems.BinarySearch(mEtalon)>=0;
		}

		//! Sort \author Serheo \date 2017/03/10 16:36:04
		public void Sort()
		{
			mItems.Sort();
		}

		//! BinarySearch \author Serheo \date 2017/05/22 17:06:54
		public int FindId(TItemId theItemId)
		{
			mEtalon.Id = theItemId;
			return mItems.BinarySearch(mEtalon);
		}

		//! RemoveId \author Serheo \date 2017/10/30 12:25:26
		public bool RemoveId(TItemId theItemId)
		{
			var anIndex = FindId(theItemId);
			if (anIndex < 0)
				return false;
			RemoveAt(anIndex);
			return true;
		}

		//! GetUniqueItemId \author Serheo \date 2017/08/29 18:15:48
		public static ushort GetUniqueItemId<TItem1>(SortedList<ushort, TItem1> theContainer) where TItem1 : DataBlock, IOwnerId<ushort>, IComparable<TItem1>, new()
		{
			var aLastItem = theContainer.Count > 0 ? theContainer[theContainer.Count-1] : null;
			ushort anId = aLastItem != null ? (ushort)(aLastItem.Id + 1u) : (ushort)0u;
			for (int anIndex = anId; anIndex < ushort.MaxValue; ++anIndex)
			{
				anId = (ushort)anIndex;
				if (!theContainer.ContainsId(anId))
				{
					return anId;
				}
			}
			return theContainer.GetInvalidItemId();
		}
		
		public static int GetUniqueItemId<TItem1>(SortedList<int, TItem1> theContainer) where TItem1 : DataBlock, IComparable<TItem1>, IOwnerId<int>, new()
		{
			var aLastItem = theContainer.Count > 0 ? theContainer[theContainer.Count-1] : null;
			int anId = aLastItem != null ? aLastItem.Id + 1 : 0;
			for (long anIndex = anId; anIndex < int.MaxValue; ++anIndex)
			{
				anId = (int)anIndex;
				if (!theContainer.ContainsId(anId))
				{
					return anId;
				}
			}
			return theContainer.GetInvalidItemId();
		}

		public static string GetUniqueItemId<TItem2>(SortedList<string, TItem2> theList, string theFormat) where TItem2 : DataBlock, IComparable<TItem2>, IOwnerId<string>, new()
		{
			string anId = string.Empty;
			var aLastItem = theList.Count > 0 ? theList[theList.Count - 1] : null;
			int aLastIndex = 0;
			if (aLastItem != null)
			{
				Engine.Common.Utils.TryParseEnd(aLastItem.Id, out aLastIndex);
			}
			for (long anIndex = 0; anIndex < uint.MaxValue; ++anIndex)
			{
				anId = string.Format(theFormat, anIndex + aLastIndex);
				if (!theList.ContainsId(anId))
				{
					return anId;
				}
			}
			return theList.GetInvalidItemId();
		}
		
	}
}
