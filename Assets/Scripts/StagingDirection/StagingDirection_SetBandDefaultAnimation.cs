using UnityEngine;
using System.Collections;

/// <summary>
/// 设置动画
/// </summary>
public class StagingDirection_SetBandMemberDefaultAnimation : StagingDirection
{
	public string m_memberName = "";

    /// <summary>
    /// 从哪一帧开始
    /// </summary>
	public int m_animationFromIndex = 0;

    /// <summary>
    /// 到哪一帧结束
    /// </summary>
	public int m_animationToIndex = 0;

	public StagingDirection_SetBandMemberDefaultAnimation() { }

    /// <summary>
    /// 开始，设置默认的动画
    /// </summary>
    public override void OnBegin()
	{
		GameObject.Find(m_memberName).GetComponent<SimpleSpriteAnimation>().SetDefaultAnimation(m_animationFromIndex
            , m_animationToIndex);
	}

    public override StagingDirectionEnum GetEnum()
	{
		return StagingDirectionEnum.SetBandMemberDefaultAnimation;
	}

    /// <summary>
    /// 从字符串数组中读取信息
    /// </summary>
    /// <param name="parameters"></param>
    public override void ReadCustomParameterFromString(string[] parameters)
	{
		m_memberName = parameters[4];
		m_animationFromIndex = int.Parse(parameters[2]);
		m_animationToIndex = int.Parse(parameters[3]);
	}
};