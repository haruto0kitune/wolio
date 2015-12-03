using System.Collections;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UniRx;
using UniRx.Triggers;

[RequireComponent(typeof(Player))]
public class PlayerControl : MonoBehaviour
{
    private Player m_Character;
    private float m_Direction;
    private bool m_Jump;
    private bool m_Attack;
    private bool m_Dash;
    private bool m_LeftShift;
    private bool m_RightShift;
    private bool m_left;
    private bool m_right;
   
    private void Awake()
    {
        m_Character = GetComponent<Player>();
    }

    private void Start()
    {
        FixedUpdateAsObservables();
        UpdateAsObservables();
    }

    private void FixedUpdateAsObservables()
    {
        this.FixedUpdateAsObservable()
            .Where(x => m_Attack == true)
            .Subscribe(_ => m_Character.FireBall());

        this.FixedUpdateAsObservable()
            .Where(x => (m_Dash || m_Character.m_isDashing) && m_Character.m_Grounded)
            .Subscribe(_ => m_Character.Dash(m_Direction, m_LeftShift));

        this.FixedUpdateAsObservable()
            .Where(x => !((m_Dash || m_Character.m_isDashing) && m_Character.m_Grounded))
            .Subscribe(_ => m_Character.TurnAround(m_Direction));

        this.FixedUpdateAsObservable()
            .Where(x => !((m_Dash || m_Character.m_isDashing) && m_Character.m_Grounded))
            .Subscribe(_ => m_Character.Run(m_Direction));

        this.FixedUpdateAsObservable()
            .Where(x => !((m_Dash || m_Character.m_isDashing) && m_Character.m_Grounded))
            .Subscribe(_ => m_Character.Jump(m_Jump));

        this.FixedUpdateAsObservable()
            .Subscribe(_ => m_Jump = false);
    }

    private void UpdateAsObservables()
    {
        this.UpdateAsObservable()
            .Subscribe(_ => m_left = CrossPlatformInputManager.GetButton("Left"));

        this.UpdateAsObservable()
            .Subscribe(_ => m_right = CrossPlatformInputManager.GetButton("Right"));

        this.UpdateAsObservable()
            .Subscribe(_ => m_Direction = CrossPlatformInputManager.GetAxisRaw("Horizontal"));

        this.UpdateAsObservable()
            .Where(x => (CrossPlatformInputManager.GetButton("Left") || CrossPlatformInputManager.GetButton("Right")) && CrossPlatformInputManager.GetButton("Left Shift"))
            .Subscribe(_ => m_Dash = true);

        this.UpdateAsObservable()
            .Where(x => !(CrossPlatformInputManager.GetButton("Left") || CrossPlatformInputManager.GetButton("Right")) && CrossPlatformInputManager.GetButton("Left Shift"))
            .Subscribe(_ => m_Dash = false);

        this.UpdateAsObservable()
            .Where(x => !m_Jump)
            .Subscribe(_ => m_Jump = CrossPlatformInputManager.GetButtonDown("Jump"));

        this.UpdateAsObservable()
            .Subscribe(_ => m_Attack = CrossPlatformInputManager.GetButton("Attack"));

        this.UpdateAsObservable()
            .Subscribe(_ => m_LeftShift = CrossPlatformInputManager.GetButton("Left Shift"));
    }
}