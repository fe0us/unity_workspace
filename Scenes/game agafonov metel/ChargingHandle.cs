using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingHandle : MonoBehaviour
{
    public bool selected = false;
    ConfigurableJoint joint;
    void Start()
    {
        joint = GetComponent<ConfigurableJoint>();
    }

    public void select(bool enter)
    {
        selected = enter;

        if (enter)
        {
            joint.xMotion = ConfigurableJointMotion.Limited;
        }
        else
        {
            joint.xMotion = ConfigurableJointMotion.Locked;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "charginghandle")
        {
            Debug.Log("Перезарядка");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
