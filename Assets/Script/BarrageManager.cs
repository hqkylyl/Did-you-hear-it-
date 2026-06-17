using UnityEngine;
using TMPro;
using System.Collections;


public class BarrageManager : MonoBehaviour
{

    //弹幕，两个模式，一个简单的，一个是爆发的

    public GameObject barragePrefab;
    //弹幕的预制体 放置的地方

    public Transform container;
    //弹幕的父对象

    public float scrollSpeed = 200f;
    //粉碎粒子特效
    public GameObject shatterParticlePrefab;

    private bool isBursting = false;
    //状态开关，决定当前是否正在持续喷发弹幕

    // --- 兼容性函数：防止 BarrageCommand.cs 报错 ---
    public void StartInfiniteBurst() { StartNormalBurst(); }
    public void BurstAllBarrages() { StartNormalBurst(); }

    // ==========================================
    // 档位 1：普通模式 (散发)
    // ==========================================
    public void StartNormalBurst()
    {
        if (isBursting) StopBurst();
        //防止多个弹幕循环同时运行
        isBursting = true;
        StartCoroutine(NormalRoutine());
    }

    IEnumerator NormalRoutine()
    {
       
        while (isBursting)
        {
           
            string[] words = { "多吃点", "都是为了你好", "听话", "懂点事", "矫情" };
            float waitTime = 0.5f; // 初始时间


            SpawnBarrage(words[Random.Range(0, words.Length)], 50, 70, false);

            // 每次循环都让等待时间缩短一点点，直到 0.1 秒
            waitTime = Mathf.Max(0.1f, waitTime - 0.05f);
            yield return new WaitForSeconds(waitTime);
        }
            
     }
    
    // ==========================================
    // 档位 2：红温模式 (吵架爆发)
    // ==========================================
    public void StartRedBurst()
    {
        if (isBursting) StopBurst();
        isBursting = true;
        StartCoroutine(RedAttackRoutine());
    }

    IEnumerator RedAttackRoutine()
    {
        string[] parentWords = { "反了你了！", "白养你了！", "自私自利！", "长本事了！" };
        // --- 【水龙头 1：起步速度】 ---
        // 数字越大，每两拍之间的间隔越长。
        float interval = 0.15f;

        while (isBursting)
        {
            // --- 【水龙头 2：每一拍生成的数量】 ---
            // 区间随机数
            int countPerTick = Random.Range(1, 6);
            for (int i = 0; i < countPerTick; i++)
            {
                SpawnBarrage(parentWords[Random.Range(0, parentWords.Length)], 80, 130, true);
            }

            // --- 【水龙头 3：加速上限】 ---
            // 原来可能是减 0.01f，最低 0.04f。
            // 改成减 0.005f，最低 0.15f。让它加速得更慢，且最终速度也不会太快。
            interval = Mathf.Max(0.17f, interval - 0.12f);

            scrollSpeed = Mathf.Min(850f, scrollSpeed + 10f); // 移动速度稍微降一点
            yield return new WaitForSeconds(interval);
        }
    }

    // --- 底层生成逻辑 ---
    void SpawnBarrage(string text, float minSize, float maxSize, bool isRedMode)
    {
        if (barragePrefab == null || container == null) return;
        GameObject go = Instantiate(barragePrefab, container);
        //把弹幕预制体“克隆”到场景中
        RectTransform rt = go.GetComponent<RectTransform>();
        TextMeshProUGUI txt = go.GetComponent<TextMeshProUGUI>();

        txt.text = text;
        txt.fontSize = Random.Range(minSize, maxSize);
        //随机字体大小

        // --- 【横向拉开的核心】 ---

        // 给 X 轴一个随机范围
        // 这样它们出生的时候就会前后错开，看起来就像是连贯飞出来的
        //float startX = Random.Range(900f, 1600f);
        //float randomY = Random.Range(-450f, 450f); 
        // 你的原始代码中坐标可能太大或太小了
        float startX = Random.Range(1000f, 1100f);
        float randomY = Random.Range(-400f, 400f);


        rt.anchoredPosition = new Vector2(startX, randomY);

        if (isRedMode)
        {
            txt.color = new Color(Random.Range(0.8f, 1f), 0, 0, 1f); // 鲜红
            rt.localRotation = Quaternion.Euler(0, 0, Random.Range(-15f, 20f)); // 稍微歪斜
        }
        else
        {
            txt.color = new Color(0.5f, 0, 0, 0.7f); // 半透明暗红
            rt.localRotation = Quaternion.identity;
        }

        StartCoroutine(MoveRoutine(rt));
    }

    public void StopBurst()
    {
        isBursting = false;
        StopAllCoroutines();
        scrollSpeed = 200f;
    }

    public void ShatterAllBarrages()
    {
        isBursting = false;
        // 不要在这里用 StopAllCoroutines()，因为它会停掉所有正在移动的弹幕
        // 导致 Destroy 执行时，物体可能已经处于“非活跃”状态

        // 获取当前所有弹幕的快照
        int childCount = container.childCount;

        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = container.GetChild(i);

            // 排除掉不该删的物体
            if (child.GetComponent<BarrageManager>() != null) continue;

            if (shatterParticlePrefab != null)
            {
                // 在弹幕消失的位置生成粒子
                GameObject particle = Instantiate(shatterParticlePrefab, child.position, Quaternion.identity);
                // 确保粒子层级足够高，能穿透黑屏
                var renderer = particle.GetComponent<ParticleSystemRenderer>();
                if (renderer != null) renderer.sortingOrder = 100;

                Destroy(particle, 2f); // 2秒后自动清理粒子，防止卡顿
            }

            Destroy(child.gameObject);
        }
    }

    IEnumerator MoveRoutine(RectTransform rt)
    {
        while (rt != null && rt.anchoredPosition.x > -1400f)
        {
            rt.anchoredPosition += Vector2.left * scrollSpeed * Time.deltaTime;
            yield return null;
        }
        if (rt != null) Destroy(rt.gameObject);
    }
}