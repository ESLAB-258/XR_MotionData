using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;

public class MotionData : MonoBehaviour
{
    [HideInInspector]
    public string fileName/* = "Assets/CSV/HandData.csv"*/;

    enum E_SPORTS_NAME
    {
        BOWLING = 0,
        GOLF,
        WALKING,
    }
    enum E_GENDER
    {
        MALE = 0,
        FEMALE,
        OTHERS,
    }

    [SerializeField]
    Transform[] devices;

    [SerializeField]
    Text timeText;

    [SerializeField]
    Text[] posTexts;

    [SerializeField]
    Text[] rotTexts;

    [SerializeField]
    E_SPORTS_NAME sports = E_SPORTS_NAME.BOWLING;

    [SerializeField]
    E_GENDER gender = E_GENDER.MALE;

    [SerializeField]
    string generation;

    [SerializeField]
    string age;

    [SerializeField]
    string subjectName;

    [SerializeField]
    string date;

    [SerializeField]
    string actNumber;

    FileStream posRotFs;
    StreamWriter posRotSw;

    FileStream physicsFs;
    StreamWriter physicsSw;

    Quaternion[] prevRotations;
    Vector3[] prevPositions;
    Vector3[] prevVelocities;
    Vector3[] prevAnguarVelocities;

    string timerStr = @"00:00:00.000";
    float totalSeconds;

    bool isUpdating;

    private void OnEnable()
    {
        isUpdating = false;

        prevPositions = new Vector3[devices.Length];
        prevRotations = new Quaternion[devices.Length];
        prevVelocities = new Vector3[devices.Length];
        prevAnguarVelocities = new Vector3[devices.Length];
        prevAnguarVelocities = new Vector3[devices.Length];

        string sportsStr = null;

        switch(sports)
        {
            case E_SPORTS_NAME.BOWLING:
                sportsStr = "Bowling";
                break;
            case E_SPORTS_NAME.GOLF:
                sportsStr = "Golf";
                break;
            case E_SPORTS_NAME.WALKING:
                sportsStr = "Walking";
                break;

        }

        string genderStr = null;

        switch (gender)
        {
            case E_GENDER.MALE:
                genderStr = "Male";
                break;
            case E_GENDER.FEMALE:
                genderStr = "Female";
                break;
            case E_GENDER.OTHERS:
                genderStr = "Other";
                break;

        }

        if (string.IsNullOrEmpty(sportsStr) || string.IsNullOrEmpty(generation) || string.IsNullOrEmpty(genderStr) || string.IsNullOrEmpty(age) || 
            string.IsNullOrEmpty(subjectName) || string.IsNullOrEmpty(date) || string.IsNullOrEmpty(actNumber))
        {
            Debug.LogError("You need to more information for making fileName.");
        }

        fileName = sportsStr + "_" + generation + "_" + genderStr + "_" + age + "_" + subjectName + "_" + date + "_" + actNumber;

        posRotFs = new FileStream("Assets/CSV/" + fileName + ".csv", FileMode.Create, FileAccess.Write);
        posRotSw = new StreamWriter(posRotFs, System.Text.Encoding.UTF8);

        physicsFs = new FileStream("Assets/CSV/" + fileName + "_Physics.csv", FileMode.Create, FileAccess.Write);
        physicsSw = new StreamWriter(physicsFs, System.Text.Encoding.UTF8);

        string posRotSchema = null;
        string physicsSchema = null;

        string[] posRotStr = { "Pos", "Rot" };
        string[] physicsStr = { "Velocity", "AngularVelocity", "Acceleration", "AngularAcceleration" };

        string[] bodyStr = { "HMD", "LeftController", "RightController", "Waist", "LeftWrist", "RightWrist", "LeftFoot", "RightFoot" };

        //isWriting = false;

        posRotSchema += "TimeStamp,";
        physicsSchema += "TimeStamp,";

        for (int i = 0; i < bodyStr.Length; i++)
        {
            for (int j = 0; j < posRotStr.Length; j++)
            {
                posRotSchema += bodyStr[i] + posRotStr[j];

                if (i != bodyStr.Length || j != posRotStr.Length)
                {
                    posRotSchema += ",";
                }
            }

            for (int j = 0; j < physicsStr.Length; j++)
            {
                physicsSchema += bodyStr[i] + physicsStr[j];

                if (i != bodyStr.Length || j != physicsStr.Length)
                {
                    physicsSchema += ",";
                }
            }
        }

        posRotSw.WriteLine(posRotSchema);
        physicsSw.WriteLine(physicsSchema);

        StartCoroutine(CoCollectMotionData());
    }

    IEnumerator CoCollectMotionData()
    {
        while (true)
        {
            timerStr = Timer();

            //if (isUpdating)
            {
                string posRotData = null;
                string physicsData = null;

                int i = 0;

                posRotData += timerStr.ToString() + ",";
                physicsData += timerStr.ToString() + ",";

                timeText.text = timerStr;

                foreach (var device in devices)
                {
                    Vector3 devicePos = device.position;
                    Quaternion deviceRot = device.rotation;

                    Vector3 velocity = (devicePos - prevPositions[i]) / Time.fixedDeltaTime;
                    Vector3 angularVelocity = (deviceRot.eulerAngles - prevRotations[i].eulerAngles) / Time.fixedDeltaTime;
                    Vector3 acceleration = (velocity - prevVelocities[i]) / Time.fixedDeltaTime;
                    Vector3 angularAcceleration = (angularVelocity - prevAnguarVelocities[i]) / Time.fixedDeltaTime;

                    prevPositions[i] = devicePos;
                    prevRotations[i] = deviceRot;
                    prevVelocities[i] = velocity;
                    prevAnguarVelocities[i] = angularVelocity;

                    posRotData += devicePos.ToString("F2") + "," + deviceRot.ToString("F2");

                    physicsData += velocity.ToString("F2") + "," + angularVelocity.ToString("F2") +
                        "," + acceleration.ToString("F2") + "," + angularAcceleration.ToString("F2");

                    //physicsData2 = time + "," + velocity.magnitude.ToString("F2") + "," + angularVelocity.magnitude.ToString("F2") +
                    //    "," + acceleration.magnitude.ToString("F2") + "," + angularAcceleration.magnitude.ToString("F2");

                    if (i != devices.Length)
                    {
                        posRotData += ",";
                        physicsData += ",";
                    }

                    posTexts[i].text = devicePos.ToString("F2");
                    rotTexts[i].text = deviceRot.ToString("F2");

                    i++;
                }

                posRotSw.WriteLine(posRotData);
                physicsSw.WriteLine(physicsData);
                Debug.Log(physicsData);

                isUpdating = false;
            }

            yield return new WaitForFixedUpdate();
        }
    }

    string Timer()
    {
        totalSeconds += Time.fixedDeltaTime;
        TimeSpan timespan = TimeSpan.FromSeconds(totalSeconds);
        string timer = string.Format("{0:00}:{1:00}:{2:00}.{3:000}", timespan.Hours, timespan.Minutes, timespan.Seconds, timespan.Milliseconds);

        return timer;
    }

    void OnApplicationQuit()
    {
        posRotSw.Close();
        posRotFs.Close();

        physicsSw.Close();
        physicsFs.Close();

        StopCoroutine(CoCollectMotionData());
    }
}
