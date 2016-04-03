using UnityEngine;
using System.Collections;

public class Obelisk : MonoBehaviour {

	public AudioSource whenHit;
	public GameObject[] pfx;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void IveBeenHit()
	{
		if (whenHit != null)
			whenHit.Play();

        if (pfx.Length > 0)
        {
            foreach (GameObject p in pfx)
            p.SetActive(true);
        }
	}

}
