using UnityEngine;
using System.Collections;

/// <summary>
/// ���ֲ��Ŷ���
/// </summary>
public class StagingDirection_SetBandMemberAction : StagingDirection
{
    /// <summary>
    /// ��Ա����
    /// </summary>
	public string m_memberName = "";

    /// <summary>
    /// ��������
    /// </summary>
	public string m_actionName = "";

    public StagingDirection_SetBandMemberAction(){}

    public override void OnBegin()
    {
		switch (m_actionName)
        {
		case "jump":
			GameObject.Find(m_memberName).GetComponent<BandMember>().Jump();
			break;
		case "actionA":
			GameObject.Find(m_memberName).GetComponent<SimpleSpriteAnimation>().BeginAnimation(1, 1);
			break;
		case "actionB":
			GameObject.Find(m_memberName).GetComponent<SimpleSpriteAnimation>().BeginAnimation(4, 4);
			break;
		}
	}
	public override StagingDirectionEnum GetEnum()
	{
		return StagingDirectionEnum.SetBandMemberAction;
	}

    /// <summary>
    /// ���ַ����г�ʼ��
    /// </summary>
    /// <param name="parameters"></param>
	public override void ReadCustomParameterFromString(string[] parameters)
    {
		m_memberName = parameters[3];
		m_actionName = parameters[2];
	}
};