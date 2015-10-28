using UnityEngine;
using System.Collections;

public class Fps : MonoBehaviour
{

    int frameCount;
    float prevTime;

    void Awake()
    {
        Application.targetFrameRate = 60;
        Time.captureFramerate = 60;
    }

    // Use this for initialization
    void Start()
    {
        frameCount = 0;
        prevTime = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        ++frameCount;
        float time = Time.realtimeSinceStartup - prevTime;

        if (time >= 0.5f)
        {
            Debug.LogFormat("{0}fps", frameCount / time);

            frameCount = 0;
            prevTime = Time.realtimeSinceStartup;
        }
    }
}