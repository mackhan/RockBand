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
    /// 目标图标的初始位置
    /// </summary>
	Vector2 markerOrigin = new Vector2(30.0f, 300.0f);

    float m_fMarkHitHeight;

    /// <summary>
    /// 目标图标的宽度
    /// </summary>
    float m_fMarkWeight = 0.0f;

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
    /// 得分的UI
    /// </summary>
    public Text TextScore;

    /// <summary>
    /// 兴奋度的能量控制
    /// </summary>
    public Image ImageTemper;

    /// <summary>
    /// UGUI中的音符，目前不使用
    /// </summary>
    GameObject m_kMato;

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
    /// <param name="_fAdditionalScore"></param>
    public void RythmHitEffect(int _iIndex
        , int actionInfoIndex
        , float _fAdditionalScore
        , float _fScore)
    {
        //-分数显示
        TextScore.text = _fScore.ToString();
        m_lastInputScore = _fAdditionalScore;
        m_rythmHitEffectCountDown[_iIndex] = rythmHitEffectShowFrameDuration;
        m_messageShowCountDown[_iIndex] = messatShowFrameDuration;

        //-兴奋闪烁颜色
        if (m_scoringManager.temper > ScoringManager.temperThreshold)
        {
            m_blinkColor.g = m_blinkColor.b
                = 0.7f + 0.3f * Mathf.Abs(Time.frameCount % Application.targetFrameRate - Application.targetFrameRate / 2) / (float)Application.targetFrameRate;
            ImageTemper.color = m_blinkColor;
        }

        //-显示兴奋度的能量槽
        ImageTemper.fillAmount = m_scoringManager.temper;
        
        //-根据操作播放音效，这里好坏的音效放在了角色的身上
        AudioClip kAudioClip;
        PlayerAction kPlayerAction = m_playerAvator.GetComponent<PlayerAction>();
        if (_fAdditionalScore == 0)
        {
            Debug.Log("Empty");
            m_messageShowCountDown[_iIndex] = 0;
            return;
        }
        else if (_fAdditionalScore < 0)
        {
            kAudioClip = kPlayerAction.headBangingSoundClip_BAD;
            messageTexture = messageTexture_Miss;
        }
        else if (_fAdditionalScore <= ScoringManager.goodScore)
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

        float fScaleH = 1334 / Screen.height;

        m_fMarkWeight = 86 * fScaleH;
        markerOrigin = new Vector2(30 * fScaleH
            , 408 * fScaleH);        
        m_fMarkHitHeight = (1334 - 215) * fScaleH + m_fMarkWeight;
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
        //-计算目标拍子的ICON的大小。显示当前需要击中的位置
        float markerSize = Screen.width / 6f;

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
        float x = markerOrigin.x + m_fMarkWeight * 2 * _iIndex;
        float y = markerOrigin.y;

        SongInfo song = m_musicManager.currentSongInfo;

        //标记开始显示。
        int begin = m_kSeekersBack.GetSeeker(_iIndex).nextIndex;

        //标记结束显示。
        int end = m_kSeekersFront.GetSeeker(_iIndex).nextIndex;

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
            float fYoffset = info.triggerBeatTiming - m_musicManager.beatCountFromStart;
            fYoffset *= m_pixelsPerBeatsY;

            Rect drawRect = new Rect(x
                , y - fYoffset
                , markerSize
                , markerSize);

            GUI.DrawTexture(drawRect, headbangingIcon);

            GUI.color = Color.white;

            // 在文本文件中显示行号。
            if (isDevelopmentMode)
            {
                //-适配用
                float fScale = Screen.height / 1334;

                GUI.skin = this.guiSkin;
                GUI.Label(new Rect(drawRect.x
                                   , drawRect.y - 10.0f
                                   , 50.0f * fScale
                                   , 30.0f * fScale), info.line_number.ToString());
                GUI.skin = null;
            }
        }

#if true //-没有问题
        for (int i = 0; i < 4; i++)
        {
            OnBeatActionInfo sTestInfo = song.onBeatActionSequence[_iIndex][m_kSeeker.GetSeeker(i).nextIndex];
            float fYoffset = sTestInfo.triggerBeatTiming - m_musicManager.beatCountFromStart;
            fYoffset *= m_pixelsPerBeatsY;
            Rect drawRect = new Rect(x 
                , y
                , markerSize
                , markerSize);

            GUI.DrawTexture(drawRect, headbangingIcon);  
        }

        for (int i = 0; i < 4; i++)
        {
            OnBeatActionInfo sTestInfo = song.onBeatActionSequence[_iIndex][m_kSeeker.GetSeeker(i).nextIndex];
            float fYoffset = sTestInfo.triggerBeatTiming - m_musicManager.beatCountFromStart;
            fYoffset *= m_pixelsPerBeatsY;
            Rect drawRect = new Rect(x 
                , m_fMarkHitHeight
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

            float baseSize = markerSize;
            Rect drawRect3 = new Rect(
                x,
                m_fMarkHitHeight,
                baseSize * scale,
                baseSize * scale);
            Graphics.DrawTexture(drawRect3, hitEffectIcon);
            m_rythmHitEffectCountDown[_iIndex]--;
        }

        //显示Perfect，Good，Bad三种提示
        if (m_messageShowCountDown[_iIndex] > 0)
        {
            GUI.color = new Color(1, 1, 1, m_messageShowCountDown[_iIndex] / 40.0f);
            GUI.DrawTexture(new Rect(x, m_fMarkHitHeight, 150, 80), messageTexture, ScaleMode.ScaleToFit, true);
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
