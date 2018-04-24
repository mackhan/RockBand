using UnityEngine;
using System.Collections;

/// <summary>
/// 乐队成员的动画
/// </summary>
public class BandMember : MonoBehaviour
{
	public bool jumpEnabled = true;
	public AudioClip GoodFeedbackVoice;
	public AudioClip BadFeedbackVoice;

    /// <summary>
    /// 成员跳动
    /// </summary>
    public void Jump()
    {
		if(jumpEnabled)
        {
			gameObject.GetComponent<SimpleActionMotor>().Jump();
			gameObject.GetComponent<SimpleSpriteAnimation>().BeginAnimation(5, 5, false);
		}
	}

    /// <summary>
    /// 成功的音效
    /// </summary>
	public void GoodFeedback()
	{
		gameObject.GetComponent<AudioSource>().clip = GoodFeedbackVoice;
		gameObject.GetComponent<AudioSource>().Play();
	}

    /// <summary>
    /// 失败的音效
    /// </summary>
	public void BadFeedback()
	{
		gameObject.GetComponent<AudioSource>().clip = BadFeedbackVoice;
		gameObject.GetComponent<AudioSource>().Play();
	}

	void Start ()
    {
		gameObject.GetComponent<SimpleSpriteAnimation>().SetDefaultAnimation(0, 0);
	}
}
