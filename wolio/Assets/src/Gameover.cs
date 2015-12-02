using UnityEngine;
using System.Collections;

public class Gameover : MonoBehaviour
{

    GameObject m_Camera;

    // Use this for initialization
    void Start()
    {
        m_Camera = GameObject.Find("Main Camera");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(m_Camera.transform.position.x, 0, 0);
    }
}
