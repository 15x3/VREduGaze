//========= Copyright 2018, HTC Corporation. All rights reserved. ===========
using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;

namespace ViveSR
{
    namespace anipal
    {
        namespace Eye
        {
            public class SRanipal_GazeRaySample_v2 : MonoBehaviour
            {
                public int LengthOfRay = 25;
                [SerializeField] private LineRenderer GazeRayRenderer;
                private static EyeData_v2 eyeData = new EyeData_v2();
                private bool eye_callback_registered = false;
                //增加变量
                private float pupilDiameterLeft, pupilDiameterRight;
                private Vector2 pupilPositionLeft, pupilPositionRight;
                private float eyeOpenLeft, eyeOpenRight;
                private string datasetFilePath;
                private StreamWriter datasetFileWriter;
                private float startTime;
                //增加变量结束
                public event Action<Vector3> CollisionPointEvent;
                //定义事件，以便将原始数据传参给其他脚本

                private void Start()
                {
                    if (!SRanipal_Eye_Framework.Instance.EnableEye)
                    {
                        enabled = false;
                        return;
                    }
                    Assert.IsNotNull(GazeRayRenderer);

                    //
                    startTime = Time.time;
                    string format = "yyyy-MM-dd_HH-mm-ss";
                    string recordTime = System.DateTime.Now.ToString(format);
                    datasetFilePath = "dataset_" + recordTime + ".txt";
                    datasetFileWriter = File.AppendText(Path.Combine(UnityEngine.Application.dataPath, datasetFilePath));
                    UnityEngine.Debug.Log("Dataset file created at: " + Path.Combine(UnityEngine.Application.dataPath, datasetFilePath));
                    UnityEngine.Debug.Log("Recording started at: " + recordTime);
                    //
                }

                private void Update()
                {
                    if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING &&
                        SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT) return;

                    if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == true && eye_callback_registered == false)
                    {
                        SRanipal_Eye_v2.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                        eye_callback_registered = true;
                    }
                    else if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == false && eye_callback_registered == true)
                    {
                        SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                        eye_callback_registered = false;
                    }

                    Vector3 GazeOriginCombinedLocal, GazeDirectionCombinedLocal;

                    if (eye_callback_registered)
                    {
                        if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal, eyeData)) { }
                        else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.LEFT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal, eyeData)) { }
                        else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.RIGHT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal, eyeData)) { }
                        else return;
                    }
                    else
                    {
                        if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal)) { }
                        else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.LEFT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal)) { }
                        else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.RIGHT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal)) { }
                        else return;
                    }

                    Vector3 GazeDirectionCombined = Camera.main.transform.TransformDirection(GazeDirectionCombinedLocal);
                    GazeRayRenderer.SetPosition(0, Camera.main.transform.position);
                    GazeRayRenderer.SetPosition(1, Camera.main.transform.position + GazeDirectionCombined * LengthOfRay);

                    //以下为新增部分
                    //pupil diameter 瞳孔的直径
                    pupilDiameterLeft = eyeData.verbose_data.left.pupil_diameter_mm;
                    pupilDiameterRight = eyeData.verbose_data.right.pupil_diameter_mm;

                    //pupil positions 瞳孔位置
                    //pupil_position_in_sensor_area手册里写的是The normalized position of a pupil in [0,1]，给坐标归一化了
                    pupilPositionLeft = eyeData.verbose_data.left.pupil_position_in_sensor_area;
                    pupilPositionRight = eyeData.verbose_data.right.pupil_position_in_sensor_area;

                    //eye open 睁眼
                    //eye_openness手册里写的是A value representing how open the eye is,也就是睁眼程度，从输出来看是在0-1之间，也归一化了
                    eyeOpenLeft = eyeData.verbose_data.left.eye_openness;
                    eyeOpenRight = eyeData.verbose_data.right.eye_openness;

                    //UnityEngine.Debug.Log("左眼瞳孔直径：" + pupilDiameterLeft + " 左眼位置坐标：" + pupilPositionLeft + "左眼睁眼程度" + eyeOpenLeft);
                    //UnityEngine.Debug.Log("右眼瞳孔直径：" + pupilDiameterRight + " 右眼位置坐标：" + pupilPositionRight + " 左眼睁眼程度" + eyeOpenRight);

                    // 调用Physics.SphereCast进行检测，并返回是否有碰撞产生
                    RaycastHit hit;
                    bool isHit = Physics.SphereCast(Camera.main.transform.position, 0.1f, GazeDirectionCombined.normalized, out hit, LengthOfRay);
                    string timestamp = (Time.time - startTime).ToString();
                    if (isHit)
                    {
                        // 碰撞到物体，返回碰撞点的坐标
                        Vector3 collisionPoint = hit.point;
                        UnityEngine.Debug.Log("相交物体：" + hit.collider.gameObject.name);
                        //UnityEngine.Debug.Log("碰撞点坐标：" + collisionPoint);

                        // 触发事件并传递碰撞点坐标
                        CollisionPointEvent?.Invoke(collisionPoint);

                        // Write the data to the dataset file
                        datasetFileWriter.WriteLine(hit.collider.gameObject.name + "," +
                            collisionPoint + "," +
                            pupilDiameterLeft + "," +
                            pupilDiameterRight + "," +
                            timestamp + "," +
                            hit.collider.gameObject.tag);
                    }
                    else
                    {
                        // 未碰撞到物体
                        UnityEngine.Debug.Log("未发生碰撞");
                    }
                }
                private void Release()
                {
                    if (eye_callback_registered == true)
                    {
                        SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                        eye_callback_registered = false;
                    }
                }
                private static void EyeCallback(ref EyeData_v2 eye_data)
                {
                    eyeData = eye_data;
                }
            }
        }
    }
}
