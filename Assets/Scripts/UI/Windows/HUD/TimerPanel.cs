using TMPro;
using UnityEngine;

namespace TestGame.Platformer.UI
{
	public class TimerPanel : MonoBehaviour
	{
		// Fields
		[SerializeField]
		private TMP_Text timerView = default;

		private Timer timer = default;

		
		//Unity cycle methods
		protected void Awake()
		{
		}

		protected void OnDestroy()
		{
			if(timer!=null)
				timer.ChangeEvent -= OnTimerChanged;
		}

		protected void Start()
		{
			UpdateTimer();
		}

		
		// Methods
		public void SetTimer(Timer timer)
		{
			if (this.timer != null)
			{
				this.timer.ChangeEvent -= OnTimerChanged;
			}
			this.timer = timer;
			if (timer != null)
				timer.ChangeEvent += OnTimerChanged;
			UpdateTimer();
		}

		private void OnTimerChanged(Timer timer1)
		{
			UpdateTimer();
		}

		private void UpdateTimer()
		{
			if (timerView)
			{
				timerView.text = FormatTime(timer?.Seconds ?? 0);
			}
		}
		
		public static string FormatTime( int seconds ) {
			int hours = (int)(seconds / 3600);
			int minutes = (int)(seconds / 60);
			seconds -= minutes * 60 + hours * 3600;
			string result = string.Empty;
			if (hours > 0)
			{
				return $"{hours:00}:{minutes:00}:{seconds:00}";
			}
			return $"{minutes:00}:{seconds:00}"; 
		}

	}
}