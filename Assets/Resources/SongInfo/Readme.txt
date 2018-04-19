本节介绍如何描述歌曲信息设置文件。

可以在此文件中设置的信息大约为以下3项。

- 歌曲的基本信息
- 在舞台上制作
- 节奏序列作为评分评估的标准

可以将信息划分为多个文件并按照以下方式进行描述。
--------------
**如何描述多个文件**
    让我们加载另一个文件的信息：
　　写作方法：
　　　include,文件名称
　　例：
　　　include,songInfoCSV_Staging.txt
--------------

--------------
**如何设置歌曲的基本信息**
　　歌曲节奏：
　　　写作方法：
　　　　beatPerSecond, BeatsPerSecond(每秒拍数= BPM / 60) 
　　　例：
　　　　beatPerSecond,2.333333

　　每部小说的节拍数量：
　　　写作方法：
　　　　beatPerBar, BeatsPerBar(每个小节的节拍数量) 
　　　例：
　　　　beatPerBar,4
--------------

--------------
**如何设置舞台导演**

　　概述：
      1.对于每个阶段，通过以下步骤输入舞台表演。
        按如下所示输入每个部分的信息。
********
stagingDirectionSequenceRegion-Begin

....(部分基本信息，舞台方向指定等)

stagingDirectionSequenceRegion-End
********
      2.部分开始时间自动设置为前一部分结束的时间。
　　　
　　　3.可以为每个部分指定重复。在这个文件中，演奏的时间由节拍指定，这被指定为重复开始点的节奏。
　　基本零件信息：
　　　写作方法：
　　　　regionParameters, 零件名称, 部分长度（节拍数）, 部分重复单位（节拍数）
　　　例：
　　　　regionParameters,Intro,32,4
　　　　*在这个例子中，部分的长度是32拍，重复单位是4拍，所以相同的方向将在32÷4总共重复8次。

    指导1.烟花：
　　　写作方法：
　　　　FireBlast, 时间（节拍数）, （0 ...烟花在舞台右侧，1 ...烟花在舞台左侧）, 火花的数量
　　　例：
　　　　FireBlast,0,0,100

　　生产2.照度变化：
　　　注意：
　　　　当变化前后的光照度相同时，外观没有变化。
　　　写作方法：
　　　　LightFade, 时间（节拍数）, 照明ID号码（0 ...红色聚光灯，1 ...蓝色聚光灯，2 ...绿色聚光灯）, 改变后的照度
　　　例：
　　　　LightFade,0,0,3

　　指导3.闪光照明：
　　　写作方法：
　　　　LightFlash , 时间（节拍数）, 照明ID号码
　　　例：
　　　　LightFlash,0,0

　　指导4 2个照明位置交换（洗牌）：
　　　注意：
　　　　当改变前后聚光灯的照度相同时，外观没有变化。
　　　写作方法：
　　　　LightShuffle , 时间（节拍数），一个灯光的ID号码，另一个灯光的ID号码，光线移动的速度
　　　　LightShuffle,0,0,1,10

　　指导5.乐队成员行动：
　　　写作方法：
　　　　SetBandMemberAction , 时间（节拍数），动作名称（actionA, actionB, jump) , 成员名称( Vocalist, Bassist, Guitarist, Drummer )
　　　例：
　　　　SetBandMemberAction,0,jump,Bassist

　　制作6.乐队成员的行动（所有成员一起指定）：
　　　写作方法：
　　　　SetAllBandMemberAction , 时间（节拍数） , 操作名称( actionA, actionB, jump )
　　　例：
　　　　SetAllBandMemberAction,0,jump

    指导7.乐队成员动画的规格：
　　　写作方法：
　　　　SetBandMemberDefaultAnimation , 时间（节拍数），动画开始帧数，动画结束帧数，成员名称
　　　例：
　　　　SetBandMemberDefaultAnimation,0,2,4,Bassist

    指导8.乐队成员动画的规格（所有成员全部一起指定）：
　　　写作方法：
　　　　AllBandMemberDefaultAnimation , 时间（节拍数），动画开始帧数，动画结束帧数
　　　例：
　　　　AllBandMemberDefaultAnimation,0,1,1

--------------

--------------
**如何设定节奏的顺序作为评分评估的参考**

　　概要：
　　　1.以及生产，为每个部分设置。
********
scoringUnitSequenceRegion-Begin

....（基本部分信息，舞台指定等）

scoringUnitSequenceRegion-End
********
　　　2.目前情况下唯一可设置的设置是“评估标准是否按照时间安排”？

　　部分基本信息（目前与生产部分的基本信息相同）：
     写作方法：
　　　　regionParameters , 部分名称，部分长度（节拍数），部分重复单位（节拍数）
　　　例：
　　　　regionParameters,Intro,32,4
	　　*在这个例子中，部分的长度是32拍，重复单位是4拍，所以相同的方向将在32÷4总共重复8次。

　　评估标准1.你是否根据时间进行了头部撞击
　　　写作方法：：
　　　　SingleShot , 时间（节拍数）
　　　例：
　　　　SingleShot,0
	※这是在重复的开始点进行一次头部敲打时的加分评价标准。

--------------