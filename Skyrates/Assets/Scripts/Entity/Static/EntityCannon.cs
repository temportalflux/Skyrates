using System.Collections;
using Skyrates.AI;
using Skyrates.Mono;
using Skyrates.Physics;
using UnityEngine;

namespace Skyrates.Entity
{

    //No inheritance because we don't want to accidentally call FireProjectile when we actually meant to shoot.
    [RequireComponent(typeof(Shooter))]
	public class EntityCannon : Entity, IDistanceCollidable
	{
		/// <summary>
		/// The relative speed to shoot at.
		/// </summary>
		public float ShootSpeed; //We use speed instead of velocity to simplify shooting.

		/// <summary>
		/// The target position to shoot at.
		/// </summary>
		public Vector3 TargetPosition;

        /// <summary>
        /// The number of seconds per shot fired.
        /// </summary>
	    public float RateOfFire;

		/// <summary>
		/// The <see cref="Mono.Shooter"/> used to fire projectiles.
		/// </summary>
		[HideInInspector]
		public Shooter Shooter;

        /// <summary>
        /// Used to move the cannon over time to look at the target
        /// </summary>
	    public Behavior RotationBehavior;

	    public GameObject CannonVerticalComponent;

	    private Behavior.DataBehavioral _dataBehavioral;
	    private Behavior.DataPersistent _dataPersistent;
	    private PhysicsData _physicsData;
	    private Coroutine _followAndFireAtRoutine;


        private Quaternion Rotation
	    {
	        get
	        {
	            return Quaternion.Euler(
	                this.CannonVerticalComponent.transform.rotation.x,
	                this.transform.rotation.y, 0.0f
	            );
	        }
	        set
	        {
	            this.transform.rotation = Quaternion.Euler(value.eulerAngles.x, value.eulerAngles.y, 0.0f);
	            this.CannonVerticalComponent.transform.rotation = Quaternion.Euler(value.eulerAngles.x, 0.0f, 0.0f);
	        }
	    }

	    protected override void Start()
        {
            base.Start();
            this._dataBehavioral = new Behavior.DataBehavioral();
            this._dataPersistent = new Behavior.DataPersistent();
            this._physicsData = new PhysicsData();
            this._followAndFireAtRoutine = null;

            this.Shooter = GetComponent<Shooter>();
			
            //StartCoroutine(ShootEverySecond()); //For debugging purposes.
		}

	    public void OnEnterEntityRadius(EntityAI source, float radius)
	    {
	        if (source is EntityPlayerShip)
	        {
                // Only fire at the first who entered the radius
	            if (this._followAndFireAtRoutine == null)
	            {
	                this._followAndFireAtRoutine = StartCoroutine(this.FollowAndFireAt(source, radius));
                }
            }
	    }

	    public void OnOverlapWith(GameObject other, float radius)
	    {
	    }

	    IEnumerator FollowAndFireAt(EntityAI target, float maxDistance)
	    {
            // Get the total distance that the target is allowed to be away from
	        float maxDistSq = maxDistance * maxDistance;

            // Initalize physics data
            {
	            this._physicsData.LinearPosition = this.transform.position;
	            this._physicsData.LinearVelocity = Vector3.zero;
                this._physicsData.LinearAccelleration = Vector3.zero;
	            this._physicsData.RotationPosition = this.Rotation;
                this._physicsData.RotationVelocity = Vector3.zero;
                this._physicsData.RotationAccelleration = Vector3.zero;
                this._physicsData.HasAesteticRotation = false;
                this._physicsData.RotationAesteticPosition = Quaternion.identity;
            }
            // Initalize the behavior data
	        {
	            this._dataBehavioral.View = this.transform;
	            this._dataBehavioral.Render = this.transform;
	            this._dataBehavioral.HasTarget = true;
	        }

	        float timePrevious = Time.time;
            // the amount of time since last fire projectile
	        float timeElapsed = 0.0f;

            while (target && (target.transform.position - this.transform.position).sqrMagnitude <= maxDistSq)
            {
                float deltaTime = Time.time - timePrevious;
                timePrevious = Time.time;

                // Update the behavior data
                {
                    this._dataBehavioral.Target = target.PhysicsData.Copy();

                    // Determine the angle and rotation we want to be at to get a good shot
                    //this.CalculateArc(this.BehaviorData.Target.LinearPosition, this.ShootSpeed);

                }
                // Get the updated physics from the behavior
	            {
	                this._dataPersistent = this.RotationBehavior.GetUpdate(ref this._physicsData, ref this._dataBehavioral, this._dataPersistent, deltaTime);
	            }
                // Rotate the cannon to its destination
	            {
	                this._physicsData.Integrate(deltaTime);
	                this.Rotation = this._physicsData.RotationPosition;
	            }
                // Fire at the target, if enough time has passed
                {
                    timeElapsed += deltaTime;
                    if (timeElapsed >= this.RateOfFire)
                    {
                        timeElapsed -= this.RateOfFire;
                        //Debug.Log("FIRE");
                        this.Shooter.FireProjectile(
                             (target.transform.position - this.transform.position).normalized,
                             Vector3.zero, 1
                        );
                    }
                }

	            yield return null;
	        }

	        this._dataBehavioral.HasTarget = false;
            
            /*
            // Return cannon to original orientation (originRotation)
	        timePrevious = Time.time;
            // the amount of time progressed in the quaternion lerp (to return cannon to original rotation)
	        timeElapsed = 0.0f;
	        Quaternion startRot = this.PhysicsData.RotationPosition;
            // MAGIC: cannons should return to their original rotation within 2 seconds
	        float maxRotationTime = 2.0f;
	        float maxRotationTimeInv = 1 / maxRotationTime;
            while (timeElapsed <= 1.0f)
            {
                float deltaTime = Time.time - timePrevious;
                timePrevious = Time.time;
                timeElapsed = Mathf.Min(1.0f, timeElapsed + deltaTime);
                this.Rotation = Quaternion.Lerp(startRot, this.originalRotation, timeElapsed * maxRotationTimeInv);
            }
            */

            // done launching at some target, who is now out of range
	        this._followAndFireAtRoutine = null;
	    }


        /// <summary>
        ///Function for testing that shoots periodically.
        /// </summary>
        IEnumerator ShootEverySecond()
		{
			while (this && gameObject)
			{
				//this._shooter.ManualArc = false;
				//Shoot(this.TargetPosition, this.ShootSpeed);
				yield return new WaitForSeconds(1);
			}

		}

		/// <summary>
		/// Fires one projectile from this cannon.
		/// </summary>
		public void Shoot(Vector3 target, float speed)
		{
			this.Shooter.UseArc = true;
			this.Shooter.FireProjectile(target, Vector3.zero, 1);
		}

	}
}
