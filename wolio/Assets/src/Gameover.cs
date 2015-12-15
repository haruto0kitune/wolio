using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;

public class Gameover : MonoBehaviour
{
    GameObject Camera;
    Player Player;
    
    void Awake()
    {
        Camera = GameObject.Find("Main Camera");
        Player = GameObject.Find("Player").GetComponent<Player>();
    }

    void Start()
    {
        this.Player
            .IsDead
            .Where(x => x)
            .DistinctUntilChanged()
            .Subscribe(_ =>
            {
                transform.position = new Vector3(Camera.transform.position.x, 0, -1);
            });
    }
}
