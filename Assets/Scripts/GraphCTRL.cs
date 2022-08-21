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
public class GraphCTRL : MonoBehaviour
{

    Graph graph;
    private GameObject eButton, eButton2, iButton, iButton2, aButton, aButton2;
    public GameObject panel, rightButtons, leftButtons;
    public GameObject preview, instantiatedPreview;
    public Material lineMaterial;
    private GameObject line, lineDiag;
    private Vector3 Elast, Enow, Enext, Ilast, Inow, Inext, Alast, Anow, Anext;
    public List<VideoClip> Clips = new List<VideoClip>();
    int count = 0;
    int x = 0;
    int drawCount = 0;
    int eCount, iCount, aCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Loads clips from Resources folder into list
        Clips.AddRange(Resources.LoadAll<VideoClip>("Clips"));
        Debug.Log(Clips.Count);
        Debug.Log(Clips[0].name);
        Debug.Log(Clips[0]);
        //load button and lineprefabs from Resources folder
        iButton = Resources.Load("iButton") as GameObject;
        iButton2 = Resources.Load("iButton2") as GameObject;
        eButton = Resources.Load("eButton") as GameObject;
        aButton = Resources.Load("aButton") as GameObject;
        aButton2 = Resources.Load("aButton2") as GameObject;
        line = Resources.Load("LineDown") as GameObject;
        lineDiag = Resources.Load("LineDiag") as GameObject;
        preview = Resources.Load("ScrubScreenBig") as GameObject;
        //Make graph, declare set vectors for buttons, add nodes to graph (these will later be defined by the user study task)
        graph = new Graph();
        Enow = new Vector3(0, 700, 0);
        Ilast = new Vector3(-400, 134, 0); Inow = new Vector3(0, 134, 0); Inext = new Vector3(400, 134, 0);
        Alast = new Vector3(-400, -432, 0); Anow = new Vector3(0, -432, 0); Anext = new Vector3(400, -432, 0);
        //Pnow = new Vector3(-300, -998, 0);
        //Pnext = new Vector3(300, -998, 0);

        var e0 = new Node() { NodeColor = Color.red, Position = Enow }; var e1 = new Node() { };
        var i0 = new Node() { NodeColor = Color.blue, Position = Inow, Parent = e0 };
        var i1 = new Node() { NodeColor = Color.blue, Position = Inext, Parent = e0 };
        var i2 = new Node() { Parent = e0 }; var i3 = new Node() { Parent = e1 }; var i4 = new Node() { Parent = e1 };
        var a0 = new Node() { NodeColor = Color.green, Position = Anow, Parent = i0, };
        var a1 = new Node() { NodeColor = Color.green, Position = Anext, Parent = i0 };
        var a2 = new Node() { Parent = i0 }; var a3 = new Node() { Parent = i0 }; var a4 = new Node() { Parent = i1 };  var a5 = new Node() { Parent = i1 };   
        var a6 = new Node() { Parent = i2 }; var a7 = new Node() { Parent = i3 }; var a8 = new Node() { Parent = i4 };

        graph.eNodes.Add(e0); graph.eNodes.Add(e1);
        graph.iNodes.Add(i0); graph.iNodes.Add(i1); graph.iNodes.Add(i2); graph.iNodes.Add(i3); graph.iNodes.Add(i4);
        graph.aNodes.Add(a0); graph.aNodes.Add(a1); graph.aNodes.Add(a2); graph.aNodes.Add(a3); graph.aNodes.Add(a4); graph.aNodes.Add(a5); graph.aNodes.Add(a6); graph.aNodes.Add(a7); graph.aNodes.Add(a8);

