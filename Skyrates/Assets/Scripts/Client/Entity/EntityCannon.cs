using Skyrates.Client.Mono;
using System.Collections;
using UnityEngine;

namespace Skyrates.Client.Entity
{
	[RequireComponent(typeof(Shooter))] //No inheritance because we don't want to accidentally call FireProjectile when we actually meant to shoot.
	class EntityCannon : MonoBehaviour
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
		/// The <see cref="Shooter"/> used to fire projectiles.
		/// </summary>
		private Shooter _shooter;
		
		void Start()
		{
			this._shooter = GetComponent<Shooter>();
			//StartCoroutine(ShootEverySecond()); //For debugging purposes.
		}

		/*void Update()
		{
			Debug.DrawLine(_shooter.spawn.position, TargetPosition); //For debugging purposes.
		}*/

		/// <summary>
		///Function for testing that shoots periodically.
		/// </summary>
		IEnumerator ShootEverySecond()
		{
			while (this && gameObject)
			{
				CalculateArc();
				Shoot();
				yield return new WaitForSeconds(1);
			}

		}

		/// <summary>
		/// Automatically calculates and sets arc angle and axis based on gravity and distance over time (speed), in the forward direction. This does not take into account impulse force or deceleration/damping, so adjust accordingly.
		/// </summary>
		public void CalculateArc()
		{
			Vector3 distanceVector = (this.TargetPosition - this._shooter.spawn.position);
			Vector3 forward = (this.TargetPosition - this._shooter.spawn.position).normalized;
			Vector3 up = Vector3.up;
			Vector3 right = Vector3.Cross(forward, up);
			this.ArcAxis = right;
			float gravity = Physics.gravity.y;
			float speed = this.ShootSpeed;
			
			distanceVector.y = 0.0f;
			float xSqr = Mathf.Min(this.ShootSpeed * this.ShootSpeed, distanceVector.sqrMagnitude);
			float y = (this.TargetPosition.y - this._shooter.spawn.position.y);
			//Formula found here: https://gamedev.stackexchange.com/questions/17467/calculating-velocity-needed-to-hit-target-in-parabolic-arc by jonas
			float substitution = (speed * speed * speed * speed) -
				gravity * (gravity * (xSqr) + 2 * y * (speed * speed));
			this.ArcAngle = Mathf.Atan2(((speed * speed) + Mathf.Sqrt(substitution)), (gravity * Mathf.Sqrt(xSqr))) * Mathf.Rad2Deg - 90.0f; //Negate because positive Y is up, not negative. +90 because origin angle is 0, not +/-90.
			//TODO: See if there is some way to get rid of these square roots.;
			//TODO: Calculate velocity from impulse force at impact time and add it to the equation (add to velocity), if force makes enough of a difference and we want to calculate the difference in arc automatically.
			//TODO: Calculate velocity lost due to deceleration/damping at impact time and add it to the equation (add to velocity), if it makes enough of a difference, etc.

		}

		/// <summary>
		/// Fires one projectile from this cannon
		/// </summary>
		public void Shoot()
		{
			Vector3 direction = Quaternion.AngleAxis(this.ArcAngle, this.ArcAxis) * (this.TargetPosition - this._shooter.spawn.position).normalized;
			this._shooter.spawn.rotation = Quaternion.LookRotation(direction);
			this._shooter.FireProjectile(direction, this.ShootSpeed * direction);
		}
	}
}
