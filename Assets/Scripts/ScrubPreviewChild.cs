using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScrubPreviewChild : MonoBehaviour, IDragHandler
{
    private Transform thisTransRef;

    void Start()
    {
        thisTransRef = transform;
    }

    public void OnDrag(PointerEventData eventData)
    {
        var world_point_mousePos = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, Mathf.Abs(Camera.main.transform.position.z)));

        thisTransRef.position = world_point_mousePos;
    }

}
