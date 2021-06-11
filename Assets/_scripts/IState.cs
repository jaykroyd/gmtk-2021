using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    string Name { get; }
    void OnEnter();
    void OnUpdate();
    void OnExit();
    void TransitionToState(IState _nextState);

}
