using UnityEngine;

namespace TestGame.Platformer.Actors
{
    public class Bot : Actor
    {
        private float time = 0;
        private float period = 1f;
        private int wallJumpCounter = 0;
        
        protected override void Update()
        {
            if (isPause)
            {
                return;
            }
            time += Time.deltaTime;
            if (time > period)
            {
                time -= period;
                if (wallJumpCounter < 6)
                {
                    if (DoAction())
                    {
                        ++wallJumpCounter;       
                    }                    
                }
                if (isActiveBottomSensor)
                    wallJumpCounter = 0;
                period = Random.Range(0.2f, 1.5f);
            }
            base.Update();
        }

    }
}