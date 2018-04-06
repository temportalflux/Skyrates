using Skyrates.Scene;
using Skyrates.Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Skyrates.Entity
{
	class EntityFort : Entity
	{
		//TODO: Fill these in with better values, and better yet, have the designers fill this in.
		public float Health = 100.0f;
		public float Defense = 0.0f;
		public float Protection = 0.0f;
		public float CreditsDelay = 5.0f;
		public GameObject Render;

		public void OnTriggerEnter(Collider other)
		{
			//Debug.Log("Trigger entered fort.");
			this.TryCalculateDamage(other);
		}

		//TODO: Merge into common interface or parent.
		private void TryCalculateDamage(Collider other)
		{
			// Check to ensure that sources and their projectiles don't collide
			// downside is that enemies cannot be hit by any projectile that came from an enemy
			//Debug.Log(other.tag);
			//Currently, comparing tag is not necessary here, but we'll keep it because it'll happen anyway if/once we merge.
			if ((other.CompareTag("Projectile-Enemy") && this.CompareTag("Enemy")) ||
				(other.CompareTag("Projectile-Player") && this.CompareTag("Player")))
			{
				return;
			}

			float damage = 0;

			// Projectile calculations
			{
				EntityProjectile entityProjectile = other.GetComponent<EntityProjectile>();
				if (entityProjectile != null)
				{
					// Calculate the total damage
					damage = CalculateDamage(
						entityProjectile.GetAttack(),
						this.Defense,
						this.Protection
					);

					// Take the necessary damage
					Entity source = entityProjectile.Shooter.Owner != null ? entityProjectile.Shooter.Owner.Ship : null;
					this.TakeDamage(source ?? entityProjectile, damage);

					// Dispatch event for hit by projectile

					// collider is a projectile
					Destroy(entityProjectile.gameObject);
				}
			}

			// Figurehead calculations
			{
				ShipFigurehead figurehead = other.GetComponent<ShipFigurehead>();
				if (figurehead != null)
				{
					// Calculate the total damage
					damage = CalculateDamage(
						figurehead.GetAttack(),
						this.Defense,
						this.Protection
					);

					// If ram, and still have health, tell source that ram attack wasn't fully successful
					if (this.TakeDamage(figurehead.Ship, damage) > 0)
					{
						// We know that the health taken was our full amount because our health is not 0, so we don't need to recalculate the amount of damage we actually took.
						figurehead.Ship.OnRamUnsucessful(this, damage);
					}
					//TODO: Dispatch event
				}
			}
		}


		//TODO: Merge into common interface or parent.
		/// <summary>
		/// Calculates how much damage should be dealt.
		/// </summary>
		/// <param name="baseAtk">The amount of base damage the source says it should deal</param>
		/// <param name="defense">How much initial damage is taken away from the attack (damage = baseAtk - defense)</param>
		/// <param name="protection">How much of the damage should actually be taken (damage = protection * damage). Must be a number between 0.0 and 100.0</param>
		/// <returns></returns>
		private static float CalculateDamage(float baseAtk, float defense, float protection)
		{
			// TODO: Add penetration (attack multiplier, the opposite of protection) if wanted.
			// TODO: Add critical hit if wanted.

			// The more protection there is, the lower the percentage of total damage should be.
			// Therefore protection must be serialized from range of [0, 100] to [1, 0]
			// Clamp protection to range [0, 100] - ensure protection is in the expected range
			protection = Mathf.Clamp(protection, 0.0f, 100.0f);
			// Serialize the range from [0, 100] to [0, 1]
			protection /= 100.0f;
			// Reverse the range from [0, 1] to [1, 0]
			protection = 1.0f - protection;

			// Subtract defense from attack
			float damage = (baseAtk - defense);
			// Can't take less than 1 damage.
			damage = Mathf.Max(damage, 1.0f);
			// and multiply the total by the protection formula.
			damage *= protection;

			return damage;
		}

		//TODO: Merge into common interface or parent.
		/// <summary>
		/// Called to cause damage to the fort.
		/// </summary>
		/// <param name="source">The entity which is causing the damage.</param>
		/// <param name="damage">The amount of damage to take.</param>
		/// <returns>The amount of health remaining after the attack</returns>
		public virtual float TakeDamage(Entity source, float damage)
		{
			if(this.Health <= 0.0f) return 0.0f;

			// Remove the damage from the health
			this.Health -= damage;

			Debug.Log("Health left: " + this.Health.ToString());

			// If we still have health, stop execution here
			if (this.Health > 0.0f) return this.Health;

			//TODO: Particle effects

			// No more health, so try to destroy

			// Get whether or not this object should be destroyed
			if (this.OnPreDestroy())
			{
				//Win state!

				//Disable render
				if (Render) Render.SetActive(false);
				
				//Wait for it...
				StartCoroutine(DelayedStartCredits(CreditsDelay));
			}

			// Return that there is no health left - we are dead
			return 0.0f;
		}

		//TODO: Possibly merge into common interface or parent.
		/// <summary>
		/// Called right before the fort is destroyed via <see cref="TakeDamage(Entity, float)"/>
		/// </summary>
		/// <returns>True if the object can be destroyed</returns>
		protected virtual bool OnPreDestroy()
		{
			// TODO: Spawn particle, do preparations for win state.

			return true;
		}

		/// <summary>
		/// Starts the credits scene.
		/// </summary>
		//TODO: Possibly merge into common interface or parent.
		public void StartCredits()
		{
			SceneLoader.Instance.Enqueue(SceneData.SceneKey.Credits);
			SceneLoader.Instance.ActivateNext();
		}

		/// <summary>
		/// Calls StartCredits() on a timer.
		/// </summary>
		/// <param name="delay">The delay time in seconds.</param>
		public IEnumerator DelayedStartCredits(float delay)
		{
			yield return new WaitForSeconds(delay);
			if(this && this.isActiveAndEnabled) StartCredits();
		}

	}
}
