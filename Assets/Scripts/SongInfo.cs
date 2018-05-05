using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

/// <summary>
/// 音乐数据
/// </summary>
public class SongInfo
{
    /// <summary>
    /// 谱面每个关键拍的打击事件
    /// </summary>
	//public List<OnBeatActionInfo> onBeatActionSequence = new List<OnBeatActionInfo>();
    public List<OnBeatActionInfo>[] onBeatActionSequence = new List<OnBeatActionInfo>[4] 
    {
        new List<OnBeatActionInfo>()
        , new List<OnBeatActionInfo>()
        , new List<OnBeatActionInfo>()
        , new List<OnBeatActionInfo>() };

    /// <summary>
    /// 舞台事件的序列队列
    /// </summary>
	public List<StagingDirection> stagingDirectionSequence = new List<StagingDirection>();

    /// <summary>
    /// 谱面区域的时间，只有调试模式有用？？？
    /// </summary>
    public List<SequenceRegion> onBeatActionRegionSequence = new List<SequenceRegion>();

    /// <summary>
    /// 每秒拍数= BPM / 60
    /// </summary>
	public float beatPerSecond = 120.0f / 60.0f;

    /// <summary>
    /// 每个小节的节拍数量，没用
    /// </summary>
	public float beatPerBar = 4.0f;
}

