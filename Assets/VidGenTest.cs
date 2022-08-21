using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VidGenTest : MonoBehaviour
{
    GameObject button;
    public GraphCTRL graph;
    void Start()
    {
        button = GameObject.Find("ButTest(Clone)");
        graph = GameObject.Find("RightPanel").GetComponent<GraphCTRL>();

        GraphCTRL.Node node = transform.parent.gameObject.GetComponent<ButData>().node;
        graph.InstantiateVid(node);
        Debug.Log("INSTANTIATEINIG");
        /*        if (graph.aNodes.Contains(node))
                {
                    graph.InstantiateVid();
                }
                */
        //button.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //button.SetActive(false);
        //Debug.Log("OFF");
    }
}
