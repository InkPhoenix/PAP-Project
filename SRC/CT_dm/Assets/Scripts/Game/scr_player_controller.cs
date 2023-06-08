using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class scr_player_controller : MonoBehaviour
{
    private CharacterController controller;

    private Vector3 movement;
    private Vector3 modified_movement;
    private bool is_movement_pressed;
    private float player_speed = 4.0f;
    private float rotation_factor_per_frame = 10f; //how fast the player rotates

    private float grounded_gravity = -0.05f;          //gravity when on the ground
    private float player_gravity = Physics.gravity.y; //gravity when off the ground
    private float terminal_velocity = -15.0f;

    private float max_jump_height = 2.0f;
    private float max_jump_time = 0.75f;
    private bool is_jumping = false;
    private float initial_jump_velocity;
    private int max_jumps = 2;
    private int jumps = 0;
    private float double_jump_multiplier = 0.8f; //force multiplier for jumps after the first one

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        //setup jump variables
        float time_to_apex = max_jump_time / 2;
        player_gravity = (-2 * max_jump_height) / Mathf.Pow(time_to_apex, 2);
        initial_jump_velocity = (2 * max_jump_height) / time_to_apex;
    }

    public void moving(InputAction.CallbackContext context)
    {
        if (context.performed) //key pressed
        {
            movement.x = context.ReadValue<Vector2>().x * player_speed;
            movement.z = context.ReadValue<Vector2>().y * player_speed;

            if (movement.x != 0 || movement.z != 0) { is_movement_pressed = true; }
            else { is_movement_pressed = false; }
        }

        if (context.canceled) //key released
        {
            movement.x = context.ReadValue<Vector2>().x;
            movement.z = context.ReadValue<Vector2>().y;

            if (movement.x != 0 || movement.z != 0) { is_movement_pressed = true; }
            else { is_movement_pressed = false; }
        }
    }

    private void playerRotation()
    {
        Vector3 pos_to_look_at;

        pos_to_look_at.x = movement.x;
        pos_to_look_at.y = 0f;
        pos_to_look_at.z = movement.z;

        Quaternion current_rotation = transform.rotation;

        if (is_movement_pressed)
        {
            Quaternion target_rotation = Quaternion.LookRotation(pos_to_look_at);
            transform.rotation = Quaternion.Slerp(current_rotation, target_rotation, rotation_factor_per_frame * Time.deltaTime);
        }
    }

    private void playerGravity()
    {
        bool is_falling = movement.y <= 0.0f || !is_jumping;
        float fall_multiplier = 2.0f;

        if (controller.isGrounded)
        {
            movement.y = grounded_gravity;
            modified_movement.y = grounded_gravity;
        }
        else if(is_falling)
        {
            float previous_Y_velocity = movement.y;
            movement.y = movement.y + (player_gravity * fall_multiplier * Time.deltaTime);
            modified_movement.y = Mathf.Max((previous_Y_velocity + movement.y) * 0.5f, terminal_velocity);
        }
        else
        {
            float previous_Y_velocity = movement.y;
            movement.y = movement.y + (player_gravity * Time.deltaTime);
            modified_movement.y = (previous_Y_velocity + movement.y) * 0.5f;
        }
    }

    public void jump(InputAction.CallbackContext context)
    {
        if (context.started) //key pressed
        {
            if (jumps != max_jumps)
            {
                jumps++;
                is_jumping = context.ReadValueAsButton();
                if (jumps > 1)
                {
                    movement.y = (initial_jump_velocity) * double_jump_multiplier;
                    modified_movement.y = (initial_jump_velocity) * double_jump_multiplier;
                }
                else
                {
                    movement.y = initial_jump_velocity;
                    modified_movement.y = initial_jump_velocity;
                }
            }
        }

        if (context.canceled) { is_jumping = context.ReadValueAsButton(); } //key released
    }

    void Update()
    {
        //Debug.Log($"is moving?: {is_movement_pressed} || movement: {movement} || grounded gravity {grounded_gravity} || is jumping? = {is_jumping} || jumps: {jumps} || controller.isGrounded: {controller.isGrounded}");
        playerRotation();

        modified_movement.x = movement.x;
        modified_movement.z = movement.z;
        controller.Move(modified_movement * Time.deltaTime);

        playerGravity();
        if (controller.isGrounded) { jumps = 0; }
    }
}