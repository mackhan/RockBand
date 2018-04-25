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
    /// 我们什么时候能开始处理？
    /// </summary>
	public float triggerBeatTiming = 0;

    /// <summary>
    /// 参数值的字符串数组（用于读取CSV等）
    /// </summary>
    /// <param name="parameters"></param>
	public virtual void ReadCustomParameterFromString(string[] parameters){}
	
    //triggerBeatTimingの原点を指定した上でクローンを生成
	public virtual MusicalElement GetClone(){
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
	public PlayerActionEnum playerActionType;//アクションの種類
	public string GetCustomParameterAsString_CSV(){
		return "SingleShot," + triggerBeatTiming.ToString() + "," + playerActionType.ToString();
	}

	public int	line_number;		// もとのテキスト中の行番号.
}
