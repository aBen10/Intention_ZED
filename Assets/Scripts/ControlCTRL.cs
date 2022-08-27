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
public class ControlCTRL : MonoBehaviour
{

    Graph graph;
    public GameObject panel;
    public List<VideoClip> eClips = new List<VideoClip>();
    public VideoPlayer vp;
    int x = 0;
    int eCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        eClips.AddRange(Resources.LoadAll<VideoClip>("iClips"));
        graph = new Graph();
        vp.clip = eClips[eCount];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) && x < 1)
        {   
            Next();
            x++;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            x = 0;
        }
    }

    public void Next()
    {
        Debug.Log("Next");
        //GameObject.Find("BotLineDiag").SetActive(true);
        if (eCount + 1 == graph.iNodes.Count)
        {
            Debug.Log("No more Nodes");
            return;
        }
        else
        {
            eCount++;
        }
        vp.clip = eClips[eCount];
        Debug.Log("End of next");
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

