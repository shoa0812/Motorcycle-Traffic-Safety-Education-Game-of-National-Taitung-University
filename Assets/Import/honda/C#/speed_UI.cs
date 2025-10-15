using System.Collections;
using UnityEngine;
using TMPro; // 引入 TextMeshPro 命名空間

public class Speed_UI : MonoBehaviour
{
    public Rigidbody target; // 要監控速度的剛體
    public float maxSpeed = 0.0f; // 最大速度，用於箭頭旋轉

    public float minSpeedArrowAngle; // 最小速度對應的箭頭角度
    public float maxSpeedArrowAngle; // 最大速度對應的箭頭角度

    [Header("UI")]
    public TMP_Text speedLabel; // 用來顯示速度的 TextMeshPro 元素

    private bool isSpeeding = false; // 記錄是否超速的變數
    private float countdownTime = 3f; // 倒數時間（3秒）
    private float originalCountdownTime = 3f; // 記錄原始倒數時間

    public GameObject warningObject; // 要啟用的物件

    private void Update()
    {
        // 計算速度，將 m/s 轉換為 km/h
        float speed = target.velocity.magnitude * 3.6f;

        // 更新 UI 上的速度顯示
        if (speedLabel != null)
            speedLabel.text = ((int)speed + " km/h"); // 將速度顯示在 TextMeshPro 元素上

        // 檢查是否超速且未進行倒數
        if (speed > 50 && !isSpeeding)
        {
            isSpeeding = true;
            StartCoroutine(DisplayWarningAndPause());

            // 啟用目標物件
            if (warningObject != null)
            {
                warningObject.SetActive(true);
            }
        }
    }

    private IEnumerator DisplayWarningAndPause()
    {
        // 倒數計時
        while (countdownTime > 0)
        {
            yield return new WaitForSeconds(1f);
            countdownTime--;
        }

        // 暫停遊戲
        Time.timeScale = 0;

        // 重置倒數時間和超速狀態，以便在恢復遊戲後重新檢測
        ResetWarning();
    }

    private void ResetWarning()
    {
        // 重置倒數時間和超速標記
        countdownTime = originalCountdownTime;
        isSpeeding = false;
        // 恢復遊戲時間
        Time.timeScale = 1;

        // 停用目標物件
        // if (warningObject != null)
        // {
        //     warningObject.SetActive(false);
        // }
    }

    private void OnGUI()
    {
        if (isSpeeding)
        {
            // 設定警告文字樣式
            GUIStyle warningStyle = new GUIStyle();
            warningStyle.fontSize = 24; // 字體大小
            warningStyle.normal.textColor = Color.red; // 字體顏色
            warningStyle.alignment = TextAnchor.MiddleCenter;

            // 顯示警告訊息和倒數計時在畫面上
            GUI.Label(new Rect(Screen.width / 2 - 100, 50, 200, 50), $"警告：已超過速限50km/h！\n倒數：{countdownTime} 秒", warningStyle);
        }
    }
}
