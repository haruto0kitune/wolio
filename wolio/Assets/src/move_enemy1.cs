using UnityEngine;
using System.Collections;

public class move_enemy1 : MonoBehaviour
{

    private Transform m_transform;
    private int counter;
    private int i;

    // Use this for initialization
    void Start()
    {
        m_transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        /*switch (counter)
        {
            case 0:
                LeftMove();
                break;
            case 1:
                LeftMove();
                break;
            case 2:
                LeftMove();
                break;
            case 3:
                RightMove();
                break;
            case 4:
                RightMove();
                break;
            case 5:
                RightMove();
                break;
        }*/

        if (counter >= 0 && counter <= 29)
        {
            LeftMove();
        }
        else if (counter >= 29 && counter <= 59)
        {
            RightMove();
        }

        if (counter == 60)
        {
            counter = 0;
        }
    }

    void LeftMove()
    {
        Vector3 v = m_transform.position;
        m_transform.position = new Vector3(v.x - 0.03f, v.y, v.z);
        counter++;
    }

    void RightMove()
    {
        Vector3 v = m_transform.position;
        m_transform.position = new Vector3(v.x + 0.03f, v.y, v.z);
        counter++;
    }
}
