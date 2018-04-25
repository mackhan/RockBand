using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 音乐播放暂停控制管理器
/// </summary>
public class MusicManager : MonoBehaviour
{    
    /// <summary>
    /// 当前帧播放到第几拍了
    /// </summary>
    float m_beatCountFromStart = 0;
    public float beatCountFromStart
    {
		get { return m_beatCountFromStart; }
	}

    /// <summary>
    /// 前一帧播放到第几拍了
    /// </summary>
    float m_previousBeatCountFromStart = 0;
    public float previousBeatCountFromStart
    {
		get { return m_previousBeatCountFromStart; }
	}

	/// <summary>
    /// 曲子的长度，以拍为单位，一共多少拍
    /// </summary>
	public float length
    {
		get { return m_audioSource.clip.length * m_currentSongInfo.beatPerSecond; }
	}

    /// <summary>
    /// 当前歌曲的信息
    /// </summary>
    private SongInfo m_currentSongInfo;
    public SongInfo currentSongInfo
    {
		set { m_currentSongInfo = value; }
		get { return m_currentSongInfo; }
	}

    /// <summary>
    /// 音乐播放
    /// </summary>
    AudioSource m_audioSource;

    /// <summary>
    /// 上一帧是否在播放
    /// </summary>
    bool m_isPlayPreviousFrame = false;

    /// <summary>
    /// 音乐是否播放完毕
    /// </summary>
    bool m_musicFinished = false;
    public bool IsFinished()
    {
        return m_musicFinished;
    }

    void Awake()
	{
		Application.targetFrameRate = 60;//-限帧60
	}
	
	void Start()
    {
		m_audioSource = gameObject.GetComponent<AudioSource>();
		m_musicFinished = false;
	}

	void Update ()
    {
        //-播放时始终检查歌曲的播放位置
        if (m_audioSource.isPlaying)
		{
			m_previousBeatCountFromStart = m_beatCountFromStart;
			m_beatCountFromStart = m_audioSource.time * m_currentSongInfo.beatPerSecond;//-计算当前播放到第几拍
			m_isPlayPreviousFrame = true;
		}
		else
		{
            //-如果上一帧是在播放，且音乐是采样是刚结束，延迟一帧设置播放完成？？？
			if (m_isPlayPreviousFrame
				&& !(0 < m_audioSource.timeSamples && m_audioSource.timeSamples < m_audioSource.clip.samples))
			{
				m_musicFinished = true;
			}
			m_isPlayPreviousFrame = false;
		}
	}

	/// <summary>
    /// 根据拍子调整音乐到应该的位置
    /// </summary>
    /// <param name="beatCount"></param>
	public void Seek(float beatCount)
    {
		m_audioSource.time =  beatCount / m_currentSongInfo.beatPerSecond;
		m_beatCountFromStart = m_previousBeatCountFromStart = beatCount;
	}

    /// <summary>
    /// 从头开始播放音乐
    /// </summary>
    public void PlayMusicFromStart()
    {
		m_musicFinished = false;
		m_isPlayPreviousFrame = false;
		m_beatCountFromStart = 0;
		m_previousBeatCountFromStart = 0;
		m_audioSource.Play();
	}

    /// <summary>
    /// 是否在播放音乐
    /// </summary>
    /// <returns></returns>
	public bool IsPlaying()
    {
		return m_audioSource.isPlaying;
	}
}
