using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

public class SequenceSeekers<ElementType> where ElementType : MusicalElement
{
    /// <summary>
    /// 时间先进的查找单位（显示结束位置）。
    /// </summary>
    SequenceSeeker<ElementType>[] m_kSeekers = new SequenceSeeker<ElementType>[4] 
    {
        new SequenceSeeker<ElementType>()
        , new SequenceSeeker<ElementType>()
        , new SequenceSeeker<ElementType>()
        , new SequenceSeeker<ElementType>()
    };

    public SequenceSeeker<ElementType> GetSeeker(int _iIndex)
    {
        return m_kSeekers[_iIndex];
    }

    /// <summary>
    /// 设置序列数据
    /// </summary>
    /// <param name="sequence"></param>
    public void SetSequence(List<ElementType>[] _kSequence)
    {
        int i = 0;
        foreach (SequenceSeeker<ElementType> kSeeker in m_kSeekers)
        {
            kSeeker.SetSequence(_kSequence[i]);
            i++;
        }
    }

    /// <summary>
    /// 直接找到一个时间之后的索引
    /// </summary>
    /// <param name="beatCount"></param>
    public void Seek(float beatCount)
    {
        foreach (SequenceSeeker<ElementType> kSeeker in m_kSeekers)
        {
            kSeeker.Seek(beatCount);
        }
    }

    /// <summary>
    /// 每帧处理
    /// </summary>
    /// <param name="deltaBeatCount">这帧走了几个拍子，一般是零点几个拍子</param>
    public void ProceedTime(float deltaBeatCount)
    {
        foreach (SequenceSeeker<ElementType> kSeeker in m_kSeekers)
        {
            kSeeker.ProceedTime(deltaBeatCount);
        }
    }
}

/// <summary>
/// 按时间对序列数据定位
/// </summary>
/// <typeparam name="ElementType"></typeparam>
public class SequenceSeeker<ElementType> where ElementType: MusicalElement
{
    /// <summary>
    /// 下一个索引（=从当前时间开始看下一个元素的索引）。
    /// </summary>
    int m_nextIndex = 0;                

    /// <summary>
    /// 当前到那个拍子
    /// </summary>
    float m_currentBeatCount = 0;           

    /// <summary>
    /// 是否当前索引已经过了进入下一索引了
    /// </summary>
    bool m_isJustPassElement = false;

    /// <summary>
    /// 要查找的序列数据。这个序列是唯一的，相同的拍子的ID也不一样
    /// </summary>
    List<ElementType> m_sequence;

    /// <summary>
    /// 设置序列数据
    /// </summary>
    /// <param name="sequence"></param>
    public void SetSequence(List<ElementType> sequence)
    {
		m_sequence = sequence;
		m_nextIndex = 0;
		m_currentBeatCount = 0;
		m_isJustPassElement=false;
	}

    /// <summary>
    /// 指示最近元素的索引号
    /// </summary>
	public int nextIndex
    {
		get
        {
            return m_nextIndex;
        }
	}

    /// <summary>
    /// 如果它传递了元素的触发位置，则为真
    /// </summary>
    public bool isJustPassElement
    {
		get{ return m_isJustPassElement; }
	}

    /// <summary>
    /// 每帧处理
    /// </summary>
    /// <param name="deltaBeatCount">这帧走了几个拍子，一般是零点几个拍子</param>
    public void ProceedTime(float deltaBeatCount)
    {
        //-计算当前到那个拍子
        m_currentBeatCount += deltaBeatCount;

        //设置标识“检索位置前进完成”瞬间的标记为false
        m_isJustPassElement = false;

        //-取得当前时刻之后的那个标记的索引
		int	index = FindNextElement(m_nextIndex);

		//-之后的标记和当前的下个标记位置不相等
		if (index != m_nextIndex)
        {
			//更新检索位置
			m_nextIndex = index;

			//把更新的标记设置为true
			m_isJustPassElement = true;
		}
	}

	/// <summary>
    /// 直接找到一个时间之后的索引
    /// </summary>
    /// <param name="beatCount"></param>
	public void Seek(float beatCount)
    {
		m_currentBeatCount = beatCount;
        m_nextIndex = FindNextElement(0);
	}

    /// <summary>
    /// 查找到当前拍子m_currentBeatCount，之后的那个索引
    /// </summary>
    /// <param name="start_index">从哪里开始检查，优化</param>
    /// <returns></returns>
	private int	FindNextElement(int start_index)
	{
		//-找到最近的一个事件
		int ret = m_sequence.Count;

		for (int i = start_index; i < m_sequence.Count; i++)
		{
            //-这是m_currentBeatCount = found后面的标记。
			if (m_sequence[i].triggerBeatTiming > m_currentBeatCount)
			{
				ret = i;
				break;
			}
		}

		return (ret);
	}
}

