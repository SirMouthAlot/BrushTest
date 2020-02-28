using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BrushPicker : MonoBehaviour
{
    public CS_BAXTER m_brushScript;
    public Material m_pressureMat;
    public Shader m_pressureShader;

    public List<Brush> m_brushes;
    public int m_activeBrush = 0;
    public int m_activeTexture = 0;
    
    
    [Range(0.5f, 2.0f)]
    public float m_pressure = 1.0f;
    
    [Range(0.01f, 90.0f)]
    public float m_angle = 90.0f;

    [SerializeField]
    private Texture2D m_brushToUseTexture;
    private RenderTexture m_brushToUse;
    

    RenderTexture CreateRenderTexture(int w, int h, int type=0){
    	var format = RenderTextureFormat.ARGBFloat;
    	if(type == 1) format = RenderTextureFormat.RFloat;
    	
    	RenderTexture theTex;
    	theTex = new RenderTexture(w,h,0, format);
    	theTex.enableRandomWrite = true;
    	theTex.Create();
    	return theTex;
    }

    void InitRenderTex(int w, int h) 
    {
        m_brushToUse = CreateRenderTexture(w,h);
    }

    private void UpdateBrush()
    {
        //FIXME: Need to switch to actual blending
        if (m_angle >= 0.0f && m_angle <= 22.5f)
        {
            m_activeTexture = 0;
        }
        else if (m_angle >= 22.51f && m_angle <= 67.5f)
        {   
            m_activeTexture = 1;
        }
        else if (m_angle >= 67.51f && m_angle <= 90.0f )
        {
            m_activeTexture = 2;
        }

        m_pressureMat.SetFloat("_BrightnessAmount", m_pressure);

        //Blits the active brush texture to the brush to use render texture
        Graphics.Blit(m_brushes[m_activeBrush][m_activeTexture], m_brushToUse, m_pressureMat);

        //Sets brush to use as active
        RenderTexture.active = m_brushToUse;

        //Reads the pixels from brush to use because the compute shader requires a texture2D
        m_brushToUseTexture.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);
        //Applies it
        m_brushToUseTexture.Apply();

        //Gives the current brush that is to be used, to the Baxter script
        m_brushScript.initialBrush = m_brushToUseTexture;
    }

    private void Start() 
    {
        //Formats texture
        m_brushToUseTexture = new Texture2D(256, 256, TextureFormat.RGB24, false);

        m_brushScript.initialBrush = m_brushes[m_activeBrush][m_activeTexture];

        InitRenderTex(m_brushScript.initialBrush.width, m_brushScript.initialBrush.height);

        //Update brush once intially.
        UpdateBrush();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateBrush();
    }
}
