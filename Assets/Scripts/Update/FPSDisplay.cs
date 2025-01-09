using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    private float deltaTime = 0.0f;

    void Update()
    {
        // 计算每帧的时间差，累加到 deltaTime
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        // 设置字体样式
        GUIStyle style = new GUIStyle();

        // 计算显示位置
        Rect rect = new Rect(10, 10, 100, 30);

        // 设置字体大小
        style.fontSize = 24;

        // 设置字体颜色
        style.normal.textColor = Color.white;

        // 计算帧率
        float fps = 1.0f / deltaTime;

        // 显示帧率
        string text = string.Format("{0:0.} FPS", fps);
        GUI.Label(rect, text, style);
    }
}
