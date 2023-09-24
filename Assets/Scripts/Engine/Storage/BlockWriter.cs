using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

namespace Engine.Storage
{
	//! BlockWriter \author Serheo
	public class BlockWriter : IDisposable
	{
		private char mSeparator;
		private StreamWriter mWriter;
		private readonly StringBuilder mBuffer = new StringBuilder();
		private int mCount;

		//! BlockWriter \author serheo 
		public BlockWriter(StreamWriter theWriter, char theSeparator = DataBlock.Separator)
		{
			mSeparator = theSeparator;
			mWriter = theWriter;
		}

		//! Dispose \author serheo 
		public void Dispose()
		{
			mWriter = null;
			mBuffer.Length = 0;
			mCount = 0;
		}

		//! Write \author serheo 
		public bool Write<TValue>(TValue theValue)
		{
			if ((mCount++) > 0)
			{
				mBuffer.Append(mSeparator);
			}
			mBuffer.Append(theValue.ToString());
			return true;
		}

		//! Write \author Serheo 
		public bool WriteArray<TValue>(ICollection<TValue> theArray)
		{
			if ((mCount++) > 0)
			{
				mBuffer.Append(mSeparator);
			}
			bool anArraySparator = false;
			foreach (var aValue in theArray)
			{
				if (anArraySparator)
				{
					mBuffer.Append(DataBlock.ArraySeparator);
				}
				mBuffer.Append(aValue);
				anArraySparator = true;
			}
			return true;
		}

		//! WriteBitArray \author Serheo 
		public bool WriteBitArray(BitArray theArray)
		{
			if ((mCount++) > 0)
			{
				mBuffer.Append(mSeparator);
			}
			for (int anIndex = 0; anIndex < theArray.Count; ++anIndex)
			{
				if (anIndex > 0)
				{
					mBuffer.Append(DataBlock.ArraySeparator);
				}
				mBuffer.Append(theArray.Get(anIndex) ? 1 : 0);
			}
			return true;
		}

		//! Write \author Serheo 
		public bool WriteCollection<TValue>(ICollection<TValue> theCollection) where TValue : DataBlock
		{
			Write(theCollection.Count);
			foreach (var aValue in theCollection)
			{
				aValue.WriteData(this);
			}
			return true;
		}

		//! Write \author serheo 
		public bool Write(string theValue)
		{
			if ((mCount++) > 0)
			{
				mBuffer.Append(mSeparator);
			}
			mBuffer.Append(theValue);
			return true;
		}

		//! WriteFormat \author Serheo 
		public bool WriteFormat(string theFormat, params object[] theParams)
		{
			if ((mCount++) > 0)
			{
				mBuffer.Append(mSeparator);
			}
			mBuffer.AppendFormat(theFormat, theParams);
			return true;
		}

		public bool Write(float theValue)
		{
			if ((mCount++) > 0)
			{
				mBuffer.Append(mSeparator);
			}
			mBuffer.Append(theValue.ToString(CultureInfo.InvariantCulture));
			return true;
		}

		public bool Write(double theValue)
		{
			if ((mCount++) > 0)
			{
				mBuffer.Append(mSeparator);
			}
			mBuffer.Append(theValue.ToString(CultureInfo.InvariantCulture));
			return true;
		}

		//! Write \author Serheo 
		public bool Write(DateTime theDateTime)
		{
			return Write(theDateTime.ToString(DataBlock.DateTimeFormat, CultureInfo.InvariantCulture));
		}

		//! Write \author serheo 
		public bool Write(Vector2 theVector)
		{
			Write(theVector.x);
			Write(theVector.y);
			return true;
		}

		//! Write \author serheo 
		public bool Write(Vector3 theVector)
		{
			Write(theVector.x);
			Write(theVector.y);
			Write(theVector.z);
			return true;
		}

		//! WriteBlock \author serheo 
		public bool WriteBlock()
		{
			if (mWriter == null)
			{
				return false;
			}
			mWriter.WriteLine(mBuffer);
			mBuffer.Length = 0;
			mCount = 0;
			return true;
		}
	}
}