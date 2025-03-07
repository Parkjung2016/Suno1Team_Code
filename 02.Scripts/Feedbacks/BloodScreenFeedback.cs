using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class BloodScreenFeedback : Feedback
{
    [SerializeField] private float _delaySecond;

    private Coroutine _delayCoroutine = null;
    private bool _isFinished;

    public override void CreateFeedback()
    {
        if (_owner.IsOwner)
        {
            BloodScreen();
        }
        else
        {
            BloodScreenClientRPC();
        }
    }


    [ClientRpc]
    private void BloodScreenClientRPC()
    {
        if (!_owner.IsOwner) return;
        BloodScreen();
    }

    private void BloodScreen()
    {
        VolumeManager.Instance.EnableBloodScreen(true);
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

        VolumeManager.Instance.EnableBloodScreen(false);


        _isFinished = true;
    }
}