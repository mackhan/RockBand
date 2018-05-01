using UnityEngine;
#if UNITY_EDITOR 
using UnityEditor;
#endif
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

/// <summary>
/// �ƹ�Ľ���
/// </summary>
[System.Serializable]
public class StagingDirection_LightFade : StagingDirection
{
    /// <summary>
    /// ���Ƶĵƹ�
    /// </summary>
	Light m_light = null;

    /// <summary>
    /// �ƹ������
    /// </summary>
    int m_lightID = 0;

    /// <summary>
    /// �ƹ��ǿ��
    /// </summary>
    float m_intensityFadeTo = 1;

    /// <summary>
    /// ��ʼ��
    /// </summary>
    /// <param name="lightID">Ĭ��0</param>
    /// <param name="intensityFadeTo">Ĭ��1.0f</param>
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
    /// ���µƹ��ǿ�ȣ�Ŀ��ֵ�͵�ǰֵ��ֵ��0.4
    /// </summary>
	public override void Update()
    {
		m_light.intensity += (m_intensityFadeTo - m_light.intensity) * 0.4f;
	}

    /// <summary>
    /// ���㹻�ӽ���ʱ�򣬶�������
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