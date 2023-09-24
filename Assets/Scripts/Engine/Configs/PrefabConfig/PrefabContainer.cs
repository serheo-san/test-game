using Engine.Configs;
using UnityEngine;

namespace Engine.ScriptableObjects.Prefabs {
	[CreateAssetMenu( fileName = "PrefabConfig", menuName = "Configs/PrefabConfig" )]
	public class PrefabContainer : Config<string, PrefabData> {
	}
}