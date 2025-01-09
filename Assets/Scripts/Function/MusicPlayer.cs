using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityTimer;

public class MusicPlayer : MonoBehaviour
{
    private static MusicPlayer _instance;
    public static MusicPlayer Instance { get { return _instance; } }

    public AudioSource music;

    public int offset = 0;// 谱面延迟
    public int delay = -200;// 本机延迟
    private int realOffset = 0;
    public int count = 0;
    public float musicTime = 0f;
    public bool isPause = false;

    public bool Timing = false;// 是否正在计时

    void Awake()
    {
        _instance = this;
    }

    public void Init()
    {
        offset = GameController.Instance.LoadChart.chart.META.offset;
        realOffset = offset + delay;
    }


    public void PlayBGM()
    {
        realOffset = offset + delay;

        if (realOffset > 0)
        {
            Timer.Register(
                realOffset / 1000f,
                () =>{ music.time = 0; music.Play(); }
            );
        } 
        else
        {
            music.time = GameController.Instance.Updater.realTime - realOffset / 1000f;
            music.Play(); 
        }

        CalibrationTime();
    }

    public void PauseBGM()
    {
        isPause = true;

        music.Pause();
        musicTime = music.time;
    }

    public void ResumeBGM()
    {
        isPause = false;

        music.time = musicTime;
        music.Play();

        CalibrationTime();
    }

    public async void CalibrationTime()
    {
        if (isPause) return;

        count++;
        if (music.clip.length - music.time < 5) 
        {   
            Timing = true;
            Timer.Register(
                music.clip.length - music.time,
                () =>{ GameEnd(); }
            );
            return; 
        }

        try
        {
            music.time = GameController.Instance.Updater.realTime - realOffset / 1000f;
        }
        catch (System.Exception)
        {
            return;
        }

        Timing = true;
        Timer.Register(
            2,
            () =>{ Timing = false; }
        );
        await UniTask.WaitUntil(() => Timing == false);
        
        CalibrationTime();
    }

    public void GameEnd()
    {
        Debug.Log("Game End");
    }
}
