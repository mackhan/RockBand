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
	public int divisionCountX=1;

    /// <summary>
    /// 动画有多少行
    /// </summary>
	public int divisionCountY=1;

    float m_frameElapsedTime = 0;
    int m_fromIndex = 0, m_toIndex = 0;
    int m_defaultFromIndex = 0;
    int m_defaultToIndex = 0;

    /// <summary>
    /// 是否循环
    /// </summary>
    bool m_loop = false;
    int m_currentIndex = 0;

    public void BeginAnimation(int fromIndex, int toIndex, bool loop=false)
    {
		m_currentIndex = m_fromIndex = fromIndex;
		m_toIndex = toIndex;
		m_loop = loop;
		m_frameElapsedTime = 0;
		SetMaterilTextureUV();
	}
	
    //現在のメインテクスチャを取得
	public Texture GetTexture(){
		return GetComponent<Renderer>().material.GetTexture("_MainTex");
	}
	
    //テクスチャ表示部分のRectを取得
	public Rect GetUVRect(int frameIndex)
    {
		int frameIndexNormalized=frameIndex;
		if(frameIndex>=divisionCountX*divisionCountY) 
			frameIndexNormalized=frameIndex%(divisionCountX*divisionCountY);
		float posX=((frameIndexNormalized%divisionCountX)/(float)divisionCountX);
		float posY=(1- ((1+(frameIndexNormalized/divisionCountX))/(float)divisionCountY));
		return new Rect( 
			posX, 
			posY, 
			GetComponent<Renderer>().material.mainTextureScale.x, 
			GetComponent<Renderer>().material.mainTextureScale.y
		);
	}
	public Rect GetCurrentFrameUVRect()
    {
		return GetUVRect(m_currentIndex);
	}

	//明確な指定が無い場合のアニメーションを設定
	public void SetDefaultAnimation( int defaultFromIndex, int defaultToIndex )
    {
		m_currentIndex = m_fromIndex = m_defaultFromIndex = defaultFromIndex;
		m_toIndex = m_defaultToIndex = defaultToIndex;
	}
	//ピクセル幅を取得
	public float GetWidth(){
		return GetComponent<Renderer>().material.mainTextureScale.x * GetComponent<Renderer>().material.GetTexture("_MainTex").width;
	}
	//ピクセル高さを取得
	public float GetHeight(){
		return GetComponent<Renderer>().material.mainTextureScale.y * GetComponent<Renderer>().material.GetTexture("_MainTex").height;
	}
	
    //アニメーションのコマを進める
	public void AdvanceFrame()
    {
		if (m_fromIndex < m_toIndex)//-如果起始动画帧小于目标动画帧正的播放
        {
			if (m_currentIndex <= m_toIndex)
            {
				m_currentIndex++;
				if (m_toIndex < m_currentIndex)
                {
					if (m_loop)
                    {
						m_currentIndex = m_fromIndex;
					}
					else
                    {
						m_currentIndex = m_fromIndex = m_defaultFromIndex;
						m_toIndex = m_defaultToIndex;
					}
				}
				SetMaterilTextureUV();
			}
		}
        else//-如果起始动画帧小于目标动画帧倒的播放
        {
			if (m_currentIndex >= m_toIndex)
            {
				m_currentIndex--;
				if (m_toIndex > m_currentIndex)
                {
					if (m_loop)
                    {
						m_currentIndex = m_fromIndex;
					}
					else
                    {
						m_currentIndex = m_fromIndex = m_defaultFromIndex;
						m_toIndex = m_defaultToIndex;
					}
				}
				SetMaterilTextureUV();
			}
		}
	}
	void Start ()
    {
		GetComponent<Renderer>().material.mainTextureScale = new Vector2(1.0f/divisionCountX,1.0f/divisionCountY);
	}
	
	void Update ()
    {
		m_frameElapsedTime = Time.deltaTime;
		if (animationFrameRateSecond < m_frameElapsedTime)//-如果经过的时间超过了动画速率播放一次
        {
			m_frameElapsedTime=0;
			AdvanceFrame();
		}
	}

	//コマ番号から適切なテクスチャ座標UVを設定
	void SetMaterilTextureUV()
    {
		Rect uvRect = GetCurrentFrameUVRect();
		GetComponent<Renderer>().material.mainTextureOffset = new Vector2(uvRect.x,uvRect.y);
	}	
}
