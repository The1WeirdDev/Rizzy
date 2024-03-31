using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    protected CharacterController controller;
    public Camera camera;
    public Transform ground_check;
    public float yaw = 0.0f;
    public float pitch = 0.0f;
    private void Awake()
    {
        transform.position = new Vector3(0, 1.76f, -5.0f);
        controller = GetComponent<CharacterController>();
    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
