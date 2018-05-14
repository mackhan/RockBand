using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// 从文件读取歌曲信息的类
/// </summary>
public class SongInfoLoader
{
	public SongInfo songInfo;

    /// <summary>
    /// 总的记录舞台事件偏移的值
    /// </summary>
    private float m_stagingDirectoionRegionOffset = 0;

    /// <summary>
    /// 总的记录谱面偏移的值
    /// </summary>
    private float[] m_onBeatActionInfoRegionOffset = new float[4] { 0, 0, 0, 0 };
    
    /// <summary>
    /// 读入谱面和动画信息
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="isEditorMode">目前没使用</param>
    public void ReadCSV(string _sName, System.IO.TextReader reader, bool isEditorMode=false)
    {
		string line;

        //多少行，调试用
		int	line_number = 0;

		while ((line = reader.ReadLine()) != null)//-读取一行
        {
			line_number++;

			string[] lineCells = line.Split(',');//-按逗号分割
			switch (lineCells[0])
            {
			case "beatPerSecond"://-每秒拍数= BPM / 60
				songInfo.beatPerSecond = float.Parse(lineCells[1]);
				break;

			case "beatPerBar"://-每个小节的节拍数量
                songInfo.beatPerBar = float.Parse(lineCells[1]);
				break;

			case "scoringUnitSequenceRegion-Begin":
				line_number = ReadCSV_OnBeatAction(reader, line_number, int.Parse(lineCells[1]));
				break;

                case "stagingDirectionSequenceRegion-Begin"://-舞台活动开始
				ReadCSV_StagingDirection(reader);
				break;

			case "include"://-如果是include，用递归的方式继续读一下个信息
				TextReader textReader;
#if UNITY_EDITOR
                if (isEditorMode)//-没啥用
                {
                        textReader = File.OpenText("Assets/Resources/" + _sName + lineCells[1] + ".txt");
                }
                else
#endif
                {
                    string data = System.Text.Encoding.UTF8.GetString
                    (
                        (Resources.Load(_sName + lineCells[1]) as TextAsset).bytes
                    );
                    textReader = new StringReader(data);
                }

                Debug.Log("include ：" + lineCells[1]);

                ReadCSV(_sName, textReader);
				break;
			}
		}
	}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
	private void ReadCSV_StagingDirection(System.IO.TextReader reader)
    {
		string line;
		float totalBeatCount = 0;
		float repeatPosition = 0;
		List<StagingDirection> sequence = new List<StagingDirection>();
		while ((line = reader.ReadLine()) != null)
        {
			string[] lineCells = line.Split(',');
			switch (lineCells[0])
            {
			case "regionParameters"://-当前区域。第一个参数没用 Ready1。
                //-regionParameters,Intro,32,4 
                //-在这个例子中，部分的长度是32拍，重复单位是4拍，所以相同的方向将在32÷4总共重复8次。
                totalBeatCount = float.Parse(lineCells[2]);//-这个区间有多少拍
				repeatPosition = float.Parse(lineCells[3]);//-重复拍子
				break;

			case "AllBandMemberDefaultAnimation"://-全部乐手一起设置默认动画
			    {
				    foreach (GameObject member in GameObject.FindGameObjectsWithTag("BandMember"))
                    {
					    StagingDirection_SetBandMemberDefaultAnimation defaultAnimationSet
						    = new StagingDirection_SetBandMemberDefaultAnimation();
					    defaultAnimationSet.triggerBeatTiming = float.Parse(lineCells[1]);//-时间（节拍数）
					    defaultAnimationSet.m_memberName = member.name;//-乐手的名字
					    defaultAnimationSet.m_animationFromIndex = int.Parse(lineCells[2]);//-动画起始帧
					    defaultAnimationSet.m_animationToIndex = int.Parse(lineCells[3]);//-动画结束帧
					    sequence.Add(defaultAnimationSet);
				    }
			    }
				break;

			case "SetAllBandMemberAction"://-全部乐手一起播放动画
                {
				    foreach (GameObject member in GameObject.FindGameObjectsWithTag("BandMember"))
                    {
					    StagingDirection_SetBandMemberAction actionSet = new StagingDirection_SetBandMemberAction();
					    actionSet.triggerBeatTiming = float.Parse(lineCells[1]);//-时间（节拍数）
                        actionSet.m_memberName = member.name;//-乐手的名字
                        actionSet.m_actionName = lineCells[2];//-动画的名字，actionA，actionB，跳
                        sequence.Add(actionSet);
				    }
			    }
				break;

			case "stagingDirectionSequenceRegion-End"://-舞台活动结束，返回
			    {
				    for (float repeatOffest = 0; repeatOffest < totalBeatCount;)//-计算要循环几次
                    {
					    foreach (StagingDirection stagingDirection in sequence)//-变量所有的动作
                        {
						    if (stagingDirection.triggerBeatTiming + repeatOffest > totalBeatCount)//-如果当前动作的执行时间加上循环时间超过区域的总时间，则抛弃这个时间
                            {
							    break;
						    }
						    StagingDirection cloned = stagingDirection.GetClone() as StagingDirection;
						    cloned.triggerBeatTiming += m_stagingDirectoionRegionOffset + repeatOffest;//-重新计算出事件真正的触发时间
						    songInfo.stagingDirectionSequence.Add(cloned);
					    }
					    repeatOffest += repeatPosition;
				    }
				    m_stagingDirectoionRegionOffset += totalBeatCount;
			    }
				return;
                // 因为↑有return，所以这个break不被执行.
                //break;

            default://-其他非全体成员的动作通过工厂类创建自动初始化
			    {
				    StagingDirection stagingDirection
					    = StagingDirectionFactory.CreateStagingDirectionFromEnum(
						    (StagingDirectionEnum)System.Enum.Parse(typeof(StagingDirectionEnum), lineCells[0]));
				    if (stagingDirection != null)
                    {
					    stagingDirection.ReadCustomParameterFromString(lineCells);
					    stagingDirection.triggerBeatTiming = float.Parse(lineCells[1]);//-通用的在第几拍
					    sequence.Add(stagingDirection);
				    }
			    }
				break;
			};
		}
		Debug.LogError("StagingDirectionSequenceRegion.ReadCSV: ParseError - missing stagingDirectionSequenceRegion-End");
	}

