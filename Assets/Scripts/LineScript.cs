// Midificed from LightPulse.cs in hw3

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineScript : MonoBehaviour {

	Renderer rend;
	void Start () {
		rend = GetComponent<Renderer> ();     
        rend.material.shader = Shader.Find("Custom/Line Shader");
	}
	

	void Update () {

		int numPartitions = 256; // partition the "histogram" into 256 segments
		float[] aveMag = new float[numPartitions]; // Each value will be the average of that particular partition... I think
		float partitionIndx = 0;
		int numDisplayedBins = 512 / 2; 

		for (int i = 0; i < numDisplayedBins; i++) 
		{
			if(i < numDisplayedBins * (partitionIndx + 1) / numPartitions){
				aveMag[(int)partitionIndx] += AudioPeer.spectrumData[i] / (512/numPartitions);
			}
			else{
				partitionIndx++;
				i--;
			}
		}

		// For each partition, average the values and store that in the appropriate place in aveMag
		for(int i = 0; i < numPartitions; i++)
		{
			aveMag[i] = (float)0.5 + aveMag[i]*100;
			if (aveMag[i] > 100) {
				aveMag[i] = 100;
			}
		}

		float[] magnitudes = new float[numPartitions];
		magnitudes = rend.material.GetFloatArray("_Magnitudes"); // The magnitudes of the sound, as they were on the previous frame
		if(magnitudes == null){
			magnitudes = new float[numPartitions]; // If this is the first frame, set it to all zeros instead
			rend.material.SetFloat("_NumPartitions", numPartitions); // and we might as well set _NumPartitions here since it won't be updated
		}

		for(int i = 0; i < numPartitions; i++){ // For each partition
			
			// Per-frame jumps in value are limited to +- 0.1
			if(Math.Abs(magnitudes[i] - aveMag[i]) > 0.1f){
				// Might as well overwrite an array we aren't gonna use anymore. We can think of it as "updatedMagnitudes"
				magnitudes[i] = (magnitudes[i]>aveMag[i] ? magnitudes[i]-0.1f : magnitudes[i]+0.1f);
			}
			else{
				magnitudes[i] = aveMag[i];
			}
		}

		rend.material.SetFloatArray("_Magnitudes", magnitudes);

	}

}

