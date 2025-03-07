using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class PlayerTargetFeedback : NetworkBehaviour
{
    private void PlayTargetHitFeedback(NetworkObjectReference reference)
    {
        if (reference.TryGet(out NetworkObject target, null))
        {
            target.transform.Find("HitFeedback").GetComponent<FeedbackPlayer>().PlayFeedback();
        }
    }

    public void CallPlayTargetHitFeedback(NetworkObjectReference reference)
    {
        PlayTargetHitFeedback(reference);
        PlayTargetHitFeedbackClientRpc(reference);
    }

    // [ServerRpc]
    // private void PlayTargetHitFeedbackServerRpc(ulong targetId)
    // {
    //     PlayTargetHitFeedback(targetId);
    //     PlayTargetHitFeedbackClientRpc(targetId);
    // }

    [ClientRpc]
    private void PlayTargetHitFeedbackClientRpc(NetworkObjectReference reference)
    {
        if (IsOwner) return;
        PlayTargetHitFeedback(reference);
    }
}