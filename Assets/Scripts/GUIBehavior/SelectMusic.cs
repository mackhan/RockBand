using UnityEngine;
using System.Collections;

/// <summary>
/// 选择音乐
/// </summary>
public class SelectMusic : MonoBehaviour
{
    /// <summary>
    /// 是否是Debug版
    /// </summary>
    public static bool Development = false;

    public static int Index = 0;

    /// <summary>
    /// 选择歌曲按钮
    /// </summary>
    /// <returns>The select.</returns>
    /// <param name="_iIndex">I index.</param>
    public void Select(int _iIndex)
    {
        Select2(_iIndex);
    }

    public static void Select2(int _iIndex)
    {
        MusicManager kMusicManager = GameObject.Find("MusicManager").GetComponent<MusicManager>();
        PhaseManager kPhaseManager = GameObject.Find("PhaseManager").GetComponent<PhaseManager>();
        string sPhase = "Play";
        if (Development)
        {
            sPhase = "DevelopmentMode";
        }
        Index = _iIndex;

        Debug.Log("Select " + Index);
        kMusicManager.SetMusic("Sounds/Music00"+ Index);
        kPhaseManager.SongName = "SongInfo00" + Index + "/";
        kPhaseManager.SetPhase(sPhase);
    }
}
