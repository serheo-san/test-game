using System;
using UnityEngine;

namespace TestGame.Platformer.Loots
{
	public class Loot : MonoBehaviour
	{
		[SerializeField]
		private int count = 1;

		[SerializeField]
		private LootType lootType = LootType.Coin;

		[SerializeField]
		private SpriteRenderer spriteRenderer = default;

		public event Action<Loot> LootTakenEvent;
		public Transform AnchorPoint { get; set; }

		public LootType LootType => lootType;
		public int Count => count;
		public Sprite Sprite => spriteRenderer ? spriteRenderer.sprite : default;

		public void Dispose()
		{
			AnchorPoint = default;
		}
		
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.gameObject.tag == "Player")
			{
				LootTakenEvent.Invoke(this);
			}
		}
		
	}
}