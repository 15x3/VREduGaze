using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 修改小球材质 : MonoBehaviour
{
    private Rigidbody rb;
    private Collider coll;
    private PhysicMaterial physicMaterial;
    private Material material;

    public float frictionCoefficient = 0.6f;
    // Start is called before the first frame update
    void Start()
    {
        // 获取物体上的刚体组件、碰撞器组件以及默认材质和物理材质
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
        material = GetComponent<Renderer>().material;
        physicMaterial = coll.material;
    }

    // Update is called once per frame
    void Update()
    {
        // 检测用户输入，以便在运行时调整摩擦力和材质属性
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            frictionCoefficient += 0.1f; // 增加摩擦力
            material.color = Color.red; // 修改材质颜色为红色
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            frictionCoefficient -= 0.1f; // 减小摩擦力
            material.color = Color.blue; // 修改材质颜色为蓝色
        }

        // 更新刚体组件上的摩擦力属性
        physicMaterial.dynamicFriction = frictionCoefficient;
        physicMaterial.staticFriction = frictionCoefficient;

        // 更新物体的材质
        coll.material = physicMaterial;
        GetComponent<Renderer>().material = material;
    }
}