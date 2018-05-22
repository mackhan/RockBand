using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 选择音乐
/// </summary>
public class PuaseGUI : MonoBehaviour
{
    public Text TextText;

    public void OnSpeedUp()
    {
        GameObject.Find("OnPlayGUI").GetComponent<OnPlayGUI>().OnSpeedUp(TextText);
    }

    public void OnSpeedDown()
    {
        GameObject.Find("OnPlayGUI").GetComponent<OnPlayGUI>().OnSpeedDown(TextText);
    }
}
