using UnityEngine;

public class Updater : MonoBehaviour
{
    // 实例化
    private static Updater _instance;
    public static Updater Instance { get { return _instance; } }

    public bool isStart = false;
    public float realTime = 0f;
    
    void Awake()
    {
        _instance = this;
    }

    public void Init()
    {
        isStart = !isStart;
    }

    public void ChangeCondition()
    {
        isStart = !isStart;
    }

    void Update()
    {
        if (!isStart) return;

        realTime += Time.deltaTime;

        GameController.Instance.Function.Updater();
        GameController.Instance.Generate.Updater();
        GameController.Instance.ProcessEvents.Updater();
    }
}
