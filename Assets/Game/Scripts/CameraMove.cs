using UnityEngine;
using LitMotion;
using LitMotion.Extensions;

public class CameraMove : MonoBehaviour
{
    public Transform shopTransform; 
    public Transform baseTransform;
    public float shopDuration = 2f;
    public float baseDuration = 2f;

    public void MoveCameraToShop()
    {
        LMotion.Create(transform.position, shopTransform.position, shopDuration)
            .BindToPosition(transform);

        LMotion.Create(transform.rotation.eulerAngles, shopTransform.rotation.eulerAngles, shopDuration)
            .BindToEulerAngles(transform);
    }

    public void MoveCameraToBase()
    {
        LMotion.Create(transform.position, baseTransform.position, baseDuration)
            .BindToPosition(transform);

        LMotion.Create(transform.rotation.eulerAngles, baseTransform.rotation.eulerAngles, baseDuration)
            .BindToEulerAngles(transform);
    }
}