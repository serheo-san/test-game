using System;
using UnityEngine;
using UnityEngine.UI;

namespace Engine.Common
{
	//////////////////////////////////////////////////////////////////////////
	//! GameObjectEx \author serheo 
	internal static class GameObjectEx
	{
		//! GetComponentInHeirarchy \author serheo 
		public static TComponent GetComponentInHierarchy<TComponent>(this GameObject theGameObject) where TComponent : Component
		{
			if (theGameObject == null)
			{
				return null;
			}
			var aComponent = theGameObject.GetComponent<TComponent>();
			if (aComponent != null)
			{
				return aComponent;
			}
			aComponent = theGameObject.GetComponentInChildren<TComponent>();
			if (aComponent != null)
			{
				return aComponent;
			}
			return theGameObject.GetComponentInParent<TComponent>();
		}

		//! FindRecursive \author Serheo 
		public static GameObject FindRecursive(this GameObject theObject, string theName, bool theIgnoreCase = false)
		{
			if (theObject!=null && theObject.name.Equals(theName, theIgnoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture))
			{
				return theObject;
			}
			var aParent = theObject != null ? theObject.transform : null;
			if (aParent == null)
			{
				return null;
			}
			int aCount = aParent.childCount;
			for (int anIndex = 0; anIndex < aCount; ++anIndex)
			{
				var aGameObject = aParent.GetChild(anIndex).gameObject.FindRecursive(theName);
				if (aGameObject != null)
				{
					return aGameObject;
				}
			}
			return null;
		}

		//! FindRecursive \author Serheo 
		public static TComponent FindRecursive<TComponent>(this GameObject theObject, string theName) where TComponent: Component
		{
			var anObject = theObject.FindRecursive(theName);
			if (anObject == null)
			{
				return null;
			}
			return anObject.GetComponent<TComponent>();
		}

		//! FindRecursive \author Serheo 
		public static TComponent FindRecursive<TComponent>(this GameObject theObject, bool theExcludeInactive=false) where TComponent : Component
		{
			var aComponent = theObject!=null ? theObject.GetComponent<TComponent>() : null;
			if (aComponent!=null && (aComponent.gameObject.activeInHierarchy || !theExcludeInactive))
			{
				return aComponent;
			}
			var aParent = theObject!=null ? theObject.transform : null;
			if (aParent == null)
			{
				return null;
			}
			int aCount = aParent.childCount;
			for (int anIndex = 0; anIndex < aCount; ++anIndex)
			{
				aComponent = aParent.GetChild(anIndex).gameObject.FindRecursive<TComponent>(theExcludeInactive);
				if (aComponent != null)
				{
					return aComponent;
				}
			}
			return null;
		}

		//! FindRecursive \author Serheo 
		public static GameObject FindRecursive(this GameObject theObject, Predicate<GameObject> thePredicate )
		{
			if (theObject!=null && thePredicate(theObject))
			{
				return theObject;
			}
			var aParent = theObject != null ? theObject.transform : null;
			if (aParent == null)
			{
				return null;
			}
			int aCount = aParent.childCount;
			for (int anIndex = 0; anIndex < aCount; ++anIndex)
			{
				var aGameObject = aParent.GetChild(anIndex).gameObject.FindRecursive(thePredicate);
				if (aGameObject != null)
				{
					return aGameObject;
				}
			}
			return null;
		}

		//! FindRecursive \author Serheo 
		public static TComponent FindRecursive<TComponent>(this GameObject theObject, Predicate<TComponent> thePredicate) where  TComponent : Component
		{
			var aComponent = theObject!=null ? theObject.GetComponent<TComponent>() : null;
			if (aComponent!=null && thePredicate(aComponent))
			{
				return aComponent;
			}
			var aParent = theObject!=null ? theObject.transform : null;
			if (aParent == null)
			{
				return null;
			}
			int aCount = aParent.childCount;
			for (int anIndex = 0; anIndex < aCount; ++anIndex)
			{
				var aGameObject = aParent.GetChild(anIndex).gameObject.FindRecursive(thePredicate);
				if (aGameObject != null)
				{
					return aGameObject;
				}
			}
			return null;
		}

