using UnityEngine;
using System.Collections;

/// <summary>
/// 实现简单的格子动画的组件
/// </summary>
public class SimpleSpriteAnimation: MonoBehaviour
{
    /// <summary>
    /// 动画的速率
    /// </summary>
	public float animationFrameRateSecond = 0.2f;

    /// <summary>
    /// 动画有多少列
    /// </summary>
	public int divisionCountX = 1;

    /// <summary>
    /// 动画有多少行
    /// </summary>
	public int divisionCountY = 1;

    float m_frameElapsedTime = 0;

    /// <summary>
    /// 从第几帧开始播
    /// </summary>
    int m_fromIndex = 0;

    /// <summary>
    /// 播放到第几帧
    /// </summary>
    int m_toIndex = 0;

    /// <summary>
    /// 默认是从第几帧开始播
    /// </summary>
    int m_defaultFromIndex = 0;

    /// <summary>
    /// 默认是播放到第几帧
    /// </summary>
    int m_defaultToIndex = 0;

    /// <summary>
    /// 是否循环
    /// </summary>
    bool m_loop = false;

    /// <summary>
    /// 当前播放到第几帧
    /// </summary>
    int m_currentIndex = 0;

    /// <summary>
    /// 从第几帧播放到第几帧
    /// </summary>
    /// <param name="fromIndex"></param>
    /// <param name="toIndex"></param>
    /// <param name="loop"></param>
    public void BeginAnimation(int fromIndex, int toIndex, bool loop=false)
    {
		m_currentIndex = m_fromIndex = fromIndex;
		m_toIndex = toIndex;
		m_loop = loop;
		m_frameElapsedTime = 0;
		SetMaterilTextureUV();
	}
	
    /// <summary>
    /// 获得当前的贴图
    /// </summary>
    /// <returns></returns>
	public Texture GetTexture()
    {
		return m_kTexture;
	}

    /// <summary>
    /// 获取纹理显示部分的矩形
    /// </summary>
    /// <param name="frameIndex"></param>
    /// <returns></returns>
	public Rect GetUVRect(int frameIndex)
    {
		int frameIndexNormalized = frameIndex;
		if (frameIndex >= divisionCountX*divisionCountY)//-如果超过限制，归一化，转成循环
        {
            frameIndexNormalized = frameIndex % (divisionCountX * divisionCountY);
        }
			
		float posX = ((frameIndexNormalized % divisionCountX) / (float)divisionCountX);//-计算x的起始位置
		float posY = (1- ((1 + (frameIndexNormalized / divisionCountX)) / (float)divisionCountY));//-计算Y的起始位置，00在左下，需要1-
		return new Rect(posX
            , posY
            , m_kScale.x
            , m_kScale.y);
	}

    /// <summary>
    /// 获取当前帧纹理显示部分的矩形
    /// </summary>
    /// <returns></returns>
	public Rect GetCurrentFrameUVRect()
    {
		return GetUVRect(m_currentIndex);
	}

	/// <summary>
    /// 默认的参数设置
    /// </summary>
    /// <param name="defaultFromIndex"></param>
    /// <param name="defaultToIndex"></param>
	public void SetDefaultAnimation(int defaultFromIndex, int defaultToIndex)
    {
		m_currentIndex = m_fromIndex = m_defaultFromIndex = defaultFromIndex;
		m_toIndex = m_defaultToIndex = defaultToIndex;
	}

    /// <summary>
    /// 获得一帧贴图的宽
    /// </summary>
    /// <returns></returns>
	public float GetWidth()
    {
		return m_kScale.x * m_kTexture.width;
	}
	
    /// <summary>
    /// 获得一帧贴图的高
    /// </summary>
    /// <returns></returns>
	public float GetHeight()
    {
		return m_kScale.y * m_kTexture.height;
	}
	
    /// <summary>
    /// 播放动画帧
    /// </summary>
	public void AdvanceFrame()
    {
		if (m_fromIndex < m_toIndex)//-如果起始动画帧小于目标动画帧正的播放
        {
			if (m_currentIndex <= m_toIndex)//-如果还没到目标帧，前进一帧
            {
				m_currentIndex++;
				if (m_toIndex < m_currentIndex)//-如果超过了
                {
                    Loop();
                }
				SetMaterilTextureUV();
			}
            else
            {
                Debug.LogError("Out of range");
            }
		}
        else//-如果起始动画帧小于目标动画帧倒的播放
        {
			if (m_currentIndex >= m_toIndex)
            {
				m_currentIndex--;
				if (m_toIndex > m_currentIndex)
                {
                    Loop();

                }
				SetMaterilTextureUV();
			}
            else
            {
                Debug.LogError("Out of range");
            }
        }
    }

    /// <summary>
    /// 当动画播放完成了执行循环或者回到默认动画的操作
    /// </summary>
    void Loop()
    {
        if (m_loop)//-如果循环回到开始帧
        {
            m_currentIndex = m_fromIndex;
        }
        else
        {
            //-如果不是循环，会到默认的初始帧
            m_currentIndex = m_fromIndex = m_defaultFromIndex;
            m_toIndex = m_defaultToIndex;
        }
    }

    /// <summary>
    /// 贴图动画每块的大小
    /// </summary>
    Vector2 m_kScale;

    /// <summary>
    /// 当前的贴图
    /// </summary>
    Texture m_kTexture;

	void Start ()
    {
        Material kMaterial = GetComponent<Renderer>().material;
        m_kTexture = kMaterial.GetTexture("_MainTex");
        m_kScale = new Vector2(1.0f / divisionCountX, 1.0f / divisionCountY);
        kMaterial.mainTextureScale = m_kScale;
    }
	
	void Update ()
    {
		m_frameElapsedTime += Time.deltaTime;
		if (animationFrameRateSecond < m_frameElapsedTime)//-如果经过的时间超过了动画速率播放一次
        {
			m_frameElapsedTime = 0;
			AdvanceFrame();
		}
	}

    /// <summary>
    /// 根据帧编号设置适当的纹理坐标UV
    /// </summary>
	void SetMaterilTextureUV()
    {
		Rect uvRect = GetCurrentFrameUVRect();
		GetComponent<Renderer>().material.mainTextureOffset = new Vector2(uvRect.x, uvRect.y);
	}	
}
