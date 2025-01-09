using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityTimer;
using Cysharp.Threading.Tasks;
using UnityEngine.Rendering;

public class ProcessEvents : MonoBehaviour
{
    // 实例化
    private static ProcessEvents _instance;
    public static ProcessEvents Instance { get { return _instance; } }

    private Chart chart;
    public float time = 0;
    private Transform TrackList;
    public Transform mainCamera;

    public void Awake()
    {
        _instance = this;
    }

    public void Init()
    {
        chart = GameController.Instance.LoadChart.chart;
        TrackList = GameController.Instance.Generate.TrackList;
    }

    public void StartPlay()
    {
        StartCoroutine(ProcessEvent());
        StartCoroutine(ApplyData());
    }

    public void Updater()
    {
        time = GameController.Instance.Updater.realTime;
        // ProcessEvent();
        // ChangeData();

        // 主线程执行 ApplyData
        // ApplyData();
    }

    private IEnumerator ProcessEvent()
    {
        while (true)
        {
            yield return null;

            //moveX事件
            processCameraEvent(chart.cameraEvents.moveXEvents, time, ref chart.cameraData.Posx);
            //moveY事件
            processCameraEvent(chart.cameraEvents.moveYEvents, time, ref chart.cameraData.Posy);
            //moveZ事件
            processCameraEvent(chart.cameraEvents.moveZEvents, time, ref chart.cameraData.Posz);
            //rotateX事件
            processCameraEvent(chart.cameraEvents.rotateXEvents, time, ref chart.cameraData.Rotx);
            //rotateY事件
            processCameraEvent(chart.cameraEvents.rotateYEvents, time, ref chart.cameraData.Roty);
            //rotateZ事件
            processCameraEvent(chart.cameraEvents.rotateZEvents, time, ref chart.cameraData.Rotz);

            for (int i = 0; i < chart.judgeLineList.Count; i++)
            {
                var eventsLayers = chart.judgeLineList[i].eventLayers;

                for (int j = 0; j < eventsLayers.Count; j++)
                {
                    //moveX事件
                    processEvent(eventsLayers[j].moveXEvents, time, chart.judgeLineList[i].posx, j);
                    //moveY事件
                    processEvent(eventsLayers[j].moveYEvents, time, chart.judgeLineList[i].posy, j);
                    //moveZ事件
                    processEvent(eventsLayers[j].moveZEvents, time, chart.judgeLineList[i].posz, j);
                    //roteX事件
                    processEvent(eventsLayers[j].rotateXEvents, time, chart.judgeLineList[i].rotx, j);
                    //roteY事件
                    processEvent(eventsLayers[j].rotateYEvents, time, chart.judgeLineList[i].roty, j);
                    //roteZ事件
                    processEvent(eventsLayers[j].rotateZEvents, time, chart.judgeLineList[i].rotz, j);
                    //alpha事件
                    processEvent(eventsLayers[j].alphaEvents, time, chart.judgeLineList[i].alpha, j);
                    //length事件
                    processEvent(eventsLayers[j].lengthEvents, time, chart.judgeLineList[i].length, j);
                }

                var track = chart.judgeLineList[i];
                track.Posx = 0;
                track.Posy = 0;
                track.Posz = 0;
                track.Rotx = 0;
                track.Roty = 0;
                track.Rotz = 0;
                track.Alpha = 0;
                track.Length = track.length[0];
                
                foreach (var x in track.posx) track.Posx += x;
                foreach (var y in track.posy) track.Posy += y;
                foreach (var z in track.posz) track.Posz += z;
                foreach (var x in track.rotx) track.Rotx += x;
                foreach (var y in track.roty) track.Roty += y;
                foreach (var z in track.rotz) track.Rotz += z;
                foreach (var a in track.alpha) track.Alpha += a;
            } 

            for (int i = 0; i < chart.judgeLineList.Count; i++)
            {
                var track = chart.judgeLineList[i];

                if (track.father != -1)
                {
                    var fatherTrack = chart.judgeLineList[track.father];

                    // 获取物体A的位置和旋转角度
                    Vector3 positionA = new Vector3(fatherTrack.Posx, fatherTrack.Posy, fatherTrack.Posz);
                    Quaternion rotationQuaternionA = Quaternion.Euler(fatherTrack.Rotx, fatherTrack.Roty, fatherTrack.Rotz);

                    // 计算物体B的全局位置
                    Vector3 relativePositionB = new Vector3(track.Posx, track.Posy, track.Posz);
                    Vector3 transformedPositionB = rotationQuaternionA * relativePositionB;
                    Vector3 positionB = positionA + transformedPositionB;

                    // 更新物体B的位置
                    track.Posx = positionB.x;
                    track.Posy = positionB.y;
                    track.Posz = positionB.z;

                    // 获取物体B相对于物体A的旋转角度
                    Quaternion relativeRotationB = Quaternion.Euler(track.Rotx, track.Roty, track.Rotz);

                    // 获取父对象（物体A）的全局旋转
                    Quaternion parentRotationA = GameController.Instance.Global.TrackTsforms[track.father].localRotation;

                    // 计算物体B相对于世界坐标系的全局旋转
                    Quaternion globalRotationB = parentRotationA * relativeRotationB;

                    // 将全局旋转转换为欧拉角并更新物体B的旋转角度
                    Vector3 globalEulerAnglesB = globalRotationB.eulerAngles;
                    track.Rotx = globalEulerAnglesB.x;
                    track.Roty = globalEulerAnglesB.y;
                    track.Rotz = globalEulerAnglesB.z;
                }

                if (track.lastPosx != track.Posx) { track.lastPosx = track.Posx; track.isMoveX = true; }
                if (track.lastPosy != track.Posy) { track.lastPosy = track.Posy; track.isMoveY = true; }
                if (track.lastPosz != track.Posz) { track.lastPosz = track.Posz; track.isMoveZ = true; }
                if (track.lastRotx != track.Rotx) { track.lastPosx = track.Rotx; track.isRotx = true; }
                if (track.lastRoty != track.Roty) { track.lastPosy = track.Roty; track.isRoty = true; }
                if (track.lastRotz != track.Rotz) { track.lastPosz = track.Rotz; track.isRotz = true; }
                if (track.lastAlpha != track.Alpha) { track.lastAlpha = track.Alpha; track.isAlpha = true; }
                if (track.lastLength != track.Length) { track.lastLength = track.Length; track.isLength = true; }
            }
        }
    }

