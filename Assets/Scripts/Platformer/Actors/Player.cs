using UnityEngine;

namespace TestGame.Platformer.Actors
{
    public class Player : Actor
    {
        protected override void Update()
        {
            if (isPause)
            {
                return;
            }
            if (activeSensors.Count > 0)
            {
                if (IsTouch())
                {
                    DoAction();
                }
            }
            base.Update();
        }
        
        protected bool IsTouch()
        {
            return Input.GetMouseButtonDown(0);
        }

    }
}