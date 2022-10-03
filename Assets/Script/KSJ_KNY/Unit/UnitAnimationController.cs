using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimationController : MonoBehaviour
{
    Animator _animator;
    
    public void SetAnimator(Animator animator)
    {
        _animator = animator;
    }

    public void Play(string animation)
    {
        if(_animator != null)
        {
            _animator.Play(animation);
            _animator.speed = 1.0f;
        }
    }

    public void Play(string animation, float playTime)
    {
        if (_animator != null)
        {
            _animator.Play(animation);
            _animator.speed = _animator.GetCurrentAnimatorClipInfo(0)[0].clip.length / playTime;
            Debug.Log(_animator.speed);
        }
    }
}