        Debug.Log(" aNodes: " + graph.aNodes.Count + " iNodes: " + graph.iNodes.Count + " eNodes: " + graph.eNodes.Count);
        Build();
        GameObject.Find("LineDiag2").GetComponent<Image>().enabled = false;
        GameObject.Find("LineDiag2Top").GetComponent<Image>().enabled = false;
        GameObject.Find("LineDownNext").GetComponent<Image>().enabled = false;
        GameObject.Find("LineDownPrev").GetComponent<Image>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.T) && x < 1)
        {
            Debug.Log("Next");
            GameObject.Find("LineDiag").GetComponent<Image>().enabled = true;
            //GameObject.Find("BotLineDiag").SetActive(true);
            rightButtons.SetActive(true);
            leftButtons.SetActive(true);
            Next();
            x++;

        }
        if (Input.GetKey(KeyCode.R))
        {
            x = 0;
        }
    }
    
    /*transitions to next action. aCount, iCount, and eCount keep track of current action and thier parents. This code uses checks on parents
     * to figure out how whether it's just the next action, or the user is moving to the next interaction or even the next event. The body
     * of the "if statements" determins what the graph should look like based on the case. Currently breaks at the end of the graph. Waiting 
     * to see how user study takes shape before making more changes
    */
    private void Next()
    {
        if (aCount + 1 == graph.aNodes.Count)
        {
            Debug.Log("No more aNodes");
            return;
        }

        if (graph.aNodes[aCount].Parent == graph.aNodes[aCount + 1].Parent)
        {
            aCount++;
            graph.aNodes[aCount - 1].Position = Alast;
            graph.aNodes[aCount].Position = Anow;
            graph.aNodes[aCount + 1].Position = Anext;
            Debug.Log("A SWITCH");
            GameObject.Find("LineDiag2").GetComponent<Image>().enabled = true;
            GameObject.Find("LineDownNext").GetComponent<Image>().enabled = false;
            GameObject.Find("aArrow").GetComponent<Image>().enabled = true;
            GameObject.Find("LineDownPrev").GetComponent<Image>().enabled = false;
            if (graph.aNodes[aCount + 1] != null && graph.aNodes[aCount].Parent != graph.aNodes[aCount + 1].Parent)
            {
                GameObject.Find("LineDiag").GetComponent<Image>().enabled = false;
                GameObject.Find("LineDownNext").GetComponent<Image>().enabled = true;
                GameObject.Find("aArrow").GetComponent<Image>().enabled = false;
            }
        }
        //else if current interaction and next interaction have the same parent event #2
        else if (graph.iNodes[iCount].Parent == graph.iNodes[iCount + 1].Parent)
        {
            iCount++;
            aCount++;
            graph.iNodes[iCount - 1].Position = Ilast;
            graph.iNodes[iCount].Position = Inow;
            if (graph.iNodes.Count > iCount + 1) { graph.iNodes[iCount + 1].Position = Inext;}
            
            graph.aNodes[aCount - 1].Position = Alast;
            graph.aNodes[aCount].Position = Anow;
            if (graph.aNodes.Count > aCount + 1){ graph.aNodes[aCount + 1].Position = Anext;}
            
            Debug.Log("I SWITCH");
            GameObject.Find("LineDiag2").GetComponent<Image>().enabled = false;
            GameObject.Find("LineDownPrev").GetComponent<Image>().enabled = true;
            GameObject.Find("LineDownNext").GetComponent<Image>().enabled = false;
            GameObject.Find("aArrow").GetComponent<Image>().enabled = true;
            GameObject.Find("LineDiag2Top").GetComponent<Image>().enabled = true;
            //if there will be a new next action and if the new next action will have a different parent interaction than the new current action
            if (graph.aNodes.Count > aCount + 1 && graph.aNodes[aCount].Parent != graph.aNodes[aCount + 1].Parent)
            {
                GameObject.Find("LineDiag").GetComponent<Image>().enabled = false;
                GameObject.Find("LineDownNext").GetComponent<Image>().enabled = true;
                GameObject.Find("aArrow").GetComponent<Image>().enabled = false;
            }
            //if there will be a new next interaction and if the new current interaction has a different parent event than that new next interaction 
            if(graph.iNodes.Count > iCount + 1 && graph.iNodes.Count > iCount + 1 && graph.iNodes[iCount].Parent != graph.iNodes[iCount + 1].Parent)
            {

                Debug.Log("E SWITCH UP NEXT");
                GameObject.Find("LineDiag2").GetComponent<Image>().enabled = false;
                GameObject.Find("LineDiagTop").GetComponent<Image>().enabled = false;
                GameObject.Find("LineDownPrev").GetComponent<Image>().enabled = true;
                GameObject.Find("LineDownNext").GetComponent<Image>().enabled = false;
                GameObject.Find("aArrow").GetComponent<Image>().enabled = false;
                GameObject.Find("iArrow").GetComponent<Image>().enabled = false;
                GameObject.Find("eArrow").GetComponent<Image>().color = Color.yellow;
                GameObject.Find("LineDiag2Top").GetComponent<Image>().enabled = true;
                Build();
                //Destroy(GameObject.Find("iButton2(Clone)"));
                //Destroy(GameObject.Find("aButton2(Clone)").gameObject);
                rightButtons.SetActive(false);
                return;
            }
        }
        //if current interaction does not have the same parent event as the next interaction #3
        else if (graph.iNodes[iCount].Parent != graph.iNodes[iCount + 1].Parent)
        {
            eCount++;
            iCount++;
            aCount++;
            graph.eNodes[eCount-1].Position = Elast;
            graph.eNodes[eCount].Position = Enow;
            graph.iNodes[iCount - 1].Position = Ilast;
            graph.iNodes[iCount].Position = Inow;
            graph.iNodes[iCount + 1].Position = Inext;
            graph.aNodes[aCount - 1].Position = Alast;
            graph.aNodes[aCount].Position = Anow;
            graph.aNodes[aCount + 1].Position = Anext;
            Debug.Log("E SWITCH");
            GameObject.Find("LineDiag2").GetComponent<Image>().enabled = false;
            GameObject.Find("LineDiag").GetComponent<Image>().enabled = false;
            GameObject.Find("LineDiagTop").GetComponent<Image>().enabled = true;
            GameObject.Find("LineDownPrev").GetComponent<Image>().enabled = false;
            GameObject.Find("LineDownNext").GetComponent<Image>().enabled = true;
            GameObject.Find("LineDiag2Top").GetComponent<Image>().enabled = false;
            GameObject.Find("aArrow").GetComponent<Image>().enabled = false;
            GameObject.Find("iArrow").GetComponent<Image>().enabled = true;
            GameObject.Find("eArrow").GetComponent<Image>().color = Color.white;
            if (graph.iNodes.Count > iCount + 1 && graph.aNodes[aCount].Parent == graph.aNodes[aCount + 1].Parent)
            {
                GameObject.Find("aArrow").GetComponent<Image>().enabled = true;
                GameObject.Find("LineDownNext").GetComponent<Image>().enabled = false;
                GameObject.Find("LineDiag").GetComponent<Image>().enabled = true;
            }

            Build();
            leftButtons.SetActive(false);
            return;
        }
        else
        {
            Debug.Log("End of graph");
            return;
        }
        Debug.Log("End of next");
        Build();
    }

    //Clears graph button game objects then draws the new current node and past/future nodes if applicable
    private void Build()
    {
        ClearNodes();
        DrawNode(graph.eNodes[eCount]);
        DrawNode(graph.iNodes[iCount]);
        DrawNode(graph.aNodes[aCount]);
        if (graph.iNodes.Count > iCount + 1) {DrawNode(graph.iNodes[iCount + 1]);}
        if (graph.aNodes.Count > aCount + 1) { DrawNode(graph.aNodes[aCount + 1]); }
        if (aCount > 0) { DrawNode(graph.aNodes[aCount - 1]); }
        if (iCount > 0) { DrawNode(graph.iNodes[iCount - 1]); }
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
        if (graph.aNodes.Contains(node))
        {
            if (node == graph.aNodes[aCount])
            {
                but = Instantiate(aButton, panel.transform, false);
                ButData data = but.GetComponent<ButData>();
                data.node = node;
                //but.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "A" + (aCount);
                //but.gameObject.GetComponent<Button>().onClick.AddListener(() => InstantiateVid(node));
                //Debug.Log("Drawing a node: " + aCount + " at position: " + node.Position);
            }
            else if (graph.aNodes.Count > aCount + 1 && node == graph.aNodes[aCount + 1])
            {
                but = Instantiate(aButton2, rightButtons.transform, false);
                ButData data = but.GetComponent<ButData>();
                data.node = node;
                //but.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "A" + (aCount + 1);
                //but.gameObject.GetComponent<Button>().onClick.AddListener(() => InstantiateVid(node));
                //Debug.Log("Drawing a node: " + (aCount + 1) + " at position: " + node.Position);
            }
            else
            {
                but = Instantiate(aButton2, leftButtons.transform, false);
                ButData data = but.GetComponent<ButData>();
                data.node = node;
                //but.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "A" + (aCount - 1);
                //but.gameObject.GetComponent<Button>().onClick.AddListener(() => InstantiateVid(node));
                //Debug.Log("Drawing a node: " + (aCount - 1) + " at position: " + node.Position);
            }
        }
        else if (graph.iNodes.Contains(node))
        {
            if (node == graph.iNodes[iCount])
            {
                but = Instantiate(iButton, panel.transform, false);
                //but.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "I" + (iCount);
                //Debug.Log("Drawing i node: " + iCount + " at position: " + node.Position);
            }
            else if (graph.iNodes.Count > iCount + 1 && node == graph.iNodes[iCount + 1])
            {
                but = Instantiate(iButton2, rightButtons.transform, false);
                //but.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "I" + (iCount + 1);
                //Debug.Log("Drawing i node: " + (iCount + 1) + " at position: " + node.Position);
            }
            else
            {
                but = Instantiate(iButton2, leftButtons.transform, false);
                //but.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "I" + (iCount-1);
                //Debug.Log("Drawing i node: " + (iCount - 1) + " at position: " + node.Position);
            }
        }
        else
        {
            but = Instantiate(eButton, panel.transform, false);
            //but.GetComponentInChildren<TextMeshPro>().text = "test";

        }
        but.GetComponent<Transform>().localPosition = panel.transform.position + node.Position + new Vector3(0,0,-20);
        //but.GetComponent<MeshRenderer>().material.SetColor("_Color", node.NodeColor);
        //Debug.Log("Drawing");
    }

    //Assigns the appropriate clip to the node/button passed in. Defines where the clip should be shown. Just does this for a few of the Action nodes as a test 
    public void InstantiateVid(Node node)
    {
        if (GameObject.Find("ScrubScreenBig(Clone)"))
        {
            Destroy(GameObject.Find("ScrubScreenBig(Clone)"));
            return;
        }
        Debug.Log("Instantiate Video");
        Vector3 instantiatePoint = panel.transform.position;
        instantiatePoint.y = instantiatePoint.y - 400;
        instantiatePoint.x = instantiatePoint.x - 1000;
        //GameObject go = Instantiate(preview, instantiatePoint, Quaternion.identity, GameObject.Find("Progress").transform);
        //GameObject go = Instantiate(preview, GameObject.Find("aButton(Clone)").transform, false);
        GameObject go = Instantiate(preview, panel.transform, false);

        Debug.Log("Initial POS: " + go.GetComponent<Transform>().position);
        instantiatedPreview = go;
        if (graph.aNodes.Contains(node))
        {
            VideoClip vid = Clips[graph.aNodes.IndexOf(node)];
            instantiatedPreview.GetComponent<Transform>().localPosition = instantiatePoint;
            instantiatedPreview.GetComponent<VideoPlayer>().clip = vid;
            StartCoroutine(VidTimer(vid));
        }
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
            return;
        }
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
        public String File { get; set; }
        public List<GameObject> Highlights { get; set; }
    }

}

