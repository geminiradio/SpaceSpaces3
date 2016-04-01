using UnityEngine;
using System.Collections;

public class FlyingPlayer : MonoBehaviour {

	public bool trigger;
	public Animator animToStart;
	public AudioSource toFadeOut;
	public AudioSource toFadeIn;
	public float fadeInTargetVolume = 0.05f;
	public float fadeTime = 10f;

	private bool active = false;
	private Interpolator2D interp;

	// Use this for initialization
	void Start () {
	
		interp = new Interpolator2D();
	}
	
	// Update is called once per frame
	void Update () {
	
		if (trigger)
		{
			trigger = false;
			active = true;

			animToStart.SetTrigger("TrigFlight");

			float fadeOutInitialVolume = toFadeOut.volume;

			// TODO this is a 2d interp being used as two 1D interps
			interp.Initialize(new Vector2(0f,fadeOutInitialVolume), new Vector2(fadeInTargetVolume,0f), fadeTime);

		}

		if (active)
		{
			Vector2 values = interp.Update();

			toFadeIn.volume = values.x;
			toFadeOut.volume = values.y;

			if (interp.complete)
				active = false;
		}
			
	}
}
