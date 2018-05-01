using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 舞台演出的管理和执行
/// </summary>
public class EventManager : MonoBehaviour
{
	void Start()
    {
		m_musicManager = GameObject.Find("MusicManager").GetComponent<MusicManager>();
	}

    /// <summary>
    /// 开始播放Sequence
    /// </summary>
    public void BeginEventSequence()
    {
		m_seekUnit.SetSequence(m_musicManager.currentSongInfo.stagingDirectionSequence);
	}

    /// <summary>
    /// 直接跳转
    /// </summary>
    /// <param name="beatCount"></param>
    public void Seek(float beatCount)
    {
		m_seekUnit.Seek(beatCount);
		m_previousIndex = m_seekUnit.nextIndex;

        //-结束时清除事件列表
        for ( LinkedListNode<StagingDirection> it = m_activeEvents.First; it != null; it = it.Next)
        {
			it.Value.OnEnd();
			m_activeEvents.Remove(it);
		}
	}

    void Update ()
    {
		SongInfo song = m_musicManager.currentSongInfo;

		if (m_musicManager.IsPlaying())
		{
            //前帧到现帧之间获得成功的舞台演出
            m_previousIndex = m_seekUnit.nextIndex;

			m_seekUnit.ProceedTime(m_musicManager.DeltaBeatCountFromStart);

            // 开始在“之前的移动位置”和“更新后的移动位置”之间的活动。
            for (int i = m_previousIndex; i < m_seekUnit.nextIndex; i++)
            {
                //复制事件数据
                StagingDirection clone = song.stagingDirectionSequence[i].GetClone() as StagingDirection;
				clone.OnBegin();

                //添加到“执行中的活动列表”
                m_activeEvents.AddLast(clone);
			}
		}

        //“执行中的活动列表”的执行，LinkedList<T>类在.NET framework中是一个双向链表
        for (LinkedListNode<StagingDirection> it = m_activeEvents.First; it != null; it = it.Next)
        {
			StagingDirection activeEvent = it.Value;
			activeEvent.Update();

            //执行结束了吗？.
            if (activeEvent.IsFinished())
            {
				activeEvent.OnEnd();

                //从“执行中的事件列表”中删除
                m_activeEvents.Remove(it);
			}
		}
	}

	MusicManager m_musicManager;

	/// <summary>
    /// 序列管理器
    /// </summary>
	SequenceSeeker<StagingDirection> m_seekUnit	= new SequenceSeeker<StagingDirection>();

    /// <summary>
    /// “执行中的事件列表”
    /// </summary>
    LinkedList<StagingDirection> m_activeEvents	= new LinkedList<StagingDirection>();

    /// <summary>
    /// 前一帧的序列索引
    /// </summary>
	int	m_previousIndex = 0;	
}

