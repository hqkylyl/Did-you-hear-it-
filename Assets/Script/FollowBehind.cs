using UnityEngine;
using System.Collections;

public class FollowBehind : MonoBehaviour
{
    [Header("目标引用")]
    public Transform player;

    [Header("偏移设置")]
    public float offsetX = -1.5f; // 负数在左
    public float offsetY = -0.5f; // 负数在下

    void Start()
    {
        // 游戏开始时自动找一次人
        FindPlayerIfNeeded();
    }

    private void FindPlayerIfNeeded()
    {
        if (player == null)
        {
            GameObject amelia = GameObject.Find("Amelia");
            if (amelia != null) player = amelia.transform;
        }
    }

    [ContextMenu("手动触发瞬移")]
    public void TeleportBehind()
    {
        // 执行前再次确认人还在不在（防止切场景或重搭）
        FindPlayerIfNeeded();

        if (player == null)
        {
            Debug.LogError("【严重错误】找不到Amelia");
            return;
        }

        // 强制同步物理坐标
        Physics2D.SyncTransforms();

        // 执行位移
        Vector3 pPos = player.position;
        transform.position = new Vector3(pPos.x + offsetX, pPos.y + offsetY, pPos.z);

        // 修正层级
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        SpriteRenderer playerSr = player.GetComponent<SpriteRenderer>();
        if (sr != null && playerSr != null)
        {
            sr.sortingOrder = playerSr.sortingOrder + 1;
        }

        Debug.Log($"<color=green>母亲瞬移成功！</color> 目标 Amelia 坐标: {pPos}，当前 Alex 坐标: {transform.position}");
    }
}