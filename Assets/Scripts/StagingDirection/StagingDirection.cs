using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

/// <summary>
/// ��̨Ч������
/// </summary>
public enum StagingDirectionEnum
{
	None,
	FireBlast,//-����һ���̻�
    LightFlash,//-�ƹ���һ��
    LightShuffle,//-��յ�ƹ�任λ��
    LightFade,//-�ƹ�Ľ���
	SetBandMemberAction,//-���������ֶӳ�Ա�Ķ���
	SetBandMemberDefaultAnimation,//-���������ֶӳ�Ա��Ĭ�϶���
    Applause//-���Ź��ڵĻ�����Ч
};

/// <summary>
/// ��̨�ݳ��Ļ���
/// </summary>
public abstract class StagingDirection : MusicalElement
{
	public StagingDirection(){}

	public virtual void OnBegin(){}

	public virtual void OnEnd(){}

	public virtual void Update(){}

	public virtual bool IsFinished() { return true; }

	public virtual StagingDirectionEnum GetEnum() { return StagingDirectionEnum.None; }
};

public class StagingDirectionFactory
{
    /// <summary>
    /// ��̨�¼��Ĺ���
    /// </summary>
    /// <param name="stagingDirectionEnum"></param>
    /// <returns></returns>
	public static StagingDirection CreateStagingDirectionFromEnum(StagingDirectionEnum stagingDirectionEnum)
    {
		if (stagingDirectionEnum == StagingDirectionEnum.FireBlast)//-����һ���̻�
        {
			return new StagingDirection_FireBlast(0, 1);
		}
		else if (stagingDirectionEnum == StagingDirectionEnum.LightFlash)//-�ƹ���һ��
        {
			return new StagingDirection_LightFlash(0);
		}
		else if (stagingDirectionEnum == StagingDirectionEnum.LightShuffle)//-��յ�ƹ�任λ��
        {
			return new StagingDirection_LightShuffle(0, 1);
		}
		else if (stagingDirectionEnum == StagingDirectionEnum.LightFade)//-�ƹ�Ľ���
        {
			return new StagingDirection_LightFade(0, 1.0f);
		}
		else if (stagingDirectionEnum == StagingDirectionEnum.SetBandMemberAction)//-���������ֶӳ�Ա�Ķ���������Ӧ���߲���
        {
            Debug.Assert(false);
			return new StagingDirection_SetBandMemberAction();
		}
		else if (stagingDirectionEnum == StagingDirectionEnum.SetBandMemberDefaultAnimation)//-���������ֶӳ�Ա��Ĭ�϶���������Ӧ���߲���
        {
            Debug.Assert(false);
            return new StagingDirection_SetBandMemberDefaultAnimation();
		}
		else if (stagingDirectionEnum == StagingDirectionEnum.Applause)//-���Ź��ڵĻ�����Ч
		{
			return new StagingDirection_Applause();
		}

		return null;
	}
}