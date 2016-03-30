using UnityEngine;
using System.Collections;

public class TriggerApproach : MonoBehaviour {

    public Animator animToTrigger;
    public WandController wandController;

	// Use this for initialization
	void Start () {

        wandController = GetComponent<WandController>();

	}
	
	// Update is called once per frame
	void Update () {
	
        if (wandController.triggerDown)
        {
            animToTrigger.SetTrigger("ApproachPlayer");

        }

	}
}
