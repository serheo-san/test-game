using System.Collections.Generic;
using Engine;
using UnityEngine;

namespace TestGame.Platformer.Actors
{
    public class Actor : MonoBehaviour
    {
        public delegate void EventHandler(Actor actor);
        
        [SerializeField]
        protected Rigidbody2D rigidBody2D = default;

        [SerializeField]
        protected float jumpForce = 30f;

        [SerializeField]
        protected float speed = 1f;
        
        [SerializeField]
        protected float pressSpeed = .5f;

        [SerializeField]
        protected TrailRenderer trailRenderer = default;

        protected ActorState state = ActorState.None;
        
        protected int moveDirectionX = 1;
        protected float moveSpeed = 0f;
        protected Vector2 currentDirection;
        protected Vector2 prevDirection;
        protected Vector2 prevPosition;

        protected Vector2 jumpPositon = default;
        protected bool noWallPress = false;
        protected bool isActiveBottomSensor = false;
        protected bool isActiveWallSensor = false;
        protected bool isActiveAdditionWallSensor = false;
    
        protected HashSet<ActorSensor> activeSensors = new HashSet<ActorSensor>();
        protected ActorSensor[] sensors;
        protected bool isPause = false;
        protected Vector2 pauseStoreVelocity;
        
        
        
        public bool IsMove => state!=ActorState.Stop;

        public event EventHandler BottomSensorEvent; 
        public event EventHandler JumpEvent;

        protected void Awake()
        {
            sensors = GetComponentsInChildren<ActorSensor>();
            foreach (var sensor in sensors)
            {
                sensor.ActivationEvent += OnActivationSensor;
                sensor.DeactivationEvent += OnDeactivationSensor;
            }
            IoC.AddInstance(this);
        }

        protected void OnDestroy()
        {
            if (sensors != null)
            {
                foreach (var sensor in sensors)
                {
                    sensor.ActivationEvent -= OnActivationSensor;
                    sensor.DeactivationEvent -= OnDeactivationSensor;
                }
                sensors = null;
            }
            IoC.RemoveInstance(this);
        }

        protected void Start()
        {
            prevPosition = transform.position;
        }

        protected virtual void Update()
        {
            if (isPause)
            {
                return;
            }
            prevDirection = currentDirection;
            currentDirection = (Vector2)transform.position - prevPosition;
            prevPosition = transform.position;
            if (currentDirection.y * prevDirection.y < 0f)
            {
                OnDirectionYChanged();
            }            
            if (rigidBody2D)
            {
                var velocity = rigidBody2D.velocity;
                velocity.x = moveDirectionX * moveSpeed;
                rigidBody2D.velocity = velocity;
            }
            if (isActiveAdditionWallSensor && !isActiveWallSensor)
            {
                switch (state)
                {
                    case ActorState.FreeJump:
                    case ActorState.SlideOnWall:
                    case ActorState.Move:
                        Fall();
                        break;
                }
            }
        }

        public void Pause()
        {
            if (isPause)
            {
                return;
            }
            isPause = true;
            if (rigidBody2D)
            {
                pauseStoreVelocity = rigidBody2D.velocity;
                rigidBody2D.simulated = false;
            }
        }

        public void Continue()
        {
            if (!isPause)
            {
                return;
            }
            isPause = false;
            if (rigidBody2D)
            {
                rigidBody2D.simulated = true;
                rigidBody2D.velocity = pauseStoreVelocity;
            }
        }

        protected bool DoAction()
        {
            if (activeSensors.Count==0)
            {
                return false;
            }
            if (state == ActorState.JumpUp || state==ActorState.SlideOnWall)
            {
                state = ActorState.FreeJump;
                moveDirectionX = moveDirectionX > 0 ? -1 : 1;
                var position = transform.position;
                position.x += moveDirectionX * 0.02f;
                transform.position = position;
                moveSpeed = speed;
                JumpUp();
                return true;
            }
            if (state == ActorState.Stop)
            {
                state = ActorState.JumpUp;
                moveSpeed = 0;
                JumpUp();
                return true;
            }
            if (state == ActorState.Move)
            {
                state = ActorState.FreeJump;
                JumpUp();
                return true;
            }
            return false;
        }
        
        protected void OnDirectionYChanged()
        {
            if (!isActiveBottomSensor && state==ActorState.JumpUp && currentDirection.y<0f)
            {
                moveSpeed = pressSpeed;
            }
        }

        protected void OnActivationSensor(ActorSensor sensor)
        {
            if (!activeSensors.Add(sensor))
            {
                return;
            }
            CheckActivatedFloorSensor(sensor);
            CheckActivatedWallSensors(sensor);
            Debug.Log($"Activate sensor='{sensor.SensorType}' count={activeSensors.Count}");
        }
        
