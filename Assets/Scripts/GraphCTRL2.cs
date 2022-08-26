using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using static System.Net.Mime.MediaTypeNames;
using Debug = UnityEngine.Debug;
using Image = UnityEngine.UI.Image;
using Button = UnityEngine.UI.Button;
using UnityEditor;
using System.Linq;

//Controls graph animation
public class GraphCTRL2 : MonoBehaviour
{

    Graph graph;
    private GameObject eButton, eButton2, iButton, iButton2, aButton, aButton2, TestB;
    public GameObject panel;
    public GameObject preview, instantiatedPreview, WristScreen, GraphScreen;
    public Material lineMaterial;
    private GameObject line, lineDiag;
    private Vector3 Elast, Enow, Enext, Ilast, Inow, Inext, Alast, Anow, Anext;
    private Vector3 eVidRightPos, eVidLeftPos, iVidLeftPos, iVidRightPos, aVidLeftPos, aVidRightPos;
    private GameObject eVidRight, eVidLeft, iVidLeft, iVidRight, aVidLeft, aVidRight;
    public List<VideoClip> aClips = new List<VideoClip>();
    public List<VideoClip> eClips = new List<VideoClip>();
    public List<VideoClip> iClips = new List<VideoClip>();
    ConcatVideos concat;
    int count = 0;
    int x = 0;
    int drawCount = 0;
    int eCount, iCount, aCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Loads clips from Resources folder into list
        aClips.AddRange(Resources.LoadAll<VideoClip>("aClips"));
        iClips.AddRange(Resources.LoadAll<VideoClip>("iClips"));
        eClips.AddRange(Resources.LoadAll<VideoClip>("eClips"));
        Debug.Log(aClips.Count);
        Debug.Log(aClips[0]);
        Debug.Log(iClips.Count);
        Debug.Log(iClips[0]);
        Debug.Log(eClips.Count);
        Debug.Log(eClips[0]);
        //load button and lineprefabs from Resources folder
        iButton = Resources.Load("iButton") as GameObject;
        iButton2 = Resources.Load("iButton2") as GameObject;
        eButton = Resources.Load("eButton") as GameObject;
        eButton2 = Resources.Load("eButton2") as GameObject;
        aButton = Resources.Load("aButton") as GameObject;
        aButton2 = Resources.Load("aButton2") as GameObject;
        line = Resources.Load("LineDown") as GameObject;
        lineDiag = Resources.Load("LineDiag") as GameObject;
        preview = Resources.Load("ButtonScreen") as GameObject;
        GraphScreen = Resources.Load("GraphScreen") as GameObject;
        //Make graph, declare set vectors for buttons, add nodes to graph (these will later be defined by the user study task)
        graph = new Graph();
        Enow = new Vector3(-250, 0, 0); Enext = new Vector3(250, 0, 0);
        eVidLeftPos = new Vector3(-1000,0,0); eVidRightPos = new Vector3(1000, 0, 0);
        iVidLeftPos = Inow - new Vector3(1000, 0, 0); iVidRightPos = Inow + new Vector3(1000, 0, 0);
        aVidLeftPos = Anow - new Vector3(1000, 0, 0); aVidRightPos = Anow + new Vector3(1000, 0, 0);

        MakeNodes();
        MakeScreens();
        Debug.Log(" aNodes: " + graph.aNodes.Count + " iNodes: " + graph.iNodes.Count + " eNodes: " + graph.eNodes.Count);
        Build();

