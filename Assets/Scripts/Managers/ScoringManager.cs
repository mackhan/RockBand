using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// 判断玩家输入是否成功，计算得分等
/// </summary>
public class ScoringManager : MonoBehaviour
{
    /// <summary>
    /// 空按的时机
    /// </summary>
    public static float timingErrorMiss = 0.5f;

    /// <summary>
    /// Good的时机，如果时间差距低于这个就是Error
    /// </summary>
	public static float timingErrorToleranceGood = 0.22f;

    /// <summary>
    /// Excellent的时机, 如果时偏差小于这个就是Good
    /// </summary>
    public static float timingErrorTorelanceExcellent = 0.12f;

    /// <summary>
    /// 失败的得分
    /// </summary>
    public static float missScore = -100.0f;

    /// <summary>
    /// Good的得分
    /// </summary>
	public static float goodScore = 200.0f;

    /// <summary>
    /// Excellent的得分
    /// </summary>
	public static float excellentScore = 400.0f;

    /// <summary>
    /// 中途判定点被判定为“失败”的得分率（得分/理论上的最高得分）
    /// </summary>
	public static float failureScoreRate = 0.3f;

    /// <summary>
    /// 中途判定点被判定为“优秀”的得分率（得分/理论上的最高得分）
    /// </summary>
	public static float excellentScoreRate = 0.85f;

    /// <summary>
    /// 失败的兴奋值
    /// </summary>
    public static float missHeatupRate = -0.08f;

    /// <summary>
    /// Good的兴奋值
    /// </summary>
	public static float goodHeatupRate = 0.01f;

    /// <summary>
    /// Excellent的兴奋值
    /// </summary>
	public static float bestHeatupRate = 0.02f;

    /// <summary>
    /// 兴奋度的门槛
    /// </summary>
    public static float temperThreshold = 0.5f;

    /// <summary>
    /// 是否开启日志功能
    /// </summary>
    public bool outScoringLog = true;

    /// <summary>
    /// 现在的合计分数
    /// </summary>
    public float score
    {
        get { return m_fScore; }
    }
    private float m_fScore;

    /// <summary>
    /// 兴奋的数值化 0.0 - 1.0
    /// </summary>
    float m_temper = 0;
    public float temper
    {
        get { return m_temper; }
        set { m_temper = Mathf.Clamp(value, 0, 1); }
    }

    /// <summary>
    /// 现在的得分率（得分/理论上的最高得分）
    /// </summary>
    public float scoreRate
    {
        get { return m_scoreRate; }
    }
    private float m_scoreRate = 0;

    /// <summary>
    /// 得分的序列计数器
    /// </summary>
    SequenceSeekers<OnBeatActionInfo> m_kScoringUnitSeekers = new SequenceSeekers<OnBeatActionInfo>();

    /// <summary>
    /// 得分系统开启
    /// </summary>
    public void BeginScoringSequence()
    {
        m_kScoringUnitSeekers.SetSequence(m_musicManager.currentSongInfo.onBeatActionSequence);
    }

    void Start()
    {
        m_musicManager = GameObject.Find("MusicManager").GetComponent<MusicManager>();
        m_playerAction = GameObject.Find("PlayerAvator").GetComponent<PlayerAction>();

        //-找到所有成员
        m_bandMembers = GameObject.FindGameObjectsWithTag("BandMember");

        //-找到所有的观众
        m_audiences = GameObject.FindGameObjectsWithTag("Audience");

        //-找到所有的音符特效
        m_noteParticles = GameObject.FindGameObjectsWithTag("NoteParticle");

        //-找到场景管理器
        m_phaseManager = GameObject.Find("PhaseManager").GetComponent<PhaseManager>();

        //-找到主游戏逻辑的界面
        m_onPlayGUI = m_phaseManager.guiList[1].GetComponent<OnPlayGUI>();

#if UNITY_EDITOR
        //-记录日志
        //-string sPath = "Assets/PlayLog/scoringLog.csv";
        //-m_logWriter = new StreamWriter(sPath);
#endif
    }

