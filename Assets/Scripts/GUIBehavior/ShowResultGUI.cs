using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using System.Collections;

/// <summary>
/// 结算界面的GUI
/// </summary>
public class ShowResultGUI : MonoBehaviour
{
    /// <summary>
    /// 标题 
    /// </summary>
	public string title = "title";

    /// <summary>
    /// 分数文字
    /// </summary>
	public string scoreLabel = "Score : ";


	public string commentLabel = "Comment : ";
	public string comment_EXCELLENT = "comment shown here";
	public string comment_GOOD = "comment shown here";
	public string comment_BAD = "comment shown here";
	public string comment = "comment shown here";
	public GUISkin guiStyle;

    ScoringManager m_scoringManager;

    void Start()
	{
		m_scoringManager = GameObject.Find("ScoringManager").GetComponent<ScoringManager>();
	}

	void OnGUI()
	{
		GUI.skin = guiStyle;
		GUI.Box(new Rect(10.0f, 10.0f, Screen.width - 20.0f, Screen.height - 20.0f), title);//-
		GUI.Label( new Rect(20, 100, 200, 40), scoreLabel );
		GUI.Label( new Rect(20, 140, 200, 40), m_scoringManager.score.ToString() );
		GUI.Label( new Rect(20, 180, 200, 40), commentLabel );
		GUILayout.BeginArea(new Rect(20.0f, 220.0f, Screen.width - 20.0f, Screen.height - 40.0f));
		GUILayout.Label(comment);
		GUILayout.EndArea();

        //-返回Menu界面
		if( GUI.Button( new Rect( (Screen.width - 150)/2.0f, 360, 150, 40 ), "Return to Menu" ) )
        {
			GameObject.Find("PhaseManager").GetComponent<PhaseManager>().SetPhase("Restart");
		}
	}
}
