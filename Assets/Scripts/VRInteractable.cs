using UnityEngine;
using System.Collections;

// what inputs does this object respond to
// TODO this should totes be a bitfield
public enum VRInteractedBy : int {
	WandTrigger
}

// what response does this object have to being interacted with
public enum VRInteractResponse : int {
	PickUp,
	Trigger
}



public class VRInteractable : MonoBehaviour {

	public VRInteractedBy interactInput;
	public VRInteractResponse interactResponse;

	public GameObject currentInteractor;

	private Transform originalParent = null;

    // throw tuning variables and data
    private int trackingPeriod = 3;  // once every how many frames do we keep track of where the controller was located?
    private int lookBackIndex = 7;  //  how far back in the position list do we check to calculate the throw trajectory?  this number x trackingPeriod/90 = how many seconds we look back
    private float throwForceMult = 14; // this one is a constant (editable in the editor) for tuning purposes

    // index 0 is the most recent frame, index 10 would be 10 x trackingPeriod / 90 seconds ago
    private Vector3[] positionList; 


	void Start ()
	{
        if (lookBackIndex < 5)
            Debug.LogError("lookBackIndex must be at least 5.");

		originalParent = this.transform.parent;

		positionList = new Vector3[20];  // with trackingPeriod of 3, this tracks 2/3 of one second
		ResetTransformList();
	}

	void FixedUpdate()
	{
		if (currentInteractor != null)
		{
			// track every trackingPeriod-th frame by marking a position
			if ((Time.frameCount % trackingPeriod) == 0)
			{
                for (int i = positionList.Length-1; i >= 1; i--)
                    positionList[i] = positionList[i - 1];

				positionList[0] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
			}

		}

	}

	public void InteractedWith (bool triggerDown, GameObject interactor)
	{
		if (interactResponse == VRInteractResponse.PickUp)
		{
			if (triggerDown)
				GetPickedUp(interactor);
			else
				GetDropped(interactor);
		}
	}


	private void GetPickedUp (GameObject holder)
	{
		currentInteractor = holder;

		this.transform.parent = holder.transform;

		Rigidbody rigidbody = GetComponent<Rigidbody>();

		if (rigidbody == null)
		{
			Debug.Log(this+" doesn't have a rigidbody.");
		}
		else
		{
			rigidbody.isKinematic = true;
		}

	}


	private void GetDropped (GameObject holder)
	{

		currentInteractor = null;

		this.transform.parent = originalParent;

		Rigidbody rigidbody = GetComponent<Rigidbody>();

		if (rigidbody == null)
		{
			Debug.Log(this+" doesn't have a rigidbody.");
		}
		else
		{
			rigidbody.isKinematic = false;
            rigidbody.AddForce(CalculateThrowImpulse(), ForceMode.Impulse);
        }


        ResetTransformList();
	}


    private Vector3 CalculateThrowImpulse()
    {

        if (V3IsVoid(positionList[lookBackIndex]))
        {
            // we don't have enough tracked data, so we'll use the furthest-back-in-time point we can find

            int i = lookBackIndex - 1;
            while ( (V3IsVoid(positionList[i])) && (i>0) )
                i--;

            if (i == 0)
                return Vector3.zero;

            Vector3 diff = transform.position - positionList[i];

            // the closest parallel to how we calculate the force if we have more data
            return (diff * diff.magnitude * throwForceMult);
        }
        else // we do have at least lookBackIndex number of data points
        {
            // old simple method
            //  Vector3 diff = transform.position - positionList[lookBackIndex];

            // direction vector (which includes some magnitude info) will be the average of 3 recent samples
            Vector3 diff1 = transform.position - positionList[lookBackIndex];
            Vector3 diff2 = transform.position - positionList[lookBackIndex - 2];
            Vector3 diff3 = transform.position - positionList[lookBackIndex - 4];

            Vector3 diff = (diff1 + diff2 + diff3) / 3f;
            //Debug.Log("diff vector = " + diff + ", magnitude = " + diff.magnitude);

            // force multiplier will be the largest magnitude of any vector in the positionList - ie - the furthest away on record that the object has been from the release point
            int i = 0;
            Vector3 magVector = transform.position - positionList[i];
            float mag = magVector.magnitude;
            float userForceMult = mag;
            while (((i + 1) < (positionList.Length - 1)) && (!V3IsVoid(positionList[i + 1])))
            {
                i++;
                magVector = transform.position - positionList[i];
                mag = magVector.magnitude;

                if (mag > userForceMult)
                    userForceMult = mag;
            }

            //Debug.Log("largest mag = " + userForceMult);

            Vector3 toReturn = diff * userForceMult * throwForceMult;
            //            Debug.Log("Magnitude of impulse applied: " + toReturn.magnitude);

            return toReturn;
        }


    }


    private void ResetTransformList ()
	{
		for (int i=0; i<positionList.Length; i++)
		{
			positionList[i].x=-99f;
			positionList[i].y=-99f;
			positionList[i].z=-99f;
		}

	}

	private bool V3IsVoid (Vector3 toTest)
	{
		return ( (toTest.x == -99f) && (toTest.y == -99f) && (toTest.z == -99f) );

	}

}
