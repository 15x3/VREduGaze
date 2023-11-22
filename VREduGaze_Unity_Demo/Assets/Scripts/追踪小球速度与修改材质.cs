using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using static System.Net.Mime.MediaTypeNames;

public class ObjectInteraction : MonoBehaviour
{
    public Transform attachingPoint;
    public UnityEngine.UI.Text velocityText;
    public Button releaseButton;

    private bool isObjectAttached = false;
    private GameObject attachedObject;
    private Rigidbody attachedObjectRigidbody;

    private void Update()
    {
        // 检测手柄接触物体
        if (SteamVR_Input.GetStateDown("GrabPinch", SteamVR_Input_Sources.RightHand))
        {
            if (!isObjectAttached)
            {
                RaycastHit hit;
                if (Physics.Raycast(attachingPoint.position, attachingPoint.forward, out hit, Mathf.Infinity))
                {
                    GameObject hitObject = hit.collider.gameObject;
                    // 吸附物体
                    AttachObject(hitObject);
                }
            }
        }

        // 实时读取物体速度
        if (isObjectAttached)
        {
            float velocityMagnitude = attachedObjectRigidbody.velocity.magnitude;
            velocityText.text = "Velocity: " + velocityMagnitude.ToString();
        }
    }

    private void AttachObject(GameObject obj)
    {
        if (obj.GetComponent<Rigidbody>() != null)
        {
            // 将物体吸附到指定位置
            obj.transform.parent = attachingPoint;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;

            attachedObject = obj;
            attachedObjectRigidbody = attachedObject.GetComponent<Rigidbody>();
            isObjectAttached = true;
        }
    }

    public void ReleaseObject()
    {
        if (isObjectAttached)
        {
            // 解除屏幕和物体的关系
            attachedObject.transform.parent = null;
            attachedObject = null;
            attachedObjectRigidbody = null;
            isObjectAttached = false;
        }
    }
}