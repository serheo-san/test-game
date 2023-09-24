using System;
using UnityEngine;

namespace TestGame.Platformer.Actors
{
	public class ActorSensor : MonoBehaviour
	{
		[SerializeField]
		private ActorSensorType sensorType = ActorSensorType.Left;

		public event Action<ActorSensor> ActivationEvent;  
		public event Action<ActorSensor> DeactivationEvent;

		public ActorSensorType SensorType => sensorType;
		
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.gameObject.layer == LayerMask.NameToLayer("Default"))
			{
				ActivationEvent?.Invoke(this);	
			}
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (other.gameObject.layer == LayerMask.NameToLayer("Default"))
			{
				DeactivationEvent?.Invoke(this);	
			}
		}
	}
}