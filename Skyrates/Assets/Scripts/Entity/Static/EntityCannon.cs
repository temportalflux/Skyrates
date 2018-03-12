using Skyrates.Client.Mono;
using System.Collections;
using Skyrates.Common.AI;
using Skyrates.Common.Entity;
using UnityEngine;

namespace Skyrates.Client.Entity
{

    //No inheritance because we don't want to accidentally call FireProjectile when we actually meant to shoot.
    [RequireComponent(typeof(Shooter))]
	public class EntityCannon : Common.Entity.Entity, DistanceCollidable
	{
		/// <summary>
		/// The amount of extra tilt in degrees needed in a direction to make the target.
		/// </summary>
		public float ArcAngle; //This can be entered manually or automatically calculated using gravity and distance over time (velocity).  Convert the angle needed into a direction vector.

		/// <summary>
		/// The axis to tilt.  Usually the positive X axis (right) relative to the player.
		/// </summary>
		public Vector3 ArcAxis = Vector3.right;

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
		/// The <see cref="Shooter"/> used to fire projectiles.
		/// </summary>
		private Shooter _shooter;

        /// <summary>
        /// Used to move the cannon over time to look at the target
        /// </summary>
	    public Behavior RotationBehavior;

	    public GameObject CannonVerticalComponent;

	    private BehaviorData BehaviorData;
	    private PhysicsData PhysicsData;
	    private Coroutine FollowAndFireAtRoutine;


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
            this.BehaviorData = new BehaviorData();
            this.PhysicsData = new PhysicsData();
            this.FollowAndFireAtRoutine = null;

            this._shooter = GetComponent<Shooter>();
			
            //StartCoroutine(ShootEverySecond()); //For debugging purposes.
		}

	    public void OnEnterEntityRadius(EntityAI source, float radius)
	    {
	        if (source is EntityPlayerShip)
	        {
                // Only fire at the first who entered the radius
	            if (this.FollowAndFireAtRoutine == null)
	            {
	                this.FollowAndFireAtRoutine = StartCoroutine(this.FollowAndFireAt(source, radius));
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
	            this.PhysicsData.LinearPosition = this.transform.position;
	            this.PhysicsData.LinearVelocity = Vector3.zero;
                this.PhysicsData.LinearAccelleration = Vector3.zero;
	            this.PhysicsData.RotationPosition = this.Rotation;
                this.PhysicsData.RotationVelocity = Vector3.zero;
                this.PhysicsData.RotationAccelleration = Vector3.zero;
                this.PhysicsData.HasAesteticRotation = false;
                this.PhysicsData.RotationAesteticPosition = Quaternion.identity;
            }
            // Initalize the behavior data
	        {
	            this.BehaviorData.View = this.transform;
	            this.BehaviorData.Render = this.transform;
	            this.BehaviorData.HasTarget = true;
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
                    this.BehaviorData.Target = target.Physics.Copy();

                    // Determine the angle and rotation we want to be at to get a good shot
                    //this.CalculateArc(this.BehaviorData.Target.LinearPosition, this.ShootSpeed);

                }
                // Get the updated physics from the behavior
	            {
	                this.RotationBehavior.GetUpdate(ref this.BehaviorData, ref this.PhysicsData, deltaTime);
	            }
                // Rotate the cannon to its destination
	            {
	                this.PhysicsData.Integrate(deltaTime);
	                this.Rotation = this.PhysicsData.RotationPosition;
	            }
                // Fire at the target, if enough time has passed
                {
                    timeElapsed += deltaTime;
                    if (timeElapsed >= this.RateOfFire)
                    {
                        timeElapsed -= this.RateOfFire;
                        //Debug.Log("FIRE");
                        this._shooter.FireProjectile(
                             (target.transform.position - this.transform.position).normalized,
                             Vector3.zero
                        );
                    }
                }

	            yield return null;
	        }

	        this.BehaviorData.HasTarget = false;
            
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
	        this.FollowAndFireAtRoutine = null;
	    }


        /// <summary>
        ///Function for testing that shoots periodically.
        /// </summary>
        IEnumerator ShootEverySecond()
		{
			while (this && gameObject)
			{
				//CalculateArc(this.TargetPosition, this.ShootSpeed);
				//Shoot(this.ArcAngle, this.ArcAxis, this.TargetPosition, this.ShootSpeed);
				yield return new WaitForSeconds(1);
			}

		}

		/// <summary>
		/// Automatically calculates and sets arc angle and axis based on gravity and distance
		/// over time (speed/velocity), in the forward direction.
		/// This does not take into account impulse force or deceleration/damping.
		/// </summary>
		public void CalculateArc(Vector3 target, float speed)
		{
			Vector3 distanceVector = (target - this._shooter.spawn.position);
			Vector3 forward = (target - this._shooter.spawn.position).normalized;
			Vector3 up = Vector3.up;
			Vector3 right = Vector3.Cross(forward, up);
			this.ArcAxis = right;
			float gravity = Physics.gravity.y;
			
			distanceVector.y = 0.0f;
			float xSqr = Mathf.Min(this.ShootSpeed * this.ShootSpeed, distanceVector.sqrMagnitude);
			float y = (target.y - this._shooter.spawn.position.y);
			//Formula found here: https://gamedev.stackexchange.com/questions/17467/calculating-velocity-needed-to-hit-target-in-parabolic-arc by jonas
			float substitution = (speed * speed * speed * speed) -
				gravity * (gravity * (xSqr) + 2 * y * (speed * speed));
			this.ArcAngle = Mathf.Atan2(((speed * speed) + Mathf.Sqrt(substitution)), (gravity * Mathf.Sqrt(xSqr))) * Mathf.Rad2Deg - 90.0f; //Negate because positive Y is up, not negative. +90 because origin angle is 0, not +/-90.
			//TODO: See if there is some way to get rid of these square roots.;
			//TODO: Calculate velocity from impulse force at impact time and add it to the equation (add to velocity), if force makes enough of a difference and we want to calculate the difference in arc automatically.
			//TODO: Calculate velocity lost due to deceleration/damping at impact time and add it to the equation (add to velocity), if it makes enough of a difference, etc.

		}

		/// <summary>
		/// Fires one projectile from this cannon.
		/// </summary>
		public void Shoot(float arcAngle, Vector3 arcAxis, Vector3 target, float speed)
		{
			Vector3 direction = Quaternion.AngleAxis(arcAngle, arcAxis) * (target - this._shooter.spawn.position).normalized;
			this._shooter.spawn.rotation = Quaternion.LookRotation(direction);
			this._shooter.FireProjectile(direction, speed * direction);
		}

	}
}
