using UnityEngine;
using System.Collections;

/// <summary>
/// //-两盏灯光变换位置
/// </summary>
[System.Serializable]
public class StagingDirection_LightShuffle : StagingDirection
{
    /// <summary>
    /// 控制的两盏灯光
    /// </summary>
	Light[] m_light = { null, null };

    /// <summary>
    /// 控制两盏灯光的ID
    /// </summary>
    int[] m_lightID = {0, 0};

    /// <summary>
    /// 灯光移动的速度
    /// </summary>
    float m_shuffleSpeed = 5.0f;

    Vector3[] m_lightPosition = { new Vector3(),new Vector3() };

    Quaternion[] m_lightRotation = { new Quaternion(), new Quaternion() };

    /// <summary>
    /// 初始化，控制两盏灯光
    /// </summary>
    /// <param name="lightIDOne"></param>
    /// <param name="lightIDAnother"></param>
    public StagingDirection_LightShuffle(int lightIDOne, int lightIDAnother)
    {
		m_lightID[0] = lightIDOne;
		m_lightID[1] = lightIDAnother;
	}

	public override void OnBegin()
    {
		m_light[0]=GameObject.Find("Light" + m_lightID[0].ToString()).GetComponent<Light>();
		m_light[1]=GameObject.Find("Light" + m_lightID[1].ToString()).GetComponent<Light>();

		m_lightPosition[0] = m_light[0].transform.position;
		m_lightPosition[1] = m_light[1].transform.position;

		m_lightRotation[0] = m_light[0].transform.rotation;
		m_lightRotation[1] = m_light[1].transform.rotation;
	}
	public override void OnEnd()
    {
		m_light[0].transform.position = m_lightPosition[1];
		m_light[0].transform.rotation = m_lightRotation[1];
		m_light[1].transform.position = m_lightPosition[0];
		m_light[1].transform.rotation = m_lightRotation[0];
	}
	public override void Update()
    {
		m_light[0].transform.position =
			Vector3.MoveTowards(m_light[0].transform.position, m_lightPosition[1], m_shuffleSpeed);
		m_light[1].transform.position =
			Vector3.MoveTowards(m_light[1].transform.position, m_lightPosition[0], m_shuffleSpeed);
	}

    /// <summary>
    /// 第一盏灯光移到第二盏灯光的位置小于0.5f就结束了
    /// </summary>
    /// <returns></returns>
	public override bool IsFinished()
    {
		return Mathf.Abs(m_light[0].transform.position.x - m_lightPosition[1].x) < 0.5f;
	}

	public override StagingDirectionEnum GetEnum()
    {
		return StagingDirectionEnum.LightShuffle;
	}

	public override void ReadCustomParameterFromString(string[] parameters)
    {
		m_lightID[0] = int.Parse(parameters[2]);
		m_lightID[1] = int.Parse(parameters[3]);
		m_shuffleSpeed = float.Parse(parameters[4]);
	}
};