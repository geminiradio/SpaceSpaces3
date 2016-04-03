using UnityEngine;
using System.Collections;

public class HurlSelfBackHome : MonoBehaviour {

    public BoxCollider home;
    public float howOftenInSeconds = 9f;
    public float impulseMag = 1f;

	
	// Update is called once per frame
	void Update () {
	
        if (Time.frameCount % (90 * howOftenInSeconds) == 0)
        {
            if (!home.bounds.Contains(transform.position))
            {
                Vector3 dir = new Vector3();
                dir = (home.transform.position + new Vector3(0,2,0)) - transform.position;
  
                //dir.Normalize();

                Vector3 impulse = dir * impulseMag;

                Rigidbody rig = GetComponent<Rigidbody>();

                rig.AddForce(impulse, ForceMode.Impulse);
            }


        }

	}
}