        protected void OnDeactivationSensor(ActorSensor sensor)
        {
            if (!activeSensors.Remove(sensor))
            {
                return;
            }
            CheckDeactivatedWallSensors(sensor);
            CheckDeactivateFloorSensor(sensor);

            Debug.Log($"Deactivate sensor='{sensor.SensorType}' count={activeSensors.Count}");
        }
        
        public void MoveRight()
        {
            moveSpeed = speed;
            state = ActorState.Move;
            moveDirectionX = 1;
            rigidBody2D.simulated = true;
            if (trailRenderer)
                trailRenderer.enabled = true;
        }

        public void Freeze()
        {
            moveSpeed = 0;
            state = ActorState.None;
            moveDirectionX = 1;
            rigidBody2D.velocity = Vector2.zero;
            rigidBody2D.simulated = false;
            if (trailRenderer)
                trailRenderer.enabled = false;

        }


        protected void JumpUp()
        {
            if (rigidBody2D)
            {
                rigidBody2D.velocity = Vector2.zero;
                rigidBody2D.AddRelativeForce(Vector2.up * jumpForce);
            }
            JumpEvent?.Invoke(this);
        }

        protected void Fall()
        {
            var velocity = rigidBody2D.velocity;
            velocity.x = 0;
            rigidBody2D.velocity = velocity;
            moveSpeed = 0f;
            state = ActorState.Falling;
        }

        protected void Stop()
        {
            
        }
        

        protected void CheckActivatedWallSensors(ActorSensor sensor)
        {
            switch (sensor.SensorType)
            {
                case ActorSensorType.Left:
                case ActorSensorType.Right:
                {
                    if (isActiveBottomSensor)
                    {
                        state = ActorState.Stop;
                        moveSpeed = pressSpeed;
                    }
                    else
                    {
                        state = ActorState.SlideOnWall;
                        moveSpeed = pressSpeed;
                        rigidBody2D.velocity = Vector2.zero;
                    }
                    isActiveWallSensor = true;
                    Debug.DrawLine(sensor.transform.position, transform.position, Color.blue, 2f);
                    break;
                }
                case ActorSensorType.LeftBottom:
                case ActorSensorType.LeftTop:
                case ActorSensorType.RightBottom:
                case ActorSensorType.RightTop:
                {
                    isActiveAdditionWallSensor = true;
                    if (state == ActorState.FreeJump && !isActiveWallSensor && !isActiveBottomSensor)
                    {
                        Fall();
                    }
                    break;
                }
            }
            //Debug.DrawLine(contact.point, transform.position, Color.green, 3f);
        }
        
        protected void CheckDeactivatedWallSensors(ActorSensor sensor)
        {
            switch (sensor.SensorType)
            {
                case ActorSensorType.Left:
                case ActorSensorType.Right:
                {
                    if (state == ActorState.SlideOnWall)
                    {
                        Fall();
                    }
                    isActiveWallSensor = false;
                    break;
                }
                case ActorSensorType.LeftBottom:
                case ActorSensorType.LeftTop:
                case ActorSensorType.RightBottom:
                case ActorSensorType.RightTop:
                {
                    isActiveAdditionWallSensor = false;
                    break;
                }
            }
            //Debug.DrawLine(contact.point, transform.position, Color.green, 3f);
        }


        protected void CheckActivatedFloorSensor(ActorSensor sensor)
        {
            if (sensor.SensorType == ActorSensorType.Bottom)
            {
                isActiveBottomSensor = true;
                switch (state)
                {
                    case ActorState.JumpUp:
                    case ActorState.SlideOnWall:
                        state = ActorState.Stop;
                        BottomSensorEvent?.Invoke(this);
                        break;
                    case ActorState.Falling:
                        if (activeSensors.Count >= 2)
                        {
                            state = ActorState.Stop;
                        }
                        else
                        {
                            state = ActorState.Move;
                            moveSpeed = speed;
                        }
                        BottomSensorEvent?.Invoke(this);
                        break;
                    case ActorState.FreeJump:
                        state = ActorState.Move;
                        moveSpeed = speed;
                        BottomSensorEvent?.Invoke(this);
                        break;
                }
            }
        }

        private void CheckDeactivateFloorSensor(ActorSensor sensor)
        {
            switch (sensor.SensorType)
            {
                case ActorSensorType.Bottom:
                {
                    isActiveBottomSensor = false;
                    break;
                }
            }
        }
    }
}