using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keyctrl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.UpArrow))
        {
            Vector3 playerPos = transform.position;
            playerPos.y += 0.006f;
            transform.position = playerPos;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            Vector3 playerPos = transform.position;
            playerPos.y -= 0.006f;
            transform.position = playerPos;
        }
    }
}
