using UnityEngine;
using System.Collections;

/// <summary>
/// 图片简单动画控制器，可以跟随音乐进行Wave，也可以执行一个跳跃动作
/// </summary>
public class SimpleActionMotor : MonoBehaviour
{
    /// <summary>
    /// 是否随着音乐摇摆
    /// </summary>
	public bool isWaveBegin = false;

    /// <summary>
    /// 摇摆的偏移，防止大家一起动
    /// </summary>
	public float wavePhaseOffset = 0;

    /// <summary>
    /// 跳动的初始高度
    /// </summary>
	public float jumpInitialVelocity = 1.0f;

    /// <summary>
    /// 这一帧是否跳跃，感觉没用
    /// </summary>
    bool m_isJustJump = false;
    public bool isJustJump
    {
		get { return m_isJustJump; }
	}

    /// <summary>
    /// 基础位置，会随着音乐Wave
    /// </summary>
    Vector3 basePosition = new Vector3();

    /// <summary>
    /// 跳跃的力
    /// </summary>
	Vector3 velocity = new Vector3();

    /// <summary>
    /// 重力
    /// </summary>
	float gravity = 0.2f;

    /// <summary>
    /// 是否跳跃
    /// </summary>
	bool m_isJumpTriggered = false;

    /// <summary>
    /// 音乐管理器
    /// </summary>
	MusicManager m_musicManager;

    /// <summary>
    /// 跳起来的偏移
    /// </summary>
    Vector3 positionOffset = new Vector3();

    /// <summary>
    /// 跳动一下
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
        //-设置标记，判断这帧是否跳动，感觉没用
		m_isJustJump = false;
		if (m_isJumpTriggered)
        {
			m_isJustJump = true;
			m_isJumpTriggered = false;
		}

        //-计算跳的力和重力
		positionOffset += velocity;
        if (positionOffset.y < 0)//-向上的力不断被重力减少，然后变成向下的，知道偏移为0
        {
            velocity.y = 0;
        }
        else
        {
            velocity.y -= gravity;
        }

		if (isWaveBegin)
        {
			if (m_musicManager.IsPlaying())//-如果正在播放，依据当前播放到第几拍了来抖动
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
