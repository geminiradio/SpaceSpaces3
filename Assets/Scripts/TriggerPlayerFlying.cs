using UnityEngine;
using System.Collections;

public class TriggerPlayerFlying : MonoBehaviour {

    public float delay = 20f;
    public FlyingPlayer fp;
    private float triggerTime;
    private bool triggered = false;



    void Update()
    {
        if ((triggered) && (Time.time > triggerTime))
        {
            triggered = false;

            fp.trigger = true;
        }
    }

    public void TriggerFlyingPlayer()
    {
        triggered = true;
        triggerTime = Time.time + delay;

    }
}
