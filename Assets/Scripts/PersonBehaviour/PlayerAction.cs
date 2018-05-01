using UnityEngine;
using System.Collections;

/// <summary>
/// 玩家的动作种类
/// </summary>
public enum PlayerActionEnum
{
	None,
	HeadBanging,
	Jump
};

/// <summary>
/// 玩家的控制类
/// </summary>
public class PlayerAction : MonoBehaviour
{
    /// <summary>
    /// Best和Good的音效
    /// </summary>
	public AudioClip headBangingSoundClip_GOOD;

    /// <summary>
    /// Bad的音效
    /// </summary>
	public AudioClip headBangingSoundClip_BAD;

    /// <summary>
    /// 玩家当前的动作
    /// </summary>
    PlayerActionEnum m_currentPlayerAction;
    public PlayerActionEnum currentPlayerAction
    {
		get{ return m_currentPlayerAction; }
	}

    /// <summary>
    /// 玩家最后的动作
    /// </summary>
    OnBeatActionInfo m_lastActionInfo = new OnBeatActionInfo();
    public OnBeatActionInfo lastActionInfo
    {
		get{ return m_lastActionInfo; }
	}

    MusicManager m_musicManager;
    
    PlayerActionEnum m_newPlayerAction;

    void Start ()
    {
		m_musicManager = GameObject.Find("MusicManager").GetComponent<MusicManager>();
	}
	
	void Update ()
    {
		m_currentPlayerAction = m_newPlayerAction;
		m_newPlayerAction = PlayerActionEnum.None;
	}

    /// <summary>
    /// 玩家有一个操作，表现的玩家就做一个动作
    /// </summary>
    /// <param name="actionType"></param>
	public void DoAction(PlayerActionEnum actionType)
    {
		m_newPlayerAction = actionType;

		OnBeatActionInfo actionInfo = new OnBeatActionInfo();
		actionInfo.triggerBeatTiming = m_musicManager.beatCountFromStart;
		actionInfo.playerActionType = m_newPlayerAction;
		m_lastActionInfo = actionInfo;

#if PLAYER
        //-播放相应的动画
        if(actionType == PlayerActionEnum. HeadBanging)
        {
			gameObject.GetComponent<SimpleSpriteAnimation>().BeginAnimation(2, 1, false);
		}
		else if (actionType == PlayerActionEnum.Jump)
		{	
			gameObject.GetComponent<SimpleActionMotor>().Jump();
			gameObject.GetComponent<SimpleSpriteAnimation>().BeginAnimation(1, 1, false);
		}
#endif
	}
}
