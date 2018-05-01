using UnityEngine;
using System.Collections;

/// <summary>
/// 开始画面的GUI
/// </summary>
public class StartupMenuGUI : MonoBehaviour
{
	public Texture titleTexture;
	public GUISkin guiStyle;
	
	void OnGUI()
    {
		GUI.skin = guiStyle;

		Graphics.DrawTexture( 
			new Rect( (Screen.width - titleTexture.width)/2.0f , 0, titleTexture.width, titleTexture.height )
			, titleTexture
		);

        //-如果按了开始键
		if( GUI.Button( new Rect( (Screen.width - 150)/2.0f, 300, 150, 40 ), "Start" ) )
        {
			GameObject.Find("PhaseManager").GetComponent<PhaseManager>().SetPhase("OnBeginInstruction");
		}

#if UNITY_EDITOR
        //-编辑器下按了开发键
		if ( GUI.Button( new Rect( (Screen.width - 150)/2.0f, 360, 150, 40 ), "Development" ) )
        {
			GameObject.Find("PhaseManager").GetComponent<PhaseManager>().SetPhase("DevelopmentMode");
		}
#endif
	}
}
