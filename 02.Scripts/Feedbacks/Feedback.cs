using Unity.Netcode;
using UnityEngine;

public abstract class Feedback : NetworkBehaviour, IFeedback
{
    protected Player _owner;

    protected virtual void Awake()
    {
        _owner = transform.root.GetComponent<Player>();
    }

    public virtual void CreateFeedback()
    {
    }

    public virtual void CompleteFeedback()
    {
    }
}