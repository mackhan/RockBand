using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum eResult
{
    eBest,
    eGood,
    eBad,
}

/// <summary>
/// 结算界面的GUI
/// </summary>
public class Settlement : MonoBehaviour
{
    /// <summary>
    /// 得分管理器
    /// </summary>
    ScoringManager m_scoringManager;

    public GameObject Best;
    public GameObject Good;
    public GameObject Bad;

    /// <summary>
    /// 
    /// </summary>
    public GameObject S;

    /// <summary>
    /// 
    /// </summary>
    public GameObject A;

    /// <summary>
    ///  
    /// </summary>
    public GameObject C;

    /// <summary>
    /// The text.
    /// </summary>
    public GameObject TextScore;

    /// <summary>
    /// Sets the result.
    /// </summary>
    /// <param name="_eResult">E result.</param>
    public void SetResult(eResult _eResult, float _iNumber)
    {
        switch (_eResult)
        {
            case eResult.eBest:
                Best.SetActive(true);
                Good.SetActive(false);
                Bad.SetActive(false);
                S.SetActive(true);
                A.SetActive(false);
                C.SetActive(false);
                break;
            case eResult.eGood:
                Best.SetActive(false);
                Good.SetActive(true);
                Bad.SetActive(false);
                S.SetActive(false);
                A.SetActive(true);
                C.SetActive(false);
                break;
            case eResult.eBad:
                Best.SetActive(false);
                Good.SetActive(false);
                Bad.SetActive(true);
                S.SetActive(false);
                A.SetActive(false);
                C.SetActive(true);
                break;
        }

        TextScore.GetComponent<Text>().text = _iNumber.ToString();
    }

    void Start()
	{
		m_scoringManager = GameObject.Find("ScoringManager").GetComponent<ScoringManager>();
	}

    /// <summary>
    /// 返回Menu界面
    /// </summary>
    public void PressReturn()
    {
        GameObject.Find("PhaseManager").GetComponent<PhaseManager>().SetPhase("Restart");
    }
}
