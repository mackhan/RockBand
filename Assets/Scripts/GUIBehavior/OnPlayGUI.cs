using UnityEngine;
using UnityEngine.UI;
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

    /// <summary>
    /// 滚动音符的图标
    /// </summary>
    public Texture headbangingIcon;

    /// <summary>
    /// 当前拍子需要击中的位置
    /// </summary>
	public Texture beatPositionIcon;

    /// <summary>
    /// 击中的特效
    /// </summary>
	public Texture hitEffectIcon;

    /// <summary>
    /// 兴奋度的血条
    /// </summary>
	public Texture temperBar;

    /// <summary>
    /// 兴奋度的底
    /// </summary>
	public Texture temperBarFrame;

    /// <summary>
    /// 何时开始显示标记，节拍要提前多少拍显示，经验值
    /// </summary>
	public static float markerEnterOffset = 2.5f;

    /// <summary>
    /// 每拍需要移动多少个像素
    /// </summary>
    float m_pixelsPerBeatsY = Screen.height * 1.0f / markerEnterOffset;

    /// <summary>
    /// 计时结束显示标记，节拍要延迟多久结束，和开始的时间差就是通过画面需要的时间
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
    int[] m_messageShowCountDown = new int[4];

    /// <summary>
    /// 点击效果显示的时长
    /// </summary>
    int[] m_rythmHitEffectCountDown = new int[4];

    /// <summary>
    /// 上一次的分数
    /// </summary>
	float m_lastInputScore = 0;

    /// <summary>
    /// 兴奋闪烁的颜色
    /// </summary>
    Color m_blinkColor = new Color(1, 1, 1);

    public bool isDevelopmentMode = false;

    /// <summary>
    /// 目标图标的中心位置
    /// </summary>
	public Vector2 markerOrigin = new Vector2(20.0f, 300.0f);

    public GUISkin guiSkin;

    ///// <summary>
    ///// 时间先进的查找单位（显示结束位置）。
    ///// </summary>
    SequenceSeekers<OnBeatActionInfo> m_kSeekersFront = new SequenceSeekers<OnBeatActionInfo>();

    /// <summary>
    /// 寻找单位（显示开始位置），在后面的时间。 
    /// </summary>
    SequenceSeekers<OnBeatActionInfo> m_kSeekersBack = new SequenceSeekers<OnBeatActionInfo>();

    /// <summary>
    /// 测试
    /// </summary>
    SequenceSeekers<OnBeatActionInfo> m_kSeeker = new SequenceSeekers<OnBeatActionInfo>();

    MusicManager m_musicManager;

    ScoringManager m_scoringManager;

    GameObject m_playerAvator;

    Texture messageTexture;

    /// <summary>
    /// UGUI中的音符，目前不使用
    /// </summary>
    GameObject m_kMato;

    /// <summary>
    /// 按钮的位置
    /// </summary>
    GameObject m_kButtonPiano;

    GameObject m_kButtonGuitar;

    GameObject m_kButtonDrum;

    GameObject m_kButtonBass;

    /// <summary>
    /// PlayUI的初始化
    /// </summary>
	public void BeginVisualization()
    {
        m_musicManager = GameObject.Find("MusicManager").GetComponent<MusicManager>();
        m_scoringManager = GameObject.Find("ScoringManager").GetComponent<ScoringManager>();

        m_kSeekersBack.SetSequence(m_musicManager.currentSongInfo.onBeatActionSequence);
        m_kSeekersBack.Seek(markerLeaveOffset);

        m_kSeekersFront.SetSequence(m_musicManager.currentSongInfo.onBeatActionSequence);
        m_kSeekersFront.Seek(markerEnterOffset);

        m_kSeeker.SetSequence(m_musicManager.currentSongInfo.onBeatActionSequence);
        m_kSeeker.Seek(0.0f);
    }

    /// <summary>
    /// 根据玩家的操作结果播放音效和显示消息
    /// </summary>
    /// <param name="actionInfoIndex"></param>
    /// <param name="score"></param>
    public void RythmHitEffect(int _iIndex, int actionInfoIndex, float score)
    {
        m_lastInputScore = score;
        m_rythmHitEffectCountDown[_iIndex] = rythmHitEffectShowFrameDuration;
        m_messageShowCountDown[_iIndex] = messatShowFrameDuration;

        //-根据操作播放音效，这里好坏的音效放在了角色的身上
        AudioClip kAudioClip;
        PlayerAction kPlayerAction = m_playerAvator.GetComponent<PlayerAction>();
        if (score == 0)
        {
            Debug.Log("Empty");
            m_messageShowCountDown[_iIndex] = 0;
            return;
        }
        else if (score < 0)
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

        m_kButtonPiano = transform.Find("ButtonPiano").gameObject;
        Debug.Assert(m_kButtonPiano != null);
        m_kButtonGuitar = transform.Find("ButtonGuitar").gameObject;
        Debug.Assert(m_kButtonGuitar != null);
        m_kButtonDrum = transform.Find("ButtonDrum").gameObject;
        Debug.Assert(m_kButtonDrum != null);
        m_kButtonBass = transform.Find("ButtonBass").gameObject;
        Debug.Assert(m_kButtonBass != null);

        markerOrigin = new Vector2(Screen.width / 8.0f, Screen.height - Screen.height * 1 / 3);
    }

    /// <summary>
    /// 定位
    /// </summary>
    /// <returns>The seek.</returns>
    /// <param name="beatCount">Beat count.</param>
    public void Seek(float beatCount)
    {
        float fCount = beatCount + markerEnterOffset;
        m_kSeekersBack.Seek(fCount);
        m_kSeekersFront.Seek(fCount);
        m_kSeeker.Seek(fCount);
    }

    void Update()
    {
        if (m_musicManager.IsPlaying())//-更新拍子，一帧可能有多个拍子
        {
            m_kSeekersBack.ProceedTime(m_musicManager.DeltaBeatCountFromStart);
            m_kSeekersFront.ProceedTime(m_musicManager.DeltaBeatCountFromStart);
            m_kSeeker.Seek(m_musicManager.DeltaBeatCountFromStart);
        }
    }

    void OnGUI()
    {
        float fScale = Screen.height / 1334;

        //-分数显示
        GUI.Box(new Rect(15 * fScale, 5 * fScale, 200 * fScale, 60 * fScale), "");
        GUI.Label(new Rect(20 * fScale, 10 * fScale, 180 * fScale, 40 * fScale)
                  , "Score: " + m_scoringManager.score);

        //-兴奋闪烁颜色
        if (m_scoringManager.temper > ScoringManager.temperThreshold)
        {
            m_blinkColor.g = m_blinkColor.b
                = 0.7f + 0.3f * Mathf.Abs(Time.frameCount % Application.targetFrameRate - Application.targetFrameRate / 2) / (float)Application.targetFrameRate;
            GUI.color = m_blinkColor;
        }

        //-兴奋度
        Rect heatBarFrameRect = new Rect(360.0f * fScale, 40.0f * fScale, 200.0f * fScale, 40.0f * fScale);

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
            , 1.0f));

        GUI.color = Color.white;

        //-调试功能，加速减速
        if (GUI.Button(new Rect(Screen.width - 300 * fScale, 80 * fScale, 300 * fScale, 80 * fScale), "Speed+"))
        {
            m_pixelsPerBeatsY += Screen.height / 10;
        }
        if (GUI.Button(new Rect(Screen.width - 300 * fScale, 240 * fScale, 300 * fScale, 80 * fScale), "Speed-"))
        {
            m_pixelsPerBeatsY -= Screen.height / 10;
        }

        //-计算目标拍子的ICON的大小。显示当前需要击中的位置
        //-float markerSize = ScoringManager.timingErrorToleranceGood * Screen.height / markerEnterOffset;
        float markerSize = Screen.width / 4.5f;

        //-显示目标拍子的ICON
        m_kButtonPiano.transform.position = new Vector2(markerOrigin.x * 1, Screen.height / 3);
        m_kButtonGuitar.transform.position = new Vector2(markerOrigin.x * 3, Screen.height / 3);
        m_kButtonDrum.transform.position = new Vector2(markerOrigin.x * 5, Screen.height / 3);
        m_kButtonBass.transform.position = new Vector2(markerOrigin.x * 7, Screen.height / 3);

        if (m_musicManager.IsPlaying() || Time.timeScale == 0.0f)
        {
            for (int i = 0; i < ConstantManager.MemberNum; i++)
            {
                Drew(i, markerSize);
            }
        }
    }

    /// <summary>
    /// Drew the specified _iIndex and markerSize.
    /// </summary>
    /// <returns>The drew.</returns>
    /// <param name="_iIndex">I index.</param>
    /// <param name="markerSize">Marker size.</param>
    void Drew(int _iIndex, float markerSize)
    {
        float fScale = Screen.height / 1334;

        float x = markerOrigin.x - markerSize / 2.0f;
        float y = markerOrigin.y - markerSize / 2.0f;

        SongInfo song = m_musicManager.currentSongInfo;

        //标记开始显示。
        int begin = m_kSeekersBack.GetSeeker(_iIndex).nextIndex;

        //标记结束显示。
        int end = m_kSeekersFront.GetSeeker(_iIndex).nextIndex;
        float x_offset = Screen.width / 4 * _iIndex;
        float fYoffset;

        //绘制一个显示动作时间的图标。
        for (int drawnIndex = begin; drawnIndex < end; drawnIndex++)
        {
            OnBeatActionInfo info = song.onBeatActionSequence[_iIndex][drawnIndex];
            //当兴奋值高，并且是跳跃的时候,放大一些
            if (m_scoringManager.temper > ScoringManager.temperThreshold
                && info.playerActionType == PlayerActionEnum.Jump)
            {
                markerSize *= 1.2f;
            }

            // 求到显示位置的X坐标的偏移,用提前显示的轨道播放的位置减去当前播放拍子的差值
            fYoffset = info.triggerBeatTiming - m_musicManager.beatCountFromStart;
            fYoffset *= m_pixelsPerBeatsY;

            Rect drawRect = new Rect(x + x_offset//x + x_offset,
                , y - fYoffset//y
                , markerSize
                , markerSize);

            GUI.DrawTexture(drawRect, headbangingIcon);

            GUI.color = Color.white;

            // 在文本文件中显示行号。
            if (isDevelopmentMode)
            {
                GUI.skin = this.guiSkin;
                GUI.Label(new Rect(drawRect.x
                                   , drawRect.y - 10.0f
                                   , 50.0f * fScale
                                   , 30.0f * fScale), info.line_number.ToString());
                GUI.skin = null;
            }
        }

#if false //-没有问题
        for (int i = 0; i < 4; i++)
        {
            OnBeatActionInfo sTestInfo = song.onBeatActionSequence[_iIndex][m_kSeeker.GetSeeker(i).nextIndex];
            fYoffset = sTestInfo.triggerBeatTiming - m_musicManager.beatCountFromStart;
            fYoffset *= m_pixelsPerBeatsY;
            Rect drawRect = new Rect(x + x_offset
                , y
                , markerSize
                , markerSize);

            GUI.DrawTexture(drawRect, hitEffectIcon);  
        }
#endif

        //-点中爆裂的效果
        if (m_rythmHitEffectCountDown[_iIndex] > 0)
        {
            float scale = 2.0f - m_rythmHitEffectCountDown[_iIndex] / (float)rythmHitEffectShowFrameDuration;//-依据点击效果的时间确认缩放，一开始1倍，最后是2倍，逐渐变大

            //-再依据上一次的得分调整倍率
            if (m_lastInputScore >= ScoringManager.excellentScore)
            {
                scale *= 2.0f;
            }
            else if (m_lastInputScore > ScoringManager.missScore)
            {
                scale *= 1.2f;
            }
            else
            {
                scale *= 0.5f;
            }

            float baseSize = markerSize;//32.0f;
            Rect drawRect3 = new Rect(
                markerOrigin.x - baseSize * scale / 2.0f + x_offset,
                markerOrigin.y - baseSize * scale / 2.0f,
                baseSize * scale,
                baseSize * scale);
            Graphics.DrawTexture(drawRect3, hitEffectIcon);
            m_rythmHitEffectCountDown[_iIndex]--;
        }

        //显示Perfect，Good，Bad三种提示
        if (m_messageShowCountDown[_iIndex] > 0)
        {
            GUI.color = new Color(1, 1, 1, m_messageShowCountDown[_iIndex] / 40.0f);
            GUI.DrawTexture(new Rect(20 + x_offset, markerOrigin.y - markerSize, 150, 80), messageTexture, ScaleMode.ScaleToFit, true);
            GUI.color = Color.white;
            m_messageShowCountDown[_iIndex]--;
        }
    }

    /// <summary>
    /// 按了暂停
    /// </summary>
    /// <param name="pause">If set to <c>true</c> pause.</param>
	public void OnPause(bool pause)
    {
        GameObject.Find("PhaseManager").GetComponent<PhaseManager>().SetPhase("Pause");
        if (Time.timeScale == 1.0f)
        {
            Debug.Log("OnPause:" + 0.0f);
            Time.timeScale = 0.0f;
            EventManager.Pause = true;
            MusicManager.Pause = true;
            ScoringManager.Pause = true;
        }
    }

    public void OnContinue()
    {
        GameObject.Find("PhaseManager").GetComponent<PhaseManager>().SetPhase("Continue");
        if (Time.timeScale == 0.0f)
        {
            Debug.Log("OnPause:" + 1.0f);
            Time.timeScale = 1.0f;
            EventManager.Pause = false;
            MusicManager.Pause = false;
            ScoringManager.Pause = false;
        }
    }

    public void OnReset()
    {
        GameObject.Find("PhaseManager").GetComponent<PhaseManager>().SetPhase("Reset");
        if (Time.timeScale == 0.0f)
        {
            Debug.Log("OnPause:" + 1.0f);
            Time.timeScale = 1.0f;
            EventManager.Pause = false;
            MusicManager.Pause = false;
            ScoringManager.Pause = false;
        }
    }

    public void OnRestart()
    {
        GameObject.Find("PhaseManager").GetComponent<PhaseManager>().SetPhase("Restart");
        if (Time.timeScale == 0.0f)
        {
            Debug.Log("OnPause:" + 1.0f);
            Time.timeScale = 1.0f;
            EventManager.Pause = false;
            MusicManager.Pause = false;
            ScoringManager.Pause = false;
        }
    }

    public void OnSpeedUp(Text TextText)
    {
        m_pixelsPerBeatsY += Screen.height / 10;
        if (m_pixelsPerBeatsY > Screen.height)
        {
            m_pixelsPerBeatsY = Screen.height;
        }
        TextText.text = m_pixelsPerBeatsY.ToString();
    }

    public void OnSpeedDown(Text TextText)
    {
        m_pixelsPerBeatsY -= Screen.height / 10;
        if (m_pixelsPerBeatsY < 0)
        {
            m_pixelsPerBeatsY = 0.0f;
        }
        TextText.text = m_pixelsPerBeatsY.ToString();
    }
}
