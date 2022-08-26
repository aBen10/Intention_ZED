using Oculus.Interaction.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WristScreenPositioner : MonoBehaviour
{
    public IHand Hand;
    public GameObject rhand;
    public GameObject rhandDirRefPalm;
    public GameObject rhandDirRefKnuckle;
    public GameObject head;

    // Update is called once per frame
    void Update()
    {
        // floatingWristPointHead();
        wristWatchRotateHead();
    }
    
    void wristWatchRotateHead()
    {
        // if (!Hand.GetJointPosesLocal(out ReadOnlyHandJointPoses localJoints))
            // return;

        // Vector3 handPos = localJoints[0].position;
        // Vector3 palmPos = localJoints[

        Vector3 x = (rhandDirRefPalm.transform.position - rhand.transform.position).normalized;
        Vector3 headDir = head.transform.position - rhand.transform.position;
        Vector3 proj = Vector3.ProjectOnPlane(headDir, x);

        Vector3 knuckleVect = rhandDirRefKnuckle.transform.position - rhand.transform.position;
        Vector3 z = Vector3.ProjectOnPlane(knuckleVect, x).normalized;
        Vector3 y = Vector3.Cross(z, x).normalized;
        float angle = Vector3.Angle(y, proj) * Mathf.Deg2Rad;
        angle *= Mathf.Sign(Vector3.Dot(Vector3.Cross(y, proj), x));
        // angle = 30 * Mathf.Deg2Rad;

        Vector3 circDir = .05f * (Mathf.Cos(angle) * y + Mathf.Sin(angle) * z).normalized;

        Vector3 wristPos = rhand.transform.position + circDir; //- 0.05f * x;
        Vector3 wristDir = circDir;

        transform.position = wristPos;
        transform.rotation = Quaternion.LookRotation(-wristDir, Vector3.up);
    }

    void floatingWristPointHead()
    {
        Transform rtrans = rhand.transform;

        transform.position = rtrans.position + new Vector3(0, .05f, .1f);

        //transform.rotation = Quaternion.LookRotation(rtrans.position - head.transform.position, Vector3.up);
        transform.rotation = Quaternion.LookRotation(-Vector3.up, Vector3.up);
    }
}
