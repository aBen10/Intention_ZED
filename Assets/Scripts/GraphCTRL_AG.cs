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
public class GraphCTRL_AG : MonoBehaviour
{

    Graph graph;
    private GameObject eButton, eButton2, iButton, iButton2, aButton, aButton2, TestB;
    public GameObject panel, rightButtons, leftButtons;
    public GameObject preview, instantiatedPreview, WristScreen, GraphScreen, GraphImage;
    public Material lineMaterial;
    private GameObject line, lineDiag;
    private Vector3 Elast, Enow, Enext, Ilast, Inow, Inext, Alast, Anow, Anext;
    private Vector3 eVidRightPos, eVidLeftPos, iVidLeftPos, iVidRightPos, aVidLeftPos, aVidRightPos;
    private GameObject eVidRight, eVidLeft, iVidLeft, iVidRight, aVidLeft, aVidRight;
    public Sprite dne;
    public List<VideoClip> aClips = new List<VideoClip>();
    public List<VideoClip> eClips = new List<VideoClip>();
    public List<VideoClip> iClips = new List<VideoClip>();
    public List<Sprite> eImages = new List<Sprite>();
    public List<Sprite> iImages = new List<Sprite>();
    public List<Sprite> aImages = new List<Sprite>();

    ConcatVideos concat;
    int count = 0;
    int x = 0;
    int drawCount = 0;
    int eCount, iCount, aCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Loads clips from Resources folder into list
        iClips.AddRange(Resources.LoadAll<VideoClip>("iClips"));
        eImages.AddRange(Resources.LoadAll<Sprite>("eImages"));
        aImages.AddRange(Resources.LoadAll<Sprite>("aImages"));
        Debug.Log(iClips.Count);
        Debug.Log(iClips[0]);
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
        Enow = new Vector3(-282, 470, 0); 
        Ilast = new Vector3(-400, 134, 0); Inow = new Vector3(-282, -90, 0); Inext = new Vector3(528, 122, 0);
        Alast = new Vector3(-400, -432, 0); Anow = new Vector3(-282, -650, 0); Anext = new Vector3(528, -432, 0);
        eVidLeftPos = Enow - new Vector3(719,0,0); eVidRightPos = Enow + new Vector3(1300, 0, 0);
        iVidLeftPos = Inow - new Vector3(719, 0, 30); iVidRightPos = Inow + new Vector3(1300, 0, -30);
        aVidLeftPos = Anow - new Vector3(719, 0, 0); aVidRightPos = Anow + new Vector3(1300, 0, 0);

        eVidRight = Instantiate(GraphImage, panel.transform, false);
        eVidRight.transform.localPosition = eVidRightPos;
        aVidLeft = Instantiate(GraphImage, panel.transform, false);
        aVidLeft.transform.localPosition = aVidLeftPos;
        aVidRight = Instantiate(GraphImage, panel.transform, false);
        aVidRight.transform.localPosition = aVidRightPos;
        MakeNodes();
        MakeScreens();
        Debug.Log(" aNodes: " + graph.aNodes.Count + " iNodes: " + graph.iNodes.Count + " eNodes: " + graph.eNodes.Count);
        Build();
        //WristScreen.GetComponent<VideoPlayer>().clip = aClips[aCount];
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
        GameObject.Find("LineDiag").GetComponent<Image>().enabled = true;
        GameObject.Find("LineDiagTop").GetComponent<Image>().enabled = true;
        GameObject.Find("iArrow").GetComponent<Image>().enabled = true;

