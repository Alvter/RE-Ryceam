using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

[System.Serializable]
    public class BPMList
    {
        public float bpm;
        public int[] startTime;
    }

    [System.Serializable]
    public class META
    {
        public int RPEVersion;
        public string background;
        public string charter;
        public string composer;
        public string id;
        public string level;
        public string name;
        public int offset;
        public string song;
    }

    [System.Serializable]
    public class EventLayers
    {
        public List<Event> moveXEvents = new();
        public List<Event> moveYEvents = new();
        public List<Event> moveZEvents = new();
        public List<Event> rotateXEvents = new();
        public List<Event> rotateYEvents = new();
        public List<Event> rotateZEvents = new();
        public List<Event> lengthEvents = new();
        public List<Event> alphaEvents = new();
        public List<Event> speedEvents = new();

    }

    public class Events
    {
        public List<Event> moveXEvents = new();
        public List<Event> moveYEvents = new();
        public List<Event> moveZEvents = new();
        public List<Event> rotateXEvents = new();
        public List<Event> rotateYEvents = new();
        public List<Event> rotateZEvents = new();

    }

    [System.Serializable]
    public class Event
    {
        public int easingType;
        public float end;
        public int[] endTime;
        public int linkgroup;
        public float start;
        public int[] startTime;

        public float st;
        public float et;
        public float set;
        public float se;
        public bool isDone;
    }

    [System.Serializable]
    public class Note
    {
        public int above;
        public float alpha;
        public int[] endTime;
        public int isFake;
        public int positionX;
        public int size;
        public int speed;
        public int[] startTime;
        public int type;
        public int DFlickID;
        public int DFlickType;
        public int visibleTime;
        public int yOffset;

        public float st;
        public float et;
        public float set;
    }

    [System.Serializable]
    public class JudgeLine
    {
        public int Group;
        public string Name;
        public string Texture;
        public int bpmfactor;
        public List<EventLayers> eventLayers;
        public List<Note> notes;
        public int noteTotal;
        public int father;

        public float[] posx;
        public float[] posy;
        public float[] posz;
        public float[] rotx;
        public float[] roty;
        public float[] rotz;
        public float[] alpha;
        public float[] length;
        public float[] speed;

        public float Posx;
        public float Posy;
        public float Posz;
        public float Rotx;
        public float Roty;
        public float Rotz;
        public float Alpha;
        public float Length;
        public float Speed;

        public float lastAlpha;
        public float lastLength;
        public float lastPosx;
        public float lastPosy;
        public float lastPosz;
        public float lastRotx;
        public float lastRoty;
        public float lastRotz;

        public bool isAlpha;
        public bool isLength;
        public bool isMoveX;
        public bool isMoveY;
        public bool isMoveZ;
        public bool isRotx;
        public bool isRoty;
        public bool isRotz;
        
    }

    [System.Serializable]
    public class CameraData
    {
        public float Posx;
        public float Posy;
        public float Posz;
        public float Rotx;
        public float Roty;
        public float Rotz;
    }

    [System.Serializable]
    public class Chart
    {
        public List<BPMList> BPMList;
        public META META;
        public int numOfNotes;
        public string[] judgeLineGroup;
        public List<JudgeLine> judgeLineList;
        public Events cameraEvents = new();
        public CameraData cameraData = new();
        public int camera;
    }

public class LoadChart : MonoBehaviour
{
    

    public TextAsset textAsset;
    public string json;
    public string chartName = "super";
    public Chart chart;

    // 用于存储文件内容的数组
    public List<string> lines = new List<string>();
    public List<string> line = new();
    public int nowLine = 0;

