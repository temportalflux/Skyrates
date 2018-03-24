using Skyrates.Effects;
using Skyrates.Entity;
using Skyrates.Game;
using Skyrates.Game.Event;
using System.Collections;
using UnityEngine;

namespace Skyrates.Mono
{
    
    /// <summary>
    /// Special component which dispatches the <see cref="EventSpawnEntityProjectile"/> game event.
    /// </summary>
    public class Shooter : MonoBehaviour
    {

        /// <summary>
        /// The prefab of the projectile to launch from this source.
        /// </summary>
        [SerializeField]
        public Entity.Entity projectilePrefab;

        /// <summary>
        /// The spawn location of the projectile.
        /// </summary>
        public Transform spawn;

        /// <summary>
        /// How much force to apply to the projectile.  If UseArc is enabled, this becomes the initial velocity.
        /// </summary>
		[Tooltip("How much force to apply to the projectile.  If UseArc is enabled, this becomes the initial velocity.")]
		public float force = 1;

		/// <summary>
		/// Should we use gravity in our calculations?
		/// </summary>
		public bool UseArc = false;

		/// <summary>
		/// Should we use CalculateArc or manually generate an arc?
		/// </summary>
		public bool ManualArc = false;

		/// <summary>
		/// Is target a direction?  (Temporary?  Used for arc calculation.)
		/// </summary>
		[Tooltip("Ignored when not using an arc.")]
		public bool TargetIsDirection = true;

		/// <summary>
		/// The amount of extra tilt in degrees needed in a direction to make the target.
		/// </summary>
		public float ArcAngle; //This can be entered manually or automatically calculated using gravity and distance over time (velocity).  Convert the angle needed into a direction vector.

		/// <summary>
		/// The axis to tilt.  Usually the positive X axis (right) relative to the player.
		/// </summary>
		public Vector3 ArcAxis = Vector3.right;

		/// <summary>
		/// Arc renderer.
		/// </summary>
		public ArcRender ArcRender; //TODO: Move this somewhere that makes more sense?

		public void Start()
		{
			if (this.ArcRender)
			{
				CalculateArc(transform.forward, this.force, true); //TEMPORARY;
				StartCoroutine(UpdateArc());
			}
		}

		public IEnumerator UpdateArc()
		{
			while (this && this.spawn && this.ArcRender)
			{
				float oldVelocity = this.ArcRender.Velocity;
				float oldAngle = this.ArcRender.AngleDegrees;
				this.ArcRender.Velocity = this.force;
				this.ArcRender.AngleDegrees = this.ArcAngle;
				if(this.ArcRender.AngleDegrees != oldAngle || this.ArcRender.Velocity != oldVelocity) this.ArcRender.RecalculateMesh(this.ArcRender.Velocity, this.ArcRender.AngleDegrees * Mathf.Deg2Rad);
				yield return null;
			}
		}

		/// <summary>
		/// Automatically calculates and sets arc angle and axis based on gravity and distance
		/// over time (speed/velocity), in the forward direction.
		/// This does not take into account impulse force or deceleration/damping.
		/// </summary>
		public void CalculateArc(Vector3 target, float speed, bool targetIsDirection)
		{
			Vector3 distanceVector = targetIsDirection ? target * speed : (target - this.spawn.position);
			Vector3 forward = targetIsDirection ? target : (target - this.spawn.position).normalized;
			Vector3 up = Vector3.up;
			Vector3 right = Vector3.Cross(forward, up);
			this.ArcAxis = right;
			float gravity = UnityEngine.Physics.gravity.y;
			
			distanceVector.y = 0.0f;
			float speedSqr = speed * speed;
			float xSqr = Mathf.Min(speedSqr, distanceVector.sqrMagnitude);
			float y = targetIsDirection ? target.y : (target.y - this.spawn.position.y);
			//Formula found here: https://gamedev.stackexchange.com/questions/17467/calculating-velocity-needed-to-hit-target-in-parabolic-arc by jonas
			if (y < 0.0f) y = 0.0f;
			float substitution = (speedSqr * speedSqr) -
				gravity * (gravity * (xSqr) + 2 * y * (speedSqr));
			this.ArcAngle = Mathf.Atan2(((speedSqr) + Mathf.Sqrt(substitution)), (gravity * Mathf.Sqrt(xSqr))) * Mathf.Rad2Deg - 90.0f; //Negate because positive Y is up, not negative. +90 because origin angle is 0, not +/-90.
			//TODO: See if there is some way to get rid of these square roots.;
			//TODO: Calculate velocity from impulse force at impact time and add it to the equation (add to velocity), if force makes enough of a difference and we want to calculate the difference in arc automatically.
			//TODO: Calculate velocity lost due to deceleration/damping at impact time and add it to the equation (add to velocity), if it makes enough of a difference, etc.
		}

        /// <summary>
        /// Returns the direction the shooter is facing.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetProjectileDirection()
        {
            return this.spawn.forward;
        }

        /// <summary>
        /// Fires off <see cref="EventSpawnEntityProjectile"/> with the spawn location, the projectile to fire, and the initial velocity.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="launchVelocity"></param>
        public void FireProjectile(Vector3 target, Vector3 launchVelocity)
        {
            // TODO: These are fired off one by one, and are often done in batches. This should just be one packet of all the projectiles to spawn.
            Entity.Entity entity = GameManager.Instance.SpawnEntity(this.projectilePrefab);
            if (entity != null)
            {
                EntityProjectile projectile = (EntityProjectile)entity;
				if (this.UseArc)
				{
					Vector3 targetDirection;
					if(this.TargetIsDirection) targetDirection = target;
					else targetDirection = (target - this.spawn.position);
					if (!this.ManualArc) CalculateArc(target, this.force, this.TargetIsDirection); //Unfortunately, launchVelocity can't be accomodated for (not without some expensive looping).
					Vector3 direction = Quaternion.AngleAxis(this.ArcAngle, this.ArcAxis) * targetDirection;
					projectile.Launch(this.spawn.position, this.spawn.rotation, this.force * direction + launchVelocity, Vector3.zero);
				}
				else
				{
					if (!Mathf.Approximately(target.sqrMagnitude, 1.0f)) target.Normalize(); //If target is not a direction vector, make it one.
					projectile.Launch(this.spawn.position, this.spawn.rotation, launchVelocity, target * this.force);
				}
            }
        }
	}
}