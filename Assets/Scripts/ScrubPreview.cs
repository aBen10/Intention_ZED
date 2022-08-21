using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScrubPreview : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public GameObject childPrefab, instantiatedChild;

    //script of the child to get the drag method
    //private child childScript;

    void Start()
    {
        //cache the prefab
        childPrefab = Resources.Load("child") as GameObject;
    }

    public void OnPointerDown(PointerEventData data)
    {
        print("mouse down");
        StartCoroutine("Instantiator");
    }

    public void OnBeginDrag(PointerEventData data)
    {
        if (instantiatedChild != null)
        {
            //childScript = instantiatedChild.GetComponent<child>();
        }
    }
    public void OnDrag(PointerEventData data)
    {
        if (instantiatedChild != null)
        {
            //childScript.OnDrag(data);
        }
    }
    public void OnEndDrag(PointerEventData data)
    {
        instantiatedChild = null;
        //childScript = null;
    }

    public void OnPointerUp(PointerEventData data)
    {
        //if mouse Up is before 2 seconds, it will not instantiate
        StopCoroutine("Instantiator");
    }

    IEnumerator Instantiator()
    {
        yield return new WaitForSeconds(2);
        GameObject go = Instantiate(childPrefab, transform.parent, false) as GameObject;
        instantiatedChild = go;
    }
}