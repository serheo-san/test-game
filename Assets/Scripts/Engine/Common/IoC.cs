using System;
using TypeContainer = System.Collections.Generic.Dictionary<System.Type, System.Collections.Generic.Dictionary<string, object>>;
using NameContainer = System.Collections.Generic.Dictionary<string, object>;

namespace Engine
{
	//! IoC \author Serheo \date 2018/11/22 15:12:50
	public class IoC
	{
		private readonly TypeContainer instances = new TypeContainer();

		public static readonly IoC Singleton = new IoC();

		//! IoC \author Serheo \date 2018/11/22 15:22:11
		protected IoC()
		{

		}

		#region Functions
		//! AddSingleton \author Serheo \date 2018/11/22 15:17:23
		public bool Add(object theInstance, string theName="")
		{
			if (theInstance == null)
			{
				return false;
			}
			var aType = theInstance.GetType();
			NameContainer aContainer;
			if (!instances.TryGetValue(aType, out aContainer))
			{
				aContainer = new NameContainer();
				instances.Add(aType, aContainer);
			}
			if (aContainer.ContainsKey(theName))
			{
				return false;
			}
			aContainer.Add(theName, theInstance);
			return true;
		}

		//! GetGetInstance \author Serheo \date 2018/11/22 15:17:36
		public bool TryGet<TItem>(out TItem theInstance, string theName="") where TItem : class
		{
			var aType = typeof(TItem);
			theInstance = default(TItem);
			NameContainer aContainer;
			if (!instances.TryGetValue(aType, out aContainer))
			{
				return false;
			}
			object anInstance;
			if (!aContainer.TryGetValue(theName, out anInstance))
			{
				return false;
			}
			theInstance = anInstance as TItem;
			return theInstance != default(TItem);
		}

		//! Remove \author Serheo \date 2018/12/26 10:44:07
		private bool Remove(Type theType, string theName="")
		{
			NameContainer aContainer;
			if (!instances.TryGetValue(theType, out aContainer))
			{
				return false;
			}
			return aContainer.Remove(theName);
		}

		//! Clear \author Serheo \date 2018/11/22 15:21:15
		public void Clear()
		{
			instances.Clear();
		}
		#endregion // Functions

		#region Static Function

		//! AddInstance \author Serheo \date 2018/11/22 15:26:55
		public static bool AddInstance(object theInstance, string theName="")
		{
			return Singleton.Add(theInstance, theName);
		}

		//! TryResolve \author Serheo \date 2018/11/22 15:29:04
		public static bool TryResolve<TInstance>(out TInstance theInstance, string theName="") where TInstance : class
		{
			return Singleton.TryGet(out theInstance, theName);
		}

		//! RemoveInstance \author Serheo \date 2018/12/26 10:43:37
		public static bool RemoveInstance(Type theType, string theName="")
		{
			return Singleton.Remove(theType, theName);
		}

		//! RemoveInstance \author Serheo \date 2019/01/10 15:42:38
		public static bool RemoveInstance<TInstance>(TInstance theInstance, string theName="")
		{
			return Singleton.Remove(typeof(TInstance), theName);
		}


		#endregion // Static Function
	}
}
