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
    /// ��ǰ���Ƶĵƹ�
    /// </summary>
	Light m_light = null;

    /// <summary>
    /// ��ǰ���Ƶĵƹ�ID
    /// </summary>
	int m_lightID = 0;

    /// <summary>
    /// ��ʼ�ĵƹ�ǿ��
    /// </summary>
	float m_lightIntensityAtBeginning;

    /// <summary>
    /// ��ʼ��
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
    /// �ָ�ԭʼ�ĵƹ�ǿ��
    /// </summary>
	public override void OnEnd()
    {
		m_light.intensity = m_lightIntensityAtBeginning;
	}

    /// <summary>
    /// �ƹ��𽥱䰵
    /// </summary>
	public override void Update()
    {
		m_light.intensity *= 0.80f;
	}

    /// <summary>
    /// �ƹ��Ѿ�����С��0.3f��
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