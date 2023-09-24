using System;
using Engine.Common;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Engine.Storage.Collections
{
	//! List \author Serheo \date 2017/04/06 17:10:51
	public class List<TItem> : DataBlock, IList<TItem>, IResettable where TItem : DataBlock, new()
	{
		protected readonly System.Collections.Generic.List<TItem> mItems = new System.Collections.Generic.List<TItem>();
		protected int mItemCount = 0;
		protected bool mFlatFormat = false;


		public List(bool theFlatFormat = false)
		{
			mFlatFormat = theFlatFormat;
			Expose();
		}

		bool ICollection<TItem>.IsReadOnly
		{
			get
			{
				return ((IList<TItem>)mItems).IsReadOnly;
			}
		}

		public TItem this[int theIndex]
		{
			get
			{
				return GetByIndex(theIndex);
			}
			set
			{
				mItems[theIndex] = value;
			}
		}

		public int Count
		{
			get
			{
				return mItems.Count;
			}
		}

		//! IEnumerable.GetEnumerator \author Serheo \date 2017/03/10 16:36:32
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		//! ICollection<TItem>.CopyTo \author Serheo \date 2017/03/10 16:36:29
		void ICollection<TItem>.CopyTo(TItem[] array, int arrayIndex)
		{
			mItems.CopyTo(array, arrayIndex);
		}

		//! GetEnumerator \author Serheo \date 2017/03/10 16:36:27
		public IEnumerator<TItem> GetEnumerator()
		{
			return mItems.GetEnumerator();
		}


		//! Dispose \author Serheo \date 2017/04/03 18:02:52
		public virtual void Dispose()
		{
			foreach (var anItem in mItems)
			{
				var aChild = anItem as IChild;
				if (aChild != null)
					aChild.Parent = null;
				DisposeItem(anItem);
			}
			mItems.Clear();
		}

		//! Expose \author Serheo \date 2017/04/03 18:02:47
		public virtual void Expose()
		{
			mItemCount = 0;
		}

		//! Add \author Serheo \date 2017/03/10 16:36:24
		public virtual bool Add(TItem theItem)
		{
			var aChild = theItem as IChild;
			if (aChild != null && !ReferenceEquals(aChild.Parent, null))
			{
				return false;
			}
			mItems.Add(theItem);
			if (aChild != null)
			{
				aChild.Parent = this;
			}
			return true;
		}

		//! Remove \author Serheo \date 2017/03/10 16:36:14
		public virtual bool Remove(TItem theItem)
		{
			var aChild = theItem as IChild;
			if (aChild != null && !ReferenceEquals(aChild.Parent, this))
			{
				return false;
			}
			var aResult = mItems.Remove(theItem);
			if (aResult && aChild != null)
			{
				aChild.Parent = null;
			}
			return aResult;
		}

		//! Insert \author Serheo \date 2017/03/10 16:36:09
		public virtual void Insert(int theIndex, TItem theItem)
		{
			var aChild = theItem as IChild;
			if (aChild != null && !ReferenceEquals(aChild.Parent, null))
			{
				return;
			}
			mItems.Insert(theIndex, theItem);
			if (aChild != null)
			{
				aChild.Parent = this;
			}
		}

		//! RemoveAt \author Serheo \date 2017/03/10 16:36:06
		public virtual void RemoveAt(int theIndex)
		{
			var anItem = this[theIndex];
			if (anItem == null)
			{
				return;
			}
			var aChild = anItem as IChild;
			if (aChild != null && !ReferenceEquals(aChild.Parent, this))
			{
				Debug.LogWarning("List item have incorrect parent!");
				return;
			}
			mItems.RemoveAt(theIndex);
			if (aChild != null)
			{
				aChild.Parent = null;
			}
			DisposeItem(anItem);
		}

		//! Clear \author Serheo \date 2017/03/10 16:36:20
		public virtual void Clear()
		{
			foreach (var anItem in mItems)
			{
				var aChild = anItem as IChild;
				if (aChild != null)
				{
					aChild.Parent = null;
				}
				DisposeItem(anItem);
			}
			mItems.Clear();
			mItemCount = 0;
		}

		//! DisposeItem \author Serheo \date 2017/08/28 12:16:33
		protected virtual void DisposeItem(TItem thePack)
		{
			var aDisposable = thePack as IDisposable;
			if (aDisposable != null)
				aDisposable.Dispose();
		}
		
		//! ReadData \author Serheo \date 2016/12/13 12:47:16
		public override void ReadData(BlockReader theReader)
		{
			mItems.Clear();
			mItemCount = theReader.ReadInt();
			if (mFlatFormat)
			{
				ReadItems(theReader);
			}
		}

		//! WriteData \author Serheo \date 2016/12/13 12:47:13
		public override void WriteData(BlockWriter theWriter)
		{
			base.WriteData(theWriter);
			theWriter.Write(mItems.Count);
			if (mFlatFormat)
			{
				WriteItems(theWriter);
			}
		}

		//! Load \author Serheo \date 2017/03/10 16:37:43
		public override void Load(StreamReader theReader)
		{
			Clear();
			base.Load(theReader);
			if (!mFlatFormat)
			{
				LoadItems(theReader);
			}
		}

		//! Save \author Serheo \date 2017/03/10 16:37:47
		public override void Save(StreamWriter theWriter)
		{
			base.Save(theWriter);
			if (!mFlatFormat)
			{
				SaveItems(theWriter);
			}
		}

		//! BeforeLoadItem \author Serheo \date 2017/05/12 10:13:10
		public virtual void BeforeLoadItem(TItem theItem)
		{

		}



		void ICollection<TItem>.Add(TItem theItem)
		{
			Add(theItem);
		}

		//! Contains \author Serheo \date 2017/03/10 16:36:18
		public bool Contains(TItem theItem)
		{
			return mItems.Contains(theItem);
		}

		//! IndexOf \author Serheo \date 2017/03/10 16:36:12
		public int IndexOf(TItem theItem)
		{
			return mItems.IndexOf(theItem);
		}

		public TItem GetByIndex(int theIndex)
		{
			if (theIndex < 0 || theIndex >= Count)
				return default(TItem);
			return mItems[theIndex];
		}

		//! LoadFlatItems \author Serheo \date 2017/10/23 16:51:10
		public void ReadItems(BlockReader theReader)
		{
			mItems.Clear();
			for (int anIndex = 0; anIndex < mItemCount; anIndex++)
			{
				var anItem = new TItem();
				BeforeLoadItem(anItem);
				anItem.ReadData(theReader);
				Add(anItem);
			}
		}

		//! WriteItems \author Serheo \date 2017/10/23 16:54:17
		public void WriteItems(BlockWriter theWriter)
		{
			for (int anIndex = 0; anIndex < mItems.Count; anIndex++)
			{
				var anItem = mItems[anIndex];
				anItem.WriteData(theWriter);
			}
		}

		//! LoadItems \author Serheo \date 2017/10/23 16:55:05
		protected void LoadItems(StreamReader theReader)
		{
			for (int anIndex = 0; anIndex < mItemCount; anIndex++)
			{
				var anItem = new TItem();
				BeforeLoadItem(anItem);
				anItem.Load(theReader);
				Add(anItem);
			}
		}

		//! SaveItems \author Serheo \date 2017/10/23 16:55:36
		protected void SaveItems(StreamWriter theWriter)
		{
			for (int anIndex = 0; anIndex < mItems.Count; anIndex++)
			{
				var anItem = mItems[anIndex];
				anItem.Save(theWriter);
			}
		}

		//! Load \author Serheo \date 2017/03/10 16:37:43
		public void Load(StreamReader theReader, bool theFlatFormat)
		{
			var aFlatFormat = mFlatFormat;
			mFlatFormat = theFlatFormat;
			Clear();
			base.Load(theReader);
			if (!mFlatFormat)
			{
				LoadItems(theReader);
			}
			mFlatFormat = aFlatFormat;
		}

		//! Save \author Serheo \date 2017/03/10 16:37:47
		public void Save(StreamWriter theWriter, bool theFlatFormat)
		{
			var aFlatFormat = mFlatFormat;
			mFlatFormat = theFlatFormat;
			base.Save(theWriter);
			if (!mFlatFormat)
			{
				SaveItems(theWriter);
			}
			mFlatFormat = aFlatFormat;
		}


	}
}
