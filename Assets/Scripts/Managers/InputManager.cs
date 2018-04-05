using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 玩家点击鼠标时的处理
/// </summary>
public class InputManager : MonoBehaviour
{

    /// <summary>
    /// 音乐管理器，播放暂停等
    /// </summary>
    MusicManager m_musicManager;

    PlayerAction m_playerAction;
    ScoringManager m_scoringManager;


    void Awake()
    {
		Application.targetFrameRate = 60;
	}

	// Use this for initialization
	void Start ()
    {
		m_musicManager=GameObject.Find("MusicManager").GetComponent<MusicManager>();
		m_playerAction=GameObject.Find("PlayerAvator").GetComponent<PlayerAction>();
		m_scoringManager=GameObject.Find("ScoringManager").GetComponent<ScoringManager>();
	}

	// Update is called once per frame
	void Update ()
    {
		//ビートカウントの記録タイミングを、InputのUpdateのタイミングで行う。
		//MusicManagerのUpdateでビートカウントの記録を行うと入力とビートカウントが
		//最大1フレーム分ずれる
		//演奏中に画面クリックでプレイヤーのアクション
		if( Input.GetMouseButtonDown(0) && m_musicManager.IsPlaying() )
        {
			PlayerActionEnum actionType;
			if (m_scoringManager.temper < ScoringManager.temperThreshold)
            {
				actionType=PlayerActionEnum.HeadBanging;
			}
			else
            {
				actionType
					=m_musicManager.currentSongInfo.onBeatActionSequence[
						m_scoringManager.GetNearestPlayerActionInfoIndex()
					].playerActionType;
			}
			m_playerAction.DoAction(actionType);
		}
	}
}
