using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;

public class MotionData : MonoBehaviour
{
    [SerializeField]
    Transform[] devices;

    [SerializeField]
    Text timeText;

    [SerializeField]
    Text[] posTexts;

    [SerializeField]
    Text[] rotTexts;

    [SerializeField]
    Text leftShoeText;

    [SerializeField]
    Text rightShoeText;

    [SerializeField]
    KATXRWalker katXRWalker;

    FileStream posRotFs;
    StreamWriter posRotSw;

    FileStream physicsFs;
    StreamWriter physicsSw;

    FileStream scalarFs;
    StreamWriter scalarSw;

    Quaternion[] prevRotations;
    Vector3[] prevPositions;
    Vector3[] prevVelocities;
    Vector3[] prevAnguarVelocities;

    string timerStr = @"00:00:00.000";
    float totalSeconds;

    string posRotSchema = null;
    string physicsSchema = null;
    string scalarSchema = null;

    private void OnEnable()
    {
        prevPositions = new Vector3[devices.Length];
        prevRotations = new Quaternion[devices.Length];
        prevVelocities = new Vector3[devices.Length];
        prevAnguarVelocities = new Vector3[devices.Length];
        prevAnguarVelocities = new Vector3[devices.Length];

        //StartCoroutine(CoCollectMotionData());
    }

    public void MakeFileName(string sportsStr, string generation, string genderStr, string age, string subjectName, string date, string actNumber)
    {
        if (posRotSw != null && physicsSw != null && scalarSw != null)
        {
            posRotSw.Close();
            posRotFs.Close();

            physicsSw.Close();
            physicsFs.Close();

            scalarSw.Close();
            scalarFs.Close();
        }

        if (string.IsNullOrEmpty(sportsStr) || string.IsNullOrEmpty(generation) || string.IsNullOrEmpty(genderStr) || string.IsNullOrEmpty(age) ||
            string.IsNullOrEmpty(subjectName) || string.IsNullOrEmpty(date) || string.IsNullOrEmpty(actNumber))
        {
            Debug.LogError("You need to more information for making fileName.");
        }

        string fileName = sportsStr + "_" + generation + "_" + genderStr + "_" + age + "_" + subjectName + "_" + date + "_" + actNumber;

        posRotFs = new FileStream("Assets/CSV/" + fileName + ".txt", FileMode.Create, FileAccess.Write);
        posRotSw = new StreamWriter(posRotFs, System.Text.Encoding.UTF8);

        physicsFs = new FileStream("Assets/CSV/" + fileName + "_Physics.txt", FileMode.Create, FileAccess.Write);
        physicsSw = new StreamWriter(physicsFs, System.Text.Encoding.UTF8);

        scalarFs = new FileStream("Assets/CSV/" + fileName + "_Scalar.csv", FileMode.Create, FileAccess.Write);
        scalarSw = new StreamWriter(scalarFs, System.Text.Encoding.UTF8);

        string[] posRotStr = { "Pos", "Rot" };
        string[] physicsStr = { "Velocity", "AngularVelocity", "Acceleration", "AngularAcceleration" };
        string[] axisStr = { "X", "Y", "Z" };

        string[] viveDeviceStr = { "HMD", "LeftController", "RightController", "Waist", "LeftWrist", "RightWrist", "LeftFoot", "RightFoot" };
        string[] treadMillStr = { "LeftShoe2DAxis", "RightShoe2DAxis" };


        //isWriting = false;

        posRotSchema += "TimeStamp,";
        physicsSchema += "TimeStamp,";
        scalarSchema += "TimeStamp,";

        for (int i = 0; i < viveDeviceStr.Length; i++)
        {
            for (int j = 0; j < posRotStr.Length; j++)
            {
                posRotSchema += viveDeviceStr[i] + posRotStr[j];

                if (i != viveDeviceStr.Length || j != posRotStr.Length)
                {
                    posRotSchema += ",";
                }
            }

            for (int j = 0; j < physicsStr.Length; j++)
            {
                scalarSchema += viveDeviceStr[i] + physicsStr[j];

                for (int k = 0; k < axisStr.Length; k++)
                {
                    physicsSchema += viveDeviceStr[i] + physicsStr[j] + axisStr[k];

                    if (k != axisStr.Length)
                    {
                        physicsSchema += ",";
                    }
                }

                if (i != viveDeviceStr.Length)
                {
                    scalarSchema += ",";
                }
            }
        }

        for (int k = 0; k < treadMillStr.Length; k++)
        {
            scalarSchema += treadMillStr[k];

            for (int l = 0; l < axisStr.Length; l++)
            {
                physicsSchema += treadMillStr[k] + axisStr[l];

                if (k != axisStr.Length)
                {
                    physicsSchema += ",";
                }
            }

            if (k != treadMillStr.Length)
            {
                scalarSchema += ",";
            }
        }
    }

