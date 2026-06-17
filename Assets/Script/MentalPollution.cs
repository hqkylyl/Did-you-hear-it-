using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class MentalPollution : MonoBehaviour
{
    [Header("核心设置")]
    public GameObject textPrefab;
    public Canvas targetCanvas;
    public string[] texts;

    [Header("节奏控制")]
    public float initialInterval = 0.5f; // 初始生成的间隔（慢慢出）
    public float minInterval = 0.02f;    // 最快生成的间隔（疯了一样出）
    public float acceleration = 0.05f;   // 每次生成后缩短多少时间（加速度）

    private bool isBursting = false;
    private List<GameObject> spawnedTexts = new List<GameObject>();

    // 由 Fungus 调用：开始爆发
    public void StartPollution()
    {
        if (!isBursting)
        {
            isBursting = true;
            StartCoroutine(SpawnRoutine());
        }
    }

    // 由 Fungus 调用：彻底清空
    public void StopAndClear()
    {
        isBursting = false;
        StopAllCoroutines();
        foreach (var t in spawnedTexts)
        {
            if (t != null) Destroy(t);
        }
        spawnedTexts.Clear();
    }

    IEnumerator SpawnRoutine()
    {
        RectTransform canvasRect = targetCanvas.GetComponent<RectTransform>();
        float currentInterval = initialInterval; // 从初始速度开始

        while (isBursting)
        {
            // 1. 生成文字
            GameObject go = Instantiate(textPrefab, targetCanvas.transform);
            spawnedTexts.Add(go);

            // 2. 设置内容
            TextMeshProUGUI t = go.GetComponent<TextMeshProUGUI>();
            if (t != null && texts.Length > 0)
            {
                t.text = texts[Random.Range(0, texts.Length)];
                // 随机微调颜色，让红色有深有浅，更有血丝感
                t.color = new Color(Random.Range(0.7f, 1f), Random.Range(0f, 0.2f), Random.Range(0f, 0.2f), 1);
            }

            // 3. 随机位置
            RectTransform rt = go.GetComponent<RectTransform>();
            float x = Random.Range(-canvasRect.rect.width / 2.2f, canvasRect.rect.width / 2.2f);
            float y = Random.Range(-canvasRect.rect.height / 2.2f, canvasRect.rect.height / 2.2f);
            rt.anchoredPosition = new Vector2(x, y);

            // 4. 随机旋转和缩放
            rt.localRotation = Quaternion.Euler(0, 0, Random.Range(-30f, 30f));
            rt.localScale = Vector3.one * Random.Range(0.6f, 1.8f);

            // 【核心：加速逻辑】
            yield return new WaitForSeconds(currentInterval);

            // 每次生成后，下次生成的时间变短，直到达到最快极限
            if (currentInterval > minInterval)
            {
                currentInterval -= acceleration;
            }
        }
    }
}