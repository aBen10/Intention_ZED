using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class httpRequest : MonoBehaviour
{
    private const string Url = "https://c36caa9cc63c202d0be74d9ad9b5a255.balena-devices.com";
    private const string API_KEY = "1khk6PUFAB6Dw7nDeGCLkoeHhWMsF8TR";
    private const string json = "application / json";
    GameObject print_bed;
    private Vector3 pos;
    private float pos_y;
    private string valueOfPoition;
    private bool toggle=false; 

    // Start is called before the first frame update
    void Start()
    {
        print_bed = GameObject.Find("Cube");
        //StartCoroutine(PostRequest(Url + "/api/printer/printhead", "{\"command\": \"jog\",\"z\": -15.0}"));
    }

    // Update is called once per frame
    void Update()
    {
        pos = print_bed.transform.position;
        pos_y = pos.y;
        pos_y = pos_y * 10;
        pos_y = limit(pos_y);
        Debug.Log(pos_y);
        OnPress(pos_y);
        //form.AddField("Position",pos.y);

        //WWW
    }

    float limit(float y)
    {
        if (y > 200.0f)
        {
            y = 200.0f;
        }
        if (y < 0.0f)
        {
            y = 0.1f;
        }
        return y;
    }

    void OnPress(float y)
    {
        
        string value_y = y.ToString();
        //if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger) && toggle==false)
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
        {
            toggle = true;
            StartCoroutine(PostRequest(Url + "/api/printer/printhead", "{\"command\": \"jog\",\"z\":-" + value_y + "}"));
            Debug.Log("Trigger is pressed");
        }
        //else if (OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger))
         else if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow))
        {
            toggle = false;            
            Debug.Log("Trigger is released");
        }
    }

    IEnumerator PostRequest(string url, string json)
    {
        var uwr = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");
        uwr.SetRequestHeader("X-Api-Key", API_KEY);

        //Send the request then wait here until it returns
        yield return uwr.SendWebRequest();

        //if (uwr.isNetworkError) - Depreciated
        if(uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.Log("Received: 123" + uwr.downloadHandler.text);         
        }
    }
}
