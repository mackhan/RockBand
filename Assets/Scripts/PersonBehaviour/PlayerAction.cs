using UnityEngine;
using System.Collections;

/// <summary>
/// 玩家的动作种类
/// </summary>
public enum PlayerActionEnum
{
	None,
	HeadBanging,//-点头
	Jump//-跳
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
    PlayerActionEnum[] m_currentPlayerAction = new PlayerActionEnum[4];

    public PlayerActionEnum GetCurrentPlayerAction(int _iIndex)
    {
		return m_currentPlayerAction[_iIndex];
	}

    /// <summary>
    /// 玩家在哪个拍子按下来了
    /// </summary>
    OnBeatActionInfo[] m_lastActionInfo = new OnBeatActionInfo[] 
    {
        new OnBeatActionInfo()
        , new OnBeatActionInfo()
        , new OnBeatActionInfo()
        , new OnBeatActionInfo()

    };

    public OnBeatActionInfo GetLastActionInof(int _iIndex)
    {
		return m_lastActionInfo[_iIndex];
	}

    /// <summary>
    /// 音乐管理器，播放暂停等
    /// </summary>
    MusicManager m_musicManager;

    /// <summary>
    /// 音乐管理器
    /// </summary>
    ScoringManager m_scoringManager;

    PlayerActionEnum[] m_newPlayerAction = new PlayerActionEnum[4];

    void Start ()
    {
		m_musicManager = GameObject.Find("MusicManager").GetComponent<MusicManager>();
        m_scoringManager = GameObject.Find("ScoringManager").GetComponent<ScoringManager>();
    }
	
	void Update ()
    {
        for (int i = 0; i < ConstantManager.MemberNum; i++)
        {
            m_currentPlayerAction[i] = m_newPlayerAction[i];
            m_newPlayerAction[i] = PlayerActionEnum.None;
        }
	}

    /// <summary>
    /// 玩家有一个操作，表现的玩家就做一个动作
    /// </summary>
    /// <param name="actionType"></param>
	public void DoAction(int _iIndex)
    {
        if (!m_musicManager.IsPlaying())
        {
            return;
        }

        if (m_scoringManager.temper < ScoringManager.temperThreshold)//-如果兴奋值比较低就一直播放点头的动作
        {
            m_newPlayerAction[_iIndex] = PlayerActionEnum.HeadBanging;
        }
        else//-如果比较兴奋了就按照脚本的动作
        {
            int iNearest = m_scoringManager.GetNearestPlayerActionInfoIndex(_iIndex);
            m_newPlayerAction[_iIndex] = m_musicManager.currentSongInfo.onBeatActionSequence[_iIndex][iNearest].playerActionType;
        }

        //获取当前在哪个拍子按下来了
		OnBeatActionInfo actionInfo = new OnBeatActionInfo();
		actionInfo.triggerBeatTiming = m_musicManager.beatCountFromStart;
		actionInfo.playerActionType = m_newPlayerAction[_iIndex];
		m_lastActionInfo[_iIndex] = actionInfo;

#if PLAYER
        //-播放相应的动画
        if(actionType == PlayerActionEnum.HeadBanging)
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
