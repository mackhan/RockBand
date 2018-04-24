using UnityEngine;
using System.Collections;

/// <summary>
/// �ֶӳ�Ա�Ķ���
/// </summary>
public class BandMember : MonoBehaviour
{
	public bool jumpEnabled = true;
	public AudioClip GoodFeedbackVoice;
	public AudioClip BadFeedbackVoice;

    /// <summary>
    /// ��Ա����
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
    /// �ɹ�����Ч
    /// </summary>
	public void GoodFeedback()
	{
		gameObject.GetComponent<AudioSource>().clip = GoodFeedbackVoice;
		gameObject.GetComponent<AudioSource>().Play();
	}

    /// <summary>
    /// ʧ�ܵ���Ч
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
