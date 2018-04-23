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
	string m_currentPhase = "Startup";

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
	
	void Start ()
    {
		m_musicManager   = GameObject.Find("MusicManager").GetComponent<MusicManager>();
		m_scoringManager = GameObject.Find("ScoringManager").GetComponent<ScoringManager>();
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
    /// 设置状态
    /// </summary>
    /// <param name="nextPhase"></param>
	public void SetPhase(string nextPhase)
    {
		switch(nextPhase)
        {
		case "Startup"://开始菜单
            DeactiveateAllGUI();
			ActivateGUI("StartupMenuxixi");
			break;
		
		case "OnBeginInstruction"://玩法介绍界面
            DeactiveateAllGUI();
			ActivateGUI("InstructionGUI");
			ActivateGUI("OnPlayGUI");
			break;

		case "Play"://主游戏
            {
			    DeactiveateAllGUI();
			    ActivateGUI("OnPlayGUI");

			    //从csv读取歌曲数据
			    TextReader textReader = new StringReader(
					    System.Text.Encoding.UTF8.GetString((Resources.Load("SongInfo/songInfoCSV") as TextAsset).bytes )
				    );
			    SongInfo songInfo = new SongInfo();
			    SongInfoLoader loader = new SongInfoLoader();
			    loader.songInfo = songInfo;
			    loader.ReadCSV(textReader);
			    m_musicManager.currentSongInfo = songInfo;

                //-三波观众开始动起来
			    foreach (GameObject audience in GameObject.FindGameObjectsWithTag("Audience"))
			    {
				    audience.GetComponent<SimpleActionMotor>().isWaveBegin = true;
			    }

                //各种效果动画（舞台演出等）开始
                GameObject.Find("EventManager").GetComponent<EventManager>().BeginEventSequence();

                //得分评价开始
                m_scoringManager.BeginScoringSequence();

                //节奏序列绘制开始
                OnPlayGUI onPlayGUI = GameObject.Find("OnPlayGUI").GetComponent<OnPlayGUI>();
			    onPlayGUI.BeginVisualization();
			    onPlayGUI.isDevelopmentMode = false;

			    //开始播放音乐
			    m_musicManager.PlayMusicFromStart();
		    }
			break;

		case "DevelopmentMode":
		    {
			    DeactiveateAllGUI();
			    ActivateGUI("DevelopmentModeGUI");
			    ActivateGUI("OnPlayGUI");

			    //csvから曲データ読み込み
			    TextReader textReader
				    = new StringReader(
					    System.Text.Encoding.UTF8.GetString((Resources.Load("SongInfo/songInfoCSV") as TextAsset).bytes )
				    );
			    SongInfo songInfo = new SongInfo();
			    SongInfoLoader loader=new SongInfoLoader();
			    loader.songInfo=songInfo;
			    loader.ReadCSV(textReader);
			    m_musicManager.currentSongInfo = songInfo;

			    foreach (GameObject audience in GameObject.FindGameObjectsWithTag("Audience"))
			    {
				    audience.GetComponent<SimpleActionMotor>().isWaveBegin = true;
			    }
			    //イベント(ステージ演出等)開始
			    GameObject.Find("EventManager").GetComponent<EventManager>().BeginEventSequence();
			    //スコア評価開始
			    m_scoringManager.BeginScoringSequence();
			    //リズムシーケンス描画開始
			    OnPlayGUI onPlayGUI = GameObject.Find("OnPlayGUI").GetComponent<OnPlayGUI>();
			    onPlayGUI.BeginVisualization();
			    onPlayGUI.isDevelopmentMode = true;
			    //developモード専用GUIシーケンス描画開始
			    GameObject.Find("DevelopmentModeGUI").GetComponent<DevelopmentModeGUI>().BeginVisualization();

                //开始播放音乐
                m_musicManager.PlayMusicFromStart();
                }
			break;

		case "GameOver":
		    {
			    DeactiveateAllGUI();
			    ActivateGUI("ShowResultGUI");
			    ShowResultGUI showResult = GameObject.Find("ShowResultGUI").GetComponent<ShowResultGUI>();
			    //スコア依存のメッセージを表示
			    Debug.Log( m_scoringManager.scoreRate );
			    Debug.Log(ScoringManager.failureScoreRate);
			    if (m_scoringManager.scoreRate <= ScoringManager.failureScoreRate)
			    {
				    showResult.comment = showResult.comment_BAD;
				    GameObject.Find("Vocalist").GetComponent<BandMember>().BadFeedback();
				
			    }
			    else if (m_scoringManager.scoreRate >= ScoringManager.excellentScoreRate)
			    {
				    showResult.comment = showResult.comment_EXCELLENT;
				    GameObject.Find("Vocalist").GetComponent<BandMember>().GoodFeedback();
				    GameObject.Find("AudienceVoice").GetComponent<AudioSource>().Play();
			    }
			    else
			    {
				    showResult.comment = showResult.comment_GOOD;
				    GameObject.Find("Vocalist").GetComponent<BandMember>().GoodFeedback();
			    }
		    }
			break;

		case "Restart"://-重新开始，直接重新加载Main场景
		    {
			    Application.LoadLevel("Main");
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
