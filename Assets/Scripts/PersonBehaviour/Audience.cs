using UnityEngine;
using System.Collections;

/// <summary>
/// 观众的控制类
/// </summary>
public class Audience : MonoBehaviour
{
	public void Jump()
    {
		GetComponent<SimpleActionMotor>().Jump();
	}
}
