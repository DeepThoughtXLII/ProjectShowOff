using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IControllerManager 
{
    public int GetControllerCount();
    public void OnGameStart();

    public void ResetControllers();
}
