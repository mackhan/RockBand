using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

/// <summary>
/// 舞台效果基类
/// </summary>
public enum StagingDirectionEnum
{
	None,
	FireBlast,//-播放一下烟火
    LightFlash,//-灯光闪一下
    LightShuffle,//-两盏灯光变换位置
    LightFade,//-灯光的渐变
	SetBandMemberAction,//-设置所有乐队成员的动作
	SetBandMemberDefaultAnimation,//-设置所有乐队成员的默认动画
    Applause//-播放观众的欢呼音效
};

/// <summary>
/// 舞台演出的基类
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
    /// 舞台事件的工程
    /// </summary>
    /// <param name="stagingDirectionEnum"></param>
    /// <returns></returns>
	public static StagingDirection CreateStagingDirectionFromEnum(StagingDirectionEnum stagingDirectionEnum)
    {
		if (stagingDirectionEnum == StagingDirectionEnum.FireBlast)//-播放一下烟火
        {
			return new StagingDirection_FireBlast(0, 1);
		}
		else if (stagingDirectionEnum == StagingDirectionEnum.LightFlash)//-灯光闪一下
        {
			return new StagingDirection_LightFlash(0);
		}
		else if (stagingDirectionEnum == StagingDirectionEnum.LightShuffle)//-两盏灯光变换位置
        {
			return new StagingDirection_LightShuffle(0, 1);
		}
		else if (stagingDirectionEnum == StagingDirectionEnum.LightFade)//-灯光的渐变
        {
			return new StagingDirection_LightFade(0, 1.0f);
		}
		else if (stagingDirectionEnum == StagingDirectionEnum.SetBandMemberAction)//-设置所有乐队成员的动作，这里应该走不到
        {
            Debug.Assert(false);
			return new StagingDirection_SetBandMemberAction();
		}
		else if (stagingDirectionEnum == StagingDirectionEnum.SetBandMemberDefaultAnimation)//-设置所有乐队成员的默认动画，这里应该走不到
        {
            Debug.Assert(false);
            return new StagingDirection_SetBandMemberDefaultAnimation();
		}
		else if (stagingDirectionEnum == StagingDirectionEnum.Applause)//-播放观众的欢呼音效
		{
			return new StagingDirection_Applause();
		}

		return null;
	}
}