    private static LoadChart _instance;
    public static LoadChart Instance { get { return _instance; } }

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        Load(chartName);
    }

    // void ReadJsonFile()
    // {
    //     textAsset = Resources.Load<TextAsset>("Chart/" + chartName + "/chart");
    //     json = textAsset.text;
    //     chart = JsonConvert.DeserializeObject<Chart>(json);
    // }

    public async void Load(string name)
    {
        _instance = this;

        // 设置屏幕分辨率
        // Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
        // 设置目标帧率
        Application.targetFrameRate = 500;
        // 使应用程序在后台运行
        Application.runInBackground = true;
        // 关闭垂直同步
        QualitySettings.vSyncCount = 0;

        await ReadTextFile();
        // await UniTask.Delay(10000);
        //await loadChart(name);

        GameController.Instance.ScriptStart();

        afterLoad();

    }

    public async Task ReadTextFile()
    {
        textAsset = Resources.Load<TextAsset>("Chart/" + chartName + "/chart");
        string text = textAsset.text;

        //加载音频
        MusicPlayer.Instance.music.clip = Resources.Load<AudioClip>("Chart/" + chartName + "/music");

        await UniTask.RunOnThreadPool(() => {

            lines = text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries).ToList();

            // offset
            line = lines[1].Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries).ToList();
            chart.META.offset = int.Parse(line[1]);

            // numOfNotes
            line = lines[2].Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries).ToList();
            chart.numOfNotes = int.Parse(line[1]);

            // bpmList
            nowLine = 3;
            line = lines[3].Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries).ToList();
            chart.BPMList = new();
            nowLine++;

            //bpm
            for (int i = 0; i < int.Parse(line[1]); i++)
            {
                line = lines[nowLine].Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries).ToList();
                
                BPMList newBPM = new()
                {
                    startTime = beatToArray(float.Parse(line[1])),
                    bpm = float.Parse(line[2])
                };
                chart.BPMList.Add(newBPM);
                nowLine++;
            }

            // trackTotal
            line = lines[nowLine].Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries).ToList();
            int trackTotal = int.Parse(line[1]);
            nowLine++;

            for (int i = 0; i < trackTotal; i++)
            {
                // track
                line = lines[nowLine].Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries).ToList();
                JudgeLine judgeLines = new()
                {
                    father = int.Parse(line[2])
                };
                chart.judgeLineList.Add(judgeLines);
                nowLine++;

                var judgeLine = chart.judgeLineList[i];
                judgeLine.notes = new();

                // noteTotal
                line = lines[nowLine].Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries).ToList();
                judgeLine.noteTotal = int.Parse(line[2]);
                nowLine++;

                // note
                for (int m = 0; m < judgeLine.noteTotal; m++)
                {
                    line = lines[nowLine].Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries).ToList();
                    Note note = new()
                    {
                        type = int.Parse(line[2]),
                        startTime = beatToArray(float.Parse(line[3])),
                        endTime = beatToArray(float.Parse(line[3]) + float.Parse(line[4])),
                        DFlickID = int.Parse(line[5]),
                        DFlickType = int.Parse(line[6]),
                        alpha = float.Parse(line[8]),
                        isFake = int.Parse(line[9])
                    };
                    judgeLine.notes.Add(note);
                    nowLine++;
                }

                // eventLayerTotal
                line = lines[nowLine].Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries).ToList();
                int eventLayersTotal = int.Parse(line[2]);
                judgeLine.eventLayers = new();
                nowLine++;
                
                // eventLayer
                for (int m = 0; m < eventLayersTotal; m++)
                {
                    EventLayers eventLayers = new();
                    judgeLine.eventLayers.Add(eventLayers);

                    // events
                    for (int k = 0; k < 12; k++)
                    {
                        // eventsTotal
                        line = lines[nowLine].Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries).ToList();
                        int type = int.Parse(line[3]);
                        int eventsTotal = int.Parse(line[4]);
                        nowLine++;

                        // event
                        for (int n = 0; n < eventsTotal; n++)
                        {
                            line = lines[nowLine].Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries).ToList();
                            Event events = new()
                            {
                                startTime = beatToArray(float.Parse(line[4])),
                                endTime = beatToArray(float.Parse(line[5])),
                                start = float.Parse(line[6]),
                                end = float.Parse(line[7]),
                                easingType = int.Parse(line[8])
                            };

                            switch (type)
                            {
                                case 0:
                                    judgeLine.eventLayers[m].moveXEvents.Add(events);
                                break;
                                case 1:
                                    judgeLine.eventLayers[m].moveYEvents.Add(events);
                                break;
                                case 2:
                                    judgeLine.eventLayers[m].moveZEvents.Add(events);
                                break;
                                case 3:
                                    judgeLine.eventLayers[m].rotateXEvents.Add(events);
                                break;
                                case 4:
                                    judgeLine.eventLayers[m].rotateYEvents.Add(events);
                                break;
                                case 5:
                                    judgeLine.eventLayers[m].rotateZEvents.Add(events);
                                break;
                                case 6:
                                    judgeLine.eventLayers[m].lengthEvents.Add(events);
                                break;
                                case 7:
                                    judgeLine.eventLayers[m].alphaEvents.Add(events);
                                break;
                                case 8:
                                    judgeLine.eventLayers[m].speedEvents.Add(events);
                                break;
                                default:
                                break;
                            }
                            nowLine++;
                        }
                    }
                }
            }

            // camera
            line = lines[nowLine].Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries).ToList();
            chart.camera = int.Parse(line[1]);
            nowLine++;

            // cameraEvents
            for (int i = 0; i < 6; i++)
            {
                // cameraEventTotal
                line = lines[nowLine].Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries).ToList();
                int type = int.Parse(line[1]);
                int eventsTotal = int.Parse(line[2]);
                nowLine++;

                //event
                for (int n = 0; n < eventsTotal; n++)
                {
                    line = lines[nowLine].Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries).ToList();
                    Event events = new()
                    {
                        startTime = beatToArray(float.Parse(line[2])),
                        endTime = beatToArray(float.Parse(line[3])),
                        start = float.Parse(line[4]),
                        end = float.Parse(line[5]),
                        easingType = int.Parse(line[6])
                    };

                    switch (type)
                    {
                        case 0:
                            chart.cameraEvents.moveXEvents.Add(events);
                        break;
                        case 1:
                            chart.cameraEvents.moveYEvents.Add(events);
                        break;
                        case 2:
                            chart.cameraEvents.moveZEvents.Add(events);
                        break;
                        case 3:
                            chart.cameraEvents.rotateXEvents.Add(events);
                        break;
                        case 4:
                            chart.cameraEvents.rotateYEvents.Add(events);
                        break;
                        case 5:
                            chart.cameraEvents.rotateZEvents.Add(events);
                        break;
                        default:
                        break;
                    }
                    nowLine++;
                }
            }
            
        });
    }

    private int[] beatToArray(float beat)
    {
        return new int[3] {(int)beat,(int)((beat - (int)beat) * 1000),1000};
    }

    private void ReadTextLine(int lineNum)
    {
        nowLine = lineNum;
        string[] line = lines[lineNum].Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
    }

    public async Task loadChart(string name)
    {
        textAsset = Resources.Load<TextAsset>("Chart/" + name + "/chart");
        json = textAsset.text;
        chart = JsonConvert.DeserializeObject<Chart>(json);

        //加载音频
        MusicPlayer.Instance.music.clip = Resources.Load<AudioClip>("Chart/" + name + "/music");

        await UniTask.WaitUntil(() => chart != null);
    }

    public void afterLoad()
    {
        ReLoadChart.Instance.ReLoad(chart);
    }
}
