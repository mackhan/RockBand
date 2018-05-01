using UnityEngine;
using System.Collections;

/// <summary>
/// ͼƬ�򵥶��������������Ը������ֽ���Wave��Ҳ����ִ��һ����Ծ����
/// </summary>
public class SimpleActionMotor : MonoBehaviour
{
    /// <summary>
    /// �Ƿ���������ҡ��
    /// </summary>
	public bool isWaveBegin = false;

    /// <summary>
    /// ҡ�ڵ�ƫ�ƣ���ֹ���һ��
    /// </summary>
	public float wavePhaseOffset = 0;

    /// <summary>
    /// �����ĳ�ʼ�߶�
    /// </summary>
	public float jumpInitialVelocity = 1.0f;

    /// <summary>
    /// ��һ֡�Ƿ���Ծ���о�û��
    /// </summary>
    bool m_isJustJump = false;
    public bool isJustJump
    {
		get { return m_isJustJump; }
	}

    /// <summary>
    /// ����λ�ã�����������Wave
    /// </summary>
    Vector3 basePosition = new Vector3();

    /// <summary>
    /// ��Ծ����
    /// </summary>
	Vector3 velocity = new Vector3();

    /// <summary>
    /// ����
    /// </summary>
	float gravity = 0.2f;

    /// <summary>
    /// �Ƿ���Ծ
    /// </summary>
	bool m_isJumpTriggered = false;

    /// <summary>
    /// ���ֹ�����
    /// </summary>
	MusicManager m_musicManager;

    /// <summary>
    /// ��������ƫ��
    /// </summary>
    Vector3 positionOffset = new Vector3();

    /// <summary>
    /// ����һ��
    /// </summary>
    public void Jump()
	{
        m_isJumpTriggered = true;
		velocity = new Vector3(0, jumpInitialVelocity, 0);
	}
    

    void Start()
	{
		basePosition = transform.position;
		m_musicManager = GameObject.Find("MusicManager").GetComponent<MusicManager>();
	}

	void Update()
	{
        //-���ñ�ǣ��ж���֡�Ƿ��������о�û��
		m_isJustJump = false;
		if (m_isJumpTriggered)
        {
			m_isJustJump = true;
			m_isJumpTriggered = false;
		}

        //-����������������
		positionOffset += velocity;
        if (positionOffset.y < 0)//-���ϵ������ϱ��������٣�Ȼ�������µģ�֪��ƫ��Ϊ0
        {
            velocity.y = 0;
        }
        else
        {
            velocity.y -= gravity;
        }

		if (isWaveBegin)
        {
			if (m_musicManager.IsPlaying())//-������ڲ��ţ����ݵ�ǰ���ŵ��ڼ�����������
            {
				basePosition 
                    = new Vector3(
						basePosition.x
						, basePosition.y + Mathf.Sin((m_musicManager.beatCountFromStart + wavePhaseOffset) * Mathf.PI) * 0.03f
						, basePosition.z);
			}
		}
		transform.position = basePosition + positionOffset;
	}
}
