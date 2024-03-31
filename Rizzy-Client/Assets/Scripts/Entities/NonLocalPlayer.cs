using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NonLocalPlayer : MonoBehaviour
{
    private float elapsed_time = 0f;
    public float time_to_reach_target = 0.075f;
    public float movement_threshold = 0.05f;

    private readonly List<NonLocalPlayerTransformUpdate> future_transform_updates = new List<NonLocalPlayerTransformUpdate>();
    private float square_movement_threshold;
    private NonLocalPlayerTransformUpdate to;
    private NonLocalPlayerTransformUpdate from;
    private NonLocalPlayerTransformUpdate previous;
    private Animator animator;

    public Transform view_object;//Gets set when created

    public bool is_crouching = false;


    // Start is called before the first frame update
    void Start()
    {
        square_movement_threshold = movement_threshold * movement_threshold;
        to = new NonLocalPlayerTransformUpdate(Game.server_tick, transform.position, Vector3.zero);
        from = new NonLocalPlayerTransformUpdate(Game.interpolation_tick, transform.position, Vector3.zero);
        previous = new NonLocalPlayerTransformUpdate(Game.interpolation_tick, transform.position, Vector3.zero);
        animator = transform.GetChild(2).GetComponent<Animator>();
    }

    public void SetCrouchingMode(bool value)
    {
        is_crouching = value;
        animator.SetBool("IsCrouching", is_crouching);
    }
    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < future_transform_updates.Count; i++)
        {
            if(Game.server_tick >= future_transform_updates[i].tick)
            {
                previous = to;
                to = future_transform_updates[i];
                from = new NonLocalPlayerTransformUpdate(Game.interpolation_tick, transform.position, Quaternion.ToEulerAngles(transform.localRotation));

                future_transform_updates.RemoveAt(i);
                i--;
                elapsed_time = 0;
                time_to_reach_target = (to.tick - from.tick) * 4.0f * Time.fixedDeltaTime;
            }
        }

        elapsed_time += Time.deltaTime;
        InterpolatePosition(elapsed_time / time_to_reach_target);
    }

    private void InterpolatePosition(float lerp_amount)
    {
        animator.SetBool("IsMoving", (to.position - previous.position).magnitude > 0.05f);

        transform.position = Vector3.Lerp(from.position, to.position, lerp_amount);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, to.rotation.y * Mathf.Rad2Deg, 0), lerp_amount);
        view_object.localRotation = Quaternion.Lerp(view_object.localRotation, Quaternion.Euler(to.rotation.x * Mathf.Rad2Deg, 0, to.rotation.z * Mathf.Rad2Deg), lerp_amount);
    }

    public void AddPositionToUpdate(uint tick, Vector3 position, Vector3 rotation)
    {
        if (tick <= Game.interpolation_tick)
            return;
        for (int i = 0; i < future_transform_updates.Count; i++)
        {
            if(tick < future_transform_updates[i].tick)
            {
                future_transform_updates.Insert(i, new NonLocalPlayerTransformUpdate(tick, position, rotation));
                return;
            }
        }
        future_transform_updates.Add(new NonLocalPlayerTransformUpdate(tick, position, rotation));
    }
}
