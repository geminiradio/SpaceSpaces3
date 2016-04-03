using UnityEngine;
using System.Collections;

public class FlyingPlayer : MonoBehaviour {

	public bool trigger; // interface for other scripts
    public bool onlyTrigerOnce = true;
    private bool everBeenTriggered = false;
	public Animator animToStart;
	public AudioSource toFadeOut1;
    public AudioSource toFadeOut2;
    public AudioSource toFadeIn;
	public float fadeInTargetVolume = 0.05f;
	public float fadeTime = 10f;

    public GameObject[] turnOffInTree;
    public Animator animToTriggerInTree;

	private bool active = false;
	private Interpolator2D interp;
    private float fadeOutInitialVolume1, fadeOutInitialVolume2;

    // Use this for initialization
    void Start () {
	
		interp = new Interpolator2D();
	}
	
	// Update is called once per frame
	void Update () {
	
		if (trigger)
		{
			trigger = false;

            if ((!onlyTrigerOnce) || (!everBeenTriggered))
            {
                active = true;

                animToStart.SetTrigger("TrigFlight");

                fadeOutInitialVolume1 = toFadeOut1.volume;
                fadeOutInitialVolume2 = toFadeOut2.volume;

                // TODO this is a 2d interp being used as two 1D interps
                interp.Initialize(new Vector2(0f, 1f), new Vector2(fadeInTargetVolume, 0f), fadeTime);
            }

            everBeenTriggered = true;


        }

        if (active)
		{
			Vector2 values = interp.Update();

			toFadeIn.volume = values.x;
			toFadeOut1.volume = values.y * fadeOutInitialVolume1;
            toFadeOut2.volume = values.y * fadeOutInitialVolume2;

            if (interp.complete)
				active = false;
		}
			
	}

    public void TurnOffThingsWhenInTree()
    {
        foreach (GameObject go in turnOffInTree)
            go.SetActive(false);

        animToTriggerInTree.SetTrigger("Patrol");
    }
}
