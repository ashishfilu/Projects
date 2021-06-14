using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public string CurrentAnimationName { get; private set; }

    private Animator _animator;

    private void Start()
    {
        _animator = gameObject.GetComponent<Animator>();
    }

    public void OnAnimationStarted(string name)
    {
        CurrentAnimationName = name;
    }

    public void OnAnimationFinished(string name)
    {
        
    }

    public void OnAnimationEventTriggered(string name)
    {
        
    }

    public void PauseCurrentAnimation()
    {
        _animator.SetFloat("SpeedMultiplier",0);
    }
    
    public void UnpauseCurrentAnimation()
    {
        _animator.SetFloat("SpeedMultiplier",1);
    }

    public void SetBoolVariableState(string variable, bool value)
    {
        _animator.SetBool(variable,value);
    }

    public bool GetBoolVariableState(string variable)
    {
        return _animator.GetBool(variable);
    }

    public void PauseAllAnimation(bool pause)
    {
        _animator.enabled = !pause;
    }
}
