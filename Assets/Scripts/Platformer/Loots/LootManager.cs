using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TestGame.Platformer.Loots
{
	public class LootManager : MonoBehaviour
	{
		[SerializeField]
		private float coinRespawnPeriod = 2f;
		
		[SerializeField]
		private float chestRespawnPeriod = 20f;

		[SerializeField]
		private int coinMaxNumber = 10;
		
		[SerializeField]
		private int chestMaxNumber = 1;

		[SerializeField]
		private Loot coinSample = default;
		
		[SerializeField]
		private Loot chestSample = default;

		[SerializeField]
		private Transform coinRespawnContainer = default;
		
		[SerializeField]
		private Transform chestRespawnContainer = default;

		
		private readonly Dictionary<LootType, Stack<Loot>> pools = new Dictionary<LootType, Stack<Loot>>();
		private readonly Dictionary<Transform, Loot> activeLoots = new Dictionary<Transform, Loot>();
		private readonly List<Transform> coinFreeRespawnPoints = new List<Transform>();
		private readonly List<Transform> chestFreeRespawnPoints = new List<Transform>();

		private float coinTimer = 0f;
		private float chestTimer = 0f;

		private int activeCoinCount = 0;
		private int activeChestCount = 0;
		
		public event Action<Loot> LootTakenEvent;

		private void Awake()
		{
			InitCoinRespawnPoints();
			InitChestRespawnPoints();
			IoC.AddInstance(this);
		}

		private void OnDestroy()
		{
			IoC.RemoveInstance(this);
		}

		private void Start()
		{
//			PlaceFirstCoins();
//			PlaceChest();
		}

		private void Update()
		{
			if (coinTimer > 0f)
			{
				coinTimer -= Time.deltaTime;
				if (coinTimer <= 0f)
				{
					PlaceCoin();
					if (activeCoinCount < coinMaxNumber)
					{
						coinTimer = coinRespawnPeriod;
					}
				}
			}
			if (chestTimer > 0f)
			{
				chestTimer -= Time.deltaTime;
				if (chestTimer <= 0f)
				{
					PlaceChest();
					if (activeChestCount < chestMaxNumber)
					{
						coinTimer = coinRespawnPeriod;
					}
				}
			}
		}

		public Loot Allocate(LootType lootType)
		{
			Loot loot = default;
			if (pools.TryGetValue(lootType, out var pool) && pool.Any())
			{
				loot = pool.Pop();
			}
			else
			{
				Loot lootSample = default;
				switch (lootType)
				{
					case LootType.Coin:
						lootSample = coinSample;
						break;
					case LootType.Chest:
						lootSample = chestSample;
						break;
				}
				loot = Instantiate<Loot>(lootSample, transform);
			}
			loot.gameObject.SetActive(true);
			return loot;
		}

		public void Free(Loot loot)
		{
			if (!pools.TryGetValue(loot.LootType, out var pool))
			{
				pool = new Stack<Loot>();
				pools.Add(loot.LootType, pool);
			}
			if (!loot)
			{
				return;
			}
			loot.Dispose();
			loot.gameObject.SetActive(false);
			pool.Push(loot);
		}

		public Loot PlaceLoot(LootType lootType, Transform point)
		{
			if (!point || activeLoots.ContainsKey(point))
			{
				return null;
			}
			var loot = Allocate(lootType);
			loot.AnchorPoint = point;
			loot.transform.position = point.position;
			loot.LootTakenEvent += OnLootTaken;
			switch (lootType)
			{
				case LootType.Coin:
					++activeCoinCount;
					coinFreeRespawnPoints.Remove(point);
					break;
				case LootType.Chest:
					++activeChestCount;
					chestFreeRespawnPoints.Remove(point);
					break;
			}
			activeLoots.Add(point, loot);
			return loot;
		}

		public void RemoveLoot(Loot loot)
		{
			if (!loot || !loot.AnchorPoint)
			{
				return;
			}
			if (!activeLoots.ContainsKey(loot.AnchorPoint))
			{
				return;
			}
			loot.LootTakenEvent -= OnLootTaken;
			switch (loot.LootType)
			{
				case LootType.Coin:
					--activeCoinCount;
					coinFreeRespawnPoints.Add(loot.AnchorPoint);
					break;
				case LootType.Chest:
					--activeChestCount;
					chestFreeRespawnPoints.Add(loot.AnchorPoint);
					break;
			}
			activeLoots.Remove(loot.AnchorPoint);
			loot.AnchorPoint = null;
			Free(loot);
		}

		public void InitCoinRespawnPoints()
		{
			for (int index=0; index<coinRespawnContainer.childCount; ++index)
			{
				var respawnTransform = coinRespawnContainer.GetChild(index);
				coinFreeRespawnPoints.Add(respawnTransform);
			}
		}
		
		public void InitChestRespawnPoints()
		{
			for (int index=0; index<chestRespawnContainer.childCount; ++index)
			{
				var respawnTransform = chestRespawnContainer.GetChild(index);
				chestFreeRespawnPoints.Add(respawnTransform);
			}
		}

		public void PlaceFirstCoins()
		{
			while (activeCoinCount < coinMaxNumber && coinFreeRespawnPoints.Any())
			{
				PlaceCoin();
			}
			if (activeCoinCount < coinMaxNumber)
			{
				coinTimer = coinRespawnPeriod;
			}
		}
		
		private void OnLootTaken(Loot loot)
		{
			LootTakenEvent?.Invoke(loot);
			RemoveLoot(loot);
			if (loot.LootType == LootType.Coin)
			{
				coinTimer = coinRespawnPeriod;				
			}
			if (loot.LootType == LootType.Chest)
			{
				chestTimer = chestRespawnPeriod;				
			}
		}

		public bool PlaceCoin()
		{
			if (!coinFreeRespawnPoints.Any() || activeCoinCount>=coinMaxNumber)
			{
				return false;
			}

			return PlaceLoot(LootType.Coin, coinFreeRespawnPoints[Random.Range(0, coinFreeRespawnPoints.Count)]);
		}
		
		public bool PlaceChest()
		{
			if (!chestFreeRespawnPoints.Any() || activeChestCount>=chestMaxNumber)
			{
				return false;
			}
			return PlaceLoot(LootType.Chest, chestFreeRespawnPoints[Random.Range(0, chestFreeRespawnPoints.Count)]);
		}
	}
}