    /// <summary>
    /// 直接跳到第几拍，调试用
    /// </summary>
    /// <param name="beatCount"></param>
    public void Seek(float beatCount)
    {
        m_kScoringUnitSeekers.Seek(beatCount);
        m_previousHitIndex = -1;
    }

    /// <summary>
    /// 找到最近的谱面索引
    /// </summary>
    /// <returns></returns>
    public int GetNearestPlayerActionInfoIndex(int _iIndex)
    {
        SongInfo song = m_musicManager.currentSongInfo;
        int nearestIndex = 0;

        List<OnBeatActionInfo> kBeatActionInfos = song.onBeatActionSequence[_iIndex];
        SequenceSeeker<OnBeatActionInfo> kSeeker = m_kScoringUnitSeekers.GetSeeker(_iIndex);

        if (kSeeker.nextIndex == 0)//-如果目的位置是开头的话，没有前一个标记，所以不比较。
        {
            nearestIndex = 0;
        }
        else if (kSeeker.nextIndex >= kBeatActionInfos.Count)//-当前索引的位置比序列的尺寸大的时候（过了最后的标记时间的时候）应该不会大于
        {
            Debug.Assert(kSeeker.nextIndex > kBeatActionInfos.Count);
            nearestIndex = kBeatActionInfos.Count - 1;
        }
        else//与前后的定时相比较
        {
            OnBeatActionInfo crnt_action = kBeatActionInfos[kSeeker.nextIndex];//-当前位置
            OnBeatActionInfo prev_action = kBeatActionInfos[kSeeker.nextIndex - 1];//-前一个位置

            float act_timing = m_playerAction.GetLastActionInof(_iIndex).triggerBeatTiming;//-玩家在哪个拍子按下的

            if (crnt_action.triggerBeatTiming - act_timing < act_timing - prev_action.triggerBeatTiming)//-如果是当前位置比较近
            {
                nearestIndex = kSeeker.nextIndex;
            }
            else//-如果是前一个位置比较近
            {
                nearestIndex = kSeeker.nextIndex - 1;
            }
        }

        return (nearestIndex);
    }

    /// <summary>
    /// 暂停控制
    /// </summary>
    public static bool Pause = false;

    void Update()
    {
        if (Pause)
        {
            return;
        }

        m_fAdditionalScore = 0;

        if (m_musicManager.IsPlaying())
        {
            m_kScoringUnitSeekers.ProceedTime(m_musicManager.DeltaBeatCountFromStart);

            for (int i = 0; i < ConstantManager.MemberNum; i++)
            {
                Process(i);
            }
        }
    }

    static public bool DebugTest = false;

