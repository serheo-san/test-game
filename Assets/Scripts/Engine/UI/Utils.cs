using UnityEngine;

namespace Engine.UI
{
	public class Utils
	{
		public static Vector2 GetStretchViewSize(Vector2 resolution, float maxWidth, float maxHeight)
		{
			var screenSize = new Vector2(Screen.width, Screen.height);
			var minAspectRatio = GetViewAspectRatio(resolution);
			var screenAspectRatio = GetViewAspectRatio(screenSize);
			Vector2 canvasSize;
			if (minAspectRatio <= screenAspectRatio)
			{
				canvasSize = new Vector2(
					Mathf.Min(maxWidth, resolution.y * screenAspectRatio),
					Mathf.Min(maxHeight, resolution.y)
				);
			}
			else
			{
				canvasSize = new Vector2(
					Mathf.Min(maxWidth, resolution.x),
					Mathf.Min(maxHeight, resolution.x / screenAspectRatio)
				);
			}

			return canvasSize;
		}

		public static float GetViewAspectRatio(Vector2 viewSize)
		{
			if (viewSize.y.Equals(0f))
			{
				return 1f;
			}

			return viewSize.x / viewSize.y;
		}
	}
}