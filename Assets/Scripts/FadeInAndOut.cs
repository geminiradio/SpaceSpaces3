using UnityEngine;
using System.Collections;

public class FadeInAndOut : MonoBehaviour {

	public float fadeInTime = 3f;
	public float fadeOutTime = 10f;

	public bool triggerFadeOut = false;

	private bool fadingIn = false;
	private bool fadingOut = false;
	private Interpolator2D interp;

	private Renderer rend;

	// Use this for initialization
	void Start () {
		rend = GetComponent<Renderer>();
		if (rend == null)
			Debug.LogError("Object has no renderer!");

		interp = new Interpolator2D();

		// start fade in - TODO this is a 1d vector but i don't have Interpolar1D yet
		interp.Initialize(new Vector2(1f,0f), new Vector2(0f,0f), fadeInTime);
		fadingIn = true;

	}
	
	// Update is called once per frame
	void Update () {
	
		if (fadingIn)
		{
			Vector2 newAlpha = interp.Update();

			rend.material.shader = Shader.Find("Standard");
			rend.material.SetColor("_Color", new Color (0,0,0, newAlpha.x));

			if (interp.complete)
				fadingIn = false;
		}
		else if (fadingOut)
		{
			Vector2 newAlpha = interp.Update();

			rend.material.shader = Shader.Find("Standard");
			rend.material.SetColor("_Color", new Color (0,0,0, newAlpha.x));

			if (interp.complete)
				fadingOut = false;
		}
		else if (triggerFadeOut)
		{
			// start fade out - TODO this is a 1d vector but i don't have Interpolar1D yet
			interp.Initialize(new Vector2(0f,0f), new Vector2(1f,0f), fadeOutTime);
			fadingOut = true;
		}


	}
}