        WristScreen.GetComponent<VideoPlayer>().clip = iClips[iCount];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) && x < 1)
        {
            Debug.Log("Next");
            //GameObject.Find("BotLineDiag").SetActive(true);
            if (GameObject.Find("GraphScreen(Clone)"))
            {
                GameObject[] screens = GameObject.FindGameObjectsWithTag("GraphScreen");
                foreach (var tv in screens)
                {
                    Destroy(tv);
                }
            }
            Next();
            MakeScreens();
            //WristScreen.GetComponent<VideoPlayer>().clip = aClips[aCount];
            //aVidLeft.GetComponent<VideoPlayer>().clip = aClips[aCount];
            x++;

        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            x = 0;
        }
    }

    /*transitions to next affordance/gesutre (A/G). aCount, iCount, and eCount keep track of current A/G and thier parents. This code uses checks on parents
     * to figure out how whether it's just the next A/G, or the user is moving to the next interaction or even the next event. The body
     * of the "if statements" determins what the graph should look like based on the case. Currently breaks at the end of the graph. Waiting 
     * to see how user study takes shape before making more changes
    */
    public void Next()
    {
        if (eCount + 1 == graph.iNodes.Count)
        {
            Debug.Log("No more Nodes");
            GameObject.Find("eArrow").SetActive(false);
            GameObject.Find("eArrow2").SetActive(false);
            return;
        }
        else
        {
            eCount++;
            graph.eNodes[eCount].Position = Enow;
            graph.eNodes[eCount - 1].Position = Elast;
            if(graph.eNodes.Count + 1 > eCount)
            {
                graph.eNodes[eCount+1].Position = Enext;
            }
            Debug.Log("E SWITCH");
            GameObject.Find("eArrow").GetComponent<Image>().color = Color.white;

        }
        GameObject.Find("eArrow").GetComponent<Image>().color = Color.white;
        Build();
        Debug.Log("End of next");
    }

    //Clears graph button game objects then draws the new current node and past/future nodes if applicable
    private void Build()
    {
        ClearNodes();
        DrawNode(graph.eNodes[eCount]);
        if (graph.eNodes.Count > eCount + 1) { DrawNode(graph.eNodes[eCount + 1]); }
        /*if (aCount > 0) { DrawNode(graph.aNodes[aCount - 1]); }
        if (iCount > 0) { DrawNode(graph.iNodes[iCount - 1]); }*/
    }

    //Tagged the prefab buttons with "GraphButton" tag. This finds them and deltes them so new ones can be added
    private void ClearNodes()
    {
        GameObject[] nodes = GameObject.FindGameObjectsWithTag("GraphButton");
        foreach (var node in nodes)
        {
            Destroy(node);
        }
    }

    //Instantiates buttons with appropriate properties (a parent, a name, an action to perform on click. Puts them in the correct location
    private void DrawNode(Node node)
    {

        GameObject but;

        if (node == graph.eNodes[eCount])
        {
            but = Instantiate(eButton, panel.transform, false);
        }
        else
        {
            but = Instantiate(eButton2, panel.transform, false);
        }
        Debug.Log(node.Name);
        Debug.Log(panel.transform.position + node.Position + new Vector3(0, 0, -20));
       
        but.GetComponent<Transform>().localPosition = panel.transform.position + node.Position + new Vector3(0,0,-20);
        if (node.Name!=null) { but.GetComponentInChildren<TMPro.TextMeshPro>().text = node.Name; }

        //but.GetComponent<MeshRenderer>().material.SetColor("_Color", node.NodeColor);
        //Debug.Log("Drawing");
    }

    //Assigns the appropriate clip to the node/button passed in. Defines where the clip should be shown.
    public void InstantiateVid(Node node)
    {
        DestroyVid();
        StopAllCoroutines();
        //Debug.Log("Instantiate Video");
        Vector3 instantiatePoint = panel.transform.position;
        instantiatePoint.y = node.Position.y;
        instantiatePoint.x = instantiatePoint.x - 1000;
        //GameObject go = Instantiate(preview, instantiatePoint, Quaternion.identity, GameObject.Find("Progress").transform);
        //GameObject go = Instantiate(preview, GameObject.Find("aButton(Clone)").transform, false);
        GameObject go = Instantiate(preview, panel.transform, false);

        instantiatedPreview = go;
        VideoClip vid;
        
        if (graph.aNodes.Contains(node)) {vid = aClips[graph.aNodes.IndexOf(node)]; }
        else if (graph.iNodes.Contains(node)) { vid = iClips[graph.iNodes.IndexOf(node)]; }
        else { vid = eClips[graph.eNodes.IndexOf(node)]; }
        
        instantiatedPreview.GetComponent<VideoPlayer>().clip = vid;
        StartCoroutine(VidTimer(vid));
        instantiatedPreview.GetComponent<Transform>().localPosition = instantiatePoint;

    }

    IEnumerator VidTimer(VideoClip vid)
    {
        yield return new WaitForSeconds((float)vid.length);
        DestroyVid();
    }
    public void DestroyVid()
    {
        if (GameObject.Find("ScrubScreenBig(Clone)"))
        {
            Destroy(GameObject.Find("ScrubScreenBig(Clone)"));
            Debug.Log("Destroying");
        }

    }

    private void MakeScreens()
    {
        eVidLeft = Instantiate(GraphScreen, panel.transform, false);
        eVidLeft.GetComponent<VideoPlayer>().clip = eClips[eCount];
        eVidLeft.GetComponent<Transform>().localPosition = eVidLeftPos;

        eVidRight = Instantiate(GraphScreen, panel.transform, false);
        // aVidRight.GetComponent<VideoPlayer>().clip = aClips[0];
        eVidRight.GetComponent<Transform>().localPosition = eVidRightPos;
        concat = eVidRight.GetComponent<ConcatVideos>();
        if(eClips.Count > eCount + 1)
        {
            concat.PlayBackToBack(eClips[eCount], eClips[eCount + 1], 3.0, 3.0);
        }
        //ConcatVideos.PlayBackToBack(aClips[0], aClips[1], 3.0, 3.0);
        //ConcatVideos.videoPlayer = aVidRight.GetComponent<VideoPlayer>();
        //aVidRight.GetComponent<VideoPlayer>().clip = ConcatVideos.PlayBackToBack(aClips[0], aClips[1], 3.0, 3.0);
        //VideoClip clip = ConcatVideos.PlayBackToBack(aClips[0], aClips[1], 3.0, 3.0);
    }

    private void MakeNodes()
    {
        //5 events
        var e0 = new Node() { Position = Enow, Name = "Base" }; var e1 = new Node() { Position = Enext, Name = "Printer" }; var e2 = new Node() { Name = "Stand" }; var e3 = new Node() { Name = "Arm" }; var e4 = new Node() { Name = "Head" };

        graph.eNodes.Add(e0); graph.eNodes.Add(e1); graph.eNodes.Add(e2); graph.eNodes.Add(e3); graph.eNodes.Add(e4);
 

    }

    public class Graph
    {
        public Graph()
        {
            eNodes = new List<Node>();
            iNodes = new List<Node>();
            aNodes = new List<Node>();
            pNodes = new List<Node>();
            //Edges = new List<Edge>();
        }

        public List<Node> eNodes { get; set; }
        public List<Node> iNodes { get; set; }
        public List<Node> aNodes { get; set; }
        public List<Node> pNodes { get; set; }

        //public List<Edge> Edges { get; set; }
    }

    public class Node
    {
        //private GameObject nodeButton;

        public Node()
        {
            NodeColor = Color.white;
            Position = Vector3.zero;
            Highlights = new List<GameObject>();
        }
        public Color NodeColor { get; set; }
        public Vector3 Position { get; set; }
        public Node Parent { get; set; }
        public String Name { get; set; }
        public String File { get; set; }
        public List<GameObject> Highlights { get; set; }
    }

}

