using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 粗糙球摩擦力模拟 : MonoBehaviour
{
    public float frictionCoefficient = 0.5f;

    private void OnCollisionStay(Collision collision)
    {
        // 确保只对特定平面施加力
        if (collision.gameObject.CompareTag("FrictionPlane"))
        {
            // 计算施加的反方向力的大小
            Vector3 frictionForce = -collision.relativeVelocity * frictionCoefficient;

            // 施加力到刚体上
            GetComponent<Rigidbody>().AddForce(frictionForce, ForceMode.Force);
        }
    }
}
