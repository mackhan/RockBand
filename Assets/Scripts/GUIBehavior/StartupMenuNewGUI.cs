using UnityEngine;
using System.Collections;

/// <summary>
/// 开始画面的GUI
/// </summary>
public class StartupMenuNewGUI : MonoBehaviour
{	
    public void PressStart()
    {
        GameObject.Find("PhaseManager").GetComponent<PhaseManager>().SetPhase("OnBeginInstruction");
    }

	void OnGUI()
    {
#if UNITY_EDITOR
        //-编辑器下按了开发键
		if ( GUI.Button( new Rect( (Screen.width - 150)/2.0f, 360, 150, 40 ), "Development" ) )
        {
			GameObject.Find("PhaseManager").GetComponent<PhaseManager>().SetPhase("DevelopmentMode");
		}
#endif
	}
}
