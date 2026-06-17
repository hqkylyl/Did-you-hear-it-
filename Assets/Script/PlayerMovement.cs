using UnityEngine;
using Fungus;
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 4f;
    public Rigidbody2D rb;
    public Animator animator;
    public Fungus.Flowchart flowchart;

    Vector2 movement;

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // 只要有任何输入，就更新方向参数
        if (movement != Vector2.zero)
        {
            animator.SetFloat("InputX", movement.x);
            animator.SetFloat("InputY", movement.y);
            animator.SetFloat("Speed", movement.sqrMagnitude);
        }
        else
        {
            // 停止时速度设为 0
            animator.SetFloat("Speed", 0);
        }
    }

    void FixedUpdate()
    {
        // 只有当 Fungus 里的 canMove 是 True 时才允许位移
        if (flowchart.GetBooleanVariable("canMove"))
        {
            rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // 如果不能动，强制速度归零，防止滑行
            rb.velocity = Vector2.zero;
        }
    }
}