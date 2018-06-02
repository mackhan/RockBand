using UnityEngine;
using System.Collections;

/// <summary>
/// 开发模式的GUI
/// </summary>
public class DevelopmentModeGUI : MonoBehaviour
{
    /// <summary>
    /// ？？？
    /// </summary>
    SequenceSeeker<SequenceRegion> m_actionInfoRegionSeeker = new SequenceSeeker<SequenceRegion>();

    MusicManager m_musicManager;
    ScoringManager m_scoringManager;
    EventManager m_eventManager;

    /// <summary>
    /// 游戏主逻辑的对象
    /// </summary>
    OnPlayGUI m_onPlayGUI;

    /// <summary>
    /// 玩家的对象
    /// </summary>
    PlayerAction m_playerAction;

    string previousHitRegionName = "";

    /// <summary>
    /// 
    /// </summary>
    private struct SeekSlider
    {
        /// <summary>
        /// 是否在拖动滑动条
        /// </summary>
		public bool is_now_dragging;

        /// <summary>
        /// 拖动位置
        /// </summary>
        public float dragging_poisition;

        /// <summary>
        /// 鼠标是否按下了. Input . GetMoug Button（0）的结果
        /// 因为GUI在后面处理，所以保存这个值
        /// </summary>
        public bool is_button_down;
    };
    private SeekSlider m_seekSlider;

    /// <summary>
    /// 开发版的界面初始化
    /// </summary>
    public void BeginVisualization()
	{
		m_musicManager = GameObject.Find("MusicManager").GetComponent<MusicManager>();
		m_actionInfoRegionSeeker.SetSequence(m_musicManager.currentSongInfo.onBeatActionRegionSequence);
		m_actionInfoRegionSeeker.Seek(0);

	}

	public void Seek(float beatCount)
	{
		m_actionInfoRegionSeeker.Seek(beatCount);
	}

	void Start ()
    {
		m_musicManager = GameObject.Find("MusicManager").GetComponent<MusicManager>();
		m_scoringManager = GameObject.Find("ScoringManager").GetComponent<ScoringManager>();
		m_eventManager = GameObject.Find("EventManager").GetComponent<EventManager>();

        //因为GUI对象有Inactie的可能性，所以不能在Find直接访问。
        m_onPlayGUI = GameObject.Find("PhaseManager").GetComponent<PhaseManager>().guiList[1].GetComponent<OnPlayGUI>();
		m_playerAction = GameObject.Find("PlayerAvator").GetComponent<PlayerAction>();
		m_seekSlider.is_now_dragging = false;
		m_seekSlider.dragging_poisition = 0.0f;
	}

	// Update is called once per frame
	void Update ()
    {
		m_actionInfoRegionSeeker.ProceedTime(m_musicManager.DeltaBeatCountFromStart);

		m_seekSlider.is_button_down = Input.GetMouseButton(0);
	}