    public IEnumerator ApplyData()
    {
        while (true)
        {
            yield return null;

            if (chart.camera != -1)
            {
                mainCamera.localPosition = new Vector3(chart.cameraData.Posx, chart.cameraData.Posy, chart.cameraData.Posz);
                mainCamera.localEulerAngles = new Vector3(chart.cameraData.Rotx, chart.cameraData.Roty, chart.cameraData.Rotz);
            }

            for (int i = 0; i < chart.judgeLineList.Count; i++)
            {
                var track = chart.judgeLineList[i];

                track.isMoveX = track.isMoveY = track.isMoveZ = true;
                track.isRotx = track.isRoty = track.isRotz = true;
                track.isAlpha = track.isLength = true;

                if (track.isMoveX || track.isMoveY || track.isMoveZ)
                {
                    GameController.Instance.Global.TrackTsforms[i].localPosition = new Vector3(track.Posx, track.Posy, track.Posz);
                    track.isMoveX = false;
                    track.isMoveY = false;
                    track.isMoveZ = false;
                }
                if (track.isRotx || track.isRoty || track.isRotz)
                {
                    GameController.Instance.Global.TrackTsforms[i].localEulerAngles = new Vector3(track.Rotx, track.Roty, track.Rotz);
                    track.isRotx = false;
                    track.isRoty = false;
                    track.isRotz = false;
                }
                
                if (track.isAlpha)
                {
                    GameController.Instance.Global.TrackSprites[i][0].color = new Color(1, 1, 1, track.Alpha);
                    GameController.Instance.Global.TrackSprites[i][1].color = new Color(1, 1, 1, track.Alpha);
                    GameController.Instance.Global.TrackSprites[i][2].color = new Color(1, 1, 1, track.Alpha);
                    GameController.Instance.Global.TrackSprites[i][3].color = new Color(1, 1, 1, track.Alpha);    
                    track.isAlpha = false;
                }

                if (track.isLength)
                {
                    Transform childTsform = GameController.Instance.Global.TrackSprites[i][1].transform;
                    childTsform.localScale = new Vector3(1, track.Length * 4, 1);
                    track.isLength = false;
                }

                SortingGroup sortingGroup = GameController.Instance.Global.TrackTsforms[i].GetComponent<SortingGroup>();
                sortingGroup.sortingOrder = (int)-Mathf.Abs(track.Posz - mainCamera.position.z);
            }
        }
    }

