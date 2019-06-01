using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPong_CellularAutomata : MonoBehaviour
{

    public Color _Alive = new Color(1,1,1,1);
    public Color _Dead = new Color(0,0,0,1);
    public int _TickSpeed = 2;
    public float posX = 0.75f;
    public float posY = 0.75f;
    Texture2D texA;
    Texture2D texB;
    Texture2D inputTex;
    Texture2D outputTex;
    RenderTexture rt1;

    Shader cellularAutomataShader;
    Shader ouputTextureShader;

    int width;
    int height;
    public int pointA;
    public int pointB;

    Renderer rend;
    int count = 0;

    void Start()
    {
        //print(SystemInfo.copyTextureSupport);

        width = 64;
        height = 64;

        texA = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texB = new Texture2D(width, height, TextureFormat.RGBA32, false);
        
        texA.filterMode = FilterMode.Point;
        texB.filterMode = FilterMode.Point;


        // Code for adding multiple colors. Just keeping it here for later.

        //float boo;
        //float states = 5;
        //for (int i = 0; i < height; i++) {
        //    for (int j = 0; j < width; j++) {
        //        boo = Random.Range(0.0f, 1.0f);
        //        float inc = 1;
        //        if (boo >= 0 && boo <= inc / states) {
        //            texA.SetPixel(i, j, Color.black);
        //            Debug.Log("black");
        //        } else if (boo > inc / states && boo <= ++inc / states) {
        //            texA.SetPixel(i, j, Color.red);
        //            Debug.Log("red");
        //        } else if (boo > inc / states && boo <= ++inc / states) {
        //            texA.SetPixel(i, j, Color.blue);
        //            Debug.Log("blue");
        //        } else if (boo > inc / states && boo <= ++inc / states) {
        //            texA.SetPixel(i, j, Color.magenta);
        //            Debug.Log("magenta");
        //        } else if (boo > inc / states && boo <= ++inc / states) {
        //            texA.SetPixel(i, j, Color.yellow);
        //            Debug.Log("yellow");
        //        } else if (boo > inc / states && boo <= ++inc / states) {
        //            texA.SetPixel(i, j, Color.cyan);
        //            Debug.Log("cyan");
        //        } else if (boo > inc / states && boo <= ++inc / states) {
        //            texA.SetPixel(i, j, Color.white);
        //            Debug.Log("white");
        //        } else {
        //            texA.SetPixel(i, j, Color.green);
        //            Debug.Log("green");
        //        }
        //    }
        //}




        // The normal code that adds random alive and dead pixels

        //for (int i = 0; i < height; i++)
        //    for (int j = 0; j < width; j++)
        //        if (Random.Range(0.0f, 1.0f) < 0.5)
        //        {
        //            texA.SetPixel(i, j, _Alive);
        //            
        //        } else {
        //            texA.SetPixel(i, j, _Dead);
        //
        //        }


        for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
                    texA.SetPixel(i, j, _Dead);

        texA.Apply(); //copy changes to the GPU


        rt1 = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);

        
        rend = GetComponent<Renderer>();
        
        cellularAutomataShader = Shader.Find("Custom/RenderToTexture_CA");
        ouputTextureShader = Shader.Find("Custom/OutputTexture");

        if(_TickSpeed < 2 || _TickSpeed%2 != 0){
            throw new System.ArgumentException("Tick Speed must be an even positive integer");
        }

    }

   
    void Update()
    {
        
        //set active shader to be a shader that computes the next timestep
        //of the Cellular Automata system
        rend.material.shader = cellularAutomataShader;
        rend.material.SetColor("_Alive", _Alive);
        rend.material.SetColor("_Dead", _Dead);
        
        if (count % _TickSpeed == 0)
        {
            inputTex = texA;
            outputTex = texB;
        }
        else if((count+(int)(_TickSpeed/2)) % _TickSpeed == 0)
        {
            inputTex = texB;
            outputTex = texA;
        }

        if (Input.GetKey("space")) { //undo til here
            int squareSize = 10; // was used to make the pixel added larger. Shader has "_PS" which controls that now.
            // Still need to remove this.
           
            float i = Random.Range(0, height - squareSize);
            float j = Random.Range(0, width - squareSize);

            i = (i) / height; // Normalizing the range
            j = (j) / width;

            rend.material.SetFloat("_PosX",i);
            rend.material.SetFloat("_PosY", j);
            rend.material.SetInt("_Changed",0); // Because shaders are too good for bools apparently
            //Debug.Log("Added pixels above "+ i +" "+ j);

            // Keeping just in case.
            //for (int x = i; x < i + squareSize; x++) {
            //    for (int y = j; y < j + squareSize; y++) {
            //        //inputTex.SetPixel(x, y, _Alive);
            //        //texA.SetPixel(x, y, _Alive);
            //        //texB.SetPixel(x, y, _Alive);
            //        rend.material.SetFloat("_PosX", x);
            //        rend.material.SetFloat("_PosY", y);
            //
            //    }
            //}
        }


        rend.material.SetTexture("_MainTex", inputTex);
        
        

        //source, destination, material
        Graphics.Blit(inputTex, rt1, rend.material);

          

        Graphics.CopyTexture(rt1, outputTex);


        //set the active shader to be a regular shader that maps the current
        //output texture onto a game object
        rend.material.shader = ouputTextureShader;
        rend.material.SetColor("_Alive", _Alive);
        rend.material.SetColor("_Dead", _Dead);
        Debug.Log(_Alive);
        

        rend.material.SetTexture("_MainTex", outputTex);
        rend.material.SetInt("_Changed", 1);


        count++;
    }
}
