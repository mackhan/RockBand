using UnityEngine;
using System.Collections;
using System.IO;

/// <summary>
/// û��ʹ��
/// </summary>
public class SongInfoExporter_CSV
{
	/// <summary>
    /// д��������Ϣ
    /// </summary>
    /// <param name="songInfo"></param>
    /// <param name="writer"></param>
	static public void GetOnBeatActionInfo(SongInfo songInfo,TextWriter writer)
    {
		writer.WriteLine("scoringUnitSequenceRegion-Begin");

        float songLength = songInfo.onBeatActionSequence[songInfo.onBeatActionSequence.Count-1].triggerBeatTiming + 1;
		writer.WriteLine("regionParameters,Unified," 
            + songLength 
            + "," + songLength);

        foreach (OnBeatActionInfo onBeatActionInfo in songInfo.onBeatActionSequence)
        {
			writer.WriteLine(onBeatActionInfo.GetCustomParameterAsString_CSV());
		}

        writer.WriteLine("scoringUnitSequenceRegion-End");
	}
}
