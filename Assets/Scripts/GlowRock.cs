using UnityEngine;
using System.Collections;



public class GlowRock : MonoBehaviour {

	public bool triggered = false;
	private bool everTriggered = false;
	public GlowRock manager;

	// this set only for manager
	public GlowRock[] allRocks;  // only for manager, list of all rocks
	private bool allRocksTriggered = false;  // only for manager
	public float allRocksTriggeredDuration = 60f; // only for manager
	private float untriggerAllRocksTime; // only for manager

	public Renderer myRend;
	public Light myLight;
	public float myLightHeight = 0.2f;  // how high above the rock should the light be?

	public float emisLow = 0.08f;
	public float emisHigh = 1f;
	public float lightIntensityLow = 0.01f;
	public float lightIntensityHigh = 1.62f;

	public float brightenDuration = 0.2f;  // stage 1 if externally-triggered (eg - by player)
	public float brightenDurationAuto = 2.5f;  // stage 1 if auto-triggered
	public float glowDuration = 6f;   // stage 2
	public float darkenDuration = 7f;   // stage 3

	private float minMagToTrigger = 5f; // magnitude of relative velocity of collision

	private enum GlowStage : int {
		Inactive,
		Brighten,
		Glowing,
		Darken
	}

	private GlowStage glowStage = GlowStage.Inactive;
	private Interpolator2D interp_emis, interp_light;
	private float glowingStartTime;


	public bool autoTrigEnabled = false;
	public float autoTrigMinTime = 10f;
	public float autoTrigMaxTime = 75f;
	private float nextAutoTrigger;


	void Start () {
	
		if (myRend == null)
			Debug.LogError(this+" material not assigned!");

		if (myLight == null)
			Debug.LogError(this+" light not assigned!");

		if (glowDuration < 0)
			Debug.LogError(this+" glowDuration not assigned correctly!");

		if (manager == null)
			Debug.LogError(this+" manager not assigned correctly!");

		if ((manager == this) && (allRocks.Length <= 0))
			Debug.LogError(this+ " is the manager, but allRocks array not assigned correctly!");

		interp_emis = new Interpolator2D();
		interp_light = new Interpolator2D();

		myRend.material.EnableKeyword("_EmissionColor");
	}


	void Update () {

		// put the light directly above the rock in world space, regardless of how the rock moves or rotates
		myLight.transform.position = transform.position + new Vector3(0f, myLightHeight, 0f);

		// if i'm the manager and it's time to do so, tell all the other rocks to stop auto-triggering after this next one
		if ( (manager == this) && (Time.time > untriggerAllRocksTime))
		{
			allRocksTriggered = false;
			foreach (GlowRock rock in allRocks)
				rock.autoTrigEnabled = false;
		}


		// ignore new triggers if you're already in the glow behavior
		if ( (triggered) && (glowStage != GlowStage.Inactive))
			triggered = false;


		if ((glowStage == GlowStage.Brighten) || (glowStage == GlowStage.Darken))
		{
			Vector2 newLight, newEmis;
			float newColor;

			newLight = interp_light.Update();
			newEmis = interp_emis.Update();
			newColor = newEmis.x;

			myRend.material.shader = Shader.Find("Standard");
			myRend.material.SetColor("_EmissionColor",new Color(newColor, newColor, newColor));
			myLight.intensity = newLight.x;

			if (interp_emis.complete && interp_light.complete)
			{
				if (glowStage == GlowStage.Brighten)
				{
					glowStage = GlowStage.Glowing;
					glowingStartTime = Time.time;

					// slam to max values
					myRend.material.shader = Shader.Find("Standard");
					myRend.material.SetColor("_EmissionColor",new Color(emisHigh, emisHigh, emisHigh));
					myLight.intensity = lightIntensityHigh;

				}
				else
				{
					glowStage = GlowStage.Inactive;

					// slam to min values
					myRend.material.shader = Shader.Find("Standard");
					myRend.material.SetColor("_EmissionColor",new Color(emisLow, emisLow, emisLow));
					myLight.intensity = lightIntensityLow;

					if (autoTrigEnabled)
						InitAutoTrigger();
				}
					
			}

		} // end Brighten or Darken


		else if (glowStage == GlowStage.Glowing)
		{
			if ((Time.time - glowingStartTime) > glowDuration)
			{
				glowStage = GlowStage.Darken;

				interp_emis.Initialize(new Vector2(emisHigh,0f), new Vector2(emisLow,0f), darkenDuration);
				interp_light.Initialize(new Vector2(lightIntensityHigh,0f), new Vector2(lightIntensityLow,0f), darkenDuration);
			}
		}  // end Glowing
			

		else if (glowStage == GlowStage.Inactive)
		{
			if (triggered)
			{
				Trigger(false);
			}
			else if (autoTrigEnabled)
			{
				if (Time.time > nextAutoTrigger)
					Trigger(true);
			}

		} // end Inactive
		
	}


	private void Trigger (bool autoTrigger)
	{
		if (!everTriggered)
			FirstTimeTriggered();

		triggered = false;

		glowStage = GlowStage.Brighten;

		if (!autoTrigger)
		{
			// TODO - implement and use 1D interpolators instead of ignoring the second dimension
			interp_emis.Initialize(new Vector2(emisLow,0f), new Vector2(emisHigh,0f), brightenDuration);
			interp_light.Initialize(new Vector2(lightIntensityLow,0f), new Vector2(lightIntensityHigh,0f), brightenDuration);
		}
		else
		{
			// TODO - implement and use 1D interpolators instead of ignoring the second dimension
			interp_emis.Initialize(new Vector2(emisLow,0f), new Vector2(emisHigh,0f), brightenDurationAuto);
			interp_light.Initialize(new Vector2(lightIntensityLow,0f), new Vector2(lightIntensityHigh,0f), brightenDurationAuto);
		}
	}

	private void FirstTimeTriggered ()
	{
		everTriggered = true;
		autoTrigEnabled = true;

		manager.TriggerAllOtherRocks();
	}

	// manager only
	public void TriggerAllOtherRocks ()
	{
		// even if all rocks are triggered, reset the timer
		untriggerAllRocksTime = Time.time + allRocksTriggeredDuration;

		if (allRocksTriggered)
			return;

		allRocksTriggered = true;

		// trigger all the other rocks
		foreach (GlowRock rock in allRocks)
			rock.InitAutoTrigger();

	}

	// auto trigger = trigger self after a delay
	public void InitAutoTrigger()
	{
		autoTrigEnabled = true;
		nextAutoTrigger = Time.time + Random.Range(autoTrigMinTime,autoTrigMaxTime) + brightenDurationAuto + glowDuration + darkenDuration;
	}


	void OnCollisionEnter (Collision collision)
	{
		if (collision.relativeVelocity.magnitude > minMagToTrigger)
		{
			triggered = true;
		}

	}

}