        if (GameObject.Find("GraphScreen(Clone)"))
        {
            GameObject[] screens = GameObject.FindGameObjectsWithTag("GraphScreen");
            foreach (var tv in screens)
            {
                Destroy(tv);
            }
        }
        if (aCount + 1 == graph.aNodes.Count)
        {
            Debug.Log("No more aNodes");
            return;
        }
        //If the current A/G and the next A/G have the same parent interaction
        if (graph.aNodes[aCount].Parent == graph.aNodes[aCount + 1].Parent)
        {
            aCount++;
            graph.aNodes[aCount - 1].Position = Alast;
            graph.aNodes[aCount].Position = Anow;
            graph.aNodes[aCount + 1].Position = Anext;
            Debug.Log("A SWITCH");
            //GameObject.Find("LineDiag2").GetComponent<Image>().enabled = true;
            //GameObject.Find("LineDownNext").GetComponent<Image>().enabled = false;
            GameObject.Find("aArrow").GetComponent<Image>().enabled = true;
            //GameObject.Find("LineDownPrev").GetComponent<Image>().enabled = false;
            if (graph.aNodes.Count > aCount + 1 && graph.aNodes[aCount].Parent != graph.aNodes[aCount + 1].Parent)
            {
                GameObject.Find("LineDiag").GetComponent<Image>().enabled = false;
                //GameObject.Find("LineDownNext").GetComponent<Image>().enabled = true;
                GameObject.Find("aArrow").GetComponent<Image>().enabled = false;
            }
            //If it's the last A/G of the event
            if (graph.aNodes.Count > aCount + 1 && graph.aNodes[aCount].Parent.Parent != graph.aNodes[aCount + 1].Parent.Parent)
            {
                Debug.Log("E SWITCH UP NEXT");
                //GameObject.Find("LineDiag2").GetComponent<Image>().enabled = true;
                GameObject.Find("LineDiagTop").GetComponent<Image>().enabled = false;
                //GameObject.Find("LineDownPrev").GetComponent<Image>().enabled = false;
                //GameObject.Find("LineDownNext").GetComponent<Image>().enabled = true;
                GameObject.Find("aArrow").GetComponent<Image>().enabled = false;
                GameObject.Find("iArrow").GetComponent<Image>().enabled = false;
                GameObject.Find("eArrow").GetComponent<Image>().color = Color.yellow;
                //GameObject.Find("LineDiag2Top").GetComponent<Image>().enabled = true;
                Build();
                MakeScreens();
                //Destroy(GameObject.Find("iButton2(Clone)"));
                //Destroy(GameObject.Find("aButton2(Clone)").gameObject);
                //rightButtons.SetActive(false);
                return;
            }
        }
        else if (graph.aNodes[aCount].Parent.Parent == graph.aNodes[aCount + 1].Parent.Parent)
        {
            iCount++;
            aCount++;
            graph.iNodes[iCount - 1].Position = Ilast;
            graph.iNodes[iCount].Position = Inow;
            if (graph.iNodes.Count > iCount + 1) { graph.iNodes[iCount + 1].Position = Inext; }

            graph.aNodes[aCount - 1].Position = Alast;
            graph.aNodes[aCount].Position = Anow;
            if (graph.aNodes.Count > aCount + 1) { graph.aNodes[aCount + 1].Position = Anext; }

            Debug.Log("I SWITCH");
            //GameObject.Find("LineDiag2").GetComponent<Image>().enabled = false;
            //GameObject.Find("LineDownPrev").GetComponent<Image>().enabled = true;
            //GameObject.Find("LineDownNext").GetComponent<Image>().enabled = false;
            GameObject.Find("aArrow").GetComponent<Image>().enabled = true;
            //GameObject.Find("LineDiag2Top").GetComponent<Image>().enabled = true;
            //if there will be a new next A/G and if the new next A/G will have a different parent interaction than the new current A/G
            if (graph.aNodes.Count > aCount + 1 && graph.aNodes[aCount].Parent != graph.aNodes[aCount + 1].Parent)
            {
                GameObject.Find("LineDiag").GetComponent<Image>().enabled = false;
                //GameObject.Find("LineDownNext").GetComponent<Image>().enabled = true;
                GameObject.Find("aArrow").GetComponent<Image>().enabled = false;
            }
            if(graph.iNodes.Count > iCount + 1 && graph.iNodes[iCount].Parent != graph.iNodes[iCount + 1].Parent)
            {
                GameObject.Find("iArrow").GetComponent<Image>().enabled = false;
                GameObject.Find("LineDiagTop").GetComponent<Image>().enabled = false;
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
            //GameObject.Find("LineDiag2").GetComponent<Image>().enabled = false;
            GameObject.Find("LineDiag").GetComponent<Image>().enabled = false;
            GameObject.Find("LineDiagTop").GetComponent<Image>().enabled = true;
            //GameObject.Find("LineDownPrev").GetComponent<Image>().enabled = false;
            //GameObject.Find("LineDownNext").GetComponent<Image>().enabled = true;
            //GameObject.Find("LineDiag2Top").GetComponent<Image>().enabled = false;
            GameObject.Find("aArrow").GetComponent<Image>().enabled = false;
            GameObject.Find("iArrow").GetComponent<Image>().enabled = true;
            GameObject.Find("eArrow").GetComponent<Image>().color = Color.white;
            if (graph.iNodes.Count > iCount + 1 && graph.aNodes[aCount].Parent == graph.aNodes[aCount + 1].Parent)
            {
                GameObject.Find("aArrow").GetComponent<Image>().enabled = true;
                //GameObject.Find("LineDownNext").GetComponent<Image>().enabled = false;
                GameObject.Find("LineDiag").GetComponent<Image>().enabled = true;
            }

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
        Debug.Log("iCount: " + iCount + "\naCount: " + aCount);
    }

    public void Prev()
    {
        if (aCount == 0)
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
            aCount--;
            graph.aNodes[aCount].Position = Anow;
            graph.aNodes[aCount + 1].Position = Anext;
            if (iCount != 0 && graph.aNodes[aCount].Parent != graph.aNodes[aCount + 1].Parent)
            {
                iCount--;
                graph.iNodes[iCount].Position = Inow;
                graph.iNodes[iCount+1].Position = Inext;
                if (eCount != 0 && graph.iNodes[iCount].Parent != graph.iNodes[iCount + 1].Parent)
                {
                    eCount--;
                    graph.eNodes[eCount].Position = Enow;
                }
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
        DrawNode(graph.aNodes[aCount]);
        if (graph.iNodes.Count > iCount + 1) {DrawNode(graph.iNodes[iCount + 1]);}
        if (graph.aNodes.Count > aCount + 1 && graph.aNodes[aCount].Parent == graph.aNodes[aCount+1].Parent) 
        { 
            DrawNode(graph.aNodes[aCount + 1]); 
        }
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
        if (graph.aNodes.Contains(node))
        {
            if (node == graph.aNodes[aCount])
            {
                but = Instantiate(aButton, panel.transform, false);
                but.GetComponentInChildren<TMPro.TextMeshPro>().text = "A" + (aCount);
                //Debug.Log("Drawing a node: " + aCount + " at position: " + node.Position);
            }
            else if (graph.aNodes.Count > aCount + 1 && node == graph.aNodes[aCount + 1])
            {
                but = Instantiate(aButton2, rightButtons.transform, false);
                but.GetComponentInChildren<TMPro.TextMeshPro>().text = "A" + (aCount + 1);
                //but.gameObject.GetComponent<Button>().onClick.AddListener(() => InstantiateVid(node));
                //Debug.Log("Drawing a node: " + (aCount + 1) + " at position: " + node.Position);
            }
            else
            {
                but = Instantiate(aButton2, leftButtons.transform, false);
                but.GetComponentInChildren<TMPro.TextMeshPro>().text = "A" + (aCount - 1);
                //but.gameObject.GetComponent<Button>().onClick.AddListener(() => InstantiateVid(node));
                //Debug.Log("Drawing a node: " + (aCount - 1) + " at position: " + node.Position);
            }
        }
        else if (graph.iNodes.Contains(node))
        {
            if (node == graph.iNodes[iCount])
            {
                but = Instantiate(iButton, panel.transform, false);
                but.GetComponentInChildren<TMPro.TextMeshPro>().text = "I" + (iCount);
                //Debug.Log("Drawing i node: " + iCount + " at position: " + node.Position);
            }
            else if (graph.iNodes.Count > iCount + 1 && node == graph.iNodes[iCount + 1])
            {
                but = Instantiate(iButton2, rightButtons.transform, false);
                but.GetComponentInChildren<TMPro.TextMeshPro>().text = "I" + (iCount + 1);
                //Debug.Log("Drawing i node: " + (iCount + 1) + " at position: " + node.Position);
            }
            else
            {
                but = Instantiate(iButton2, leftButtons.transform, false);
                but.GetComponentInChildren<TMPro.TextMeshPro>().text = "I" + (iCount - 1);
                //Debug.Log("Drawing i node: " + (iCount - 1) + " at position: " + node.Position);
            }
        }
        else
        {
            but = Instantiate(eButton, panel.transform, false);
            //but.GetComponentInChildren<TextMeshPro>().text = "test";

        }
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
        //iVidRight.GetComponent<VideoPlayer>().clip = iClips[0];
        iVidRight.GetComponent<Transform>().localPosition = iVidRightPos;
        concat = iVidRight.GetComponent<ConcatVideos>();
        if(iClips.Count > iCount + 1)
        {
            concat.PlayBackToBack(iClips[iCount], iClips[iCount + 1], 4.0, 4.0);
        }
        eVidRight.GetComponentInChildren<Image>().sprite = eImages[eCount];
        
        if (aImages.Count > aCount + 1 && graph.aNodes[aCount].Parent == graph.aNodes[aCount+1].Parent)
        {
            aVidRight.GetComponentInChildren<Image>().sprite = aImages[aCount+1];
        }
        else if(aImages.Count > aCount + 1)
        {
            aVidRight.GetComponentInChildren<Image>().sprite = dne;
            GameObject.Find("LineDiag").GetComponent<Image>().enabled = false;
        }
        aVidLeft.GetComponentInChildren<Image>().sprite = aImages[aCount];
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

        //Each event 1 interaction has 1 aNode
        var a0 = new Node() { Position = Anow, Parent = i0, };  var a1 = new Node() { Position = Anext, Parent = i1 }; var a2 = new Node() { Parent = i2 }; var a3 = new Node() { Parent = i3 };
        //i5 has 2 aNodes, i6 has 1, i7 has 1, i8 has 1, i9 has 2, i10 has 1, i11 has 1, i12 has 1
        var a4 = new Node() { Parent = i5 }; var a5 = new Node() { Parent = i5 }; var a6 = new Node() { Parent= i6 }; var a7 = new Node() { Parent = i7 }; var a8 = new Node() { Parent = i8 }; var a9 = new Node() {Parent = i9}; var a10 = new Node() { Parent = i9 }; var a11 = new Node() { Parent = i10 }; var a12 = new Node() { Parent = i11 }; var a13 = new Node() { Parent = i12 };
        //i13 has 2, i14 has 1, i15 has 2, i16 has 1
        var a14 = new Node() { Parent = i13 }; /*var a15 = new Node() {Parent = i13 };*/ var a33 = new Node() { Parent = i26 }; var a16 = new Node() {Parent = i14 }; var a17 = new Node() {Parent = i15 }; var a18 = new Node() { Parent = i15 }; var a19 = new Node() {Parent = i16 }; 
        //i17 has 2, i18 has 1, i19 has 2
        var a20 = new Node() {Parent = i17 }; var a21 = new Node() {Parent = i27 }; var a39 = new Node() { Parent = i28 }; var a22 = new Node() { Parent = i18 };  var a23 = new Node() {Parent = i19 }; var a24 = new Node() { Parent = i19 };
        //i20 has 1, i21 has 2, i22 has 2, i23 has 1, i24 has 1, i25 has 1
        var a25 = new Node() {Parent = i20 }; var a26 = new Node() {Parent= i21 }; var a27 = new Node() {Parent = i21 }; var a28 = new Node() {Parent = i22 }; var a29 = new Node() { Parent= i22 }; var a30 = new Node() {Parent = i23 }; var a38 = new Node() { Parent = i29 }; var a31 = new Node() {Parent = i24 }; var a32 = new Node() { Parent = i30 }; var a34 = new Node() { Parent = i30 };


        graph.eNodes.Add(e0); graph.eNodes.Add(e1); graph.eNodes.Add(e2); graph.eNodes.Add(e3); graph.eNodes.Add(e4);
        graph.iNodes.Add(i0); graph.iNodes.Add(i1); graph.iNodes.Add(i2); graph.iNodes.Add(i3); graph.iNodes.Add(i5); graph.iNodes.Add(i6); graph.iNodes.Add(i7); graph.iNodes.Add(i8); graph.iNodes.Add(i9); graph.iNodes.Add(i10); graph.iNodes.Add(i11); graph.iNodes.Add(i12); graph.iNodes.Add(i13); graph.iNodes.Add(i26); graph.iNodes.Add(i14); graph.iNodes.Add(i15); graph.iNodes.Add(i16);
        graph.iNodes.Add(i17); graph.iNodes.Add(i27); graph.iNodes.Add(i28); graph.iNodes.Add(i18); graph.iNodes.Add(i19); graph.iNodes.Add(i20); graph.iNodes.Add(i21); graph.iNodes.Add(i22); graph.iNodes.Add(i23); graph.iNodes.Add(i29); graph.iNodes.Add(i24); graph.iNodes.Add(i30);
        
        graph.aNodes.Add(a0); graph.aNodes.Add(a1); graph.aNodes.Add(a2); graph.aNodes.Add(a3); graph.aNodes.Add(a4); graph.aNodes.Add(a5); graph.aNodes.Add(a6); graph.aNodes.Add(a7); graph.aNodes.Add(a8); graph.aNodes.Add(a9); graph.aNodes.Add(a10); graph.aNodes.Add(a11); graph.aNodes.Add(a12); graph.aNodes.Add(a13); graph.aNodes.Add(a14); graph.aNodes.Add(a33); /*graph.aNodes.Add(a15);*/ graph.aNodes.Add(a16); 
        graph.aNodes.Add(a17); graph.aNodes.Add(a18); graph.aNodes.Add(a19); graph.aNodes.Add(a20); graph.aNodes.Add(a21); graph.aNodes.Add(a39); graph.aNodes.Add(a22);  graph.aNodes.Add(a23); graph.aNodes.Add(a24); graph.aNodes.Add(a25); graph.aNodes.Add(a26); graph.aNodes.Add(a27); graph.aNodes.Add(a28); graph.aNodes.Add(a29); graph.aNodes.Add(a30); graph.aNodes.Add(a38); graph.aNodes.Add(a31); graph.aNodes.Add(a32); graph.aNodes.Add(a34);
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

