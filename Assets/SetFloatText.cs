using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetFloatText : MonoBehaviour
{
    TMPro.TextMeshPro textBox;
    int count = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        textBox = GetComponent<TMPro.TextMeshPro>();
    }

    void Next()
    {
        SetText("Next");
    }
    
    void Prev()
    {
        SetText("Prev");
    }

    void SetText(string str)
    {
        count++;
        textBox.text = str + " " + count.ToString();
    }
}
