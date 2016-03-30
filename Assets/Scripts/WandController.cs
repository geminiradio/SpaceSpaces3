using UnityEngine;
using System.Collections;

public class WandController : MonoBehaviour {

	public bool triggerDown = false;
	public bool triggerUp = false;
	public bool triggerPressed = false;

	public VRInteractable currentInteractable = null;

	private Valve.VR.EVRButtonId trigger = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;

	private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input ((int)trackedObj.index);  }  }
	private SteamVR_TrackedObject trackedObj;

	// Use this for initialization
	void Start () {
	
		trackedObj = GetComponent<SteamVR_TrackedObject> ();

	}
	
	// Update is called once per frame
	void Update () {

		if (controller == null) {
			Debug.Log ("Controller not initialized.");
			return;
		}
	

		triggerDown = controller.GetPressDown (trigger);
		triggerUp = controller.GetPressUp (trigger);
		triggerPressed = controller.GetPress (trigger);


		if ((triggerDown) && (currentInteractable != null) 
			&& (currentInteractable.interactInput == VRInteractedBy.WandTrigger))
		{
			currentInteractable.InteractedWith(true, this.gameObject);
		}
		else if ((triggerUp) && (currentInteractable != null) 
			&& (currentInteractable.interactInput == VRInteractedBy.WandTrigger))
		{
			currentInteractable.InteractedWith(false, this.gameObject);
		}


	}


	void OnTriggerEnter (Collider other)
	{
		VRInteractable newInteractable = other.gameObject.GetComponent<VRInteractable>();

		if (newInteractable != null)
		{
			if (currentInteractable != null)
			{
				Debug.Log ("This WandController already has a current interactable "+currentInteractable.gameObject+", so ignoring this new one "+newInteractable.gameObject);
				return;
			}
				
			currentInteractable = newInteractable;
		}
		else
		{
			Debug.Log("WandController collided with a non-VRInteractable object, so ignoring it; it's "+newInteractable.gameObject);
		}
			
	}

	void OnTriggerExit (Collider other)
	{
		VRInteractable newInteractable = other.gameObject.GetComponent<VRInteractable>();

		if (newInteractable == null)
			return;

		if (currentInteractable != null)
		{
			if (currentInteractable == newInteractable)
			{
				currentInteractable = null;
			}
		}
	}


}