    /// <summary>
    /// Process the specified _iIndex.
    /// </summary>
    /// <returns>The process.</returns>
    /// <param name="_iIndex">I index.</param>
    void Process(int _iIndex)
    {
        float additionalTemper = 0;
#if false
        bool hitBefore = false;
        bool hitAfter = false;
#endif

        SequenceSeeker<OnBeatActionInfo> kSeeker = m_kScoringUnitSeekers.GetSeeker(_iIndex);

        int nearestIndex = GetNearestPlayerActionInfoIndex(_iIndex);
        SongInfo song = m_musicManager.currentSongInfo;
        if (song.onBeatActionSequence[_iIndex] == null)
        {
            Debug.Log("Out of range : " + _iIndex);
        }
        if (nearestIndex >= song.onBeatActionSequence[_iIndex].Count)
        {
            //-Debug.Log("Out of range : " + nearestIndex + " >= " + song.onBeatActionSequence[_iIndex].Count);
            return;
        }
        OnBeatActionInfo marker_act = song.onBeatActionSequence[_iIndex][nearestIndex];//-找到最近的谱面
        OnBeatActionInfo player_act = m_playerAction.GetLastActionInof(_iIndex);//-找到玩家操作的信息

        bool bHit = false;
        if (marker_act.triggerBeatTiming <= m_musicManager.beatCountFromStart
            && nearestIndex != m_previousHitIndex && DebugTest == true)
        {
            bHit = true;

            OnBeatActionInfo actionInfo = new OnBeatActionInfo();
            actionInfo.triggerBeatTiming = m_musicManager.beatCountFromStart;
            actionInfo.playerActionType = PlayerActionEnum.Jump;
            player_act = actionInfo;
        }


        if (m_playerAction.GetCurrentPlayerAction(_iIndex) 
            != PlayerActionEnum.None || bHit)//-如果这一拍玩家有操作？？？
        {
            m_lastResults[_iIndex].timingError = player_act.triggerBeatTiming 
                - marker_act.triggerBeatTiming;//-找到玩家操作的拍子和最近谱面时间的差值
            m_lastResults[_iIndex].markerIndex = nearestIndex;

            if (nearestIndex == m_previousHitIndex)//-如果已经判定过了，则扣分
            {
                m_fAdditionalScore = missScore;
                additionalTemper = missHeatupRate; 
            }
            else//-计算得分
            {
                m_fAdditionalScore = CheckScore(nearestIndex
                                               , m_lastResults[_iIndex].timingError
                                               , out additionalTemper);
            }

            if (m_fAdditionalScore > 0)//-得分
            {
                //-记录当前的索引，防止将相同的标记判定为两次
                m_previousHitIndex = nearestIndex;
#if false
                if (nearestIndex == kSeeker.nextIndex)//-判断当前按下的是前一个还是后一个
                {
                    hitAfter = true;
                }
                else
                {
                    hitBefore = true;
                }
#endif

                //增加分数的时候播放动画
                OnScoreAdded(_iIndex, nearestIndex);
            }

            m_fScore += m_fAdditionalScore;
            temper += additionalTemper;
            m_onPlayGUI.RythmHitEffect(_iIndex
                                       , m_previousHitIndex
                                       , m_fAdditionalScore
                                       , m_fScore);

#if false
            //-记录日志
            DebugWriteLogPrev();
            DebugWriteLogPost(hitBefore, hitAfter);
#endif
        }

        if (kSeeker.nextIndex > 0)//-如果还没结束，计算得分率，用总分数，除以假设全部是最佳的得分
        {
            m_scoreRate = m_fScore / (kSeeker.nextIndex * excellentScore);
        }
    }

    /// <summary>
    /// 判定输入的结果（好/不好/错误）
    /// </summary>
    /// <param name="actionInfoIndex">应该计算的谱面索引</param>
    /// <param name="timingError">误差</param>
    /// <param name="heatup">输出的兴奋值</param>
    /// <returns></returns>
    float CheckScore(int actionInfoIndex, float timingError, out float heatup)
    {
		float score = 0;

		timingError = Mathf.Abs(timingError);//-取出误差的绝对值

		do
        {
            if (timingError > timingErrorMiss)//-如果超过了Miss的判定范围就不计算
            {
                Debug.Log("Empty1");
                score = 0.0f;
                heatup = 0.0f;
                break;
            }

			if (timingError >= timingErrorToleranceGood)//-如果低于了Good范围，增加0分，兴奋值为0
            {
                score = missScore;
                heatup = missHeatupRate; 
				break;
			}
			
			if(timingError >= timingErrorTorelanceExcellent)//-“Good和Excellent”之间的时候认为是Good，增加good的分数和兴奋值
            {
				score = goodScore;
				heatup = goodHeatupRate;
				break;
			}

			//Excellent
			score  = excellentScore;
			heatup = bestHeatupRate;

		} while(false);

		return score;
	}

