﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPong_CellularAutomata : MonoBehaviour
{

    //public Color _Alive = new Color(1,1,1,1);
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

    static int width;
    static int height;
    public int pointA;
    public int pointB;

    static Renderer rend;
    bool runPixelChange;
    int count = 0;

    void Start()
    {
        //print(SystemInfo.copyTextureSupport);

        width = 128;
        height = 128;

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

    public void AddPixel(){
        float i = Random.Range(0, height);
        float j = Random.Range(0, width);

        i = (i) / height; // Normalizing the range
        j = (j) / width;

        rend.material.SetFloat("_PosX", i);
        rend.material.SetFloat("_PosY", j);
        rend.material.SetInt("_Changed", 0); // Because shaders are too good for bools apparently
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

    //void pixelChange()
    //{
    //
    //    //rend.material.SetFloat("_PosX", i);
    //    //rend.material.SetFloat("_PosY", j);
    //    //rend.material.SetInt("_Changed", 0); // Because shaders are too good for bools apparently
    //    //Debug.Log("Added pixels above "+ i +" "+ j);
    //    
    //    // Keeping just in case.
    //   
    //}

    // undo until here
    IEnumerator pixelChange()
    {
        float i = Random.Range(0, height);
        float j = Random.Range(0, width);
        float squareSize = 6;

        i = (i) / height; // Normalizing the range
        j = (j) / width;
        for (float x = i; x < i + (squareSize/height); x+=(2f/height)) {
            for (float y = j; y < j + (squareSize/width); y+=(2f/width)) {
                //inputTex.SetPixel(x, y, _Alive);
                //texA.SetPixel(x, y, _Alive);
                //texB.SetPixel(x, y, _Alive);
                //Shader old = rend.material.shader;
                //rend.material.shader = cellularAutomataShader;
                Debug.Log("Changed "+x+" "+y );
                rend.material.SetInt("_Changed", 0);
                rend.material.SetFloat("_PosX", x);
                rend.material.SetFloat("_PosY", y);
                
                //rend.material.shader = old;
                yield return new WaitForSeconds(.001f);
            }
        }
    }
    void Update(){
        
        //set active shader to be a shader that computes the next timestep
        //of the Cellular Automata system
        rend.material.shader = cellularAutomataShader;
        //rend.material.SetColor("_Alive", _Alive);
        rend.material.SetColor("_Dead", _Dead);
        Debug.Log("Test 1");
        

       // if (runPixelChange == true && count % _TickSpeed == 0) {
       // }

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
        
        
      //  Debug.Log("_Test "+rend.material.GetColor("_Test"));

        rend.material.SetTexture("_MainTex", inputTex);
       if (Input.GetKeyDown("space")) {
            int temp = Random.Range(0, 7);
            rend.material.SetInt("_colorControl",temp);
            StartCoroutine("pixelChange");
            Debug.Log("Test 2");
        }
        
        if (Input.GetKey("d")) {
            float i = height/2;
            float j = width/2;

            i = (i) / height; // Normalizing the range
            j = (j) / width;

            rend.material.SetFloat("_PosX", i);
            rend.material.SetFloat("_PosY", j);
            rend.material.SetInt("_Changed", 0); // Because shaders are too good for bools apparently
        }

        //source, destination, material
        Graphics.Blit(inputTex, rt1, rend.material);

          

        Graphics.CopyTexture(rt1, outputTex);


        //set the active shader to be a regular shader that maps the current
        //output texture onto a game object
        rend.material.shader = ouputTextureShader;
        //rend.material.SetColor("_Alive", _Alive);
        rend.material.SetColor("_Dead", _Dead);
        //Debug.Log("_Alive "+_Alive);
       
        

        rend.material.SetTexture("_MainTex", outputTex);
        rend.material.SetInt("_Changed", 1);
        

        count++;
    }
}
