using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

/*Additions to be hardcoded based on task:
 * 
 * Second Progress bar for video review by learner with multiple instances for each video clip
 * Chapter marker that follows cursor on drag similar to video panel (used ot split video into clips by author)
 * VideoPlayer video populated from Unity Assets in Script
 */

public class VideoProgressBar : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField]
    private VideoPlayer videoPlayer;

    private Image progress;
    private float progressFill;

    public GameObject preview, instantiatedPreview;
    

    
    private void Start()
    {
        progress = GetComponent<Image>();
        preview = Resources.Load("ScrubScreenBig") as GameObject;
    }

    private void Update()
    {
        if (videoPlayer.frameCount > 0)
        {
            progress.fillAmount = (float)videoPlayer.frame / (float)videoPlayer.frameCount;
            progressFill = progress.fillAmount;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(instantiatedPreview == null)
        {
            ActivatePreview(eventData);
            Debug.Log("ACTIVATING");
        }

        
    }
    public void OnDrag(PointerEventData eventData)
    {
        
        TrySkip(eventData);

        Vector3 temp = instantiatedPreview.GetComponent<Transform>().position;
        float xD = eventData.delta.x * (float).02;
        temp.x += xD;
        Debug.Log("Initial: " + temp);
        Debug.Log("x Delta: " + xD);
        Debug.Log("Final: " + instantiatedPreview.GetComponent<Transform>().position);
        instantiatedPreview.GetComponent<Transform>().position = temp;
        //instantiatedPreview.GetComponent<Transform>().position += new Vector3(eventData.delta.x, instantiatedPreview.GetComponent<Transform>().position.y, instantiatedPreview.GetComponent<Transform>().position.z);
        //instantiatedPreview.GetComponent<Transform>().position.x += eventData.delta.x;
        //instantiatedPreview.GetComponent<Transform>().position += (Vector3)eventData.delta;

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //TrySkip(eventData);
        videoPlayer.frame = 0;
    }

    private void TrySkip(PointerEventData eventData)
    {
        Vector2 localPoint;
        //preview.SetActive(true);
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            progress.rectTransform, eventData.position, Camera.main, out localPoint))
        {
            float pct = Mathf.InverseLerp(progress.rectTransform.rect.xMin, progress.rectTransform.rect.xMax, localPoint.x);
            Debug.Log("BAR: " + localPoint); 
            SkipToPercent(pct);
            if(instantiatedPreview != null)
            {
                instantiatedPreview.GetComponent<VideoPlayer>().frame = videoPlayer.frame;
            }
            //StartCoroutine("Instantiator");
        }
        //preview.SetActive(false);
    }

    public void OnPointerUp(PointerEventData data)
    {
        //if mouse Up is before 2 seconds, it will not instantiate
        //StopCoroutine("Instantiator");
    }

    public void OnEndDrag(PointerEventData data)
    {
        instantiatedPreview = null;
        //preview = null;
        Destroy(GameObject.Find("ScrubScreenBig(Clone)"));
        Debug.Log("DESTROYED");
    }

    private void SkipToPercent(float pct)
    {
        var frame = videoPlayer.frameCount * pct;
        videoPlayer.frame = (long)frame;
    }

    private void ActivatePreview(PointerEventData eventData)
    {
        Vector3 instantiatePoint = progress.transform.position;
        Vector2 localPoint;
        instantiatePoint.y = (float)instantiatePoint.y - (float) 126;
        //instantiatePoint.x = (progress.rectTransform.rect.xMax - progress.rectTransform.rect.xMin)*progressFill + progress.rectTransform.rect.xMin;
        //instantiatePoint.x = eventData.position.x;
        //Debug.Log(eventData.position.x);
        //preview.SetActive(true);
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(progress.rectTransform, eventData.position, Camera.main, out localPoint))
        {
            instantiatePoint.x = localPoint.x;
            Debug.Log("Instanstiating AT: "+ localPoint);
        }
        else
        {
            instantiatePoint.x = -300;
        }
        Debug.Log(progress.rectTransform.rect.xMin);
        Debug.Log(progress.rectTransform.rect.xMax);
        Debug.Log(progressFill);
        Debug.Log(instantiatePoint.x);
        //GameObject go = Instantiate(preview, instantiatePoint, Quaternion.identity, GameObject.Find("Progress").transform);
        GameObject go = Instantiate(preview, GameObject.Find("Progress").transform, false);
        //private VideoPlayer vp = go.GetComponent<VideoPlayer>();
        //go.GetComponent(videoPlayer.frame) = videoPlayer.frame;
        go.GetComponent<VideoPlayer>().frame = videoPlayer.frame;
        //go.GetComponent<Transform>().position = instantiatePoint;
        
        Debug.Log("Initial POS: " + go.GetComponent<Transform>().position);
        instantiatedPreview = go;
        instantiatedPreview.GetComponent<Transform>().localPosition = instantiatePoint;
    }
/*
    IEnumerator Instantiator()
    {
        yield return new WaitForSeconds(1);
        Vector3 instantiatePoint = progress.transform.position;
        instantiatePoint.y = (float) instantiatePoint.y - (float) 126;
        GameObject go = Instantiate(preview, instantiatePoint, Quaternion.identity);
        //private VideoPlayer vp = go.GetComponent<VideoPlayer>();
        //go.GetComponent(videoPlayer.frame) = videoPlayer.frame;
        go.GetComponent<VideoPlayer>().frame = videoPlayer.frame;
        instantiatedPreview = go;
    }*/
}