using System.Collections;
using UnityEngine;

public class BlinkFeedback : Feedback
{
    private SpriteRenderer[] _spriteRenderers;
    [SerializeField] private float _delaySecond;
    [SerializeField] private float _blinkValue;

    private bool _isFinished;
    private Coroutine _delayCoroutine = null;

    protected override void Awake()
    {
        base.Awake();
        _spriteRenderers = _owner.GetComponentsInChildren<SpriteRenderer>();
    }

    public override void CreateFeedback()
    {
        _owner.SpriteEffectCompo.SetHitEffect(Color.red, _blinkValue);
        _delayCoroutine = StartCoroutine(SetNormalAfterDelay());
    }

    private IEnumerator SetNormalAfterDelay()
    {
        _isFinished = false;
        yield return new WaitForSeconds(_delaySecond);

        if (_isFinished == false)
        {
            CompleteFeedback();
        }
    }

    public override void CompleteFeedback()
    {
        if (_delayCoroutine != null)
        {
            StopCoroutine(_delayCoroutine);
        }

        _isFinished = true;
        _owner.SpriteEffectCompo.SetHitEffect(Color.red, 0);
    }
}