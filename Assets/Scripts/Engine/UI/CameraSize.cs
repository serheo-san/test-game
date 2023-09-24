using Engine.Components;
using UnityEngine;

namespace Engine.UI
{
	[RequireComponent(typeof(Camera))]
	public class CameraSize : StateBehaviour
	{
		[SerializeField]
		protected Vector2 resolution = new Vector2(360, 640);

		[SerializeField]
		protected float maxWidth = 480f;

		[SerializeField]
		protected float maxHeight = 860;

		protected Camera sizeCamera = default;

		protected Vector2 screenSize = Vector2.zero;

		protected override void OnExpose()
		{
			base.OnExpose();
			sizeCamera = GetComponent<Camera>();
		}

		protected override void OnDispose()
		{
			base.OnDispose();
			sizeCamera = null;
		}

		protected override void OnEnable()
		{
			InvalidateState();
			base.OnEnable();
		}

		protected override void Update()
		{
			if (screenSize.x != Screen.width || screenSize.y != Screen.height)
			{
				InvalidateState();
			}

			base.Update();
		}

		[ContextMenu("Update State")]
		protected override void OnUpdateState()
		{
			if (sizeCamera && sizeCamera.orthographic)
			{
				var viewSize = Utils.GetStretchViewSize(resolution, maxWidth, maxHeight);
				sizeCamera.orthographicSize = viewSize.y * 0.5f;
			}
		}
#if UNITY_EDITOR
		private void OnValidate()
		{
			InvalidateState();
		}
#endif //UNITY_EDITOR 
	}
}