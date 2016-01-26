using System.Collections;
using UnityEngine;
using UnityEngine.UI;
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
    private bool Guard;
    private bool WallKickJump;
    private bool duringWKJ;

    public Text WallKickJumpText;
    public Text IsTouchingWallText;
    public Text JumpText;
    public Text IsGroundedText;

    const int ThrottleFirstFrameTimeSpan = 180;

    private void Awake()
    {
        Character = GetComponent<Player>();
    }

    private void Start()
    {
        OnTriggerEnter2DAsObservables();
        OnTriggerStay2DAsObservables();
        OnTriggerExit2DAsObservables();
        FixedUpdateAsObservables();
        UpdateAsObservables();
    }

    private void OnTriggerEnter2DAsObservables()
    {
        this.OnTriggerEnter2DAsObservable()
            .Where(x => transform.parent == null && x.gameObject.tag == "MovingFloor" && !(CrossPlatformInputManager.GetButton("Left")) && !(CrossPlatformInputManager.GetButton("Right")))
            .Subscribe(_ => transform.parent = _.gameObject.transform);

        this.OnTriggerEnter2DAsObservable()
            .Where(x => !(gameObject.layer == LayerMask.NameToLayer("Invincible")))
            .Where(x => x.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            .ThrottleFirstFrame(ThrottleFirstFrameTimeSpan)
            .Subscribe(_ => Character.Hp.Value--);

        this.OnTriggerEnter2DAsObservable()
            .Where(x => !(gameObject.layer == LayerMask.NameToLayer("Invincible")))
            .Where(x => x.gameObject.layer == LayerMask.NameToLayer("EnemyBullet"))
            .ThrottleFirstFrame(ThrottleFirstFrameTimeSpan)
            .Subscribe(_ => Character.Hp.Value--);
    }

    private void OnTriggerStay2DAsObservables()
    {
        this.OnTriggerStay2DAsObservable()
            .Where(x => x.gameObject.tag == "Wall")
            .Subscribe(_ => Character.IsTouchingWall.Value = true);

        this.OnTriggerStay2DAsObservable()
            .Where(x => x.gameObject.tag == "MovingFloor")
            .Where(x => !CrossPlatformInputManager.GetButton("Left") && !CrossPlatformInputManager.GetButton("Right"))
            .Subscribe(_ => transform.parent = _.gameObject.transform);

        this.OnTriggerStay2DAsObservable()
            .Where(x => x.gameObject.tag == "MovingFloor")
            .Where(x => CrossPlatformInputManager.GetButton("Left") || CrossPlatformInputManager.GetButton("Right"))
            .Subscribe(_ => transform.parent = null);

        this.OnTriggerStay2DAsObservable()
            .Where(x => !(gameObject.layer == LayerMask.NameToLayer("Invincible")))
            .Where(x => x.gameObject.layer == LayerMask.NameToLayer("Splinter"))
            .ThrottleFirstFrame(ThrottleFirstFrameTimeSpan)
            .Subscribe(_ => Character.Hp.Value--);

        this.OnTriggerStay2DAsObservable()
            .Where(x => x.gameObject.layer == LayerMask.NameToLayer("EnemyBullet"))
            .Where(x => this.gameObject.layer == LayerMask.NameToLayer("Guard"))
            .Subscribe(_ => Character.Guard());
    }

    private void OnTriggerExit2DAsObservables()
    {
        this.OnTriggerExit2DAsObservable()
            .Where(x => x.gameObject.tag == "Wall")
            .Subscribe(_ => Character.IsTouchingWall.Value = false);

        this.OnTriggerExit2DAsObservable()
            .Where(x => transform.parent != null && x.gameObject.tag == "MovingFloor" && (CrossPlatformInputManager.GetButton("Left")) || (CrossPlatformInputManager.GetButton("Right")))
            .Subscribe(_ => transform.parent = null);

        this.OnTriggerExit2DAsObservable()
            .Where(x => transform.parent != null && x.gameObject.tag == "MovingFloor")
            .Subscribe(_ => transform.parent = null);
    }

    private void FixedUpdateAsObservables()
    {
        this.FixedUpdateAsObservable()
            .Subscribe(_ => WallKickJumpText.text = "WallKickJump: " + WallKickJump.ToString());

        this.FixedUpdateAsObservable()
            .Subscribe(_ => IsTouchingWallText.text = "IsTouchingWall: " + Character.IsTouchingWall.Value.ToString());

        this.FixedUpdateAsObservable()
            .Subscribe(_ => JumpText.text = "Jump: " + Jump.ToString());

        this.FixedUpdateAsObservable()
            .Subscribe(_ => IsGroundedText.text = "IsGrounded: " + Character.IsGrounded.Value.ToString());

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
            .Where(x => Guard && Character.IsGrounded.Value)
            .Subscribe(_ => Character.Guard());

        this.FixedUpdateAsObservable()
            .Where(x => !((Dash || Character.IsDashing.Value) && Character.IsGrounded.Value))
            .Subscribe(_ => Character.Jump(Jump));

        this.FixedUpdateAsObservable()
            .Subscribe(_ => Jump = false);

        this.FixedUpdateAsObservable()
            .Where(x => !Character.IsGrounded.Value && Character.IsTouchingWall.Value && WallKickJump)
            .Subscribe(_ => StartCoroutine(Character.WallKickJump())); 
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
            .Subscribe(_ => WallKickJump = CrossPlatformInputManager.GetButtonDown("WallKickJump"));

        this.UpdateAsObservable()
            .Subscribe(_ => Attack = CrossPlatformInputManager.GetButton("Attack"));

        this.UpdateAsObservable()
            .Subscribe(_ => LeftShift = CrossPlatformInputManager.GetButton("Left Shift"));

        this.UpdateAsObservable()
            .Subscribe(_ => Guard = CrossPlatformInputManager.GetButton("Guard"));
    }
}