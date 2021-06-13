using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthEffect : VineEffect
{
    public EarthEffect(Player _controller, Ethereal _ethereal, Color _mainColor, Color _linkColor, int _modelIndex, float _timeInForm, float _cooldown) : base(_controller, _ethereal, _mainColor, _linkColor, _modelIndex, _timeInForm, _cooldown)
    {    

    }

    public override void RetrieveStart()
    {
        if(!isAttached)
            return;

        Pull();
    }
}
