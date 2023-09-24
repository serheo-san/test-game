using Engine.Attributes;
using UnityEngine;

namespace Engine.UI
{
	public class LayerOrderReserve : MonoBehaviour
	{
		[SerializeField, SortingLayer]
		private int sortingLayer;

		[SerializeField]
		private int sortingOrder = 0;

		public int SortingLayer => sortingLayer;

		public int SortingOrder
		{
			get => sortingOrder;
			set => sortingOrder = value;
		}
	}
}