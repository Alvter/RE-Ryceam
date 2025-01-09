using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class GameController : MonoBehaviour
{
    // 实例化
    private static GameController _instance;
    public static GameController Instance { get { return _instance; } }

    public LoadChart LoadChart{ get; private set;}
    public Updater Updater{ get; private set;}
    public ProcessEvents ProcessEvents { get; private set; }
    public Generate Generate { get; private set; }
    public ReLoadChart ReLoadChart { get; private set; }
    public Function Function { get; private set; }
    public MusicPlayer MusicPlayer { get; private set; }
    public Global Global { get; private set; }


    public void Awake()
    {
        _instance = this;
    }

    public void ScriptStart()
    {
        LoadChart = LoadChart.Instance;
        Updater = Updater.Instance;
        ProcessEvents = ProcessEvents.Instance;
        Generate = Generate.Instance;
        ReLoadChart = ReLoadChart.Instance;
        Function = Function.Instance;
        MusicPlayer = MusicPlayer.Instance;
        Global = Global.Instance;

        Init();
    }

    public void Init()
    {
        MusicPlayer.Init();
        Function.Init();
        Generate.Init();
        ProcessEvents.Init();
    }
}
