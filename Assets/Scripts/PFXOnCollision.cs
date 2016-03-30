using UnityEngine;
using System.Collections;

public class PFXOnCollision : MonoBehaviour {

	public ParticleSystem myParticleSystem;
	public GameObject mustCollideWith;   // if not null, must collide with this to produce a particle fx
	public bool reuse = false; // if this is false, clone myParticleSystem rather than reuse it (ie - false = it's a prefab) 
	public float minCollisionMag = 1f;  // ignore collisions below this
	public float normalCollsionMag = 5f;
	public float multDampener = 1f; // set this to 1 for no effect
	public float multScale = 1f;  // this allows the user to scale the size of the pfx after all calculations

	private float veryEarlyInTheGame = 2f; // ignore collisions that happen this many seconds in the beginning of the game

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
		if (Time.time < veryEarlyInTheGame)
			return;

		if ( (mustCollideWith != null) && (collision.collider.gameObject != mustCollideWith) )
		{
			Debug.Log("collided with the wrong thing, specifically: "+collision.collider.gameObject);
			return;
		}

		Debug.Log("collided with relativeVelocity "+collision.relativeVelocity.magnitude);

		if (collision.relativeVelocity.magnitude > minCollisionMag)
		{
			float mult = (collision.relativeVelocity.magnitude / normalCollsionMag);
			float diff = mult - 1f;
			mult = 1f + (diff * multDampener);

			Debug.Log("dampened mult = "+mult);

			float newStartSpeed = originalStartSpeed * mult * multScale;
			Debug.Log("newStartSpeed = "+newStartSpeed);

			if (reuse)
			{
				myParticleSystem.startSpeed = newStartSpeed;
				myParticleSystem.Play();
			}
			else
			{
				Quaternion rot = myParticleSystem.transform.rotation;
				ParticleSystem newPart = (ParticleSystem) Instantiate(myParticleSystem, collision.contacts[0].point, rot) as ParticleSystem;
				newPart.transform.parent = null;
				newPart.startSpeed = newStartSpeed;
				newPart.Play();
			}
		}

	}
}
