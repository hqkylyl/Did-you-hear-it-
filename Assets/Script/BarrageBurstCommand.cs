using UnityEngine;
using Fungus;

[CommandInfo("Custom", "BarrageBurst", "一键触发全屏弹幕爆发")]
//让插件有这个可以选  custom 插件分类，调用的名字，备注
public class BarrageBurstCommand : Command
{
    public override void OnEnter()
    {

        var manager = GameObject.FindObjectOfType<BarrageManager>();
        if (manager != null)
        {
            // 直接调用写好的爆发函数
            
            manager.StartInfiniteBurst();
        }
        Continue(); // 继续执行下一条指令
    }

    public override Color GetButtonColor()
    {
        //在插件里改这个按钮的自定义颜色
        return new Color32(255, 0, 0, 255); 
    }
}