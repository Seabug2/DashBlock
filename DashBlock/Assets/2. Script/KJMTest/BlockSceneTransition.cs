using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BlockSceneTransition : Singleton
{

    public UnityAction onStageLoad;
    public UnityAction onStageStart;
    [SerializeField] private List<UIBlockControl> uiBlocks = new List<UIBlockControl>();

    protected override void Awake()
    {
        base.Awake();
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            onStageLoad?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            onStageStart?.Invoke();
        }
        
    }


}
