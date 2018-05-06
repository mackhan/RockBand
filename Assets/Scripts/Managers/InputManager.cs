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

    /// <summary>
    /// 玩家的对象
    /// </summary>
    PlayerAction m_playerAction;

    /// <summary>
    /// 音乐管理器
    /// </summary>
    ScoringManager m_scoringManager;
    
    void Awake()
    {
		Application.targetFrameRate = 60;
	}

	void Start ()
    {
		m_musicManager = GameObject.Find("MusicManager").GetComponent<MusicManager>();
		m_playerAction = GameObject.Find("PlayerAvator").GetComponent<PlayerAction>();
		m_scoringManager = GameObject.Find("ScoringManager").GetComponent<ScoringManager>();
	}

	//void Update ()
 //   {
 //       if (Input.GetMouseButtonDown(0))//-如果有操作并且音乐正在播放
 //       {
	//		m_playerAction.DoAction(0);
	//	}
	//}
}
