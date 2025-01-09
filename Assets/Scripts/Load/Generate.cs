using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class Generate : MonoBehaviour
{
    // 实例化
    private static Generate _instance;
    public static Generate Instance { get { return _instance; } }

    public Transform TrackList;
    public GameObject Track;
    public GameObject Camera;
    private Chart chart;
    private int[] index;

    public void Awake()
    {
        _instance = this;
    }

    public void Init()
    {
        chart = GameController.Instance.LoadChart.chart;
        for (int i = 0; i < chart.judgeLineList.Count; i++)
        {
            GameObject track = Instantiate(Track, Vector3.zero, Quaternion.identity, TrackList);

            GameController.Instance.Global.TrackTsforms.Add(track.transform);

            GameController.Instance.Global.TrackSprite = new();
            GameController.Instance.Global.TrackSprites.Add(GameController.Instance.Global.TrackSprite);

            GameController.Instance.Global.TrackImage = new();
            GameController.Instance.Global.TrackImages.Add(GameController.Instance.Global.TrackImage);

            List<Transform> newTransformList = new List<Transform>();
            GameController.Instance.Global.TrackChildTsforms.Add(newTransformList);

            for (int m = 0; m < 5; m++)
            {
                SpriteRenderer sprite = track.transform.GetChild(m).GetComponent<SpriteRenderer>();
                GameController.Instance.Global.TrackSprites[i].Add(sprite);

                Image image = track.transform.GetChild(m).GetComponent<Image>();
                GameController.Instance.Global.TrackImages[i].Add(image);

                Transform transform =  track.transform.GetChild(m).transform;
                GameController.Instance.Global.TrackChildTsforms[i].Add(transform);
            }
        }
        if (chart.camera == -1)
        {
            Camera.transform.localPosition = new Vector3(0, 0, -10);
        }
        else
        {
            Camera.transform.SetParent(TrackList.GetChild(chart.camera));
            Camera.transform.localPosition = Vector3.zero;
            Camera.transform.localEulerAngles = Vector3.zero;
        }

        index = new int[chart.judgeLineList.Count];
        
    }

    public void Updater()
    {
        GenerateNotes();
    }
    public void GenerateNotes()
    {
        for (int i = 0; i < chart.judgeLineList.Count; i++)
        {
            GenerateNote(i);
        }
    }

    // public void GenerateNote(int i)
    // {
    //     if (index[i] >= chart.judgeLineList[i].notes.Count) return;
    //     var note = chart.judgeLineList[i].notes[index[i]];

    //     // if (note.isFake == 1)
    //     // {
    //     //     index[i]++;
    //     //     return;
    //     // }
    //     if (note.st - GameController.Instance.Updater.realTime > 3) return;
    //     GameObject noteObj = null;
    //     switch (note.type)
    //     {
    //         case 0:
    //             noteObj = GameController.Instance.Global.TapPool.Get();
    //         break;
    //         case 1:
    //             noteObj = GameController.Instance.Global.DragPool.Get();
    //         break;
    //         case 2:
    //             if (note.DFlickType == 0) noteObj = GameController.Instance.Global.TapPool.Get();
    //             else noteObj = GameController.Instance.Global.DFlickPool.Get();
    //         break;
    //         case 3:
    //             noteObj = GameController.Instance.Global.HoldPool.Get();
    //         break;
    //         case 4:
    //             noteObj = GameController.Instance.Global.FlickPool.Get();
    //         break;
    //     }
    //     NoteMgr noteMgr = noteObj.AddComponent<NoteMgr>();
    //     noteMgr.Init(note, i, index[i]);
    //     noteObj.transform.SetParent(Global.Instance.TrackChildTsforms[i][4].transform);
    //     //noteObj.transform.SetParent(TrackList.GetChild(i));
    //     noteObj.transform.localPosition = new Vector3(0, 100, 0);
    //     noteObj.transform.localEulerAngles = Vector3.zero;
    //     // noteObj.GetComponent<Image>().color = new Color(1, 1, 1, note.alpha);
    //     if (note.type != 3) noteObj.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, note.alpha);
        
    //     index[i]++;
    //     //GenerateNote(i);
    // }    

    public void GenerateNote(int i)
    {
        // for (int j = 0; j < chart.judgeLineList[i].notes.Count; j++)
        // {
            if (index[i] >= chart.judgeLineList[i].notes.Count) return;
            var note = chart.judgeLineList[i].notes[index[i]];

            if (Function.Instance.noteYpos(i, note.st) > 2.5 * chart.judgeLineList[i].Length * 4) return;
            GameObject noteObj = null;
            switch (note.type)
            {
                case 0:
                    noteObj = GameController.Instance.Global.TapPool.Get();
                break;
                case 1:
                    noteObj = GameController.Instance.Global.DragPool.Get();
                break;
                case 2:
                    if (note.DFlickType == 0) noteObj = GameController.Instance.Global.TapPool.Get();
                    else noteObj = GameController.Instance.Global.DFlickPool.Get();
                break;
                case 3:
                    noteObj = GameController.Instance.Global.HoldPool.Get();
                break;
                case 4:
                    noteObj = GameController.Instance.Global.FlickPool.Get();
                break;
            }
            NoteMgr noteMgr = noteObj.AddComponent<NoteMgr>();
            noteMgr.Init(note, i, index[i]);
            noteObj.transform.SetParent(Global.Instance.TrackChildTsforms[i][4].transform);
            //noteObj.transform.SetParent(TrackList.GetChild(i));
            noteObj.transform.localPosition = new Vector3(0, 100, 0);
            noteObj.transform.localEulerAngles = Vector3.zero;
            // noteObj.GetComponent<Image>().color = new Color(1, 1, 1, note.alpha);
            if (note.type != 3) noteObj.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, note.alpha);
            
            index[i]++;
        // }
        //GenerateNote(i);
    }    
}
