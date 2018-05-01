using UnityEngine;
#if UNITY_EDITOR 
using UnityEditor;
#endif
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

/// <summary>
/// 灯光的渐变
/// </summary>
[System.Serializable]
public class StagingDirection_LightFade : StagingDirection
{
    /// <summary>
    /// 控制的灯光
    /// </summary>
	Light m_light = null;

    /// <summary>
    /// 灯光的索引
    /// </summary>
    int m_lightID = 0;

    /// <summary>
    /// 灯光的强度
    /// </summary>
    float m_intensityFadeTo = 1;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="lightID">默认0</param>
    /// <param name="intensityFadeTo">默认1.0f</param>
    public StagingDirection_LightFade(int lightID, float intensityFadeTo)
    {
		m_lightID = lightID;
		m_intensityFadeTo = intensityFadeTo;
	}

	public override void OnBegin()
    {
		m_light = GameObject.Find("Light" + m_lightID.ToString()).GetComponent<Light>();
	}

	public override void OnEnd()
    {
		m_light.intensity = m_intensityFadeTo;
	}

    /// <summary>
    /// 更新灯光的强度，目标值和当前值差值的0.4
    /// </summary>
	public override void Update()
    {
		m_light.intensity += (m_intensityFadeTo - m_light.intensity) * 0.4f;
	}

    /// <summary>
    /// 当足够接近的时候，动画结束
    /// </summary>
    /// <returns></returns>
	public override bool IsFinished()
    {
		return Mathf.Abs(m_intensityFadeTo - m_light.intensity) < 0.1f;
	}

	public override StagingDirectionEnum GetEnum()
    {
		return StagingDirectionEnum.LightFade;
	}

	public override void ReadCustomParameterFromString(string[] parameters)
    {
		m_lightID = int.Parse(parameters[2]);
		m_intensityFadeTo = float.Parse(parameters[3]);
	}
};