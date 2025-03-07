using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class ChromaticAberrationFeedback : Feedback
{
    [SerializeField] private float _delaySecond;
    [SerializeField] private float _intensity;

    private Coroutine _delayCoroutine = null;
    private bool _isFinished;

    public override void CreateFeedback()
    {
        if (_owner.IsOwner)
        {
            ChromaticAberration();
        }
        else
        {
            ChromaticAberrationClientRPC();
        }
    }


    [ClientRpc]
    private void ChromaticAberrationClientRPC()
    {
        if (!_owner.IsOwner) return;
        ChromaticAberration();
    }

    private void ChromaticAberration()
    {
        VolumeManager.Instance.SetChromaticAberration(_intensity);
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
        if (!_owner.IsOwner) return;

        if (_delayCoroutine != null)
        {
            StopCoroutine(_delayCoroutine);
        }

        VolumeManager.Instance.SetChromaticAberration(0);

        _isFinished = true;
    }
}