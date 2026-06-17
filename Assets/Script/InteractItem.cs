using UnityEngine;
using Fungus;

public class InteractItem : MonoBehaviour
{
    [Header("交互配置")]
    public GameObject tipIcon;      // 拖入该物体头上的感叹号 (场景中默认设为显示)
    public Flowchart flowchart;    // 拖入场景里的 Flowchart 物体
    public string targetBlockName;  // 在 Inspector 填入块名，例如 "Check Photo"

    [Header("剧情分支")]
    public string normalBlock;      // 妈妈出来前的普通探索（如 "Check TV"）
    public string eventBlock;       // 妈妈走后的特殊结局（如 "TV_Ending_1"）

    private bool isPlayerIn = false;
    bool hasInteracted = false; // 记录是否交互过
    void Start()
    {
        // 检查引用是否漏填
        if (flowchart == null) Debug.LogWarning(gameObject.name + " 缺少 Flowchart 引用！");
        if (tipIcon == null) Debug.LogWarning(gameObject.name + " 缺少感叹号 TipIcon 引用！");
    }

    void Update()
    {
        // 增加 !hasInteracted 判断
        if (isPlayerIn && Input.GetKeyDown(KeyCode.F) && !hasInteracted)
        {
            hasInteracted = true; // 触发一次后立刻锁死
            flowchart.ExecuteBlock(targetBlockName);
            if (tipIcon != null) tipIcon.SetActive(false); // 隐藏感叹号
        }
        if (isPlayerIn && Input.GetKeyDown(KeyCode.F))
        {
            // 从 Fungus 获取妈妈是否走掉的变量
            bool momLeft = flowchart.GetBooleanVariable("isMomLeft");

            if (!momLeft)
            {
                // 情况 A：妈妈还没出来/没走，正常探索
                Debug.Log("执行普通探索：" + normalBlock);
                flowchart.ExecuteBlock(normalBlock);
            }
            else
            {
                // 情况 B：妈妈交代完任务了，触发特殊结局/对话
                Debug.Log("触发特殊剧情：" + eventBlock);
                flowchart.ExecuteBlock(eventBlock);
                // 这种关键剧情触发后，感叹号消失
                if (tipIcon != null) tipIcon.SetActive(false);
            }
        }

        // 只有玩家在范围内，且按下 F 键
        if (isPlayerIn && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log($"[交互] 玩家在 {gameObject.name} 按下了 F 键");

            if (flowchart != null && !string.IsNullOrEmpty(targetBlockName))
            {
                // 1. 执行 Fungus 对话
                flowchart.ExecuteBlock(targetBlockName);
                Debug.Log($"[成功] 已通知 Fungus 执行块: {targetBlockName}");

                // 2. 对话开始，感叹号消失
                if (tipIcon != null)
                {
                    tipIcon.SetActive(false);
                    Debug.Log($"[UI] {gameObject.name} 的感叹号已隐藏");
                }
            }
            else
            {
                Debug.LogError($"[错误] {gameObject.name} 的 targetBlockName 为空或未挂载 Flowchart！");
            }
        }


    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerIn = true;
            Debug.Log($"[物理] 玩家进入了 {gameObject.name} 的触发区域");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerIn = false;
            Debug.Log($"[物理] 玩家离开了 {gameObject.name} 的触发区域");
        }
    }
}