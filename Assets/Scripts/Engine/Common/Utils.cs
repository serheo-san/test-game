using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Engine.Common
{
	public class Utils
	{
		//! TryGetEndNumber \author Serheo \date 2017/06/13 16:23:18
		public static bool TryParseEnd(string theString, out int theValue)
		{
			theValue = default(int);
			int anIndex = theString.Length - 1;
			for (; anIndex >= 0 && char.IsNumber(theString[anIndex]); --anIndex)
			{
			}
			if (anIndex == theString.Length - 1)
			{
				return false;
			}
			return int.TryParse(theString.Substring(anIndex + 1), out theValue);
		}

		//! Swap \author Serheo \date 2018/12/04 14:58:49
		public static void Swap<TValue>(IList<TValue> theArray, int theFrom, int theTo)
		{
			var temp = theArray[theFrom];
			theArray[theFrom] = theArray[theTo];
			theArray[theTo] = temp;
		}



		//! FillDerivedTypeList \author Serheo \date 2017/06/15 10:43:39
		public static void FillDerivedTypeList(Type theType, List<Type> theList, int theDepth=0, int theMaxDepth=5)
		{
			if (theDepth > theMaxDepth)
			{
				return;
			}
			Type[] theTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.BaseType == theType).ToArray();
			foreach (Type aType in theTypes)
			{
				theList.Add(aType);
				FillDerivedTypeList(aType, theList);
			}
		}

	}
}
