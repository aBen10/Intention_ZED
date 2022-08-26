using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PlayBackSegment
{
    public VideoClip clip;
    public double start;
    public double end;

    public PlayBackSegment(VideoClip clip, double start, double end)
    {
        this.clip = clip;
        this.start = start;
        this.end = end;
    }
}
public class ConcatVideos : MonoBehaviour
{
    //[SerializeField]
    public VideoPlayer videoPlayer;

    List<PlayBackSegment> segments;
    int currentSegmentIdx = 0;

    Coroutine loopingCoro;

    List<VideoClip> aClips;

    // DELETE BELOW - IS TEST CODE ///////////////////////
    // Start is called before the first frame update
/*    void Start()
    {
        aClips = new List<VideoClip>();
        aClips.AddRange(Resources.LoadAll<VideoClip>("aClips"));
        PlayBackToBack(aClips[1], aClips[2], 5, 5);

        StartCoroutine(TestPlayLater());
    }

    IEnumerator TestPlayLater()
    {
        yield return new WaitForSeconds(20);
        PlayBackToBack(aClips[3], aClips[4], 5, 5);
    }*/
    // END DELETE BLOCK ////////////////////////////////////

    public void PlayBackToBack(VideoClip clip1, VideoClip clip2, double secondsBeforeEnd, double secondsAfterStart)
    {
        segments = new List<PlayBackSegment>();
        currentSegmentIdx = 0;

        PlayBackSegment seg = new PlayBackSegment(clip1, clip1.length - secondsBeforeEnd, clip1.length);
        segments.Add(seg);

        seg = new PlayBackSegment(clip2, 0, secondsAfterStart);
        segments.Add(seg);

        StartVideoLoop();
    }

    void StartVideoLoop()
    {
        if (loopingCoro != null)
            StopCoroutine(loopingCoro);

        PlayNextVideo();
    }

    void StopVideoLoop()
    {
        StopCoroutine(loopingCoro);
        loopingCoro = null;
        videoPlayer.Stop();
    }

    void PlayNextVideo()
    {
        PlayBackSegment currSegment = segments[currentSegmentIdx];
        loopingCoro = StartCoroutine(PlayVideo(currSegment.clip, currSegment.start, currSegment.end));
        currentSegmentIdx = (currentSegmentIdx + 1) % segments.Count;
    }

    IEnumerator PlayVideo(VideoClip vid, double start = 0, double end = 0)
    {
        videoPlayer.clip = vid;
        videoPlayer.time = start;
        if (end == 0)
        {
            end = vid.length;
        }
        yield return new WaitForSeconds((float) (end - start));

        PlayNextVideo();
    }
}