    /// <summary>
    /// 读入谱面信息
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="line_number"></param>
    /// <returns></returns>
	private int	ReadCSV_OnBeatAction(System.IO.TextReader reader, int line_number, int _iIndex)
    {
		string line;
		SequenceRegion region = new SequenceRegion();

		List<OnBeatActionInfo> sequence = new List<OnBeatActionInfo>();

		while ((line = reader.ReadLine()) != null)
        {
			line_number++;
            Debug.Log("line_number ：" + line_number);

            string[] lineCells = line.Split(',');
			switch (lineCells[0])
            {
			case "regionParameters":
				region.name = lineCells[1];//-区域的名字调试用
				region.totalBeatCount = float.Parse(lineCells[2]);//-一共多少拍
				region.repeatPosition = float.Parse(lineCells[3]);//-部分重复单位（节拍数）
                break;

			case "scoringUnitSequenceRegion-End"://-区域结束，应该只有一块
			    {
                    region.triggerBeatTiming = m_onBeatActionInfoRegionOffset[_iIndex - 1];
				    songInfo.onBeatActionRegionSequence.Add(region);
				    for (float repeatOffest = 0; repeatOffest < region.totalBeatCount; repeatOffest += region.repeatPosition)
				    {
					    foreach(OnBeatActionInfo onBeatActionInfo in sequence)
                        {
						    if (onBeatActionInfo.triggerBeatTiming + repeatOffest > region.totalBeatCount)
						    {
							    break;
						    }
						    OnBeatActionInfo cloned = onBeatActionInfo.GetClone() as OnBeatActionInfo;
						    cloned.triggerBeatTiming += m_onBeatActionInfoRegionOffset[_iIndex - 1] + repeatOffest;
						    songInfo.onBeatActionSequence[_iIndex-1].Add(cloned);
					    }
				    }
				    m_onBeatActionInfoRegionOffset[_iIndex - 1] += region.totalBeatCount;
			    }
                return (line_number);
                // 因为↑有return，所以这个break不被执行.
                //break;

                case "SingleShot":
			    {
				    OnBeatActionInfo onBeatActionInfo = new OnBeatActionInfo();
				    if (lineCells[2] != "")
                    {
					    onBeatActionInfo.playerActionType
						    = (PlayerActionEnum)System.Enum.Parse(typeof(PlayerActionEnum), lineCells[2]);
				    }
                    else//-默认就是HeadBanging
                    {
					    onBeatActionInfo.playerActionType = PlayerActionEnum.HeadBanging;
				    }
                    Debug.Log("triggerBeatTiming ：" + float.Parse(lineCells[1]));
				    onBeatActionInfo.triggerBeatTiming = float.Parse(lineCells[1]);

				    //-记录行号
				    onBeatActionInfo.line_number = line_number;

				    sequence.Add(onBeatActionInfo);
			    }
				break;
			};

		}
		Debug.LogError("ScoringUnitSequenceRegion.ReadCSV: ParseError - missing ScoringUnitSequenceRegion-End");

		return(line_number);
	}
}
