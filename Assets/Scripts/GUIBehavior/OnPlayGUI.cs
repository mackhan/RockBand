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
    /// 何时开始显示标记，节拍要提前多少拍显示，经验值
    /// </summary>
	public static float markerEnterOffset = 2.5f;

    /// <summary>
    /// 每拍需要移动多少个像素
    /// </summary>
    float m_pixelsPerBeatsY;

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
    /// 目标图标的初始X，就是边缘的宽度30
    /// </summary>
	float m_fMarkerOriginX;

    /// <summary>
    /// 目标图标的初始Y，就是舞台的位置
    /// </summary>
    float m_fMarkerOriginY;

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

    public GameObject[] RhythmGameObject;

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

    GameObject[] m_kHitEffects = new GameObject[4];

    float fScale;

    void Start()
    {
        m_musicManager = GameObject.Find("MusicManager").GetComponent<MusicManager>();
        m_scoringManager = GameObject.Find("ScoringManager").GetComponent<ScoringManager>();
        m_playerAvator = GameObject.Find("PlayerAvator");

        m_kMato = transform.Find("mato").gameObject;
        Debug.Assert(m_kMato != null);

        fScale = Screen.width / 750.0f;

        m_fMarkWeight = 86 * fScale;
        m_fMarkerOriginX = 30 * fScale;
        m_fMarkerOriginY = (1334 - 215) * fScale;

        //-每秒移动这么多
        m_pixelsPerBeatsY = (Screen.height * fScale - 408 * fScale) * 1.0f / markerEnterOffset;

        //Object kHitEffect = Resources.Load("Perfeb/HitEffect", typeof(GameObject)); 
        //m_kHitEffects[0] = Instantiate(kHitEffect) as GameObject;
        //m_kHitEffects[1] = Instantiate(kHitEffect) as GameObject;
        //m_kHitEffects[2] = Instantiate(kHitEffect) as GameObject;
        //m_kHitEffects[3] = Instantiate(kHitEffect) as GameObject;
    }

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

    int[] HitIndex = new int[4];

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
        HitIndex[_iIndex] = actionInfoIndex;

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
#if UNITY_EDITOR
        //-编辑器下按了开发键
        if (GUI.Button(new Rect((Screen.width - 150) / 2.0f, 360, 150, 40), "Development"))
        {
            if (ScoringManager.DebugTest == true)
            {
                ScoringManager.DebugTest = false;
            }
            else
            {
                ScoringManager.DebugTest = true;
            }
        }
#endif

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

    //Vector3 m_kVerctorTemp = Vector3.zero;

    /// <summary>
    /// Drew the specified _iIndex and markerSize.
    /// </summary>
    /// <returns>The drew.</returns>
    /// <param name="_iIndex">I index.</param>
    /// <param name="markerSize">Marker size.</param>
    void Drew(int _iIndex, float markerSize)
    {
        //-X的位置，每个需要增加m_fMarkWeight，因为是用GUI渲染需要再减去markerSize / 2.0f
        float x = m_fMarkerOriginX + m_fMarkWeight + m_fMarkWeight * 2 * _iIndex - markerSize / 2.0f;
        float y = m_fMarkerOriginY - m_fMarkWeight;

        RhythmGameObject[_iIndex].transform.position = new Vector3(x + markerSize / 2.0f, 408*fScale, 0.0f);

        SongInfo song = m_musicManager.currentSongInfo;

        //标记开始和结束索引显示。
        int begin = m_kSeekersBack.GetSeeker(_iIndex).nextIndex;
        int end = m_kSeekersFront.GetSeeker(_iIndex).nextIndex;

        //绘制一个显示动作时间的图标。
        for (int drawnIndex = begin; drawnIndex < end; drawnIndex++)
        {
            if (HitIndex[_iIndex] == drawnIndex)
            {
                continue;
            }
            OnBeatActionInfo info = song.onBeatActionSequence[_iIndex][drawnIndex];
            //当兴奋值高，并且是跳跃的时候,放大一些
            if (m_scoringManager.temper > ScoringManager.temperThreshold
                && info.playerActionType == PlayerActionEnum.Jump)
            {
                markerSize *= 1.2f;
            }

            // 求到显示位置的X坐标的偏移,用提前显示的轨道播放的位置减去当前播放拍子的差值
            float fYoffset = info.triggerBeatTiming - m_musicManager.beatCountFromStart;
            //-Debug.Log("fYoffset:" + fYoffset);
            fYoffset *= m_pixelsPerBeatsY;//-时间乘以速度等于距离

            Rect drawRect = new Rect(x 
                , y - fYoffset
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

#if false //-调试用，画出命中的位置
        for (int i = 0; i < 4; i++)
        {
            OnBeatActionInfo sTestInfo = song.onBeatActionSequence[_iIndex][m_kSeeker.GetSeeker(i).nextIndex];
            float fYoffset = sTestInfo.triggerBeatTiming - m_musicManager.beatCountFromStart;
            fYoffset *= m_pixelsPerBeatsY;
            Rect drawRect = new Rect(x 
                , m_fMarkerOriginY - markerSize / 2.0f
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

            float fSize = markerSize * scale;
            Rect drawRect3 = new Rect(
                x + markerSize / 2.0f - fSize / 2.0f,
                m_fMarkerOriginY - m_fMarkWeight - fSize / 2.0f,
                fSize,
                fSize);
            GUI.DrawTexture(drawRect3, hitEffectIcon);

            //Vector3 kPos1 = new Vector3(x
            //    , m_fMarkerOriginY, Camera.main.nearClipPlane);
            //Vector3 kPos2 = Camera.main.ScreenToWorldPoint(kPos1);
            //Debug.Log("Pos : " + kPos1 + " : " + kPos2);

            //m_kHitEffects[_iIndex].transform.position = kPos2;
            //m_kHitEffects[_iIndex].active = true;
            //m_kHitEffects[_iIndex].GetComponent<SimpleSpriteAnimation>().BeginAnimation(0, 8);

            m_rythmHitEffectCountDown[_iIndex]--;
        }
        //else
        //{
        //    m_kHitEffects[_iIndex].active = false;
        //}

        //显示Perfect，Good，Bad三种提示
        if (m_messageShowCountDown[_iIndex] > 0)
        {
            GUI.color = new Color(1, 1, 1, m_messageShowCountDown[_iIndex] / 40.0f);
            GUI.DrawTexture(new Rect(x, m_fMarkerOriginY - m_fMarkWeight*2 - 80 * fScale, 150 * fScale, 80 * fScale)
                , messageTexture
                , ScaleMode.ScaleToFit
                , true);
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
