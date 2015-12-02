using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    private GameObject m_Camera;

    // Use this for initialization
    void Start()
    {
        m_Camera = GameObject.Find("Main Camera");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x + 0.1f, transform.position.y, transform.position.z);
        Release();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if((other.gameObject.tag == "Enemy") || (other.gameObject.tag == "Obstacle"))
        {
            Destroy(gameObject);
        }
    }

    void Release()
    {
        if((m_Camera.transform.position.x + 8.3) <= transform.position.x)
        {
            Destroy(gameObject);
        }
    }
}
