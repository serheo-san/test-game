using System;

namespace TestGame
{
	public class Timer
	{
		public delegate void EventHandler(Timer timer);
		
		private int seconds = 0;
		private float msTimer = 0f;

		public event EventHandler StopEvent;
		public event EventHandler ChangeEvent;

		public bool IsRunning => seconds > 0;
		public int Seconds => seconds; 

		public void Update(float deltaTime)
		{
			if (seconds <= 0)
			{
				return;
			}
			msTimer += deltaTime;
			if (msTimer > 1f)
			{
				msTimer -= 1f;
				--seconds;
				ChangeEvent?.Invoke(this);
				if (seconds <= 0)
				{
					StopEvent?.Invoke(this);
				}
			}
		}

		public void Start(int seconds)
		{
			this.seconds = seconds;
			msTimer = 0;
			ChangeEvent?.Invoke(this);
		}
		
	}
}