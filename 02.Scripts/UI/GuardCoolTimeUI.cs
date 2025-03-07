using DG.Tweening;
using Unity.Netcode;
using UnityEngine.UI;

public class GuardCoolTimeUI : NetworkBehaviour
{
    private Image _coolTimeProgressBar;

    private void Awake()
    {
        _coolTimeProgressBar = transform.Find("CoolTime").GetComponent<Image>();
    }

    public void RegisterGuardSkillCooldownEvent(GuardSkill guardSkill)
    {
        guardSkill.OnCooldownEvent += HandleCooldownEvent;
    }

    private void HandleCooldownEvent(float current, float total)
    {
        CooldownServerRPC(current, total);
    }

    [ServerRpc]
    private void CooldownServerRPC(float current, float total)
    {
        _coolTimeProgressBar.fillAmount = current / total;
        CooldownClientRPC(current, total);
    }

    [ClientRpc]
    private void CooldownClientRPC(float current, float total)
    {
        _coolTimeProgressBar.fillAmount = current / total;
    }
}