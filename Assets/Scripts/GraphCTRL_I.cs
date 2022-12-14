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
public class GraphCTRL_I : MonoBehaviour
{

    Graph graph;
    private GameObject eButton, eButton2, iButton, iButton2, aButton, aButton2, TestB;
    public GameObject panel;
    public GameObject preview, instantiatedPreview, WristScreen, GraphScreen, GraphImage;
    public Material lineMaterial;
    private GameObject line, lineDiag;
    private Vector3 Elast, Enow, Enext, Ilast, Inow, Inext, Alast, Anow, Anext;
    private Vector3 eVidRightPos, eVidLeftPos, iVidLeftPos, iVidRightPos, aVidLeftPos, aVidRightPos;
    private GameObject eVidRight, eVidLeft, iVidLeft, iVidRight, aVidLeft, aVidRight;
    public List<VideoClip> aClips = new List<VideoClip>();
    public List<VideoClip> eClips = new List<VideoClip>();
    public List<VideoClip> iClips = new List<VideoClip>();
    public List<Sprite> eImages = new List<Sprite>();
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
        eImages.AddRange(Resources.LoadAll<Sprite>("eImages"));
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
        aButton = Resources.Load("aButton") as GameObject;
        aButton2 = Resources.Load("aButton2") as GameObject;
        line = Resources.Load("LineDown") as GameObject;
        lineDiag = Resources.Load("LineDiag") as GameObject;
        preview = Resources.Load("ButtonScreen") as GameObject;
        GraphScreen = Resources.Load("GraphScreen") as GameObject;
        GraphImage = Resources.Load("GraphImage") as GameObject;
        //Make graph, declare set vectors for buttons, add nodes to graph (these will later be defined by the user study task)
        graph = new Graph();
        Enow = new Vector3(-240, 250, 0);
        Ilast = new Vector3(-400, -320, 0); Inow = new Vector3(-282, -320, 0); Inext = new Vector3(300, -320, 0);
        eVidLeftPos = Enow - new Vector3(719, 0,0); eVidRightPos = Enow + new Vector3(1336, 0, 0);
        iVidLeftPos = Inow - new Vector3(798, 0, 0); iVidRightPos = Inow + new Vector3(1379, 0, 0);
        //Pnow = new Vector3(-300, -998, 0);
        //Pnext = new Vector3(300, -998, 0);
        eVidRight = Instantiate(GraphImage, panel.transform, false);
        eVidRight.transform.localPosition = eVidRightPos;

