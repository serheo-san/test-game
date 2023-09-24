using System;
using System.Collections.Generic;
using Engine;
using UnityEngine;

namespace TestGame.Platformer.Carriers
{
	public class CarrierManager : MonoBehaviour
	{
		[SerializeField]
		private Carrier carrierPrefab = default;
		
		
		private static Stack<Carrier> pool = new Stack<Carrier>();

		protected void Awake()
		{
			IoC.AddInstance(this);
		}

		protected void OnDestroy()
		{
			IoC.RemoveInstance(this);
		}

		protected Carrier AllocateCarrier(  ) {
			GameObject gameObject;
			Carrier carrier;
			while ( pool.Count > 0 ) {
				carrier = pool.Pop();
				if ( !carrier ) {
					continue;
				}
				if ( carrier ) {
					carrier.transform.SetParent( transform, false );
					carrier.gameObject.SetActive( true );
					carrier.Expose();
					return carrier;
				}
			}
			
			if ( !carrierPrefab ) {
				Debug.LogError( $"Can't find prefab for Carrier" );
				return null;
			}
			gameObject = Instantiate( carrierPrefab.gameObject,  transform, false );
			if ( gameObject ) {
				gameObject.name = "Carrier";
			}
			carrier = gameObject.GetComponent<Carrier>();
			gameObject.SetActive(true);
			return carrier;
		}

		protected void FreeCarrier(Carrier carrier)
		{
			if (!carrier)
			{
				return;
			}
			if (pool.Contains(carrier))
			{
				return;
			}
			carrier.Dispose();
			carrier.gameObject.SetActive(false);
			pool.Push(carrier);
		}
		
		private Carrier DeliverRaw( Sprite sprite, Vector3 scale, Vector2 fromLocalPosition, Vector2 toLocalPosition,
			Action<Carrier> onMoveStop = null ) {
			if ( !sprite  ) {
				return null;
			}
			var carrier = AllocateCarrier();
			if ( !carrier ) {
				return null;
			}
			if ( !carrier.Deliver( sprite, scale, fromLocalPosition, toLocalPosition) ) {
				FreeCarrier( carrier );
				return null;
			}
			carrier.StopMoveEvent += OnCarrierMoveStop;
			carrier.StopMoveEvent += onMoveStop;
			return carrier;
		}
		
		void OnCarrierMoveStop(Carrier stoppedCarrier)
		{
			stoppedCarrier.StopMoveEvent -= OnCarrierMoveStop;
			FreeCarrier(stoppedCarrier);
		}

		private Carrier Deliver( Sprite sprite, Vector3 scale, Vector2 fromPosition, Vector2 toPosition,
			Action<Carrier> onMoveStop = null ) {
			if (!sprite) {
				return null;
			}
			var fromLocalPosition = transform.InverseTransformPoint( fromPosition );
			var toLocalPosition = transform.InverseTransformPoint( toPosition );
			return DeliverRaw( sprite, scale, fromLocalPosition, toLocalPosition, onMoveStop );
		}

		public Carrier Deliver( Sprite sprite, Vector3 scale, Transform from, Transform to, 
			Action<Carrier> onMoveStop = null ) {
			return Deliver( sprite, scale, from.position, to.position, onMoveStop );
		}

		public Carrier Deliver( Sprite sprite, Vector3 scale, Transform from, Transform to, Camera uiCamera, 
			Action<Carrier> onMoveStop = null )
		{
			if (!uiCamera || !Camera.main)
			{
				return null;
			}
			var viewPortPos = uiCamera.WorldToViewportPoint(to.position);
			var worldPosition = Camera.main.ViewportToWorldPoint(viewPortPos);
			return Deliver( sprite, scale, from.position, worldPosition, onMoveStop );
		}
		

	}
}