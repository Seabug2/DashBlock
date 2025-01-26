using System.Collections;
using UnityEngine;

public class UIBlockControl : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Vector2 _startPosition;

    [SerializeField] private float _borderPositionY;

    private BlockSceneTransition _blockSceneTransition;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _startPosition = _rectTransform.anchoredPosition;
        _blockSceneTransition = GameObject.FindObjectOfType<BlockSceneTransition>();
        _blockSceneTransition.onStageLoad += OnStageLoad;
        _blockSceneTransition.onStageStart += OnStageStart;
    }

    private void Update()
    {
        if (_rectTransform.position.y <= _borderPositionY)
        {
            CheckOutOfScreen();
        }
    }

    private void CheckOutOfScreen()
    {
        _blockSceneTransition.NotifyUIBlock();
        gameObject.SetActive(false); 
    }

    public void OnStageLoad()
    {
        _rb.isKinematic = true;
        _rectTransform.anchoredPosition = _startPosition;
        _rectTransform.rotation = Quaternion.identity;
    }

    public void OnStageStart()
    {
        Vector2 random = new Vector2(Random.Range(-10f, 10f), Random.Range(30f, 50f));

        _rb.isKinematic = false;
        _rb.gravityScale = 50;

        _rb.AddForce(random * 5, ForceMode2D.Impulse);
        _rb.AddTorque(Random.Range(-1f, 1f), ForceMode2D.Impulse);
    }
}
