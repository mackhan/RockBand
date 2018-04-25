using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;


/// <summary>
/// 舞台演出和节拍标记的基类
/// </summary>
public abstract class MusicalElement
{
    /// <summary>
    /// 当前帧播放到第几拍了
    /// </summary>
	public float triggerBeatTiming = 0;

    /// <summary>
    /// 参数值的字符串数组（用于读取CSV等）
    /// </summary>
    /// <param name="parameters"></param>
	public virtual void ReadCustomParameterFromString(string[] parameters){}

    /// <summary>
    /// 指定triggerbatTiming的原点后生成克隆
    /// </summary>
    /// <returns></returns>
    public virtual MusicalElement GetClone()
    {
		MusicalElement clone = this.MemberwiseClone() as MusicalElement;
		return clone;
	}

    public System.Xml.Schema.XmlSchema GetSchema(){return null;}
};

/// <summary>
/// 保存信息的类（旋律，铁锈等）信息
/// </summary>
public class SequenceRegion: MusicalElement
{
	public float totalBeatCount;
	public string name;
	public float repeatPosition;
};

/// <summary>
/// 玩家可以配合音乐进行的动作信息
/// </summary>
public class OnBeatActionInfo : MusicalElement
{
    /// <summary>
    /// 动作类型
    /// </summary>
	public PlayerActionEnum playerActionType;

	public string GetCustomParameterAsString_CSV()
    {
		return "SingleShot," + triggerBeatTiming.ToString() + "," + playerActionType.ToString();
	}

    /// <summary>
    /// 原来文本中的行号
    /// </summary>
	public int	line_number;		
}
