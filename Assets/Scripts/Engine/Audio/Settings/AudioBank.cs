using Engine.Configs;
using UnityEngine;

namespace Engine.Audio.Settings {
	[CreateAssetMenu( fileName = "AudioBank", menuName = "Engine/Audio/Audio Bank" )]
	public class AudioBank : Config<string, AudioEvent> {
		
	}
}
