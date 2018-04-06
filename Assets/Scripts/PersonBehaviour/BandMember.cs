using UnityEngine;
using System.Collections;

/// <summary>
/// 乐队成员的动画
/// </summary>
public class BandMember : MonoBehaviour {
	public bool jumpEnabled = true;
	public AudioClip GoodFeedbackVoice;
	public AudioClip BadFeedbackVoice;
	public void Jump(){
		if(jumpEnabled){
			gameObject.GetComponent<SimpleActionMotor>().Jump();
			gameObject.GetComponent<SimpleSpriteAnimation>().BeginAnimation(5,5,false);
		}
	}
	public void GoodFeedback()
	{
		gameObject.GetComponent<AudioSource>().clip = GoodFeedbackVoice;
		gameObject.GetComponent<AudioSource>().Play();
	}
	public void BadFeedback()
	{
		gameObject.GetComponent<AudioSource>().clip = BadFeedbackVoice;
		gameObject.GetComponent<AudioSource>().Play();
	}

	void Start () {
		gameObject.GetComponent<SimpleSpriteAnimation>().SetDefaultAnimation(0,0);
	}
}
