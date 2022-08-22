using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WristScreenPositioner : MonoBehaviour
{
    public GameObject rhand;
    public GameObject head;

    // Update is called once per frame
    void Update()
    {
        Transform rtrans = rhand.transform;
            
        transform.position = rtrans.position + new Vector3(0,.15f,.1f);
        
        transform.rotation = Quaternion.LookRotation(rtrans.position - head.transform.position, Vector3.up);
    }
}
