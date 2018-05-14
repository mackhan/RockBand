using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

/// <summary>
/// 游戏整体的分段管理（主题画面和游戏进行中等）
/// </summary>
public class PhaseManager : MonoBehaviour
{
    /// <summary>
    /// 音乐控制管理器
    /// </summary>
    MusicManager m_musicManager;

    /// <summary>
    /// 得分管理器
    /// </summary>
	ScoringManager m_scoringManager;

    /// <summary>
    /// 当前的状态，默认是Startup
    /// </summary>
    string m_currentPhase = "StartupMenu";

    /// <summary>
    /// 获得当前的状态
    /// </summary>
    public string currentPhase
    {
		get{ return m_currentPhase; }
	}

    /// <summary>
    /// 所有UI的管理器
    /// </summary>
    public GameObject[] guiList;

    public string SongName = "";

    void Awake()
    {
        Application.targetFrameRate = 60;
    }
	
	void Start ()
    {
		m_musicManager   = GameObject.Find("MusicManager").GetComponent<MusicManager>();
		m_scoringManager = GameObject.Find("ScoringManager").GetComponent<ScoringManager>();

        SetPhase(m_currentPhase);
    }
	
	void Update ()
    {
		switch (currentPhase)
        {
		case "Play" :
			if (m_musicManager.IsFinished())//-如果检测到结束了，进入结束状态
            {
				SetPhase("GameOver");
			}
			break;
		}
	}

    /// <summary>
    /// 游戏开始的逻辑
    /// </summary>
    /// <param name="_bDevelopment"></param>
    void Play(bool _bDevelopment)
    {
        DeactiveateAllGUI();
        ActivateGUI("OnPlayGUI");
        if (_bDevelopment)
        {
            ActivateGUI("DevelopmentModeGUI");
        }

        //从csv读取歌曲数据
        TextReader textReader = new StringReader(
            System.Text.Encoding.UTF8.GetString((Resources.Load(SongName + "songInfoCSV") as TextAsset).bytes));
        SongInfo songInfo = new SongInfo();
        SongInfoLoader loader = new SongInfoLoader();
        loader.songInfo = songInfo;
        loader.ReadCSV(SongName, textReader);
        m_musicManager.currentSongInfo = songInfo;

        //-三波观众开始动起来
        //foreach (GameObject audience in GameObject.FindGameObjectsWithTag("Audience"))
        //{
        //    audience.GetComponent<SimpleActionMotor>().isWaveBegin = true;
        //}

        //-各种效果动画（舞台演出等）开始
        GameObject.Find("EventManager").GetComponent<EventManager>().BeginEventSequence();

        //-得分评价开始
        m_scoringManager.BeginScoringSequence();

        //-节奏序列绘制开始
        OnPlayGUI onPlayGUI = GameObject.Find("OnPlayGUI").GetComponent<OnPlayGUI>();
        onPlayGUI.BeginVisualization();
        onPlayGUI.isDevelopmentMode = _bDevelopment;

        //-绘制开发版的特殊界面
        if (_bDevelopment)
        {
            GameObject.Find("DevelopmentModeGUI").GetComponent<DevelopmentModeGUI>().BeginVisualization();
        }

        //开始播放音乐
        m_musicManager.PlayMusicFromStart();
    }

    /// <summary>
    /// 设置状态
    /// </summary>
    /// <param name="nextPhase"></param>
	public void SetPhase(string nextPhase)
    {
		switch(nextPhase)
        {
        case "StartupMenu"://开始菜单
            DeactiveateAllGUI();
			ActivateGUI("StartupMenu");
			break;
		
		//case "OnBeginInstruction"://玩法介绍界面
  //          DeactiveateAllGUI();
		//	ActivateGUI("InstructionGUI");
		//	ActivateGUI("OnPlayGUI");
		//	break;

        case "SelectMusic":
            DeactiveateAllGUI();
            ActivateGUI("SelectMusic");                
            break;

		case "Play"://主游戏
            Play(false);
			break;

		case "DevelopmentMode":
            Play(true);
			break;

		case "GameOver":
		    {
			    DeactiveateAllGUI();
                ActivateGUI("Settlement");
                Settlement showResult = GameObject.Find("Settlement").GetComponent<Settlement>();

			    //-显示结算结果
			    Debug.Log(m_scoringManager.scoreRate);
			    Debug.Log(ScoringManager.failureScoreRate);
			    if (m_scoringManager.scoreRate <= ScoringManager.failureScoreRate)//-如果是失败的得分率，乐队成员发出失败的声音
			    {
                    showResult.SetResult(eResult.eBad, m_scoringManager.score);
				    GameObject.Find("Vocalist").GetComponent<BandMember>().BadFeedback();
				
			    }
			    else if (m_scoringManager.scoreRate >= ScoringManager.excellentScoreRate)//-如果是Excellent乐队成员和观众一起欢呼
			    {
                    showResult.SetResult(eResult.eBest, m_scoringManager.score);
				    GameObject.Find("Vocalist").GetComponent<BandMember>().GoodFeedback();
				    GameObject.Find("AudienceVoice").GetComponent<AudioSource>().Play();
			    }
			    else//-如果是Good，乐队成员欢呼
			    {
                    showResult.SetResult(eResult.eGood, m_scoringManager.score);
				    GameObject.Find("Vocalist").GetComponent<BandMember>().GoodFeedback();
			    }
		    }
			break;

		case "Restart"://-重新开始，直接重新加载Main场景
		    {
				UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
		    }
			break;

		default:
			Debug.LogError("unknown phase: " + nextPhase);
			break;
		}

		m_currentPhase = nextPhase;
	}

    /// <summary>
    /// 关闭所有UI
    /// </summary>
	private void DeactiveateAllGUI()
    {
		foreach (GameObject gui in guiList)
        {
			gui.SetActive(false);
		}
	}

    /// <summary>
    /// 打开某个UI
    /// </summary>
    /// <param name="guiName"></param>
	private void ActivateGUI(string guiName)
	{
		foreach (GameObject gui in guiList)
		{
            if (gui.name == guiName)
            {
                gui.SetActive(true);
            }
		}
	}
}
