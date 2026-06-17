using UnityEngine;
using Fungus;

// 这行代码决定了它在 Fungus 菜单里的位置
[CommandInfo("Custom", "SayBarrage", "触发或关闭说教弹幕")]
public class BarrageCommand : Command
{
    [Tooltip("是否开启弹幕")]
    [SerializeField] protected bool active = true;

    [Tooltip("弹幕移动速度")]
    [SerializeField] protected float speed = 200f;

    public override void OnEnter()
    {
        // 寻找场景中的管理器并下达指令
        var manager = GameObject.FindObjectOfType<BarrageManager>();
        if (manager != null)
        {
            manager.scrollSpeed = speed;
            manager.StartInfiniteBurst();
        }
        Continue(); // 让 Fungus 继续执行下一条命令
    }

    public override Color GetButtonColor()
    {
        return new Color32(139, 0, 0, 255); // 在编辑器里显示为暗红色
    }
}