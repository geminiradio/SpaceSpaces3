using UnityEngine;
using System.Collections;

public class PFXOnCollision : MonoBehaviour {

	public ParticleSystem myParticleSystem;
	public float minCollisionMag = 1f;
	public float normalCollsionMag = 5f;
	public float multDampener = 1f; // set this to 1 for no effect

	private float originalStartSpeed;

	void Start ()
	{
		if (myParticleSystem == null)
			Debug.LogError("myParticleSystem not assigned.");
		else
			originalStartSpeed = myParticleSystem.startSpeed;
	}


	void OnCollisionEnter (Collision collision)
	{
		Debug.Log("collided with relativeVelocity "+collision.relativeVelocity.magnitude);

		if (collision.relativeVelocity.magnitude > minCollisionMag)
		{
			float mult = (collision.relativeVelocity.magnitude / normalCollsionMag);
			float diff = mult - 1f;
			mult = 1f + (diff * multDampener);

			Debug.Log("mult = "+mult);

			myParticleSystem.startSpeed = originalStartSpeed * mult;
			Debug.Log("myParticleSystem.startSpeed = "+myParticleSystem.startSpeed);

			myParticleSystem.Play();
		}

	}
}
