using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class positionTracker : MonoBehaviour
{
    GameObject print_bed;
    private Vector3 pos;
    private float pos_y;

    // Start is called before the first frame update
    void Start()
    {
        print_bed = GameObject.Find("Cube");
        //proceduralManager = checkpoint_counter.GetComponent<ProceduralManager>();
    }

    // Update is called once per frame
    void Update()
    {
        pos = print_bed.transform.position;
        pos_y = pos.y;
        Debug.Log(pos_y);
    }
}
