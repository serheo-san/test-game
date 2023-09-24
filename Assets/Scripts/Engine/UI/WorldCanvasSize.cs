using Engine.Components;
using UnityEngine;

namespace Engine.UI
{
	public class WorldCanvasSize : StateBehaviour
	{
		[SerializeField]
		protected Vector2 resolution = new Vector2(360, 640);

		[SerializeField]
		protected float maxWidth = 480f;

		[SerializeField]
		protected float maxHeight = 860;

		protected Canvas canvas = default;
		protected Vector2 screenSize = Vector2.zero;

		protected override void OnExpose()
		{
			base.OnExpose();
			canvas = GetComponentInChildren<Canvas>();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			canvas = null;
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

		protected override void OnUpdateState()
		{
			screenSize = new Vector2(Screen.width, Screen.height);
			if (canvas && canvas.renderMode == RenderMode.WorldSpace)
			{
				var rectTransform = canvas.GetComponent<RectTransform>();
				var canvasSize = Utils.GetStretchViewSize(resolution, maxWidth, maxHeight);
				if (canvasSize != rectTransform.sizeDelta)
				{
					rectTransform.sizeDelta = canvasSize;
				}
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