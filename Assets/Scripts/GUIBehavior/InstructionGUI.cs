using UnityEngine;
using System.Collections;

/// <summary>
/// 教学的操作画面的GUI
/// </summary>
public class InstructionGUI : MonoBehaviour
{
    /// <summary>
    /// 游戏说明
    /// </summary>
	public string title="title";

    /// <summary>
    /// 如回去玩
    /// </summary>
	public string instruction="How to Play";
	public string bandMemberLabel="bandMember";
	public string guageLabel= "guageLabel";
	public string playerAvatorLabel="playerAvator";
	public string actionMarkerLabel="actionMarker";
	public string targetMarkerLabel="targetmarker";

	public GUISkin guiStyle;


	public SimpleSpriteAnimation sampleBandMemberAniamtion;

    /// <summary>
    /// 玩家的动画
    /// </summary>
    public SimpleSpriteAnimation playerAvatorAnimation;


    public Texture actinoMarker;
	
	void Update ()
    {
        //每个时间都会让角色动画。
        animationCounter += Time.deltaTime;
		if (animationCounter > 1.0f)
        {
			sampleBandMemberAniamtion.BeginAnimation(1, 1, false);
			playerAvatorAnimation.BeginAnimation(2, 1, false);
			animationCounter=0;
		}
        //点击下一步，如果检测到玩家点击了鼠标，开始游戏
        if (Input.GetMouseButton(0))
        {
			GameObject.Find("PhaseManager").GetComponent<PhaseManager>().SetPhase("Play");
		}
	}

    float animationCounter = 0;

    void OnGUI()
    {
		GUI.skin = guiStyle;
		GUI.Label( new Rect(20, 60, 100, 40), bandMemberLabel );
		GUI.Label( new Rect(150, 40, 180, 40), guageLabel );
		GUI.Label( new Rect(60, 210, 150, 40), playerAvatorLabel );
		GUI.Label( new Rect(5, 260, 210, 80), targetMarkerLabel );
		GUI.Label( new Rect(200, 260, 210, 80), actionMarkerLabel );
		GUI.DrawTexture( new Rect(200, 285, actinoMarker.width, actinoMarker.height), actinoMarker);
		GUI.Box( new Rect(20, 370, Screen.width-20, 150), instruction );
	}
}
