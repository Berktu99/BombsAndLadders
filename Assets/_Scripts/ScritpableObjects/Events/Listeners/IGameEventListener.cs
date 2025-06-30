using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameEventListener<T> 
{
    public void OnEventRaised(T item);
}
