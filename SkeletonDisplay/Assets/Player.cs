using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Player : MonoBehaviour {

    [Range(0, 1)]
    public float time;
    private float timePre;

    Vector3 skeletonBase = Vector3.zero;
    Vector3 spineBase;
    Vector3 shoulderL;
    Vector3 elbowL;
    Vector3 wristL;
    Vector3 shoulderR;
    Vector3 elbowR;
    Vector3 wristR;

    Vector3 wristLV;
    Vector3 wristLA;
    Vector3 elbowLV;
    Vector3 elbowLA;
    Vector3 wristRV;
    Vector3 wristRA;
    Vector3 elbowRV;
    Vector3 elbowRA;

    string[] data;

    public LineRenderer[] skeleton;

    public float skeletonScale = 5;
    public float velocityScale = 5;
    public float accelerationScale = 10f;

    bool loaded = false;

	// Use this for initialization
	void Start () {
        Load("test2.txt");
	}
	
	// Update is called once per frame
	void Update () {
		if(time != timePre || loaded)
        {
            loaded = false;
            float targetTime = Mathf.Lerp((float)GetTime(data[0]), (float)GetTime(data[data.Length - 1]), time);

            int min = 0;
            float dist = (float)GetTime(data[0]);

            for (int i = 0; i < data.Length; i++)
            {
                float t = (float)GetTime(data[i]);
                if (Mathf.Abs(targetTime - t) < dist)
                {
                    dist = Mathf.Abs(targetTime - t);
                    min = i;
                }
            }
            Data2Poses(data[min]);
            UpdateSkeleton();
        }
        timePre = time;
	}

    public void Load(string fileName)
    {
        data = File.ReadAllLines(fileName);
        loaded = true;
    }

    void UpdateSkeleton()
    {
        skeleton[0].SetPositions(new Vector3[] { skeletonBase * skeletonScale, spineBase * skeletonScale });
        skeleton[1].SetPositions(new Vector3[] { skeletonBase * skeletonScale, shoulderL * skeletonScale });
        skeleton[2].SetPositions(new Vector3[] { shoulderL * skeletonScale, elbowL * skeletonScale });
        skeleton[3].SetPositions(new Vector3[] { elbowL * skeletonScale, wristL * skeletonScale });
        skeleton[4].SetPositions(new Vector3[] { skeletonBase * skeletonScale, shoulderR * skeletonScale });
        skeleton[5].SetPositions(new Vector3[] { shoulderR * skeletonScale, elbowR * skeletonScale });
        skeleton[6].SetPositions(new Vector3[] { elbowR * skeletonScale, wristR * skeletonScale });

        skeleton[7].SetPositions(new Vector3[] { elbowL * skeletonScale, elbowL * skeletonScale + elbowLV * velocityScale });
        skeleton[8].SetPositions(new Vector3[] { elbowL * skeletonScale, elbowL * skeletonScale + elbowLA * accelerationScale });
        skeleton[9].SetPositions(new Vector3[] { wristL * skeletonScale, wristL * skeletonScale + wristLV * velocityScale });
        skeleton[10].SetPositions(new Vector3[] { wristL * skeletonScale, wristL * skeletonScale + wristLA * accelerationScale });
        skeleton[11].SetPositions(new Vector3[] { elbowR * skeletonScale, elbowR * skeletonScale + elbowRV * velocityScale });
        skeleton[12].SetPositions(new Vector3[] { elbowR * skeletonScale, elbowR * skeletonScale + elbowRA * accelerationScale });
        skeleton[13].SetPositions(new Vector3[] { wristR * skeletonScale, wristR * skeletonScale + wristRV * velocityScale });
        skeleton[14].SetPositions(new Vector3[] { wristR * skeletonScale, wristR * skeletonScale + wristRA * accelerationScale });
    }

    int GetTime(string d)
    {
        string[] dt = d.Split('\t');
        return (int)(long.Parse(dt[0]));
    }

    void Data2Poses(string d)
    {
        string[] dt = d.Split('\t');
        shoulderR = new Vector3(
            float.Parse(dt[1]),
            float.Parse(dt[2]),
            float.Parse(dt[3]));
        shoulderL = new Vector3(
            float.Parse(dt[4]),
            float.Parse(dt[5]),
            float.Parse(dt[6]));
        spineBase = new Vector3(
            float.Parse(dt[7]),
            float.Parse(dt[8]),
            float.Parse(dt[9]));
        elbowR = new Vector3(
            float.Parse(dt[10]),
            float.Parse(dt[11]),
            float.Parse(dt[12])) + shoulderR;
        elbowRV = new Vector3(
            float.Parse(dt[13]),
            float.Parse(dt[14]),
            float.Parse(dt[15]));
        elbowRA = new Vector3(
            float.Parse(dt[16]),
            float.Parse(dt[17]),
            float.Parse(dt[18]));
        wristR = new Vector3(
            float.Parse(dt[19]),
            float.Parse(dt[20]),
            float.Parse(dt[21])) + elbowR;
        wristRV = new Vector3(
            float.Parse(dt[22]),
            float.Parse(dt[23]),
            float.Parse(dt[24]));
        wristRA = new Vector3(
            float.Parse(dt[25]),
            float.Parse(dt[26]),
            float.Parse(dt[27]));

        elbowL = new Vector3(
            float.Parse(dt[28]),
            float.Parse(dt[29]),
            float.Parse(dt[30])) + shoulderL;
        elbowLV = new Vector3(
            float.Parse(dt[31]),
            float.Parse(dt[32]),
            float.Parse(dt[33]));
        elbowLA = new Vector3(
            float.Parse(dt[34]),
            float.Parse(dt[35]),
            float.Parse(dt[36]));
        wristL = new Vector3(
            float.Parse(dt[37]),
            float.Parse(dt[38]),
            float.Parse(dt[39])) + elbowL;
        wristLV = new Vector3(
            float.Parse(dt[40]),
            float.Parse(dt[41]),
            float.Parse(dt[42]));
        wristLA = new Vector3(
            float.Parse(dt[43]),
            float.Parse(dt[44]),
            float.Parse(dt[45]));
    }
}
