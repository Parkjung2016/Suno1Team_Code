using System;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpriteEffect : NetworkBehaviour
{
    private SpriteRenderer[] _sprites;
    private Player _player;
    private readonly int HitEffectBlendHash = Shader.PropertyToID("_HitEffectBlend");
    private readonly int HitEffectColorHash = Shader.PropertyToID("_HitEffectColor");
    private readonly int FadeAmountHash = Shader.PropertyToID("_FadeAmount");

    private GameObject _nameText;
    [SerializeField] private GameObject _guardCanvas;

    private void Awake()
    {
        _nameText = transform.Find("NameText").gameObject;
        _player = GetComponent<Player>();
        _sprites = transform.GetComponentsInChildren<SpriteRenderer>();
    }

    public void SetHitEffect(Color color, float blend)
    {
        for (int i = 0; i < _sprites.Length; i++)
        {
            _sprites[i].material.SetColor(HitEffectColorHash, color);
            _sprites[i].material.SetFloat(HitEffectBlendHash, blend);
        }
    }

    public void CallSetHitEffect(Color color, float blend)
    {
        if (!IsOwner) return;

        SetHitEffectServerRpc(color, blend);
    }

    private void Fade(float value, Action callBack = null)
    {
        if (value <= 0)
        {
            _nameText?.SetActive(true);
            _guardCanvas?.SetActive(true);
        }
        else
        {
            _nameText?.SetActive(false);
            _guardCanvas?.SetActive(false);
        }

        float duration = 3f;
        Sequence seq = DOTween.Sequence();
        for (int i = 0; i < _sprites.Length; i++)
        {
            seq.Join(_sprites[i].material.DOFloat(value, FadeAmountHash, duration));
        }

        seq.Join(_player.EquipmentCompo.GetEquipment<Weapon>(SocketType.HandR).spriteRenderer.material
            .DOFloat(value, FadeAmountHash, duration));
        seq.OnComplete(() => { callBack?.Invoke(); });
    }


    public void CallFade(float fade, Action callBack = null)
    {
        if (!IsOwner) return;

        FadeOutServerRpc(fade);
    }

    [ServerRpc]
    private void FadeOutServerRpc(float fade)
    {
        FadeOutClientRpc(fade);
    }

    [ClientRpc]
    private void FadeOutClientRpc(float fade)
    {
        Fade(fade);
    }

    [ServerRpc]
    private void SetHitEffectServerRpc(Color color, float blend)
    {
        SetHitEffectClientRpc(color, blend);
    }

    [ClientRpc]
    private void SetHitEffectClientRpc(Color color, float blend)
    {
        SetHitEffect(color, blend);
    }
}