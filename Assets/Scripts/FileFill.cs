using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileFill : MonoBehaviour
{
    public GameObject button; 
    public GameObject menu; 

    // Start is called before the first frame update
    void Start()
    {
        //button = GameObject.Find("availableFileButton");
        menu = GameObject.Find("Files Canvas");
        LoadFile();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadFile()
    {
        Debug.Log("Loading");
        //public static string[] GetFiles (string path, string searchPattern);
        List<string> files = new List<string>(Directory.GetFiles(@"D:\Downloads", "*.STL"));
        //List<string> files2 = new List<string>(Directory.GetFiles(@"D:\Downloads", "*.stl"));
        List<string> files3 = new List<string>(Directory.GetFiles(@"D:\Downloads", "*.gcode"));
        //List<string> files4 = new List<string>(Directory.GetFiles(@"/home/moiz/Downloads", "*.GCO"));
        //files.AddRange(files2);
        files.AddRange(files3);
        //files.AddRange(files4);
        /*foreach (string file in files)
        {
            Debug.Log("Available file: " + Path.GetFileName(file));
        }*/
        Debug.Log("In files: " + files[0] + files[1] + files[2]);
        //Debug.Log(files[1]);
        int counter = 0;
        foreach (string file in files)
        {
            counter++;
            Debug.Log(counter);
            string fileName = Path.GetFileName(file);
            Debug.Log("Available file: " + fileName);
            GameObject option1 = Instantiate(button, new Vector3((float)-1.314, (float)(1.743-(counter*.3)),(float) 1.497), Quaternion.identity, menu.transform);
            System.Func<System.Type> test = option1.GetType;
            Debug.Log("Test type: " + test);
            GameObject changer = GameObject.Find("TestButton(Clone)");

            option1.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = " " + fileName;
            //changer.GetComponentInChildren<TextMesh>().text = fileName;
            //option1.transform.position += Vector3.down;
        }
    }
}
