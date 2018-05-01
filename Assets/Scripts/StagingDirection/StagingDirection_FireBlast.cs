using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

/// <summary>
/// 播放一下烟火
/// </summary>
public class StagingDirection_FireBlast : StagingDirection
{
    /// <summary>
    /// 控制的烟火的ID
    /// </summary>
	int m_fireBlasterID = 0;

    /// <summary>
    /// 烟火粒子的个数
    /// </summary>
	int m_fireBlasterEmmitionCount = 10;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="fireBlasterID"></param>
    /// <param name="fireBlasterEmmitionCount"></param>
    public StagingDirection_FireBlast(int fireBlasterID, int fireBlasterEmmitionCount)
    {
		m_fireBlasterID = fireBlasterID;
		m_fireBlasterEmmitionCount = fireBlasterEmmitionCount;
	}

    /// <summary>
    /// 播放烟火的粒子，并发出一些奇怪的声音
    /// </summary>
    public override void OnBegin()
    {
		GameObject.Find("Fire" + m_fireBlasterID.ToString()).GetComponent<ParticleSystem>().Emit(m_fireBlasterEmmitionCount);
		GameObject.Find("Fire" + m_fireBlasterID.ToString()).GetComponent<AudioSource>().Play();
	}

    public override void ReadCustomParameterFromString(string[] parameters)
    {
		m_fireBlasterID = int.Parse(parameters[2]);
		m_fireBlasterEmmitionCount = int.Parse(parameters[3]);
	}

    public override bool IsFinished()
    {
		return true;
	}

    public override StagingDirectionEnum GetEnum()
    {
		return StagingDirectionEnum.FireBlast;
    }
};