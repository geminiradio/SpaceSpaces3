using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]

public class ObjectDistributor : MonoBehaviour {

	public bool triggerDistribute = false;
	public GameObject toDistribute;
	public int numToDistribute;
	public float xMin, xMax, yMin, yMax, zMin, zMax;
	public bool rotateOnY = true;

	public GameObject[] distributedObjects;

    public bool triggerStore = false; // toggle this in game mode to copy transforms from toStore to storedTransforms
    public GameObject[] toStoreOrCopy; // assign this in editor
    public Transform[] storedTransforms; // these get stored
    public bool triggerCopy = false; // toggle this in editor to copy transforms from storedTransforms to toStoreOrCopy

	void Start ()
	{
		distributedObjects = new GameObject[200];
        storedTransforms = new Transform[200];

    }

    // Update is called once per frame
    void Update () {

		if (triggerDistribute)
		{
			triggerDistribute = false;
			RemoveAllOldObjects();
			DistributeObjects();
		}

        if (triggerStore)
        {
            triggerStore = false;
            for (int i = 0; i < toStoreOrCopy.Length; i++)
                storedTransforms[i] = toStoreOrCopy[i].transform;
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
