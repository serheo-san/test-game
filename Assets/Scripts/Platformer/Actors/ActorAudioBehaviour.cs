using Engine;
using Engine.Audio;
using UnityEngine;

namespace TestGame.Platformer.Actors
{
	public class ActorAudioBehaviour : MonoBehaviour
	{
		private Actor actor = default;
		private AudioManager audioManager = default;
		
		protected void Awake()
		{
			actor = GetComponent<Actor>();
			if(actor)
			{
				actor.BottomSensorEvent += OnActorBottomSensor;
				actor.JumpEvent += OnJump;
			}
			IoC.TryResolve(out audioManager);
		}

		private void OnJump(Actor actor1)
		{
			if (audioManager != null)
			{
				audioManager.Play("JumpEvent");
			}
		}

		protected void OnDestroy()
		{
			if(actor)
			{
				actor.BottomSensorEvent -= OnActorBottomSensor;
				actor.JumpEvent -= OnJump;
			}
			audioManager = null;
		}

		private void OnActorBottomSensor(Actor actor)
		{
			if (audioManager != null)
			{
				audioManager.Play("ActorBottomEvent");
			}
		}
	}
}