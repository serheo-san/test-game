
using System;
using Random = UnityEngine.Random;

namespace Engine.Common
{
	//! RangeI \author Serheo \date 2019/01/10 12:57:07
	[System.Serializable]
	public struct RangeI : ICopyable<RangeI>, IEquatable<RangeI>
	{
		public int Min;
		public int Max;

		public static readonly RangeI Zero = new RangeI(0, 0);

		public RangeI(int theMin, int theMax) 
		{
			Min = theMin;
			Max = theMax;
		}

		public RangeI(int theMin, uint theRange)
		{
			Min = theMin;
			Max = Min + (int)theRange;
		}

		public RangeI(RangeI theOther)
		{
			Min = theOther.Min;
			Max = theOther.Max;
		}

		//! ToString \author Serheo \date 2017/01/20 13:07:16
		public override string ToString()
		{
			return string.Format("{0}-{1}", Min, Max);
		}

		//! GetHashCode \author Serheo \date 2018/08/01 15:44:35
		public override int GetHashCode()
		{
			return 31* Min.GetHashCode() + Max.GetHashCode();
		}

		//! Equals \author Serheo \date 2018/08/01 15:44:55
		public override bool Equals(object theOther)
		{
			if (!(theOther is RangeI))
				return false;
			var anOther = (RangeI)theOther;
			return Equals(anOther);
		}

		//! Equals \author Serheo \date 2018/08/01 15:44:03
		public bool Equals(RangeI theOther)
		{
			return Min.Equals(theOther.Min) && Max.Equals(theOther.Max);
		}


		// получить значение случайным образом между min и max
		public int GetValue()
		{
			return Random.Range(Min, Max+1);
		}

		//! Set \author Serheo \date 2017/04/18 13:17:29
		public void Set(int theMin, int theMax)
		{
			Min = theMin;
			Max = theMax;
		}

		//! Normolize \author Serheo \date 2017/08/28 13:37:22
		public void Normolize()
		{
			if (Min <= Max)
			{
				return;
			}
			var aTmp = Min;
			Min = Max;
			Max = aTmp;
		}

		//! Copy \author serheo \date 2016/05/20 10:52:29
		public RangeI Copy(RangeI theValue)
		{
			Min = theValue.Min;
			Max = theValue.Max;
			return this;
		}

		//! Contains \author Serheo \date 2017/08/28 15:54:14
		public bool Contains(int theValue)
		{
			return theValue >= Min && theValue <= Max;
		}

		//! operator == \author Serheo \date 2017/11/03 12:06:23
		public static bool operator == (RangeI theLeft, RangeI theRight)
		{
			return theLeft.Min == theRight.Min && theLeft.Max==theRight.Max;
		}

		//! operator != \author Serheo \date 2017/11/03 12:06:21
		public static bool operator !=(RangeI theLeft, RangeI theRight)
		{
			return theLeft.Min != theRight.Min || theLeft.Max != theRight.Max;
		}

	}
}
