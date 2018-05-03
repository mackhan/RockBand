using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 游戏进行中的GUI
/// </summary>
public class OnPlayGUI : MonoBehaviour
{
    /// <summary>
    /// 最佳的图标
    /// </summary>
	public Texture messageTexture_Best;

    /// <summary>
    /// Good的图标
    /// </summary>
	public Texture messageTexture_Good;

    /// <summary>
    /// Miss的图标
    /// </summary>
	public Texture messageTexture_Miss;

    public Texture headbangingIcon;

    /// <summary>
    /// 当前拍子需要击中的位置
    /// </summary>
	public Texture beatPositionIcon;

    /// <summary>
    /// 集中的特效
    /// </summary>
	public Texture hitEffectIcon;

    /// <summary>
    /// 兴奋度的血条
    /// </summary>
	public Texture temperBar;
	public Texture temperBarFrame;

    /// <summary>
    /// 何时开始显示标记，节拍要提前多久显示
    /// </summary>
	public static float markerEnterOffset = 2.5f;

    /// <summary>
    /// 转换成提前多少个像素
    /// </summary>
    float m_pixelsPerBeats = Screen.width * 1.0f / markerEnterOffset;

    /// <summary>
    /// 计时结束显示标记，节拍要延迟多久结束
    /// </summary>
	public static float markerLeaveOffset = -1.0f;   

    /// <summary>
    /// 消息显示的时长
    /// </summary>
    public static int messatShowFrameDuration = 40;

    /// <summary>
    /// 点击效果显示的时长
    /// </summary>
    public static int rythmHitEffectShowFrameDuration = 20;

    /// <summary>
    /// Perfect，good，bad图标显示的时长
    /// </summary>
    int m_messageShowCountDown = 0;

    /// <summary>
    /// 点击效果显示的时长
    /// </summary>
    int m_rythmHitEffectCountDown = 0;

    /// <summary>
    /// 上一次的分数
    /// </summary>
	float m_lastInputScore = 0;

    Color m_blinkColor = new Color(1, 1, 1);
    
    public bool isDevelopmentMode = false;

    /// <summary>
    /// 目标图标的中心位置
    /// </summary>
	public Vector2 markerOrigin = new Vector2(20.0f, 300.0f);

	public GUISkin guiSkin;

    /// <summary>
    /// 时间先进的查找单位（显示结束位置）。
    /// </summary>
    SequenceSeeker<OnBeatActionInfo> m_seekerFront = new SequenceSeeker<OnBeatActionInfo>();

    /// <summary>
    /// 寻找单位（显示开始位置），在后面的时间。 
    /// </summary>
    SequenceSeeker<OnBeatActionInfo> m_seekerBack = new SequenceSeeker<OnBeatActionInfo>();

    MusicManager m_musicManager;

    ScoringManager m_scoringManager;

    GameObject m_playerAvator;

    Texture messageTexture;

    GameObject m_kMato;

    /// <summary>
    /// PlayUI的初始化
    /// </summary>
	public void BeginVisualization()
    {
		m_musicManager = GameObject.Find("MusicManager").GetComponent<MusicManager>();
		m_scoringManager = GameObject.Find("ScoringManager").GetComponent<ScoringManager>();

        m_seekerBack.SetSequence(m_musicManager.currentSongInfo.onBeatActionSequence);
        m_seekerBack.Seek(markerLeaveOffset);

        m_seekerFront.SetSequence(m_musicManager.currentSongInfo.onBeatActionSequence);
		m_seekerFront.Seek(markerEnterOffset);
	}

    /// <summary>
    /// 根据玩家的操作结果播放音效和显示消息
    /// </summary>
    /// <param name="actionInfoIndex"></param>
    /// <param name="score"></param>
    public void RythmHitEffect(int actionInfoIndex, float score)
    {
        m_lastInputScore = score;
		m_rythmHitEffectCountDown = rythmHitEffectShowFrameDuration;
		m_messageShowCountDown = messatShowFrameDuration;

        //-根据操作播放音效，这里好坏的音效放在了角色的身上
        AudioClip kAudioClip;
        PlayerAction kPlayerAction = m_playerAvator.GetComponent<PlayerAction>();
        if (score < 0)
        {
            kAudioClip = kPlayerAction.headBangingSoundClip_BAD;
			messageTexture = messageTexture_Miss;
		}
		else if (score <= ScoringManager.goodScore)
        {
            kAudioClip = kPlayerAction.headBangingSoundClip_GOOD;
			messageTexture = messageTexture_Good;
		}
		else
        {
            kAudioClip = kPlayerAction.headBangingSoundClip_GOOD;
			messageTexture = messageTexture_Best;
		}
        
        //-播放Best，Good，bad的音效，播放器也放在主角的身上
        AudioSource kAudioSource = m_playerAvator.GetComponent<AudioSource>();
        kAudioSource.clip = kAudioClip;
        kAudioSource.Play();
    }

	void Start()
    {
		m_musicManager = GameObject.Find("MusicManager").GetComponent<MusicManager>();
		m_scoringManager = GameObject.Find("ScoringManager").GetComponent<ScoringManager>();
		m_playerAvator = GameObject.Find("PlayerAvator");

        m_kMato = transform.Find("mato").gameObject;
        Debug.Assert(m_kMato != null);
    }

