using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

/// <summary>
/// 观众的欢呼，不需要参数！
/// </summary>
public class StagingDirection_Applause : StagingDirection
{
    /// <summary>
    /// 播放观众的欢呼音效
    /// </summary>
	public override void OnBegin()
	{	
		ScoringManager scoringManager = GameObject.Find("ScoringManager").GetComponent<ScoringManager>();
		if (scoringManager.temper > ScoringManager.temperThreshold)
        {
            GameObject.Find("AudienceVoice").GetComponent<AudioSource>().Play();
        }
	}

	public override bool IsFinished()
	{
		return true;
	}

	public override StagingDirectionEnum GetEnum()
	{
		return StagingDirectionEnum.Applause;
	}
};