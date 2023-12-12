using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_CameraMovement : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private Vector3 targetOffset;
    [SerializeField]
    private float movementSpeed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        MoveCamera();
    }

    void MoveCamera()
    {
        //Camera will keep updating and follow where the player is.
        transform.position = Vector3.Lerp(transform.position, target.position + targetOffset, movementSpeed * Time.deltaTime);
    }
}
