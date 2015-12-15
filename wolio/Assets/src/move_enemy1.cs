using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Collections;

public class move_enemy1 : MonoBehaviour
{
    private int counter;
    private int i;

    // Use this for initialization
    void Start()
    {
        this.UpdateAsObservable()
            .Where(x => counter >= 0 && counter <= 29)
            .Subscribe(_ => LeftMove());

        this.UpdateAsObservable()
            .Where(x => counter >= 29 && counter <= 59)
            .Subscribe(_ => RightMove());

        this.ObserveEveryValueChanged(x => x.counter)
            .Where(x => x == 60)
            .Subscribe(_ => counter = 0);
    }

    void LeftMove()
    {
        Vector3 v = transform.position;
        transform.position = new Vector3(v.x - 0.03f, v.y, v.z);
        counter++;
    }

    void RightMove()
    {
        Vector3 v = transform.position;
        transform.position = new Vector3(v.x + 0.03f, v.y, v.z);
        counter++;
    }
}
