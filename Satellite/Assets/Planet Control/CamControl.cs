using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CamControl : MonoBehaviour
{
    public float speedH = 2.0f;
    public float speedV = 2.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;
    [SerializeField]
    protected float moveSpeed = 2;
    protected Vector3 dir = Vector3.zero;
    protected Vector2 rotations = Vector2.zero;
    [SerializeField]
    protected Transform camera;

    void Update()
    {
        //yaw += speedH * Input.GetAxis("Mouse X");
        //pitch -= speedV * Input.GetAxis("Mouse Y");

        //transform.eulerAngles = new Vector3(transform.eulerAngles.x + pitch, transform.eulerAngles.y + yaw, transform.eulerAngles.z);
        //float xInput = Input.GetAxis("Horizontal");
        //float zInput = Input.GetAxis("Vertical");
        //Vector3 dir = transform.forward * zInput + transform.right * xInput;

        camera.position += dir * moveSpeed * Time.deltaTime;
        camera.eulerAngles = new Vector3(camera.eulerAngles.x - rotations.y, camera.eulerAngles.y + rotations.x, camera.eulerAngles.z);
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        dir = camera.forward * context.ReadValue<Vector2>().y + camera.right * context.ReadValue<Vector2>().x;
    }
    public void OnRotate(InputAction.CallbackContext context)
    {
        rotations = context.ReadValue<Vector2>();
    }
}
