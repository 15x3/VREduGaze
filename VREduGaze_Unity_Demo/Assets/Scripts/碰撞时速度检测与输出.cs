using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class 碰撞时速度检测与输出 : MonoBehaviour
{
    private Rigidbody rigidbody;
    public UnityEngine.UI.Text debugText;

    private void Start()
    {
        // 获取物体上的刚体组件
        rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 检查是否有刚体被碰撞
        if (collision.rigidbody != null)
        {
            Vector3 velocity = collision.relativeVelocity;
            debugText.text = "速度为 " + velocity;
        }
    }
}