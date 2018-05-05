using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

/// <summary>
/// ��������
/// </summary>
public class SongInfo
{
    /// <summary>
    /// ����ÿ���ؼ��ĵĴ���¼�
    /// </summary>
	//public List<OnBeatActionInfo> onBeatActionSequence = new List<OnBeatActionInfo>();
    public List<OnBeatActionInfo>[] onBeatActionSequence = new List<OnBeatActionInfo>[4] 
    {
        new List<OnBeatActionInfo>()
        , new List<OnBeatActionInfo>()
        , new List<OnBeatActionInfo>()
        , new List<OnBeatActionInfo>() };

    /// <summary>
    /// ��̨�¼������ж���
    /// </summary>
	public List<StagingDirection> stagingDirectionSequence = new List<StagingDirection>();

    /// <summary>
    /// ���������ʱ�䣬ֻ�е���ģʽ���ã�����
    /// </summary>
    public List<SequenceRegion> onBeatActionRegionSequence = new List<SequenceRegion>();

    /// <summary>
    /// ÿ������= BPM / 60
    /// </summary>
	public float beatPerSecond = 120.0f / 60.0f;

    /// <summary>
    /// ÿ��С�ڵĽ���������û��
    /// </summary>
	public float beatPerBar = 4.0f;
}

