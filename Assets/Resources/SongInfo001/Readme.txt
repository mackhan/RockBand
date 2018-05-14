本节介绍如何描述歌曲信息设置文件。

可以在此文件中设置的信息大约为以下3项。
- 歌曲的基本信息
- 在舞台上的动画
- 节奏序列作为评分评估的标准

可以将信息保存在多个文件中，按照以下方式进行设置。
--------------
**如何使用多个文件**
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
**如何设置舞台动画**
　　概述：
      1.对于每个阶段，通过以下步骤输入舞台表演。
        按如下所示输入每个部分的信息。
********
stagingDirectionSequenceRegion-Begin
....(部分基本信息，舞台动画设定等)
stagingDirectionSequenceRegion-End
********

     2.每个阶段的开始时间自动设置为前一部分结束的时间。
　　　
　　　3.可以为每个阶段指定重复几次。在这个文件中，演奏的时间由填写的总节拍数确定，每个区域的时间由区域的拍数确定，用阶段的拍数除以区域的拍数就得到重复的次数。
　　基本零件信息：
　　　写作方法：
　　　　regionParameters, 区域名称, 阶段长度（节拍数）, 区域的长度（节拍数）
　　　例：
　　　　regionParameters,Intro,32,4
　　　　*在这个例子中，阶段的长度是32拍，每个区域持续4拍，所以相同的动画会32÷4总共重复8次。

    示例1：烟花：
　　　写作方法：
　　　　FireBlast, 时间（节拍数，在这个区域的第几拍开始）, （0 ...舞台右侧放烟花，1 ...舞台左侧放烟花）, 烟花的数量
　　　例：
　　　　FireBlast,0,0,100

　　示例2：灯光亮度的改变：
　　　注意：
　　　　当变化前后的光亮度相同时，外观没有变化。
　　　写作方法：
　　　　LightFade, 时间（节拍数）, 灯光ID号码（0 ...红色聚光灯，1 ...蓝色聚光灯，2 ...绿色聚光灯）, 改变后的亮度
　　　例：
　　　　LightFade,0,0,3

　　示例3：闪光灯，会闪一下：
　　　写作方法：
　　　　LightFlash , 时间（节拍数）, 灯光的ID号码
　　　例：
　　　　LightFlash,0,0

　　示例4：2个灯光位置交换：
　　　注意：
　　　　当改变前后聚光灯的亮度相同时，外观没有变化。
　　　写作方法：
　　　　LightShuffle , 时间（节拍数），一个灯光的ID号码，另一个灯光的ID号码，光线移动的速度
　　　　LightShuffle,0,0,1,10

　　示例5：乐队成员动画（例如跳等，出了动画还可以有声音等，其实是一个Action）：
　　　写作方法：
　　　　SetBandMemberAction , 时间（节拍数），动作名称（actionA, actionB, jump) , 成员名称(Vocalist, Bassist, Guitarist, Drummer)
　　　例：
　　　　SetBandMemberAction,0,jump,Bassist

　　示例6：所有乐队成员的动画：
　　　写作方法：
　　　　SetAllBandMemberAction , 时间（节拍数） , 操作名称( actionA, actionB, jump )
　　　例：
　　　　SetAllBandMemberAction,0,jump

    示例7：乐队成员的默认动画：
　　　写作方法：
　　　　SetBandMemberDefaultAnimation , 时间（节拍数），动画开始帧数，动画结束帧数，成员名称
　　　例：
　　　　SetBandMemberDefaultAnimation,0,2,4,Bassist

    示例8：所有乐队成员的默认动画：
　　　写作方法：
　　　　AllBandMemberDefaultAnimation , 时间（节拍数），动画开始帧数，动画结束帧数
　　　例：
　　　　AllBandMemberDefaultAnimation,0,1,1

--------------

--------------
**如何设定音乐谱面**

　　概要：
　　　1.每个阶段的设置。
********
scoringUnitSequenceRegion-Begin

....（基本部分信息，谱面信息等）

scoringUnitSequenceRegion-End
********

　　　2.目前情况下唯一可设置的设置是“评估标准是否按照时间安排”？

　　阶段基本信息（目前与阶段的基本信息相同）：
     写作方法：
　　　　regionParameters , 区域名称，阶段长度（节拍数），区域的长度（节拍数）
　　　例：
　　　　regionParameters,Intro,32,4
	　　*在这个例子中，阶段的长度是32拍，重复单位是4拍，所以相同的谱面将32÷4重复8次。

　　评估标准1.你是否在指定节拍进行了点击
　　　写作方法：：
　　　　SingleShot , 时间（节拍数）
　　　例：
　　　　SingleShot,0
	※这表示是否在第0拍进行一次点击，加分评价也以这个为标准。

--------------