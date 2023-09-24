using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace Engine.Storage
{
	//! BlockReader \author Serheo
	public class BlockReader : IDisposable
	{

		#region Members

		private char mSeparator;
		private StreamReader mReader;
		private string[] mBuffer = null;
		private int mBufferIndex = 0;

		#endregion // Members

		#region Constructor

		//! BlockReader \author serheo 
		public BlockReader(StreamReader theReader, char theSeparator = DataBlock.Separator)
		{
			mReader = theReader;
			mSeparator = theSeparator;
		}

		#endregion // Constructor

		#region properties
		public bool Eob // end of buffer (проверка на окончание блока данных)
		{
			get
			{
				return mBuffer == null || mBufferIndex >= mBuffer.Length;
			}
		}

		public int BufferLength
		{
			get
			{
				return mBuffer != null ? mBuffer.Length : 0;
			}
		}

		#endregion

		#region Functions

		//! Dispose \author serheo 
		public void Dispose()
		{
			mBufferIndex = 0;
			mBuffer = null;
			mReader = null;
		}

		//! ReadBlock \author serheo 
		public bool ReadBlock()
		{
			if (mReader == null || mReader.EndOfStream)
			{
				return false;
			}
			mBufferIndex = 0;
			var aString = mReader.ReadLine();
			if (!string.IsNullOrEmpty(aString))
			{
				mBuffer = aString.Split(mSeparator);
			}
			return true;
		}

		//! TryReadString \author serheo 
		public bool TryReadString(out string theValue)
		{
			if (mBuffer == null || mBufferIndex >= mBuffer.Length)
			{
				theValue = string.Empty;
				return false;
			}
			theValue = mBuffer[mBufferIndex++];
			return true;
		}

		//! TryRead \author Serheo 
		public bool TryRead(out bool theValue)
		{
			string aValue;
			if (!TryReadString(out aValue))
			{
				theValue = default(bool);
				return false;
			}
			return bool.TryParse(aValue, out theValue);
		}

		//! TryRead \author Serheo 
		public bool TryRead(out byte theValue)
		{
			string aValue;
			if (!TryReadString(out aValue))
			{
				theValue = default(byte);
				return false;
			}
			return byte.TryParse(aValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out theValue);
		}

		//! TryRead \author Serheo 
		public bool TryRead(out short theValue)
		{
			string aValue;
			if (!TryReadString(out aValue))
			{
				theValue = default(short);
				return false;
			}
			return short.TryParse(aValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out theValue);
		}

		//! TryRead \author Serheo 
		public bool TryRead(out ushort theValue)
		{
			string aValue;
			if (!TryReadString(out aValue))
			{
				theValue = default(ushort);
				return false;
			}
			return ushort.TryParse(aValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out theValue);
		}

		//! TryRead \author Serheo 
		public bool TryRead(out int theValue)
		{
			string aValue;
			if (!TryReadString(out aValue))
			{
				theValue = default(int);
				return false;
			}
			return int.TryParse(aValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out theValue);
		}

		//! TryRead \author Serheo 
		public bool TryRead(out uint theValue)
		{
			string aValue;
			if (!TryReadString(out aValue))
			{
				theValue = default(uint);
				return false;
			}
			return uint.TryParse(aValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out theValue);
		}

		//! TryRead \author Serheo 
		public bool TryRead(out long theValue)
		{
			string aValue;
			if (!TryReadString(out aValue))
			{
				theValue = default(long);
				return false;
			}
			return long.TryParse(aValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out theValue);
		}

		//! TryRead \author Serheo 
		public bool TryRead(out ulong theValue)
		{
			string aValue;
			if (!TryReadString(out aValue))
			{
				theValue = default(ulong);
				return false;
			}
			return ulong.TryParse(aValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out theValue);
		}

		//! TryRead \author Serheo 
		public bool TryRead(out float theValue)
		{
			string aValue;
			if (!TryReadString(out aValue))
			{
				theValue = default(float);
				return false;
			}
			return float.TryParse(aValue, NumberStyles.Float, CultureInfo.InvariantCulture, out theValue);
		}

		//! TryRead \author Serheo 
		public bool TryRead(out double theValue)
		{
			string aValue;
			if (!TryReadString(out aValue))
			{
				theValue = default(double);
				return false;
			}
			return double.TryParse(aValue, NumberStyles.Float, CultureInfo.InvariantCulture, out theValue);
		}

		//! TryRead \author Serheo 
		public bool TryRead(out string theValue)
		{
			return TryReadString(out theValue);
		}

		//! TryReadEnum \author Serheo 
		public bool TryReadEnum<TEnum>(out TEnum theValue)
		{
			string aString;
			if (!TryReadString(out aString))
			{
				theValue = default(TEnum);
				return false;
			}
			try
			{
				theValue = (TEnum)Enum.Parse(typeof(TEnum), aString, true);
			}
			catch
			{
				theValue = default(TEnum);
			}
			return true;
		}

		//! TryRead \author Serheo 
		public bool TryRead(out Vector2 theVector)
		{
			theVector = Vector2.zero;
			if (!TryRead(out theVector.x))
				return false;
			if (!TryRead(out theVector.y))
				return false;
			return true;
		}

		//! TryRead \author Serheo 
		public bool TryRead(out Vector3 theVector)
		{
			theVector = Vector3.zero;
			if (!TryRead(out theVector.x))
				return false;
			if (!TryRead(out theVector.y))
				return false;
			if (!TryRead(out theVector.z))
				return false;
			return true;
		}

		//! ReadArray \author Serheo 
		public bool TryRead(IList<byte> theBytes)
		{
			string aString;
			if (!TryReadString(out aString))
			{
				return false;
			}
			var aStrings = aString.Split(DataBlock.ArraySeparator);
			if (aStrings.Length == 0)
			{
				return false;
			}
			if (theBytes.IsReadOnly)
			{
				int aCount = Math.Min(aStrings.Length, theBytes.Count);
				for (int anIndex = 0; anIndex < aCount; ++anIndex)
				{
					byte aValue;
					if (byte.TryParse(aStrings[anIndex], NumberStyles.Integer, CultureInfo.InvariantCulture, out aValue))
					{
						theBytes[anIndex] = aValue;
					}
				}
			}
			else
			{
				theBytes.Clear();
				int aCount = aStrings.Length;
				for (int anIndex = 0; anIndex < aCount; ++anIndex)
				{
					byte aValue;
					if (byte.TryParse(aStrings[anIndex], NumberStyles.Integer, CultureInfo.InvariantCulture, out aValue))
					{
						theBytes.Add(aValue);
					}
				}
			}
			return true;
		}

		//! TryReadArray \author Serheo 
		public bool TryRead(IList<ushort> theBytes)
		{
			string aString;
			if (!TryReadString(out aString))
			{
				return false;
			}
			var aStrings = aString.Split(DataBlock.ArraySeparator);
			if (aStrings.Length == 0)
			{
				return false;
			}
			if (theBytes.IsReadOnly)
			{
				int aCount = Math.Min(aStrings.Length, theBytes.Count);
				for (int anIndex = 0; anIndex < aCount; ++anIndex)
				{
					ushort aValue;
					if (ushort.TryParse(aStrings[anIndex], NumberStyles.Integer, CultureInfo.InvariantCulture, out aValue))
					{
						theBytes[anIndex] = aValue;
					}
				}
			}
			else
			{
				theBytes.Clear();
				int aCount = aStrings.Length;
				for (int anIndex = 0; anIndex < aCount; ++anIndex)
				{
					ushort aValue;
					if (ushort.TryParse(aStrings[anIndex], NumberStyles.Integer, CultureInfo.InvariantCulture, out aValue))
					{
						theBytes.Add(aValue);
					}
				}
			}
			return true;
		}

		//! ReadArray \author Serheo 
		public bool TryRead(IList<int> theList)
		{
			string aString;
			if (!TryReadString(out aString))
			{
				return false;
			}
			var aStrings = aString.Split(DataBlock.ArraySeparator);
			if (aStrings.Length == 0)
			{
				return false;
			}
			if (theList.IsReadOnly)
			{
				int aCount = Math.Min(aStrings.Length, theList.Count);
				for (int anIndex = 0; anIndex < aCount; ++anIndex)
				{
					int aValue;
					if (int.TryParse(aStrings[anIndex], NumberStyles.Integer, CultureInfo.InvariantCulture, out aValue))
					{
						theList[anIndex] = aValue;
					}
				}
			}
			else
			{
				theList.Clear();
				int aCount = aStrings.Length;
				for (int anIndex = 0; anIndex < aCount; ++anIndex)
				{
					int aValue;
					if (int.TryParse(aStrings[anIndex], NumberStyles.Integer, CultureInfo.InvariantCulture, out aValue))
					{
						theList.Add(aValue);
					}
				}
			}
			return true;
		}

		//! ReadArray \author Serheo 
		public bool TryRead(IList<string> theArray)
		{
			string aString;
			if (!TryReadString(out aString))
			{
				return false;
			}
			var aStrings = aString.Split(DataBlock.ArraySeparator);
			if (aStrings.Length == 0)
			{
				return false;
			}
			if (theArray.IsReadOnly)
			{
				int aCount = Math.Min(aStrings.Length, theArray.Count);
				for (int anIndex = 0; anIndex < aCount; ++anIndex)
				{
					theArray[anIndex] = aStrings[anIndex];
				}
			}
			else
			{
				theArray.Clear();
				int aCount = aStrings.Length;
				for (int anIndex = 0; anIndex < aCount; ++anIndex)
				{
					theArray.Add(aStrings[anIndex]);
				}
			}
			return true;
		}

		//! ReadArray \author Serheo 
		public bool TryRead(BitArray theArray)
		{
			string aString;
			if (!TryReadString(out aString))
			{
				return false;
			}
			var aStrings = aString.Split(DataBlock.ArraySeparator);
			if (aStrings.Length == 0)
			{
				return false;
			}
			int aCount = Math.Min(theArray.Count, aStrings.Length);
			for (int anIndex = 0; anIndex < aCount; ++anIndex)
			{
				byte aValue;
				if (byte.TryParse(aStrings[anIndex], NumberStyles.Integer, CultureInfo.InvariantCulture, out aValue))
				{
					theArray[anIndex] = aValue != 0;
				}
			}
			return true;
		}

		//! Read \author Serheo 
		public bool TryRead<TValue>(ICollection<TValue> theCollection) where TValue : DataBlock, new()
		{
			theCollection.Clear();
			int aCount = 0;
			if (!TryRead(out aCount))
			{
				return false;
			}
			for (int anIndex = 0; anIndex < aCount; ++anIndex)
			{
				if (Eob) // данные закончились раньше чем было рассчитано
					return false;
				var aValue = new TValue();
				aValue.ReadData(this);
				theCollection.Add(aValue);
			}
			return true;
		}

		//! TryRead \author Serheo 
		public bool TryRead(out DateTime theDateTime)
		{
			theDateTime = DateTime.MinValue;
			string aString;
			if (!TryReadString(out aString))
			{
				return false;
			}
			if (!DateTime.TryParseExact(aString, DataBlock.DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out theDateTime))
			{
				return false;
			}
			return true;
		}

		//! ReadString \author serheo 
		public string ReadString()
		{
			string aString;
			TryReadString(out aString);
			return aString;
		}

		//! ReadBool \author serheo 
		public bool ReadBool()
		{
			string aString;
			if (!TryReadString(out aString))
			{
				return default(bool);
			}
			bool aValue;
			if (!bool.TryParse(aString, out aValue))
			{
				return default(bool);
			}
			return aValue;
		}

		//! ReadByte \author serheo 
		public byte ReadByte()
		{
			string aString;
			if (!TryReadString(out aString))
			{
				return default(byte);
			}
			byte aValue;
			if (!byte.TryParse(aString, NumberStyles.Integer, CultureInfo.InvariantCulture, out aValue))
			{
				return default(byte);
			}
			return aValue;
		}

		//! ReadShort \author Serheo 
		public short ReadShort()
		{
			string aString;
			if (!TryReadString(out aString))
			{
				return default(short);
			}
			short aValue;
			if (!short.TryParse(aString, NumberStyles.Integer, CultureInfo.InvariantCulture, out aValue))
			{
				return default(short);
			}
			return aValue;
		}

		//! ReadUShort \author Serheo 
		public ushort ReadUShort()
		{
			string aString;
			if (!TryReadString(out aString))
			{
				return default(ushort);
			}
			ushort aValue;
			if (!ushort.TryParse(aString, NumberStyles.Integer, CultureInfo.InvariantCulture, out aValue))
			{
				return default(ushort);
			}
			return aValue;
		}

		//! ReadInt \author serheo 
		public int ReadInt()
		{
			string aString;
			if (!TryReadString(out aString))
			{
				return default(int);
			}
			int aValue;
			if (!int.TryParse(aString, NumberStyles.Integer, CultureInfo.InvariantCulture, out aValue))
			{
				return default(int);
			}
			return aValue;
		}

		//! ReadUInt \author serheo 
		public uint ReadUInt()
		{
			string aString;
			if (!TryReadString(out aString))
			{
				return default(uint);
			}
			uint aValue;
			if (!uint.TryParse(aString, NumberStyles.Integer, CultureInfo.InvariantCulture, out aValue))
			{
				return default(uint);
			}
			return aValue;
		}

		//! ReadULong \author serheo 
		public ulong ReadULong()
		{
			string aString;
			if (!TryReadString(out aString))
			{
				return default(ulong);
			}
			ulong aValue;
			if (!ulong.TryParse(aString, NumberStyles.Integer, CultureInfo.InvariantCulture, out aValue))
			{
				return default(ulong);
			}
			return aValue;
		}

		//! ReadLong \author serheo 
		public long ReadLong()
		{
			string aString;
			if (!TryReadString(out aString))
			{
				return default(long);
			}
			long aValue;
			if (!long.TryParse(aString, NumberStyles.Integer, CultureInfo.InvariantCulture, out aValue))
			{
				return default(long);
			}
			return aValue;
		}

		//! ReadFloat \author serheo 
		public float ReadFloat()
		{
			string aString;
			if (!TryReadString(out aString))
			{
				return default(float);
			}
			float aValue;
			if (!float.TryParse(aString, NumberStyles.Float, CultureInfo.InvariantCulture, out aValue))
			{
				return default(float);
			}
			return aValue;
		}

		//! ReadDouble \author serheo 
		public double ReadDouble()
		{
			string aString;
			if (!TryReadString(out aString))
			{
				return default(double);
			}
			double aValue;
			if (!double.TryParse(aString, NumberStyles.Float, CultureInfo.InvariantCulture, out aValue))
			{
				return default(double);
			}
			return aValue;
		}

		//! ReadEnum \author serheo 
		public TEnum ReadEnum<TEnum>()
		{
			string aString;
			if (!TryReadString(out aString))
			{
				return default(TEnum);
			}
			return (TEnum)Enum.Parse(typeof(TEnum), aString, true);
		}

		//! ReadDateTime \author Serheo 
		public DateTime ReadDateTime()
		{
			string aString;
			if (!TryReadString(out aString))
			{
				return DateTime.MinValue;
			}
			DateTime aDateTime;
			if (!DateTime.TryParseExact(aString, DataBlock.DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None,  out aDateTime))
			{
				return DateTime.MinValue;
			}
			return aDateTime;
		}

		//! Read \author Serheo 
		public TValue Read<TValue>()
		{
			var aType = typeof(TValue);
			var aTypeCode = Type.GetTypeCode(aType);
			switch (aTypeCode)
			{
				case TypeCode.Boolean:
					return (TValue)(object)ReadBool();
				case TypeCode.Byte:
					return (TValue)(object)ReadByte();
				case TypeCode.Int16:
					return (TValue)(object)ReadShort();
				case TypeCode.UInt16:
					return (TValue)(object)ReadUShort();
				case TypeCode.Int32:
					return (TValue)(object)ReadInt();
				case TypeCode.UInt32:
					return (TValue)(object)ReadUInt();
				case TypeCode.Int64:
					return (TValue)(object)ReadLong();
				case TypeCode.UInt64:
					return (TValue)(object)ReadULong();
				case TypeCode.Single:
					return (TValue)(object)ReadFloat();
				case TypeCode.Double:
					return (TValue)(object)ReadDouble();
				case TypeCode.String:
					return (TValue)(object)ReadString();
				case TypeCode.DateTime:
					return (TValue)(object)ReadDateTime();
			}
			if (aTypeCode == TypeCode.Object)
			{
				
			}
			return default(TValue);
		}


		//! ReadVector2 \author serheo 
		public Vector2 ReadVector2()
		{
			return new Vector2 { x = ReadFloat(), y = ReadFloat() };
		}

		//! ReadVector3 \author serheo 
		public Vector3 ReadVector3()
		{
			return new Vector3 { x = ReadFloat(), y = ReadFloat(), z = ReadFloat() };
		}

		#endregion // Functions
	}
}