		//! FindChild \author Serheo 
		public static GameObject FindChild(this GameObject theObject, Predicate<GameObject> thePredicate)
		{
			if (theObject != null && thePredicate(theObject))
			{
				return theObject;
			}
			var aParent = theObject != null ? theObject.transform : null;
			if (aParent == null)
			{
				return null;
			}
			int aCount = aParent.childCount;
			for (int anIndex = 0; anIndex < aCount; ++anIndex)
			{
				var aGameObject = aParent.GetChild(anIndex).gameObject;
				if (aGameObject == null)
				{
					continue;
				}
				if (thePredicate(aGameObject))
				{
					return aGameObject;
				}
			}
			return null;
		}

		//! FindChild \author Serheo 
		public static TComponent FindChild<TComponent>(this GameObject theObject, string theName, bool theIgnoreCase=false) where TComponent : Component
		{
			var aParent = theObject != null ? theObject.transform : null;
			if (aParent == null)
			{
				return null;
			}
			int aCount = aParent.childCount;
			for (int anIndex = 0; anIndex < aCount; ++anIndex)
			{
				var aGameObject = aParent.GetChild(anIndex).gameObject;
				if (aGameObject == null)
				{
					continue;
				}
				if (aGameObject.name.Equals(theName, theIgnoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture))
				{
					return aGameObject.GetComponent<TComponent>();
				}
			}
			return null;
		}

		//! FindChild \author Serheo 
		public static GameObject FindChild(this GameObject theObject, string theName, bool theIgnoreCase = false)
		{
			var aParent = theObject != null ? theObject.transform : null;
			if (aParent == null)
			{
				return null;
			}
			int aCount = aParent.childCount;
			for (int anIndex = 0; anIndex < aCount; ++anIndex)
			{
				var aGameObject = aParent.GetChild(anIndex).gameObject;
				if (aGameObject == null)
				{
					continue;
				}
				if (aGameObject.name.Equals(theName, theIgnoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture))
				{
					return aGameObject;
				}
			}
			return null;
		}


		//! FirstChild \author Serheo 
		public static GameObject FirstChild(this GameObject theObject)
		{
			if (theObject==null || theObject.transform == null || theObject.transform.childCount == 0)
			{
				return null;
			}
			return theObject.transform.GetChild(0).gameObject;
		}

		//! LastChild \author Serheo 
		public static GameObject LastChild(this GameObject theObject)
		{
			if (theObject == null || theObject.transform == null || theObject.transform.childCount == 0)
			{
				return null;
			}
			return theObject.transform.GetChild(theObject.transform.childCount-1).gameObject;
		}

		public static Selectable FindSelectable(this GameObject theGameObject)
		{
			return theGameObject.FindRecursive<Selectable>(theSelectable => theSelectable.isActiveAndEnabled);
		}

		//! GetParent \author Serheo 
		public static GameObject GetParent(this GameObject theGameObject)
		{
			var aTransform = theGameObject != null ? theGameObject.transform : null;
			var aParentTransform = aTransform != null ? aTransform.parent : null;
			return aParentTransform != null ? aParentTransform.gameObject : null;
		}

		//! GetSiblingIndex \author Serheo 
		public static int GetSiblingIndex(this GameObject theGameObject)
		{
			return theGameObject != null && theGameObject.transform != null ? theGameObject.transform.GetSiblingIndex() : -1;
		}

		//! SetSiblingIndex \author Serheo \date 2018/05/22 15:24:10
		public static void SetSiblingIndex(this GameObject theGameObject, int theIndex)
		{
			if(theGameObject != null && theGameObject.transform != null)
				theGameObject.transform.SetSiblingIndex(theIndex);
		}


		//! GetChild \author Serheo 
		public static GameObject GetChild(this GameObject theGameObject, int theIndex)
		{
			var aTransform = theGameObject != null ? theGameObject.transform : null;
			var aChildTransform = aTransform != null ? aTransform.GetChild(theIndex) : null;
			return aChildTransform!=null ? aChildTransform.gameObject : null;
		}

		//! GetChildCount \author Serheo 
		public static int GetChildCount(this GameObject theGameObject)
		{
			var aTransform = theGameObject != null ? theGameObject.transform : null;
			return aTransform != null ? aTransform.childCount: 0;
		}

	}
}
