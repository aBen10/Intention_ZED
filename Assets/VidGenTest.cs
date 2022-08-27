using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VidGenTest : MonoBehaviour
{
    GameObject button;
    public GraphCTRL_AG graph;
    bool VideoOn = false;
    void Start()
    {
        //button = GameObject.Find("ButTest(Clone)");
        graph = GameObject.Find("RightPanel").GetComponent<GraphCTRL_AG>();

        GraphCTRL_AG.Node node = transform.parent.gameObject.GetComponent<ButData>().node;
        /*if (!VideoOn) { graph.InstantiateVid(node); }
        else { graph.DestroyVid(); }*/
        graph.DestroyVid();
        graph.InstantiateVid(node);
        Debug.Log("INSTANTIATEING");
        //button.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
