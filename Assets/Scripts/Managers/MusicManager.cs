using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 音乐播放暂停控制管理器
/// </summary>
public class MusicManager : MonoBehaviour
{
	private SongInfo m_currentSongInfo;

    //当前帧的歌曲再现位置
    public float beatCountFromStart
    {
		get{ return m_beatCountFromStart;}
	}

	public float beatCount
    {
		get{ return m_beatCountFromStart;}
	}
    
    //前一帧的歌曲再现位置
    public float previousBeatCountFromStart{
		get{ return m_previousBeatCountFromStart;}
	}

	public float previousBeatCount{
		get{ return m_previousBeatCountFromStart;}
	}
	//曲の長さ(拍単位)
	public float length{
		get{ return m_audioSource.clip.length * m_currentSongInfo.beatPerSecond ; }
	}
	//曲情報
	public SongInfo currentSongInfo{
		set{ m_currentSongInfo = value; }
		get{ return m_currentSongInfo; }
	}

	void Awake()
	{
		Application.targetFrameRate = 60;
	}

	
	void Start()
    {
		//Assume gomeObject has AudioSource component
		m_audioSource = gameObject.GetComponent<AudioSource>();
		m_musicFinished = false;
	}

	// Update is called once per frame
	void Update ()
    {
        //播放时始终检查歌曲的播放位置
        if (m_audioSource.isPlaying)
		{
			m_previousBeatCountFromStart = m_beatCountFromStart;
			m_beatCountFromStart = m_audioSource.time * m_currentSongInfo.beatPerSecond;//-当前播放的时间，乘以2，一秒钟两面
			m_isPlayPreviousFrame = true;
		}
		else
		{
			if (m_isPlayPreviousFrame
				&& !(0 < m_audioSource.timeSamples && m_audioSource.timeSamples < m_audioSource.clip.samples)
			)
			{
				m_musicFinished = true;
			}
			m_isPlayPreviousFrame = false;
		}
	}

	//再生位置指定
	public void Seek(float beatCount){
		m_audioSource.time =  beatCount / m_currentSongInfo.beatPerSecond;
		m_beatCountFromStart = m_previousBeatCountFromStart = beatCount;
	}

    public void PlayMusicFromStart()
    {
		m_musicFinished = false;
		m_isPlayPreviousFrame = false;
		m_beatCountFromStart=0;
		m_previousBeatCountFromStart=0;
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

    /// <summary>
    /// 是否播放结束
    /// </summary>
    /// <returns></returns>
	public bool IsFinished()
    {
		return m_musicFinished;
	}
	
	//Variables
	AudioSource m_audioSource;
	float m_beatCountFromStart=0;
	float m_previousBeatCountFromStart=0;
	bool m_isPlayPreviousFrame=false;
	bool m_musicFinished=false;
}
