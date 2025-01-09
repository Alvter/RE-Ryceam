using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using System.Linq;
using UnityEngine;
using UnityTimer;
using static Function;
using UnityEditor;

public class ReLoadChart : MonoBehaviour
{
    // 实例化
    private static ReLoadChart _instance;
    public static ReLoadChart Instance { get { return _instance; } }

    public void Awake()
    {
        _instance = this;
    }
    
    public void Init()
    {
        //chart = GameController.Instance.LoadChart.chart;
    }

    // 重新加载数据
    public void ReLoad(Chart chart)
    {
        reLoadChart(chart);
        Debug.Log("Reload is over.");
        AfterReLoad();
    }

    public void AfterReLoad()
    {
        // GameController.Instance.Generate.GenerateNote();
        GameController.Instance.Updater.ChangeCondition();
        GameController.Instance.ProcessEvents.StartPlay();
        GameController.Instance.MusicPlayer.PlayBGM();
    }

    // 将数据按时间排序
    public void reLoadChart(Chart chart)
    {
            // for (int i = 0; i < chart.judgeLineList.Count; i++)
            // {
            //     var eventsLayers = chart.judgeLineList[i].eventLayers;
            //     for (int j = 0; j < eventsLayers.Count; j++)
            //     {
            //         eventsLayers[j].moveXEvents.ToList().Sort((event1, event2) => event1.st.CompareTo(event2.st));
            //     }
            // }
               
        // 补速度事件
        loadSpeedEvent(chart);

        // camera事件
        var cameraEvents = chart.cameraEvents;
        var moveXEvent = cameraEvents.moveXEvents;

        for (int k = 0; k < moveXEvent.Count; k++)
        {
            var events = moveXEvent[k];

            events.st = bpmTime(events.startTime);
            events.et = bpmTime(events.endTime);
            events.set = events.et - events.st;
            events.se = events.end - events.start;
        }

        var moveYEvent = cameraEvents.moveYEvents;

        for (int k = 0; k < moveYEvent.Count; k++)
        {
            var events = moveYEvent[k];

            events.st = bpmTime(events.startTime);
            events.et = bpmTime(events.endTime);
            events.set = events.et - events.st;
            events.se = events.end - events.start;
        }

        var moveZEvent = cameraEvents.moveZEvents;

        for (int k = 0; k < moveZEvent.Count; k++)
        {
            var events = moveZEvent[k];

            events.st = bpmTime(events.startTime);
            events.et = bpmTime(events.endTime);
            events.set = events.et - events.st;
            events.se = events.end - events.start;
        }

        var rotatXEvents = cameraEvents.rotateXEvents;
        for (int k = 0; k < rotatXEvents.Count; k++)
        {
            var events = rotatXEvents[k];

            events.st = bpmTime(events.startTime);
            events.et = bpmTime(events.endTime);
            events.set = events.et - events.st;
            events.se = events.end - events.start;
        }

        var rotatYEvents = cameraEvents.rotateYEvents;
        for (int k = 0; k < rotatYEvents.Count; k++)
        {
            var events = rotatYEvents[k];

            events.st = bpmTime(events.startTime);
            events.et = bpmTime(events.endTime);
            events.set = events.et - events.st;
            events.se = events.end - events.start;
        }

        var rotatZEvents = cameraEvents.rotateZEvents;
        for (int k = 0; k < rotatZEvents.Count; k++)
        {
            var events = rotatZEvents[k];

            events.st = bpmTime(events.startTime);
            events.et = bpmTime(events.endTime);
            events.set = events.et - events.st;
            events.se = events.end - events.start;
        }

        // track事件
        for (int i = 0; i < chart.judgeLineList.Count; i++)
        {
            var eventsLayers = chart.judgeLineList[i].eventLayers;
            chart.judgeLineList[i].posx = new float[eventsLayers.Count];
            chart.judgeLineList[i].posy = new float[eventsLayers.Count];
            chart.judgeLineList[i].posz = new float[eventsLayers.Count];
            chart.judgeLineList[i].rotx = new float[eventsLayers.Count];
            chart.judgeLineList[i].roty = new float[eventsLayers.Count];
            chart.judgeLineList[i].rotz = new float[eventsLayers.Count];
            chart.judgeLineList[i].alpha = new float[eventsLayers.Count];
            chart.judgeLineList[i].length = new float[eventsLayers.Count];
            chart.judgeLineList[i].speed = new float[eventsLayers.Count];

            // note预处理
            for (int j = 0; j < chart.judgeLineList[i].notes.Count; j++)
            {
                var note = chart.judgeLineList[i].notes[j];
                note.st = bpmTime(note.startTime);
                note.et = bpmTime(note.endTime);
                note.set = note.et - note.st;
            }
            var noteList = chart.judgeLineList[i].notes;
            // note排序
            for (int j = 0; j < noteList.Count; j++)
            {
                if (chart.judgeLineList[i].notes == null) continue;

                noteList.Sort((note1, note2) => note1.st.CompareTo(note2.st));
                chart.judgeLineList[i].notes = noteList;
            }
            
            // move事件
            for (int j = 0; j < eventsLayers.Count; j++)
            {
                var moveXEvents = eventsLayers[j].moveXEvents;

                for (int k = 0; k < moveXEvents.Count; k++)
                {
                    var events = moveXEvents[k];

                    events.st = bpmTime(events.startTime);
                    events.et = bpmTime(events.endTime);
                    events.set = events.et - events.st;
                    events.se = events.end - events.start;
                }
                moveXEvents.Sort((event1, event2) => event1.st.CompareTo(event2.st));
            }
            for (int j = 0; j < eventsLayers.Count; j++)
            {
                var moveYEvents = eventsLayers[j].moveYEvents;

                for (int k = 0; k < moveYEvents.Count; k++)
                {
                    var events = moveYEvents[k];

                    events.st = bpmTime(events.startTime);
                    events.et = bpmTime(events.endTime);
                    events.set = events.et - events.st;
                    events.se = events.end - events.start;
                }
            }
            for (int j = 0; j < eventsLayers.Count; j++)
            {
                var moveZEvents = eventsLayers[j].moveZEvents;

                for (int k = 0; k < moveZEvents.Count; k++)
                {
                    var events = moveZEvents[k];

                    events.st = bpmTime(events.startTime);
                    events.et = bpmTime(events.endTime);
                    events.set = events.et - events.st;
                    events.se = events.end - events.start;
                }
            }

            // rotate事件
            for (int j = 0; j < eventsLayers.Count; j++)
            {
                var rotateXEvents = eventsLayers[j].rotateXEvents;

                for (int k = 0; k < rotateXEvents.Count; k++)
                {
                    var events = rotateXEvents[k];

                    events.st = bpmTime(events.startTime);
                    events.et = bpmTime(events.endTime);
                    events.set = events.et - events.st;
                    events.se = events.end - events.start;
                }
            }
            for (int j = 0; j < eventsLayers.Count; j++)
            {
                var rotateYEvents = eventsLayers[j].rotateYEvents;

                for (int k = 0; k < rotateYEvents.Count; k++)
                {
                    var events = rotateYEvents[k];

                    events.st = bpmTime(events.startTime);
                    events.et = bpmTime(events.endTime);
                    events.set = events.et - events.st;
                    events.se = events.end - events.start;
                }
            }
            for (int j = 0; j < eventsLayers.Count; j++)
            {
                var rotateZEvents = eventsLayers[j].rotateZEvents;

                for (int k = 0; k < rotateZEvents.Count; k++)
                {
                    var events = rotateZEvents[k];

                    events.st = bpmTime(events.startTime);
                    events.et = bpmTime(events.endTime);
                    events.set = events.et - events.st;
                    events.se = events.end - events.start;
                }
            }

            // alpha事件
            for (int j = 0; j < eventsLayers.Count; j++)
            {
                var alphaEvents = eventsLayers[j].alphaEvents;

                for (int k = 0; k < alphaEvents.Count; k++)
                {
                    var events = alphaEvents[k];

                    events.st = bpmTime(events.startTime);
                    events.et = bpmTime(events.endTime);
                    events.set = events.et - events.st;
                    events.se = events.end - events.start;
                }
            }

            // length事件
            for (int j = 0; j < eventsLayers.Count; j++)
            {
                var lengthEvents = eventsLayers[j].lengthEvents;

                for (int k = 0; k < lengthEvents.Count; k++)
                {
                    var events = lengthEvents[k];

                    events.st = bpmTime(events.startTime);
                    events.et = bpmTime(events.endTime);
                    events.set = events.et - events.st;
                    events.se = events.end - events.start;
                }
            }

            // speed事件
            for (int j = 0; j < eventsLayers.Count; j++)
            {
                var speedEvents = eventsLayers[j].speedEvents;

                for (int k = 0; k < speedEvents.Count; k++)
                {
                    var events = speedEvents[k];

                    events.st = bpmTime(events.startTime);
                    events.et = bpmTime(events.endTime);
                    events.set = events.et - events.st;
                    events.se = events.end - events.start;
                }
                speedEvents.Sort((event1, event2) => event1.st.CompareTo(event2.st));
            }
        }
        return;
    }