	void OnGUI()
    {
        float fScaleH = Screen.height / 1334;

#if true
        GUI.Label(new Rect( 5, 100, 150, 40 ),"Current");

		//-滑动进度条的控制
		SeekSliderControl();

        //-调试信息，显示当前的拍子数和总共的拍子数
		GUI.TextArea(
			new Rect( 250, 100, 200 * fScaleH, 40 * fScaleH),
			((int)m_musicManager.beatCountFromStart).ToString() + "/" + ((int)m_musicManager.length).ToString()
		);

		//-调试信息，显示滑动条当前的拍子数，只在拖动的时候显示
		if(this.m_seekSlider.is_now_dragging)
        {
			GUI.Label(new Rect(252, 120, 200 * fScaleH, 40 * fScaleH), ((int)this.m_seekSlider.dragging_poisition).ToString());
		}

		//结束按钮，按了重启
		if( GUI.Button(new Rect((Screen.width - 150)/2.0f, 350, 150 * fScaleH, 40 * fScaleH), "End"))
        {
			GameObject.Find("PhaseManager").GetComponent<PhaseManager>().SetPhase("Restart");
		}

        //显示之前输入的定时偏离了多少。
        GUI.Label(new Rect(5, 400, 150 * fScaleH, 40 * fScaleH)
                  , "Input Gap:" 
                  + m_scoringManager.m_lastResults[0].timingError);

        //显示上一次是在哪个拍子按下了
		GUI.Label(new Rect(5, 420, 150 * fScaleH, 40 * fScaleH)
                  , "Previous Input:" 
                  + m_playerAction.GetLastActionInof(0).triggerBeatTiming.ToString());

		GUI.Label(new Rect( 5, 440, 150 * fScaleH, 40 * fScaleH)
                  , "Nearest(beat):" 
                  + m_musicManager.currentSongInfo.onBeatActionSequence[0][m_scoringManager.m_lastResults[0].markerIndex].triggerBeatTiming.ToString());

		GUI.Label(new Rect( 150, 440, 150 * fScaleH, 40 * fScaleH)
                  , "Nearest(index):" 
                  + m_musicManager.currentSongInfo.onBeatActionSequence[0][m_scoringManager.m_lastResults[0].markerIndex].line_number.ToString());

        // 显示相关的部分名称
        if (m_musicManager.currentSongInfo.onBeatActionRegionSequence.Count > 0)
        {
            //确认当前部分的索引
            int currentReginIndex = m_actionInfoRegionSeeker.nextIndex - 1;
            if (currentReginIndex < 0)
            {
                currentReginIndex = 0;
            }

            //显示上次输入时的部分
            if (m_playerAction.GetCurrentPlayerAction(0) != PlayerActionEnum.None)
			{	
				previousHitRegionName
					= m_musicManager.currentSongInfo.onBeatActionRegionSequence[currentReginIndex].name;
			}
			GUI.Label(new Rect(150, 420, 250 * fScaleH, 40 * fScaleH), "region ...:" + previousHitRegionName);

            //显示当前部分
            GUI.Label(new Rect(5, 460, 150 * fScaleH, 40 * fScaleH), "Current:" + m_musicManager.beatCountFromStart);
			GUI.Label(new Rect(150, 460, 250 * fScaleH, 40 * fScaleH), "region ...:" + m_musicManager.currentSongInfo.onBeatActionRegionSequence[currentReginIndex].name);
		}
#endif
	}

    /// <summary>
    /// 滑动进度条的控制
    /// </summary>
    private void SeekSliderControl()
	{
		Rect slider_rect = new Rect( (Screen.width - 100)/2.0f, 100, 130, 40 );

		if (!m_seekSlider.is_now_dragging)//-如果没在拖动
        {
			float new_position = GUI.HorizontalSlider(slider_rect
                , m_musicManager.beatCountFromStart
                , 0
                , m_musicManager.length);

            //-如果滑动条的位置和当前的拍子不一样，说明开始拖动
            if (new_position != m_musicManager.beatCountFromStart)
            {
				m_seekSlider.dragging_poisition = new_position;
				m_seekSlider.is_now_dragging = true;
			}
		}
        else//-如果在拖动，则滑动条的位置就是当前拖动的位置
        {
			m_seekSlider.dragging_poisition = GUI.HorizontalSlider(slider_rect
                , m_seekSlider.dragging_poisition
                , 0
                , m_musicManager.length);

            // 释放按钮（拖动完成）设置所有的控件到当前位置
			if (!m_seekSlider.is_button_down)
            {
				m_musicManager.Seek(m_seekSlider.dragging_poisition);
				m_eventManager.Seek(m_seekSlider.dragging_poisition);
				m_scoringManager.Seek(m_seekSlider.dragging_poisition);
				m_onPlayGUI.Seek(m_seekSlider.dragging_poisition);

				Seek(m_seekSlider.dragging_poisition);

				//-设置滑动结束
				m_seekSlider.is_now_dragging = false;
			}
		}
	}
}
