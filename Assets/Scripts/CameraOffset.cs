using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOffset : MonoBehaviour
{
    [SerializeField]
    GameObject target;

    [SerializeField]
    Vector3 positionOffset;

    [SerializeField]
    Vector3 rotationOffset;

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = target.transform.position + positionOffset;
        transform.rotation = Quaternion.Euler(rotationOffset);
    }
}