    IEnumerator CoCollectMotionData()
    {
        string posRotData = null;
        string physicsData = null;
        string scalarData = null;

        Vector3 devicePos = Vector3.zero;
        Quaternion deviceRot = Quaternion.identity;

        Vector3 velocity = Vector3.zero;
        Vector3 angularVelocity = Vector3.zero;
        Vector3 acceleration = Vector3.zero;
        Vector3 angularAcceleration = Vector3.zero;

        Vector3 leftShoe2dAxis = Vector3.zero;
        Vector3 rightShoe2dAxis = Vector3.zero;

        int i = 0;

        while (true)
        {
            timerStr = Timer();

            i = 0;

            posRotData = null;
            physicsData = null;
            scalarData = null;

            posRotData += timerStr.ToString() + ",";
            physicsData += timerStr.ToString() + ",";
            scalarData += timerStr.ToString() + ",";

            timeText.text = timerStr;

            foreach (var device in devices)
            {
                devicePos = device.position;
                deviceRot = device.rotation;

                velocity = (devicePos - prevPositions[i]) / Time.fixedDeltaTime;
                angularVelocity = (deviceRot.eulerAngles - prevRotations[i].eulerAngles) / Time.fixedDeltaTime;
                acceleration = (velocity - prevVelocities[i]) / Time.fixedDeltaTime;
                angularAcceleration = (angularVelocity - prevAnguarVelocities[i]) / Time.fixedDeltaTime;

                prevPositions[i] = devicePos;
                prevRotations[i] = deviceRot;
                prevVelocities[i] = velocity;
                prevAnguarVelocities[i] = angularVelocity;

                posRotData += devicePos.ToString("F2") + "," + deviceRot.ToString("F2");

                physicsData += velocity.x.ToString("F2") + "," + velocity.y.ToString("F2") + "," + velocity.z.ToString("F2") + "," 
                        + angularVelocity.x.ToString("F2") + "," + angularVelocity.y.ToString("F2") +"," +angularVelocity.z.ToString("F2") +","
                        + acceleration.x.ToString("F2") + "," + acceleration.y.ToString("F2") + "," + acceleration.z.ToString("F2") + "," 
                        + angularAcceleration.x.ToString("F2") + "," + angularAcceleration.y.ToString("F2") + "," + angularAcceleration.z.ToString("F2");

                scalarData += velocity.magnitude.ToString("F2") + "," + angularVelocity.magnitude.ToString("F2") + ","
                            + acceleration.magnitude.ToString("F2") + "," + angularAcceleration.magnitude.ToString("F2");

                if (i != devices.Length)
                {
                    posRotData += ",";
                    physicsData += ",";
                    scalarData += ",";
                }

                posTexts[i].text = devicePos.ToString("F2");
                rotTexts[i].text = deviceRot.ToString("F2");

                i++;
            }

            KATNativeSDK.TreadMillData walkStatus = KATNativeSDK.GetWalkStatus();
            WalkC2ExtraData.extraInfo walkData = WalkC2ExtraData.GetExtraInfoC2(walkStatus);

            leftShoe2dAxis = walkData.lFootSpeed;
            rightShoe2dAxis = walkData.rFootSpeed;

            leftShoeText.text = leftShoe2dAxis.ToString("F2");
            rightShoeText.text = rightShoe2dAxis.ToString("F2");

            physicsData += leftShoe2dAxis.x.ToString("F2") + "," + leftShoe2dAxis.y.ToString("F2") + "," + leftShoe2dAxis.z.ToString("F2") + ","
                        + rightShoe2dAxis.x.ToString("F2") + "," + rightShoe2dAxis.y.ToString("F2") + "," + rightShoe2dAxis.z.ToString("F2") + ",";

            scalarData += leftShoe2dAxis.magnitude.ToString("F2") + "," + rightShoe2dAxis.magnitude.ToString("F2") + ",";

            posRotSw.WriteLine(posRotData);
            physicsSw.WriteLine(physicsData);
            scalarSw.WriteLine(scalarData);

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

        scalarSw.Close();
        scalarFs.Close();

        //StopCoroutine(CoCollectMotionData());
    }
}
