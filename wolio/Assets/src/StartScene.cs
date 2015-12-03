using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UniRx;
using UniRx.Triggers;


public class StartScene : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (CrossPlatformInputManager.GetButton("Space"))
        {
            Application.LoadLevelAsync("stage1");
        }

    }
}
