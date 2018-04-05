using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

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
    /// 当前时间
    /// </summary>
    float m_currentBeatCount = 0;           

    /// <summary>
    /// 是否当前索引已经过了进入下一索引了
    /// </summary>
    bool m_isJustPassElement = false;

    /// <summary>
    /// 要查找的序列数据。
    /// </summary>
    List<ElementType> m_sequence;			

    /// <summary>
    /// 走査するシーケンスデータをセット
    /// </summary>
    /// <param name="sequence"></param>
	public void SetSequence( List<ElementType> sequence )
    {
		m_sequence = sequence;
		m_nextIndex=0;
		m_currentBeatCount=0;
		m_isJustPassElement=false;
	}
	
    //一番近い次の要素を示すインデックス番号
	public int nextIndex
    {
		get
        {
            return m_nextIndex;
        }
	}
	
    //要素のトリガー位置を通過した時にtrue
	public bool isJustPassElement
    {
			get{return m_isJustPassElement;}
	}

    /// <summary>
    /// 每帧处理
    /// </summary>
    /// <param name="deltaBeatCount"></param>
    public void ProceedTime(float deltaBeatCount)
    {
        //-计算当前时间
        m_currentBeatCount += deltaBeatCount;

        //设置标识“检索位置前进完成”瞬间的标记为false
        m_isJustPassElement = false;

        //-取得当前时刻之后的那个标记的索引
		int	index = find_next_element(m_nextIndex);

		//-之后的标记和当前的下个标记位置不相等
		if (index != m_nextIndex)
        {
			//更新检索位置
			m_nextIndex = index;

			//把更新的标记设置为true
			m_isJustPassElement=true;
		}
	}

	/// <summary>
    /// 直接找到一个时间之后的索引
    /// </summary>
    /// <param name="beatCount"></param>
	public void Seek(float beatCount)
    {
		m_currentBeatCount = beatCount;

		int	index = find_next_element(0);

		m_nextIndex = index;
	}

    /// <summary>
    /// 查找到当前时刻m_currentBeatCount，之后的那个索引
    /// </summary>
    /// <param name="start_index">从哪里开始检查，优化</param>
    /// <returns></returns>
	private int	find_next_element(int start_index)
	{
		//-通过表示超过了最后标记的时刻的值进行初始化
		int ret = m_sequence.Count;

		for (int i = start_index;i < m_sequence.Count; i++)
		{
			// m_currentBeatCount よりも後ろにあるマーカーだった＝見つかった.
			if(m_sequence[i].triggerBeatTiming > m_currentBeatCount)
			{
				ret = i;
				break;
			}
		}

		return (ret);
	}
}

