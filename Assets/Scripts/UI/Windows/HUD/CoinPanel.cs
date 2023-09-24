using TMPro;
using UnityEngine;

namespace TestGame.Platformer.UI
{
	public class CoinPanel : MonoBehaviour
	{
		// Fields
		[SerializeField]
		private TMP_Text coinCountView = default;
		[SerializeField]
		private float changeDuration = 0.1f;


		private int coinsCount = 0;
		private int targetCoinsCount = 0;

		private float normolizedTime = 0;
		private float normolizedSpeed = 0;

		

		protected void Awake()
		{
			normolizedSpeed = 1f / changeDuration;
		}

		protected void OnDestroy()
		{

		}

		protected void Start()
		{
			UpdateCoins();
		}

		protected void Update()
		{
			if (targetCoinsCount != coinsCount)
			{
				normolizedTime += normolizedSpeed * Time.deltaTime;
				if (normolizedTime >= 1f)
				{
					if (targetCoinsCount < coinsCount)
					{
						--coinsCount;
					}
					else
					{
						++coinsCount;
					}
					UpdateCoins();
					normolizedTime -= 1f;
				}
			}
		}

		public void SetCoins(int coinsCount)
		{
			targetCoinsCount = coinsCount;
			this.coinsCount = coinsCount;
			UpdateCoins();
		}

		public void AnimateCoins(int coinsCount)
		{
			targetCoinsCount = coinsCount;
		}

		private void UpdateCoins()
		{
			if (coinCountView)
			{
				coinCountView.text = coinsCount.ToString();
			}
		}
	}
}