using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class RefreshButton : MonoBehaviour
{

    public GameObject button;
    // Start is called before the first frame update
    void Start()
    {
        button = GameObject.Find("RButton");
        button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "\u21BB";
        button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "\u27F3";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
