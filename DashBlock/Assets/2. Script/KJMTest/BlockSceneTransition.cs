using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class BlockSceneTransition : Singleton
{
    public UnityAction onStageLoad;
    public UnityAction onStageStart;

    [SerializeField] private bool _canIShow = true;

    [SerializeField] private UIBlockControl[] _uiBlocks; 
    [SerializeField] private float activationDelay = 0.05f; 
    private int _inactiveBlockCount = 0;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        foreach (var block in _uiBlocks)
        {
            block.gameObject.SetActive(false); 
        }
    }

    private void Update()
    {
        if (_canIShow)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                _canIShow = false; 
                onStageLoad?.Invoke();
                ShuffleBlocks();
                StartCoroutine(ActivateBlocks());
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                _canIShow = false;
                onStageStart?.Invoke();
            }
        }
    }


    private void ShuffleBlocks()
    {
        for (int i = 0; i < _uiBlocks.Length; i++)
        {
            int randomIndex = Random.Range(i, _uiBlocks.Length);

            var temp = _uiBlocks[i];
            _uiBlocks[i] = _uiBlocks[randomIndex];
            _uiBlocks[randomIndex] = temp;
        }
    }


    IEnumerator ActivateBlocks()
    {
        _inactiveBlockCount = 0; 

        foreach (var block in _uiBlocks)
        {
            block.gameObject.SetActive(true);
            Debug.Log($"Activated Block: {block.name}");

            yield return new WaitForSeconds(activationDelay);
        }
        _canIShow = true;
    }


    public void NotifyUIBlock()
    {
        _inactiveBlockCount++;


        if (_inactiveBlockCount >= _uiBlocks.Length)
        {
            _canIShow = true; 
        }
    }
}