    /// <summary>
    /// 增加分数的时候播放动画
    /// </summary>
    /// <param name="nearestIndex">最近的谱面</param>
	private void OnScoreAdded(int _iIndex, int nearestIndex)
    {
        SongInfo song = m_musicManager.currentSongInfo;

        if (nearestIndex >= song.onBeatActionSequence[_iIndex].Count)
        {
            Debug.Log("Out of range : " + nearestIndex
                           + " total : " + song.onBeatActionSequence[_iIndex].Count);
            return;
        }

		if (song.onBeatActionSequence[_iIndex][nearestIndex].playerActionType == PlayerActionEnum.Jump
			&& temper > temperThreshold)//-当行为是跳跃的时候，并且是非常兴奋的时候，并且成功输入的时候
		{
            //-所有乐队成员跳一下
			foreach (GameObject bandMember in m_bandMembers)
			{
				bandMember.GetComponent<BandMember>().Jump();
			}

            //-所有观众跳一下
            foreach (GameObject audience in m_audiences)
			{
				audience.GetComponent<Audience>().Jump();
			}

            //-所有的音符效果,激发一下
            foreach (GameObject noteParticle in m_noteParticles)
			{
				noteParticle.GetComponent<ParticleSystem>().Emit(20);
			}
		}
		else if (song.onBeatActionSequence[_iIndex][nearestIndex].playerActionType 
            == PlayerActionEnum.HeadBanging)//-如果只是headBanging，那乐队成员动一下
		{
			foreach (GameObject bandMember in m_bandMembers)
			{
				bandMember.GetComponent<SimpleSpriteAnimation>().BeginAnimation(1, 1);
			}
		}
	}

#if false
    /// <summary>
    /// 输出debug信息，这一帧玩家是有的操作的
    /// </summary>
    private void DebugWriteLogPrev()
    {
#if UNITY_EDITOR
        if (m_scoringUnitSeeker.isJustPassElement)//-如果当前过了一个拍子
        {
            if (outScoringLog)
            {
                OnBeatActionInfo onBeatActionInfo
                    = m_musicManager.currentSongInfo.onBeatActionSequence[0][m_scoringUnitSeeker.nextIndex - 1];
                m_logWriter.WriteLine(
                    onBeatActionInfo.triggerBeatTiming.ToString() + ","
                    + "IdealAction,,"
                    + onBeatActionInfo.playerActionType.ToString()
                );
                m_logWriter.Flush();
            }
        }
#endif
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hitBefore"></param>
    /// <param name="hitAfter"></param>
    private void DebugWriteLogPost(bool hitBefore, bool hitAfter)
	{
#if UNITY_EDITOR
		if (outScoringLog)
        {
			string relation = "";

			if (hitBefore)
            {
				relation = "HIT ABOVE";
			}
			if (hitAfter)
            {
				relation = "HIT BELOW";
			}

			string scoreTypeString = "MISS";
            if (m_additionalScore >= excellentScore)
            {
                scoreTypeString = "BEST";
            }
            else if (m_additionalScore >= goodScore)
            {
                scoreTypeString = "GOOD";
            }
			m_logWriter.WriteLine(
				m_playerAction.lastActionInfo.triggerBeatTiming.ToString() + ","
				+ " PlayerAction,"
				+ relation + " " + scoreTypeString + ","
				+ m_playerAction.lastActionInfo.playerActionType.ToString() + ","
				+ "Score=" + m_additionalScore
			);
			m_logWriter.Flush();
		}
#endif
	}
#endif

    /// <summary>
    /// 当前增加的分数
    /// </summary>
    float m_fAdditionalScore;

	MusicManager	m_musicManager;
	PlayerAction	m_playerAction;
	OnPlayGUI		m_onPlayGUI;
	int				m_previousHitIndex = -1;

    /// <summary>
    /// 所有成员
    /// </summary>
	GameObject[]	m_bandMembers;

    /// <summary>
    /// 所有的观众 
    /// </summary>
	GameObject[]    m_audiences;

    /// <summary>
    /// 所有的音符效果
    /// </summary>
    GameObject[]    m_noteParticles;

    /// <summary>
    /// 日志记录
    /// </summary>
    TextWriter		m_logWriter;

	PhaseManager m_phaseManager;

    /// <summary>
    /// 玩家输入的结果
    /// </summary>
    public struct Result
    {
		public float	timingError;        // 时机的偏差快的加……晚了）
        public int		markerIndex;        // 被比较的标记的索引,就是当前应该按的那个谱面
    };

    /// <summary>
    /// 之前玩家输入的结果
    /// </summary>
    public Result[] m_lastResults = new Result[4]
    {
        new Result(),
        new Result(),
        new Result(),
        new Result(),
    };
}

