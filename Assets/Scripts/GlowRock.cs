using UnityEngine;
using System.Collections;



public class GlowRock : MonoBehaviour {

	public bool triggered = false;
	private bool everTriggered = false;
	public GlowRock manager;

	// this set only for manager
	public float timeBeforeEmerge = 55f;  // how many seconds before the glowing rocks appear
	public float emergeDuration = 5f;  // how long does it take the glowing rocks to fade into existence
	private bool emergingStarted = false;
	private Interpolator2D interp_emerge;
	private Interpolator2D interp_emergeSfx;
	private float emergeSfxDuration = 3f;
	public AudioSource emergeSfx;
	public GlowRock[] allRocks;  // only for manager, list of all rocks
	private bool allRocksTriggered = false;  // only for manager
	public float allRocksTriggeredDuration = 60f; // only for manager
	private float untriggerAllRocksTime; // only for manager

	private static bool emergingComplete = false;


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

	private Vector3 originalScale;

	public GameObject intelligentSphere;

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

		if (intelligentSphere == null)
			Debug.LogError(this+" intelligentSphere not assigned correctly!");
		

		interp_emis = new Interpolator2D();
		interp_light = new Interpolator2D();
		interp_emerge = new Interpolator2D();
		interp_emergeSfx = new Interpolator2D();

		myRend.material.EnableKeyword("_EmissionColor");

		// all glowrocks start tiny and fully turned off other than manager
		originalScale = transform.localScale;
		Vector3 verySmall = new Vector3 (0.00001f, 0.00001f, 0.00001f);
		transform.localScale = verySmall;
		TurnStuffOnOff(false);
		if (manager != this)
			gameObject.SetActive(false);

	}


	void Update () {

		// no glow rocks exist in the scene yet
		if ((manager == this) && (Time.time < timeBeforeEmerge))
			return;

		// if emerging isn't finished and you're not a manager, leave now
		if ((manager != this) && (!emergingComplete))
			return;

		// kick off the emerge behavior
		if ((manager == this) && (Time.time >= emergeDuration) && (!emergingStarted))
		{
			emergingStarted = true;

			// TODO - this is a 2dinterpolator being used as a 1dinterpoolator
			interp_emerge.Initialize(new Vector2(0f,0f), new Vector2(1f,0f), emergeDuration);
			interp_emergeSfx.Initialize(new Vector2(0f,0f), new Vector2(0.05f,0f), emergeSfxDuration); // hardcoded from vol 0 to vol 0.05

			// enable all glow rocks and turn on all glow rock's render and collider meshes (only)
			MeshRenderer myRend = GetComponent<MeshRenderer>();
			myRend.enabled = true;
			MeshCollider myCol = GetComponent<MeshCollider>();
			myCol.enabled = true;
			for (int i=0; i<allRocks.Length; i++)
			{
				allRocks[i].gameObject.SetActive(true);
				MeshRenderer rend = allRocks[i].GetComponent<MeshRenderer>();
				rend.enabled = true;

				MeshCollider col = allRocks[i].GetComponent<MeshCollider>();
				col.enabled = true;
			}

			return;
		}

		// continue the emerge behavior
		if ((manager == this) && (emergingStarted) && (!emergingComplete))
		{
			// sfx first
			if (emergeSfx != null)
			{
				emergeSfx.volume = interp_emergeSfx.Update().x;
			}
				
			// scale all the glow rocks with the interpolator, up to their stored originalScale
			Vector2 vec = interp_emerge.Update();
			float newScaleMag = vec.x;

			for (int i=0; i<allRocks.Length; i++)
			{
				Vector3 newScale = new Vector3 (newScaleMag * allRocks[i].originalScale.x,newScaleMag * allRocks[i].originalScale.y,newScaleMag * allRocks[i].originalScale.z);
				allRocks[i].transform.localScale = newScale;
			}

			// manager too
			Vector3 myNewScale = new Vector3 (newScaleMag*originalScale.x,newScaleMag*originalScale.y,newScaleMag*originalScale.z);
			transform.localScale = myNewScale;

		
			// check to see if emerging is complete yet
			if (interp_emerge.complete)
			{
				emergingComplete = true;
				TurnStuffOnOff(true);
				for (int i=0; i<allRocks.Length; i++)
					allRocks[i].TurnStuffOnOff(true);
			}


			return;
		}


		// if the code gets this far, the emerging process is complete
		if (!emergingComplete)
			Debug.LogError("emerging should be complete by now");

		// LERP the sfx out.  this is crappy but whatever
		if ((manager == this) && (emergeSfx != null))
		{
			float percent = (Time.time - emergeDuration - timeBeforeEmerge) / (emergeSfxDuration);

			if (percent > 1f)
				percent = 1f;
			percent = 1-percent;

			emergeSfx.volume = (0.05f * percent);
		}
			

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

			if (collision.collider.gameObject.name == "NearestObelisk")
			{
				Animator anim = intelligentSphere.GetComponent<Animator>();
				anim.SetTrigger("ApproachPlayer");

				Obelisk ob = collision.collider.gameObject.GetComponent<Obelisk>();
				ob.IveBeenHit();
			}
				
		}

	}


	void TurnStuffOnOff (bool onoroff)
	{
		VRInteractable vrscript = GetComponent<VRInteractable>();
		vrscript.enabled = onoroff;

		MeshCollider collid = GetComponent<MeshCollider>();
		collid.enabled = onoroff;

		Rigidbody rig = GetComponent<Rigidbody>();
		rig.isKinematic = !onoroff;

		MeshRenderer rend = GetComponent<MeshRenderer>();
		rend.enabled = onoroff;

	}

}
