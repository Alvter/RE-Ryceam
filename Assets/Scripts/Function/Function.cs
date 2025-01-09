using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Updater;
using static LoadChart;

public class Function : MonoBehaviour
{
    private static Function _instance;
    public static Function Instance { get { return _instance; } }

    private Chart chart;
    private static List<BPMList> bpmList;
    private float time;

    public int[] speedIndex;

    public void Awake()
    {
        _instance = this;
    }

    public void Init()
    {
        chart = LoadChart.Instance.chart;
        bpmList = LoadChart.Instance.chart.BPMList;

        for (int i = 0; i < chart.judgeLineList.Count; i++)
        {
            speedIndex = new int[chart.judgeLineList.Count];
            speedIndex[i] = 0;
        }
    }

    public void Updater()
    {
        time = GameController.Instance.Updater.realTime;
    }

    /*public static float BpmTime(int[] x)
    {
        float time = (x[0] + (float)x[1] / x[2])*60/startGame.bpm;
        return time;

    }*/

    public static float bpmTime(int[] x)
    {

        float beat = x[0] + (float)x[1] / x[2];

        if (bpmList.Count == 1)
        {
            return (float)beat * 60 / (float)bpmList[0].bpm;
        }
        for (int i = 0; i < bpmList.Count; i++)
        {

            float time = 0;

            if (beat >= bpmBeat(i))
            {
                return addTime(beat, time, i);
            }
        }
        return 0;

    }

    public static float addTime(float beat,float time,int i)
    {
        for (int m = 0; m < i; m++)
        {
            time += (bpmBeat(m + 1) - bpmBeat(m)) * (float)60 / bpmList[m].bpm;
        }
        time += (beat - bpmBeat(i)) * (float)60 / bpmList[i].bpm;
        return time;
    }

    public static float bpmBeat(int x)
    {
        return bpmList[x].startTime[0] + (float)bpmList[x].startTime[1] / bpmList[x].startTime[2];
    }

    // public List<Event> speedEvent;
    public float noteYpos(int m,float t)
    {
        List<Event> speedEvent = chart.judgeLineList[m].eventLayers[0].speedEvents;
        int index = speedIndex[m];

        float posy = 0;//y坐标

        for (int i = index; i < speedEvent.Count; i++)
        {
            var evt = speedEvent[i];
            
            var startTime = evt.st;//事件开始时间
            var endTime = evt.et;//事件结束时间

            var startSpeed = evt.start;//事件开始速度
            var endSpeed = evt.end;//事件结束速度

            var noteTime = t;//note开始时间

            if (time > endTime) 
            {
                chart.judgeLineList[m].eventLayers[0].speedEvents.RemoveAt(0);
                i--;
                continue;
            };

            if (time > startTime)
            {
                startTime = time;
                startSpeed = (float)Mathf.Lerp(startSpeed, endSpeed, (time - evt.st) / (evt.et - evt.st));
            }
            if (endTime > noteTime)
            {
                endSpeed = (float)Mathf.Lerp(startSpeed, endSpeed, (noteTime - evt.st) / (evt.et - evt.st));
                endTime = noteTime;
            }
            if (startTime > noteTime) return posy;

            posy += (startSpeed + endSpeed) * (float)(endTime - startTime) / 2;

        }
        return posy;
    }
    
}