    public void Seek(float beatCount)
    {
		m_seekerBack.Seek(beatCount + markerLeaveOffset);
		m_seekerFront.Seek(beatCount + markerEnterOffset);
	}

	void Update ()
    {
		if(m_musicManager.IsPlaying())//-更新拍子，一帧可能有多个拍子
        {
			m_seekerBack.ProceedTime(m_musicManager.DeltaBeatCountFromStart);
			m_seekerFront.ProceedTime(m_musicManager.DeltaBeatCountFromStart);
		}
	}

	void OnGUI()
    {
        //-分数显示
        GUI.Box(new Rect(15,5,100,30), "");
		GUI.Label(new Rect(20,10,90,20), "Score: " + m_scoringManager.score);

        //-兴奋闪烁颜色
        if (m_scoringManager.temper > ScoringManager.temperThreshold)
		{
			m_blinkColor.g = m_blinkColor.b	
                = 0.7f + 0.3f * Mathf.Abs(Time.frameCount % Application.targetFrameRate - Application.targetFrameRate / 2) / (float)Application.targetFrameRate;
			GUI.color = m_blinkColor;
		}

        //-兴奋度
        Rect heatBarFrameRect = new Rect(180.0f, 20.0f, 100.0f, 20.0f);

        //-显示兴奋度的文字
        Rect heatBarLabelRect = heatBarFrameRect;
        heatBarLabelRect.y = heatBarFrameRect.y - 20;
        GUI.Label(heatBarLabelRect, "Temper");

        //-显示兴奋度的能量槽
        Rect heatBarRect = heatBarFrameRect;
		heatBarRect.width *= m_scoringManager.temper;
		GUI.Box(heatBarFrameRect, "");

        //-显示兴奋度的值
		GUI.DrawTextureWithTexCoords(heatBarRect
            , temperBar
            , new Rect(0.0f, 0.0f, 1.0f * m_scoringManager.temper
            , 1.0f)
		);

		GUI.color = Color.white;

        //-计算目标拍子的ICON的大小。显示当前需要击中的位置
        float markerSize = ScoringManager.timingErrorToleranceGood * m_pixelsPerBeats;
        float x = markerOrigin.x - markerSize / 2.0f;
        float y = markerOrigin.y - markerSize / 2.0f;

        //-显示目标拍子的ICON
  //      Graphics.DrawTexture(
  //	new Rect(x, y, markerSize, markerSize)
  //	, beatPositionIcon
  //);

        if (m_musicManager.IsPlaying())
        {
			SongInfo song =  m_musicManager.currentSongInfo;

            //标记开始显示。
            int begin = m_seekerBack.nextIndex;

            //标记结束显示。
            int end   = m_seekerFront.nextIndex;
			float x_offset;

            //绘制一个显示动作时间的图标。
            for (int drawnIndex = begin; drawnIndex < end; drawnIndex++)
            {
				OnBeatActionInfo info = song.onBeatActionSequence[drawnIndex];
                //当兴奋值高，并且是跳跃的时候,放大一些
                if (m_scoringManager.temper > ScoringManager.temperThreshold 
                    && info.playerActionType == PlayerActionEnum.Jump)
				{
                    markerSize *= 1.5f;
				}

                // 求到显示位置的X坐标的偏移.???
                x_offset = info.triggerBeatTiming - m_musicManager.beatCountFromStart;
				x_offset *= m_pixelsPerBeats;

				Rect drawRect = new Rect(
                    x + x_offset,
                    y,
                    markerSize,
                    markerSize
                );

                GUI.DrawTexture(drawRect, headbangingIcon);
                //-m_kMato.transform.position = new Vector3(x + x_offset, y, 0.0f);

                GUI.color = Color.white;

                // 在文本文件中显示行号。
                if (isDevelopmentMode)
                {
					GUI.skin = this.guiSkin;
					GUI.Label(new Rect(drawRect.x, drawRect.y - 10.0f, 50.0f, 30.0f), info.line_number.ToString());
					GUI.skin = null;
				}
			}

            //-点中爆裂的效果
            if (m_rythmHitEffectCountDown > 0)
            {
				float scale  = 2.0f - m_rythmHitEffectCountDown / (float)rythmHitEffectShowFrameDuration;//-依据点击效果的时间确认缩放，一开始1倍，最后是2倍，逐渐变大

                //-再依据上一次的得分调整倍率
                if (m_lastInputScore >= ScoringManager.excellentScore)
                {
					scale *= 2.0f;
				}
				else if( m_lastInputScore > ScoringManager.missScore)
                {
					scale *= 1.2f;
				}
				else
                {
					scale *= 0.5f;
				}
				float baseSize = 32.0f;
				Rect drawRect3 = new Rect(
					markerOrigin.x - baseSize * scale / 2.0f,
					markerOrigin.y - baseSize * scale / 2.0f,
					baseSize * scale,
					baseSize * scale
				);
				Graphics.DrawTexture(drawRect3, hitEffectIcon);
				m_rythmHitEffectCountDown--;
			}

			//显示Perfect，Good，Bad三种提示
			if (m_messageShowCountDown > 0)
            {
				GUI.color = new Color(1, 1, 1, m_messageShowCountDown / 40.0f);
				GUI.DrawTexture(new Rect(20 ,230, 150, 50), messageTexture, ScaleMode.ScaleAndCrop, true);
				GUI.color = Color.white;
				m_messageShowCountDown--;
			}
		}
	}
}
