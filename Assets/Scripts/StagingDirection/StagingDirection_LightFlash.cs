using UnityEngine;
#if UNITY_EDITOR 
using UnityEditor;
#endif
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
[System.Serializable]
public class StagingDirection_LightFlash : StagingDirection
{
    /// <summary>
    /// 当前控制的灯光
    /// </summary>
	Light m_light = null;

    /// <summary>
    /// 当前控制的灯光ID
    /// </summary>
	int m_lightID = 0;

    /// <summary>
    /// 初始的灯光强度
    /// </summary>
	float m_lightIntensityAtBeginning;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="lightID"></param>
	public StagingDirection_LightFlash(int lightID)
    {
		m_lightID=lightID;
	}

	public override void OnBegin()
    {
		m_light = GameObject.Find("Light" + m_lightID.ToString()).GetComponent<Light>();
		m_lightIntensityAtBeginning = m_light.intensity;
		m_light.intensity *= 3.0f;
	}

    /// <summary>
    /// 恢复原始的灯光强度
    /// </summary>
	public override void OnEnd()
    {
		m_light.intensity = m_lightIntensityAtBeginning;
	}

    /// <summary>
    /// 灯光逐渐变暗
    /// </summary>
	public override void Update()
    {
		m_light.intensity *= 0.80f;
	}

    /// <summary>
    /// 灯光已经暗的小于0.3f了
    /// </summary>
    /// <returns></returns>
	public override bool IsFinished()
    {
		return Mathf.Abs(m_light.intensity - m_lightIntensityAtBeginning) < 0.3f;
	}

    public override StagingDirectionEnum GetEnum()
    {
		return StagingDirectionEnum.LightFlash;
	}

	public override void ReadCustomParameterFromString(string[] parameters)
    {
		m_lightID = int.Parse(parameters[2]);
	}
};