using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

/*
 * Based on Brackeys' 2D Character Controller https://github.com/Brackeys/2D-Character-Controller
 */

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class CharacterController2D : MonoBehaviour
{
    [Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f; // How much to smooth out the movement
    
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    private Vector3 _velocity = Vector3.zero;
    private Vector2 _direction;
    private static readonly int Direction = Animator.StringToHash("direction");
    private static readonly int Speed = Animator.StringToHash("speed");

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        Move(_direction);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _direction = context.action.ReadValue<Vector2>();
    }
    
    public void Move(Vector2 move)
    {
        // Move the character by finding the target velocity
        Vector3 targetVelocity = move * 10f;
        // And then smoothing it out and applying it to the character
        _rigidbody2D.velocity = Vector3.SmoothDamp(_rigidbody2D.velocity, targetVelocity, ref _velocity,
            movementSmoothing);

        Vector2 direction = _rigidbody2D.velocity;
        
        _animator.SetFloat(Speed, direction.magnitude);
        
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x < 0)
            {
                // Left
                _animator.SetInteger(Direction, 3);
            }
            else
            {
                // Right
                _animator.SetInteger(Direction, 1);
            }
        }
        else
        {
            if (direction.y < 0)
            {
                // Down
                _animator.SetInteger(Direction, 2);
            }
            else
            {
                // Up
                _animator.SetInteger(Direction, 0);
            }
        }
    }
}