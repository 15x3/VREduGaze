using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using ViveSR.anipal.Eye;
using System.IO;

public class ScreenPointCalculate : MonoBehaviour
{
    public Camera targetCamera; // 在 Inspector 中指定目标摄像机
    public Vector3 screenPoint; // 声明 screenPoint 公共变量

    private string datasetFilePath;
    private StreamWriter datasetFileWriter;
    private float startTime;

    private void Start()
    {
        if (targetCamera == null)
        {
            UnityEngine.Debug.LogError("请在 Inspector 中指定目标摄像机！");
            return;
        }

        SRanipal_GazeRaySample_v2 gazeRaySample = FindObjectOfType<SRanipal_GazeRaySample_v2>();
        if (gazeRaySample != null)
        {
            gazeRaySample.CollisionPointEvent += OnCollisionPointEvent;
        }

        //
        startTime = Time.time;
        string format = "yyyy-MM-dd_HH-mm-ss";
        string recordTime = System.DateTime.Now.ToString(format);
        datasetFilePath = "screenpoint_" + recordTime + ".txt";
        datasetFileWriter = File.AppendText(Path.Combine(UnityEngine.Application.dataPath, datasetFilePath));
        UnityEngine.Debug.Log("Dataset file created at: " + Path.Combine(UnityEngine.Application.dataPath, datasetFilePath));
        UnityEngine.Debug.Log("Recording started at: " + recordTime);
        //
    }

    private void OnDestroy()
    {
        SRanipal_GazeRaySample_v2 gazeRaySample = FindObjectOfType<SRanipal_GazeRaySample_v2>();
        if (gazeRaySample != null)
        {
            gazeRaySample.CollisionPointEvent -= OnCollisionPointEvent;
        }
    }

    private void OnCollisionPointEvent(Vector3 collisionPoint)
    {
        // 计算碰撞点在屏幕上的坐标
        Vector3 screenPoint = targetCamera.WorldToScreenPoint(collisionPoint);



        string timestamp = (Time.time - startTime).ToString();
        // Write the data to the dataset file
        datasetFileWriter.WriteLine(screenPoint + "," + timestamp);
    }

    void OnDrawGizmos()
    {
        // 使用Gizmos可视化屏幕坐标
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(screenPoint, 0.1f);
    }

}