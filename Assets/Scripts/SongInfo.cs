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
	public List<OnBeatActionInfo> onBeatActionSequence = new List<OnBeatActionInfo>();

	public List<StagingDirection> stagingDirectionSequence = new List<StagingDirection>();

    public List<SequenceRegion> onBeatActionRegionSequence = new List<SequenceRegion>();

    /// <summary>
    /// 每秒拍数= BPM / 60
    /// </summary>
	public float beatPerSecond = 120.0f/60.0f;

    /// <summary>
    /// 每个小节的节拍数量
    /// </summary>
	public float beatPerBar = 4.0f;
}

