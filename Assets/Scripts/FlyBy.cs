using UnityEngine;
using System.Collections;

public class FlyBy : MonoBehaviour {

    public int DEBUG_indexOverride = -1;

	public float minWait;
	public float maxWait;

	public Transform[] StartPoints;
	public Transform[] EndPoints;
	public float[] durations;

	private Transform start, end;

	private float nextStart;
	private bool goingNow = false;
	private Interpolator2D interp;

	// Use this for initialization
	void Start () {
	
		nextStart = Time.time + Random.Range(minWait,maxWait);

		interp = new Interpolator2D();

	}
		
	// Update is called once per frame
	void Update () {
	
		if (!goingNow)
		{
			if (Time.time > nextStart)
				StartFlyBy();

		}
		else  // goingNow == true
		{
			if (interp.complete)
			{
				EndFlyBy();
			}
			else
			{
				Vector2 newPos = interp.Update();
				Vector3 newPos3D = new Vector3(newPos.x,transform.position.y,newPos.y);

				transform.position = newPos3D;
			}
		}


	}


	private void StartFlyBy()
	{
		goingNow = true;

		int whichPath = Random.Range(0,StartPoints.Length);

        if (DEBUG_indexOverride != -1)
            whichPath = DEBUG_indexOverride;

		start = StartPoints[whichPath];
		end = EndPoints[whichPath];
		float duration = durations[whichPath];

		Vector2 start2D = new Vector2 (start.position.x, start.position.z);
		Vector2 end2D = new Vector2 (end.position.x, end.position.z);

		interp.Initialize(start2D,end2D,duration);

		// put the capsule in the start position.
		transform.position = start.position;
        transform.rotation = start.rotation;

	}

	private void EndFlyBy()
	{
		goingNow = false;

		nextStart = Time.time + Random.Range(minWait,maxWait);
	}

}
