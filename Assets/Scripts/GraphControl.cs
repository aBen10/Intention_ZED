/*using System;
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


public class GraphControl : MonoBehaviour
{

    Graph graph;
    private GameObject iButton, eButton, aButton, pButton;
    public GameObject panel, cup;
    public GameObject preview, instantiatedPreview;
    public Material lineMaterial;
    private GameObject line, lineDiag;
    private Vector3 Elast, Enow, Enext, Ilast, Inow, Inext, Alast, Anow, Anext, Pnow, Pnext;
    int count = 0;
    int x = 0;
    int drawCount = 0;
    int eCount, iCount, aCount, pCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        iButton = Resources.Load("iButton") as GameObject;
        eButton = Resources.Load("eButton") as GameObject;
        aButton = Resources.Load("aButton") as GameObject;
        pButton = Resources.Load("pButton") as GameObject;
        line = Resources.Load("LineDown") as GameObject;
        lineDiag = Resources.Load("LineDiag") as GameObject;
        preview = Resources.Load("ScrubScreenBig") as GameObject;
        graph = new Graph();
        Elast = new Vector3(-900, 700, 0);
        Enow = new Vector3(-300, 700, 0);
        Enext = new Vector3(300, 700, 0);
        Ilast = new Vector3(-900, 134, 0);
        Inow = new Vector3(-300, 134, 0);
        Inext = new Vector3(300, 134, 0);
        Alast = new Vector3(-900, -432, 0);
        Anow = new Vector3(-300, -432, 0);
        Anext = new Vector3(300, -432, 0);
        Pnow = new Vector3(-300, -998, 0);
        Pnext = new Vector3(300, -998, 0);

        var e1 = new Node() { NodeColor = Color.red, Position = Enow };
        var e2 = new Node() { NodeColor = Color.red, Position = Enext };
        var i1 = new Node() { NodeColor = Color.blue, Position = Inow, Parent = e1 };
        var i2 = new Node() { NodeColor = Color.blue, Position = Inext, Parent = e1 };
        var a1 = new Node() { NodeColor = Color.green, Position = Anow, Parent = i1, };
        var a2 = new Node() { NodeColor = Color.green, Position = Anext, Parent = i1 };
        var a3 = new Node() { Parent = i1 };
        var a4 = new Node() { Parent = i1 };
        var p1 = new Node() { NodeColor = Color.yellow, Position = Pnow, Parent = a1 };
        var p2 = new Node() { NodeColor = Color.yellow, Position = Pnext, Parent = a1 };
        var p3 = new Node() { Parent = a1 };
        var p4 = new Node() { Parent = a2 };
        var p5 = new Node() { Parent = a2 };

        p1.Highlights.Add(cup);

        graph.eNodes.Add(e1); graph.eNodes.Add(e2);
        graph.iNodes.Add(i1); graph.iNodes.Add(i2);
        graph.aNodes.Add(a1); graph.aNodes.Add(a2); graph.aNodes.Add(a3); graph.aNodes.Add(a4);
        graph.pNodes.Add(p1); graph.pNodes.Add(p2); graph.pNodes.Add(p3); graph.pNodes.Add(p4); graph.pNodes.Add(p5);

        Debug.Log("pNodes: " + graph.pNodes.Count + " aNodes: " + graph.aNodes.Count + " iNodes: " + graph.iNodes.Count + " eNodes: " + graph.eNodes.Count);
        Build();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.T) && x < 1)
        {
            Debug.Log("Next");
            GameObject.Find("BotLineDiag").GetComponent<Image>().enabled = true;
            //GameObject.Find("BotLineDiag").SetActive(true);
            Next();
            x++;

        }
        if (Input.GetKey(KeyCode.R))
        {
            x = 0;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (cup.GetComponentInChildren<Outline>().isActiveAndEnabled == true)
            {
                cup.GetComponentInChildren<Outline>().enabled = false;
            }
            else
            {
                cup.GetComponentInChildren<Outline>().enabled = true;
            }
            //cup.GetComponentInChildren<Outline>().enabled = !cup.GetComponentInChildren<Outline>().enabled;
        }
    }

    private void Next()
    {
        if (pCount + 1 == graph.pNodes.Count)
        {
            Debug.Log("No more pNodes");
            return;
        }

        if (graph.pNodes[pCount].Parent == graph.pNodes[pCount + 1].Parent)
        {
            //tracks index in pNodes of the current node (the bottom left button)
            pCount++;
            graph.pNodes[pCount].Position = Pnow;
            graph.pNodes[pCount + 1].Position = Pnext;
            if (graph.pNodes[pCount + 1] != null && graph.pNodes[pCount].Parent != graph.pNodes[pCount + 1].Parent)
            {
                GameObject.Find("BotLineDiag").GetComponent<Image>().enabled = false;
                //GameObject.Find("BotLineDiag").SetActive(false);
            }
            //else GameObject.Find("BotLineDiag").SetActive(true);
        }
        else if (graph.aNodes[aCount].Parent == graph.aNodes[aCount + 1].Parent)
        {
            aCount++;
            pCount++;
            graph.aNodes[aCount].Position = Anow;
            graph.aNodes[aCount + 1].Position = Anext;
            graph.pNodes[pCount].Position = Pnow;
            graph.pNodes[pCount + 1].Position = Pnext;
        }

        else
        {
            Debug.Log("End of graph");
        }
        Build();
    }

    private void Build()
    {
        int i = 0;
        //drawCount = 0;
        ClearNodes();
        while (i < 2)
        {
            DrawNode(graph.eNodes[eCount + i]);
            DrawNode(graph.iNodes[iCount + i]);
            DrawNode(graph.aNodes[aCount + i]);
            DrawNode(graph.pNodes[pCount + i]);
            i++;
        }
    }

    private void ClearNodes()
    {
        GameObject[] nodes = GameObject.FindGameObjectsWithTag("GraphButton");
        foreach (var node in nodes)
        {
            Destroy(node);
        }
    }

    private void DrawNode(Node node)
    {

        GameObject but;
        if (graph.pNodes.Contains(node))
        {
            but = Instantiate(pButton, panel.transform, false);
            if (node == graph.pNodes[pCount])
            {
                but.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "P" + (pCount + 1);
                but.gameObject.GetComponent<Button>().onClick.AddListener(() => HighlightObj(node));
                Debug.Log("Drawing p node: " + pCount + " at position: " + node.Position);
            }
            else
            {
                but.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "P" + (pCount + 2);
                Debug.Log("Drawing p node: " + (pCount + 1) + " at position: " + node.Position);
            }

        }
        else if (node.Position.y < 0)
        {
            but = Instantiate(aButton, panel.transform, false);
            if (node == graph.aNodes[aCount])
            {
                but.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "A" + (aCount + 1);
                but.gameObject.GetComponent<Button>().onClick.AddListener(() => InstantiateVid(node));
                Debug.Log("Drawing a node: " + aCount + " at position: " + node.Position);
            }
            else
            {
                but.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "A" + (aCount + 2);
                Debug.Log("Drawing a node: " + (aCount + 1) + " at position: " + node.Position);
            }
        }
        else if (node.Position.y < 500)
        {
            but = Instantiate(iButton, panel.transform, false);
            if (node == graph.iNodes[iCount])
            {
                but.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "I" + (iCount + 1);
                Debug.Log("Drawing i node: " + iCount + " at position: " + node.Position);
            }
            else
            {
                but.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "I" + (iCount + 2);
                Debug.Log("Drawing i node: " + (iCount + 1) + " at position: " + node.Position);
            }
        }
        else
        {
            but = Instantiate(eButton, panel.transform, false);
            //but.GetComponentInChildren<TextMeshPro>().text = "test";

        }
        but.GetComponent<Transform>().localPosition = panel.transform.position + node.Position;
        but.GetComponent<MeshRenderer>().material.SetColor("_Color", node.NodeColor);
        drawCount++;
        //Debug.Log("Drawing");
    }

    private void HighlightObj(Node node)
    {
        Debug.Log("Highlighting");
        foreach (GameObject n in node.Highlights)
        {
            Debug.Log("Highlighting: " + n.name);
            if (n.GetComponentInChildren<Outline>().isActiveAndEnabled == true)
            {
                n.GetComponentInChildren<Outline>().enabled = false;
            }
            else
            {
                n.GetComponentInChildren<Outline>().enabled = true;
            }
        }


    }

    private void InstantiateVid(Node node)
    {
        if (GameObject.Find("ScrubScreenBig(Clone)"))
        {
            Destroy(GameObject.Find("ScrubScreenBig(Clone)"));
            return;
        }
        Debug.Log("Instantiate Video");
        Vector3 instantiatePoint = node.Position;
        instantiatePoint.y = instantiatePoint.y + 400;
        //GameObject go = Instantiate(preview, instantiatePoint, Quaternion.identity, GameObject.Find("Progress").transform);
        GameObject go = Instantiate(preview, GameObject.Find("aButton(Clone)").transform, false);

        Debug.Log("Initial POS: " + go.GetComponent<Transform>().position);
        instantiatedPreview = go;
        instantiatedPreview.GetComponent<Transform>().localPosition = instantiatePoint;
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

*/