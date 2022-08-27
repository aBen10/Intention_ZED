using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButData : MonoBehaviour
{

    public GraphCTRL_AG.Node node;
    public GraphCTRL_AG graph;
    
    // Start is called before the first frame update
    void Start()
    {
        graph = GameObject.Find("RightPanel").GetComponent<GraphCTRL_AG>();
    }

    void CreateVideo()
    {
        return;
        graph.DestroyVid();
        graph.InstantiateVid(node);
        Debug.Log("INSTANTIATEING");
    }
}
