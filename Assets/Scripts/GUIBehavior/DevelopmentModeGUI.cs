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
        /// 拖动？
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
		m_musicManager=GameObject.Find("MusicManager").GetComponent<MusicManager>();
		m_scoringManager=GameObject.Find("ScoringManager").GetComponent<ScoringManager>();
		m_eventManager=GameObject.Find("EventManager").GetComponent<EventManager>();

        //因为GUI对象有Inactie的可能性，所以不能在Find直接访问。
        m_onPlayGUI = GameObject.Find("PhaseManager").GetComponent<PhaseManager>().guiList[1].GetComponent<OnPlayGUI>();
		m_playerAction = GameObject.Find("PlayerAvator").GetComponent<PlayerAction>();
		m_seekSlider.is_now_dragging = false;
		m_seekSlider.dragging_poisition = 0.0f;
	}

	// Update is called once per frame
	void Update ()
    {
		m_actionInfoRegionSeeker.ProceedTime(m_musicManager.beatCountFromStart 
            - m_musicManager.previousBeatCountFromStart);

		m_seekSlider.is_button_down = Input.GetMouseButton(0);
	}

	void OnGUI()
    {
		GUI.Label(new Rect( 5, 100, 150, 40 ),"Current");

		//-滑动进度条的控制
		SeekSliderControl();

		GUI.TextArea(
			new Rect( 250, 100, 200, 40 ),
			((int)m_musicManager.beatCountFromStart).ToString() + "/" + ((int)m_musicManager.length).ToString()
		);

		// シーク中だけ、シークバー上の位置を表示する.
		if(this.m_seekSlider.is_now_dragging)
        {
			GUI.Label(new Rect( 252, 120, 200, 40 ), ((int)this.m_seekSlider.dragging_poisition).ToString());
		}

		//
		if( GUI.Button( new Rect( (Screen.width - 150)/2.0f, 350, 150, 40 ), "End" ) )
        {
			GameObject.Find("PhaseManager").GetComponent<PhaseManager>().SetPhase("Restart");
		}

		// 直前の入力のタイミングがどれくらいずれていたかを表示する.
		GUI.Label(new Rect( 5, 400, 150, 40 ),"Input Gap:" + m_scoringManager.m_lastResult.timingError);

		GUI.Label(
			new Rect( 5, 420, 150, 40 ),
			"Previous Input:"
			+ m_playerAction.lastActionInfo.triggerBeatTiming.ToString());
		GUI.Label(new Rect( 5, 440, 150, 40 ),
			"Nearest(beat):"
			+ m_musicManager.currentSongInfo.onBeatActionSequence[m_scoringManager.m_lastResult.markerIndex].triggerBeatTiming.ToString());
		GUI.Label(
			new Rect( 150, 440, 150, 40 ),
			"Nearest(index):"
			+ m_musicManager.currentSongInfo.onBeatActionSequence[m_scoringManager.m_lastResult.markerIndex].line_number.ToString());
		
		// 関連するパート名を表示
		if( m_musicManager.currentSongInfo.onBeatActionRegionSequence.Count>0 ){
			//現在のパートのインデックスを確認
			int currentReginIndex = m_actionInfoRegionSeeker.nextIndex - 1;
			if (currentReginIndex < 0)
				currentReginIndex = 0;
			//前回入力時のパートを表示
			if (m_playerAction.currentPlayerAction != PlayerActionEnum.None)
			{	
				previousHitRegionName
					= m_musicManager.currentSongInfo.onBeatActionRegionSequence[currentReginIndex].name;
			}
			GUI.Label(new Rect(150, 420, 250, 40), "region ...:" + previousHitRegionName);
            //显示当前部分
            GUI.Label(new Rect(5, 460, 150, 40), "Current:" + m_musicManager.beatCountFromStart);
			GUI.Label(new Rect(150, 460, 250, 40), "region ...:" + m_musicManager.currentSongInfo.onBeatActionRegionSequence[currentReginIndex].name);
		}

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
