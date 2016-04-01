using UnityEngine;
using System.Collections;

public class SFXOnCollide : MonoBehaviour {

	public AudioSource toPlay;
	public float minPitch = .75F;
	public float maxPitch = 1.5F;
	public float velToVol = .2F;

	private float dontRetriggerTime = 0.15f;
	private float lastTriggerTime=2f;   // if positive, no collision sounds will happen for that many seconds into the game

	void Start () {
	
		if (toPlay == null)
			Debug.LogError("toPlay sfx not assigned.");

	}


	void OnCollisionEnter (Collision coll)
	{
		if ((Time.time - lastTriggerTime) < dontRetriggerTime)
			return;

		lastTriggerTime = Time.time;

		toPlay.pitch = Random.Range (minPitch,maxPitch);
		float hitVol = coll.relativeVelocity.magnitude * velToVol;
		toPlay.PlayOneShot( toPlay.clip,hitVol);

	}

}
