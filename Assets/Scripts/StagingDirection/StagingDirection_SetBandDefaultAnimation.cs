using UnityEngine;
using System.Collections;

/// <summary>
/// ���ö���
/// </summary>
public class StagingDirection_SetBandMemberDefaultAnimation : StagingDirection
{
	public string m_memberName = "";

    /// <summary>
    /// ����һ֡��ʼ
    /// </summary>
	public int m_animationFromIndex = 0;

    /// <summary>
    /// ����һ֡����
    /// </summary>
	public int m_animationToIndex = 0;

	public StagingDirection_SetBandMemberDefaultAnimation() { }

    /// <summary>
    /// ��ʼ������Ĭ�ϵĶ���
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
    /// ���ַ��������ж�ȡ��Ϣ
    /// </summary>
    /// <param name="parameters"></param>
    public override void ReadCustomParameterFromString(string[] parameters)
	{
		m_memberName = parameters[4];
		m_animationFromIndex = int.Parse(parameters[2]);
		m_animationToIndex = int.Parse(parameters[3]);
	}
};