        MakeNodes();
        MakeScreens();
        Debug.Log(" aNodes: " + graph.aNodes.Count + " iNodes: " + graph.iNodes.Count + " eNodes: " + graph.eNodes.Count);
        Build();
        /*GameObject.Find("LineDiag2").GetComponent<Image>().enabled = false;
        GameObject.Find("LineDiag2Top").GetComponent<Image>().enabled = false;
        GameObject.Find("LineDownNext").GetComponent<Image>().enabled = false;
        GameObject.Find("LineDownPrev").GetComponent<Image>().enabled = false;*/
        //WristScreen.GetComponent<VideoPlayer>().clip = iClips[iCount];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {           
            Next();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Prev();
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            if (iVidLeft.GetComponent<VideoPlayer>().frame > 30)
            {
                iVidLeft.GetComponent<VideoPlayer>().frame = iVidLeft.GetComponent<VideoPlayer>().frame - 30;
            }
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            if (iVidLeft.GetComponent<VideoPlayer>().frame < (long)iVidLeft.GetComponent<VideoPlayer>().clip.frameCount - 20)
            {
                iVidLeft.GetComponent<VideoPlayer>().frame = iVidLeft.GetComponent<VideoPlayer>().frame + 20;
            }
        }
    }

    /*transitions to next affordance/gesutre (A/G). aCount, iCount, and eCount keep track of current A/G and thier parents. This code uses checks on parents
     * to figure out how whether it's just the next A/G, or the user is moving to the next interaction or even the next event. The body
     * of the "if statements" determins what the graph should look like based on the case. Currently breaks at the end of the graph. Waiting 
     * to see how user study takes shape before making more changes
    */
    public void Next()
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
        GameObject.Find("LineDiagTop").GetComponent<Image>().enabled = true;
        if (iCount + 1 == graph.iNodes.Count)
        {
            Debug.Log("No more Nodes");
            return;
        }
        else if (graph.iNodes[iCount].Parent == graph.iNodes[iCount + 1].Parent)
        {
            iCount++;
            graph.iNodes[iCount - 1].Position = Ilast;
            graph.iNodes[iCount].Position = Inow;
            if (graph.iNodes.Count > iCount + 1) { graph.iNodes[iCount + 1].Position = Inext; }

            Debug.Log("I SWITCH");
            GameObject.Find("LineDiagTop").GetComponent<Image>().enabled = true;
            if (graph.iNodes.Count > iCount + 1 && graph.iNodes[iCount].Parent != graph.iNodes[iCount + 1].Parent)
            {
                GameObject.Find("iArrow").GetComponent<Image>().enabled = false;
                GameObject.Find("LineDiagTop").GetComponent<Image>().enabled = false;
                GameObject.Find("eArrow").GetComponent<Image>().color = Color.yellow;

            }

        }
        //if current interaction does not have the same parent event as the next interaction #3
        else if (graph.iNodes[iCount].Parent != graph.iNodes[iCount + 1].Parent)
        {
            eCount++;
            iCount++;
            graph.eNodes[eCount-1].Position = Elast;
            graph.eNodes[eCount].Position = Enow;
            graph.iNodes[iCount - 1].Position = Ilast;
            graph.iNodes[iCount].Position = Inow;
            graph.iNodes[iCount + 1].Position = Inext;
            Debug.Log("E SWITCH");
            //GameObject.Find("LineDiag2").GetComponent<Image>().enabled = false;
            //GameObject.Find("LineDiagTop").GetComponent<Image>().enabled = true;
            //GameObject.Find("LineDownPrev").GetComponent<Image>().enabled = false;
            //GameObject.Find("LineDownNext").GetComponent<Image>().enabled = true;
            //GameObject.Find("LineDiag2Top").GetComponent<Image>().enabled = false;
            GameObject.Find("iArrow").GetComponent<Image>().enabled = true;
            GameObject.Find("eArrow").GetComponent<Image>().color = Color.white;
            Build();
            MakeScreens();
            //leftButtons.SetActive(false);
            return;
        }
        else
        {
            Debug.Log("End of graph");
            return;
        }
        GameObject.Find("eArrow").GetComponent<Image>().color = Color.white;
        Build();
        MakeScreens();
        Debug.Log("End of next");
    }

    public void Prev()
    {
        if (iCount == 0)
        {
            return;
        }
        else
        {
            if (GameObject.Find("GraphScreen(Clone)"))
            {
                GameObject[] screens = GameObject.FindGameObjectsWithTag("GraphScreen");
                foreach (var tv in screens)
                {
                    Destroy(tv);
                }
            }
            iCount--;
            graph.iNodes[iCount].Position = Inow;
            graph.iNodes[iCount + 1].Position = Inext;
            if(eCount != 0)
            {
                eCount--;
                graph.eNodes[eCount].Position = Enow;
            }
        }
        GameObject.Find("iArrow").GetComponent<Image>().enabled = true;
        GameObject.Find("LineDiagTop").GetComponent<Image>().enabled = true;
        Build();
        MakeScreens();
    }

    //Clears graph button game objects then draws the new current node and past/future nodes if applicable
    private void Build()
    {
        ClearNodes();
        DrawNode(graph.eNodes[eCount]);
        DrawNode(graph.iNodes[iCount]);
        //DrawNode(graph.aNodes[aCount]);
        if (graph.iNodes.Count > iCount + 1) {DrawNode(graph.iNodes[iCount + 1]);}
        //if (graph.aNodes.Count > aCount + 1) { DrawNode(graph.aNodes[aCount + 1]); }
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
        if (graph.iNodes.Contains(node))
        {
            if (node == graph.iNodes[iCount])
            {
                but = Instantiate(iButton, panel.transform, false);
                but.GetComponentInChildren<TMPro.TextMeshPro>().text = "I" + (iCount);
                //Debug.Log("Drawing i node: " + iCount + " at position: " + node.Position);
            }
            else if (graph.iNodes.Count > iCount + 1 && node == graph.iNodes[iCount + 1])
            {
                but = Instantiate(iButton2, panel.transform, false);
                but.GetComponentInChildren<TMPro.TextMeshPro>().text = "I" + (iCount + 1);
                //Debug.Log("Drawing i node: " + (iCount + 1) + " at position: " + node.Position);
            }
            else
            {
                but = Instantiate(iButton2, panel.transform, false);
                but.GetComponentInChildren<TMPro.TextMeshPro>().text = "I" + (iCount - 1);
                //Debug.Log("Drawing i node: " + (iCount - 1) + " at position: " + node.Position);
            }
        }
        else
        {
            but = Instantiate(eButton, panel.transform, false);
            //but.GetComponentInChildren<TextMeshPro>().text = "test";

        }
        //ButData data = but.GetComponent<ButData>();
        //data.node = node;
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
        iVidLeft = Instantiate(GraphScreen, panel.transform, false);
        iVidLeft.GetComponent<VideoPlayer>().clip = iClips[iCount];
        iVidLeft.GetComponent<Transform>().localPosition = iVidLeftPos;

        iVidRight = Instantiate(GraphScreen, panel.transform, false);
        // aVidRight.GetComponent<VideoPlayer>().clip = aClips[0];
        iVidRight.GetComponent<Transform>().localPosition = iVidRightPos;
        concat = iVidRight.GetComponent<ConcatVideos>();
        if(iClips.Count > iCount + 1)
        {
            concat.PlayBackToBack(iClips[iCount], iClips[iCount + 1], 5.0, 5.0);
        }
        eVidRight.GetComponentInChildren<Image>().sprite = eImages[eCount];
    }

    private void MakeNodes()
    {
        //5 events
        var e0 = new Node() { Position = Enow, Name = "Base" }; var e1 = new Node() { Name = "Printer" }; var e2 = new Node() { Name = "Stand" }; var e3 = new Node() { Name = "Arm" }; var e4 = new Node() { Name = "Mount" };
        //Event 0 has 5 interactions
        var i0 = new Node() { Position = Inow, Parent = e0, Name = "Move base to desk" }; var i1 = new Node() { Position = Inext, Parent = e0, Name = "Fix to desk w clamp A" }; var i2 = new Node() { Parent = e0, Name = "Fix to desk w clamp B" }; var i3 = new Node() { Parent = e0, Name = "Hammer to test" };
        //Event 1 has 8 interactions
        var i5 = new Node() { Parent = e1, Name = "Reach back of printer" }; var i6 = new Node() { Parent = e1, Name = "Attach roll 1" }; var i7 = new Node() { Parent = e1, Name = "Attach roll 2" }; var i8 = new Node() { Parent = e1, Name = "Attach roll 3" }; var i9 = new Node() { Parent = e1, Name = "Reach front of printer" }; var i10 = new Node() { Parent = e1, Name = "Insert thumb drive" }; var i11 = new Node() { Parent = e1, Name = "Turn on printer" }; var i12 = new Node() { Parent = e1, Name = "Grab the ruler" };
        //Event 2 has 4 interactions
        var i13 = new Node() { Parent = e2, Name = "Measure 1ft. in" }; var i26 = new Node() { Parent = e2, Name = "Mark location" }; var i14 = new Node() { Parent = e2, Name = "Move screw nut" }; var i15 = new Node() { Parent = e2, Name = "Align hole" }; var i16 = new Node() { Parent = e2, Name = "Fasten screw" };
        //Event 3 has 3 interactions
        var i17 = new Node() { Parent = e3, Name = "Measure 1ft. from top" }; var i27 = new Node() { Parent = e3, Name = "Mark location" }; var i28 = new Node() { Parent = e3, Name = "Remove ruler" }; var i18 = new Node() { Parent = e3, Name = "Slide arm in" }; var i19 = new Node() { Parent = e3, Name = "Fasten screw" };
        //Event 4 has 6 interactions
        var i20 = new Node() { Parent = e4, Name = "Slide mount onto arm" }; var i21 = new Node() { Parent = e4, Name = "Fix mount w screwdriver" }; var i22 = new Node() { Parent = e4, Name = "Align cardboard" }; var i23 = new Node() { Parent = e4, Name = "Drill holes" }; var i29 = new Node() { Parent = e4, Name = "Insert screws" }; var i24 = new Node() { Parent = e4, Name = "Fix plate with wrench" }; var i30 = new Node() { Parent = e4, Name = "Finish" };

        graph.eNodes.Add(e0); graph.eNodes.Add(e1); graph.eNodes.Add(e2); graph.eNodes.Add(e3); graph.eNodes.Add(e4);
        graph.iNodes.Add(i0); graph.iNodes.Add(i1); graph.iNodes.Add(i2); graph.iNodes.Add(i3); graph.iNodes.Add(i5); graph.iNodes.Add(i6); graph.iNodes.Add(i7); graph.iNodes.Add(i8); graph.iNodes.Add(i9); graph.iNodes.Add(i10); graph.iNodes.Add(i11); graph.iNodes.Add(i12); graph.iNodes.Add(i13); graph.iNodes.Add(i26); graph.iNodes.Add(i14); graph.iNodes.Add(i15); graph.iNodes.Add(i16); 
        graph.iNodes.Add(i17); graph.iNodes.Add(i27); graph.iNodes.Add(i28); graph.iNodes.Add(i18); graph.iNodes.Add(i19); graph.iNodes.Add(i20); graph.iNodes.Add(i21); graph.iNodes.Add(i22); graph.iNodes.Add(i23); graph.iNodes.Add(i29); graph.iNodes.Add(i24); graph.iNodes.Add(i30);

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

