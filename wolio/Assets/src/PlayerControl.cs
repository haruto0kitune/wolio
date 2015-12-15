using System.Collections;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UniRx;
using UniRx.Triggers;

[RequireComponent(typeof(Player))]
public class PlayerControl : MonoBehaviour
{
    private Player Character;
    private float Direction;
    private bool Jump;
    private bool Attack;
    private bool Dash;
    private bool LeftShift;
    private bool RightShift;
    private bool left;
    private bool right;
   
    private void Awake()
    {
        Character = GetComponent<Player>();
    }

    private void Start()
    {
        FixedUpdateAsObservables();
        UpdateAsObservables();
    }

    private void FixedUpdateAsObservables()
    {
        this.FixedUpdateAsObservable()
            .Where(x => Attack == true)
            .Subscribe(_ => Character.FireBall());

        this.FixedUpdateAsObservable()
            .Where(x => (Dash || Character.IsDashing.Value) && Character.IsGrounded.Value)
            .Subscribe(_ => Character.Dash(Direction, LeftShift));

        this.FixedUpdateAsObservable()
            .Where(x => !((Dash || Character.IsDashing.Value) && Character.IsGrounded.Value))
            .Subscribe(_ => Character.TurnAround(Direction));

        this.FixedUpdateAsObservable()
            .Where(x => !((Dash || Character.IsDashing.Value) && Character.IsGrounded.Value))
            .Subscribe(_ => Character.Run(Direction));

        this.FixedUpdateAsObservable()
            .Where(x => !((Dash || Character.IsDashing.Value) && Character.IsGrounded.Value))
            .Subscribe(_ => Character.Jump(Jump));

        this.FixedUpdateAsObservable()
            .Subscribe(_ => Jump = false);
    }

    private void UpdateAsObservables()
    {
        this.UpdateAsObservable()
            .Subscribe(_ => left = CrossPlatformInputManager.GetButton("Left"));

        this.UpdateAsObservable()
            .Subscribe(_ => right = CrossPlatformInputManager.GetButton("Right"));

        this.UpdateAsObservable()
            .Subscribe(_ => Direction = CrossPlatformInputManager.GetAxisRaw("Horizontal"));

        this.UpdateAsObservable()
            .Where(x => (CrossPlatformInputManager.GetButton("Left") || CrossPlatformInputManager.GetButton("Right")) && CrossPlatformInputManager.GetButton("Left Shift"))
            .Subscribe(_ => Dash = true);

        this.UpdateAsObservable()
            .Where(x => !(CrossPlatformInputManager.GetButton("Left") || CrossPlatformInputManager.GetButton("Right")) && CrossPlatformInputManager.GetButton("Left Shift"))
            .Subscribe(_ => Dash = false);

        this.UpdateAsObservable()
            .Where(x => !Jump)
            .Subscribe(_ => Jump = CrossPlatformInputManager.GetButtonDown("Jump"));

        this.UpdateAsObservable()
            .Subscribe(_ => Attack = CrossPlatformInputManager.GetButton("Attack"));

        this.UpdateAsObservable()
            .Subscribe(_ => LeftShift = CrossPlatformInputManager.GetButton("Left Shift"));
    }
}