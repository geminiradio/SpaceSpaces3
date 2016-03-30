using UnityEngine;
using System.Collections;

[ExecuteInEditMode]

public class ObjectDistributor : MonoBehaviour {

	public bool triggerBehavior = false;
	public GameObject toDistribute;
	public int numToDistribute;
	public float xMin, xMax, yMin, yMax, zMin, zMax;
	public bool rotateOnY = true;

	public GameObject[] distributedObjects;

	void Start ()
	{
		distributedObjects = new GameObject[200];

	}

	// Update is called once per frame
	void Update () {

		if (triggerBehavior)
		{
			triggerBehavior = false;
			RemoveAllOldObjects();
			DistributeObjects();
		}
	
	}

	void RemoveAllOldObjects()
	{

		GameObject go;
		for (int i=0;i<distributedObjects.Length;i++)
		{
			if (distributedObjects[i] != null)
			{
				go = distributedObjects[i];
				DestroyImmediate(go);
			}
		}

	}


	void DistributeObjects()
	{
		GameObject go;
		for (int i=0; i<numToDistribute; i++)
		{
			Vector3 loc = new Vector3 ( Random.Range(xMin,xMax), Random.Range(yMin,yMax), Random.Range(zMin,zMax) );
			Vector3 rot = new Vector3 (0, 0, 0);
			if (rotateOnY)
				rot.y = Random.Range(0,360);
			
			go = (GameObject) Instantiate(toDistribute,loc,Quaternion.Euler(rot));
			go.transform.parent = transform;
			distributedObjects[i] = go;
		}

	}
}