    private void processCameraEvent(List<Event> evt, float time, ref float data)
    {
        for (int k = 0; k < evt.Count; k++)
        {
            var events = evt[k];

            if (events.isDone) continue;
            if (time < events.st) break;
            
            if (time > events.et)
            {
                data = events.end;
                events.isDone = true;
            }
            else
            {
                data = events.start +
                       events.se * 
                       EasingType.Ease(events.easingType, (time - events.st) / events.set);
            }
            
        }
    }

    private void processEvent(List<Event> evt, float time, float[] targetArray, int j)
    {
        if (evt.Count == 0) return;
        var _event = evt[0];

        if (time < _event.st) return;
        
        if (time > _event.et)
        {
            targetArray[j] = _event.end;
            evt.RemoveAt(0);
        }
        else
        {
            targetArray[j] = _event.start +
                             _event.se * 
                             EasingType.Ease(_event.easingType, (time - _event.st) / _event.set);
        }
    }
}

// public void ApplyData()
//     {
//         if (chart.camera != -1)
//         {
//             mainCamera.localPosition = new Vector3(chart.cameraData.Posx, chart.cameraData.Posy, chart.cameraData.Posz);
//             mainCamera.localEulerAngles = new Vector3(chart.cameraData.Rotx, chart.cameraData.Roty, chart.cameraData.Rotz);
//         }

//         for (int i = 0; i < chart.judgeLineList.Count; i++)
//         {
//             var track = chart.judgeLineList[i];

//             GameController.Instance.Global.TrackTsforms[i].localPosition = new Vector3(track.Posx, track.Posy, track.Posz);
//             GameController.Instance.Global.TrackTsforms[i].localEulerAngles = new Vector3(track.Rotx, track.Roty, track.Rotz);

//             SortingGroup sortingGroup = GameController.Instance.Global.TrackTsforms[i].GetComponent<SortingGroup>();
//             sortingGroup.sortingOrder = (int)-Mathf.Abs(track.Posz - mainCamera.position.z);

//             for (int j = 0; j < 5; j++)
//             {
//                 // SpriteRenderer child = GameController.Instance.Global.TrackSprites[i][j];
//                 // child.color = new Color(1, 1, 1, track.Alpha);

//                 // if (j != 2) continue;
//                 // Transform childTsform = child.transform;
//                 // childTsform.localScale = new Vector3(1, track.Length * 4, 1);
//                 SpriteRenderer child = GameController.Instance.Global.TrackSprites[i][j];
//                 //Image child = GameController.Instance.Global.TrackImages[i][j];
//                 if (j != 4) child.color = new Color(1, 1, 1, track.Alpha);
            
//                 if (j == 1)
//                 {
//                     // RectTransform childTsform = child.rectTransform;
//                     //childTsform.sizeDelta = new Vector2(childTsform.sizeDelta.x, track.Length * 10);
//                     Transform childTsform = child.transform;
//                     childTsform.localScale = new Vector3(1, track.Length * 4, 1);
//                 }
//                 // else if (j == 4)
//                 // {
//                 //     // RectTransform childTsform = child.rectTransform;
//                 //     // Transform childTsform = child.transform;
//                 //     // Transform childTsform = GameController.Instance.Global.TrackChildTsforms[i][j];
//                 //     // childTsform.localScale = new Vector3(1, track.Length * 5, 1);
//                 //     // if (track.Length == 0) childTsform.localScale = new Vector3(1, 0.001f, 1);
//                 //     // Image childs = GameController.Instance.Global.TrackImages[i][j];
//                 //     // RectTransform childTsform = childs.rectTransform;
//                 //     // childTsform.sizeDelta = new Vector2(childTsform.sizeDelta.x, track.Length * 12);
//                 // }
                
//                 //childTsform.localScale = new Vector3(1, track.Length * 4, 1);

                
//             }
//             // Debug.Log(GameController.Instance.Global.TrackSprites.Count + "," + GameController.Instance.Global.TrackSprites[i].Count);
//             // for (int j = 0; j < 4; j++)
//             // {
//             //     var child = TrackList.GetChild(i).GetChild(j);
//             //     child.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, track.Alpha);
//             // }
//             // foreach (var child in GameController.Instance.Global.TrackSprites[i])
//             // {
//             //     child.color = new Color(1, 1, 1, track.Alpha);
//             // }
//         }
//     }
