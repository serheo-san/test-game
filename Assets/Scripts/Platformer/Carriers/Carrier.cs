using System;
using DG.Tweening;
using Engine.Components;
using UnityEngine;

namespace TestGame.Platformer.Carriers
{
	public enum CarrierPathType {
		Line = 0,
		ExtensionInMiddle,
		ExtensionAtStart,
		ExtensionAtFinish,
	}
	
	public class Carrier : ResettableBehaviour
	{
		[SerializeField]
		private SpriteRenderer spriteRenderer = null;
		[SerializeField]
		private float extensionFactor = 0.12f;
		[SerializeField]
		private Ease easing = Ease.InOutQuad;
		[SerializeField]
		private CarrierPathType pathType = CarrierPathType.ExtensionInMiddle;
		[SerializeField]
		private float moveDuration = 0.6f;
		
		private readonly Vector3[] movePath = new Vector3[2];

		private bool isMove = false;
		
		private Action<Carrier> stopMoveEvent = null;
		

		public float OffsetFactor {
			get => extensionFactor;
			set => extensionFactor = value;
		}

		public Ease Easing {
			get => easing;
			set => easing = value;
		}

		public CarrierPathType PathType {
			get => pathType;
			set => pathType = value;
		}
		
		
		public event Action<Carrier> StopMoveEvent {
			add {
				stopMoveEvent -= value;
				stopMoveEvent += value;
			}
			remove {
				stopMoveEvent -= value;
			}
		}

		protected override void OnExpose() {
			base.OnExpose();
			movePath[0] = Vector3.zero;
			movePath[1] = Vector3.zero;
		}

		protected override void OnDispose() {
			base.OnDispose();
			stopMoveEvent = null;
			if (spriteRenderer)
				spriteRenderer.sprite = null;
		}

		public bool Deliver(Sprite sprite, Vector3 scale, Vector2 from, Vector2 to) {
			if ( !this || sprite == null ) {
				return false;
			}
			transform.localPosition = from;
			spriteRenderer.sprite = sprite;
			spriteRenderer.transform.localScale = scale;
			BuildPath( from, to );
			Move();
			return true;
		}

	
		private void BuildPath( Vector2 fromPoint, Vector2 toPoint ) {
			var middlePoint = Vector2.Lerp( fromPoint, toPoint, 0.5f );
			switch ( pathType ) {
				case CarrierPathType.Line: {
					break;
				}
				case CarrierPathType.ExtensionAtStart: {
					var distance = (toPoint - fromPoint).magnitude;
					var radius = extensionFactor * distance;
					var angle = UnityEngine.Random.Range( -Mathf.PI, Mathf.PI );
					middlePoint = fromPoint +  new Vector2(radius * Mathf.Cos(angle), radius*Mathf.Sin(angle));
					break;
				}
				case CarrierPathType.ExtensionInMiddle: {
					var direction = (toPoint - fromPoint);
					var normal = new Vector2(-direction.y, direction.x);
					middlePoint += UnityEngine.Random.Range( -extensionFactor, extensionFactor ) * normal;
					break;
				}
				case CarrierPathType.ExtensionAtFinish: {
					var distance = (toPoint - fromPoint).magnitude;
					var radius = extensionFactor * distance;
					var angle = UnityEngine.Random.Range( -Mathf.PI, Mathf.PI );
					middlePoint = toPoint +  new Vector2(radius * Mathf.Cos(angle), radius*Mathf.Sin(angle));
					break;
				}
			}
			movePath[0] = middlePoint;
			movePath[1] = toPoint;
		}

		private void Move() {
			if ( isMove ) {
				return;
			}
			isMove = true;
			transform.DOLocalPath(movePath, moveDuration, DG.Tweening.PathType.CatmullRom, PathMode.Sidescroller2D)
				.SetEase( easing )
				.OnComplete( OnStopMove );
		}
		
		private void OnStopMove() {
			isMove = false;
			stopMoveEvent?.Invoke(this);
		}

		
	}
}