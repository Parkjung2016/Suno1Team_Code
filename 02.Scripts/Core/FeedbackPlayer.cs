using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FeedbackPlayer : MonoBehaviour
{
    private List<IFeedback> _feedbackList = new List<IFeedback>();

    private void Awake()
    {
        GetComponents<IFeedback>(_feedbackList);
    }

    public void PlayFeedback()
    {
        foreach (IFeedback feedback in _feedbackList)
        {
            feedback.CompleteFeedback();
            feedback.CreateFeedback();
        }
    }

}