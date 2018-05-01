using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

/// <summary>
/// ���ڵĻ���������Ҫ������
/// </summary>
public class StagingDirection_Applause : StagingDirection
{
    /// <summary>
    /// ���Ź��ڵĻ�����Ч
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