    void loadSpeedEvent(Chart chart)
    {
        List<JudgeLine> tempJudgeLineList = chart.judgeLineList;
        for (int i = 0; i < tempJudgeLineList.Count; i++)
        {
            List<Event> speedEvent = chart.judgeLineList[i].eventLayers[0].speedEvents;
            List<Event> tempSpeedList = new();
            Event lastSpeedeEvent = new();

            for (int m = 0; m < speedEvent.Count; m++)
            {
                Event tempSpeedEvent = new();
                tempSpeedList.Add(speedEvent[m]);
                if (m < speedEvent.Count - 1)
                {
                    tempSpeedEvent.startTime = speedEvent[m].endTime;
                    tempSpeedEvent.endTime = speedEvent[m + 1].startTime;
                    tempSpeedEvent.start = speedEvent[m].end;
                    tempSpeedEvent.end = speedEvent[m].end;
                    if (!isEqual(tempSpeedEvent.startTime, tempSpeedEvent.endTime))
                    {
                        tempSpeedList.Add(tempSpeedEvent);
                    }
                }
                int[] lastTime = new int[] { 99999, 0, 1 };

                lastSpeedeEvent.startTime = speedEvent[m].endTime;
                lastSpeedeEvent.endTime = lastTime;
                lastSpeedeEvent.start = speedEvent[m].end;
                lastSpeedeEvent.end = speedEvent[m].end;

            }
            tempSpeedList.Add(lastSpeedeEvent);
            tempJudgeLineList[i].eventLayers[0].speedEvents = tempSpeedList;

        }
        chart.judgeLineList = tempJudgeLineList;
    }

    public static bool isEqual(int[] a, int[] b)//两个数组是否相等
    {
        return a.SequenceEqual(b);
    }
}
