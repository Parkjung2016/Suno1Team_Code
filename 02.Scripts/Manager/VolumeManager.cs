using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

public class VolumeManager
{
    private Beautify.Universal.Beautify _beautify;
    public static VolumeManager Instance;

    private Sequence _chromaticAberrationSeq, _bloodScreenSeq;

    public VolumeManager()
    {
        Volume volume = Object.FindObjectOfType<Volume>();
        volume.profile.TryGet(out _beautify);
    }

    public void SetChromaticAberration(float intensity, float duration = .25f)
    {
        if (_chromaticAberrationSeq != null && _chromaticAberrationSeq.IsActive())
        {
            _chromaticAberrationSeq.Kill();
        }


        _chromaticAberrationSeq = DOTween.Sequence();
        _chromaticAberrationSeq.Append(DOTween.To(() => _beautify.chromaticAberrationIntensity.value,
            x => _beautify.chromaticAberrationIntensity.value = x, intensity, duration));
    }

    public void EnableBloodScreen(bool enabled)
    {
        float duration = .25f;

        if (_bloodScreenSeq != null && _bloodScreenSeq.IsActive())
        {
            _bloodScreenSeq.Kill();
        }

        Color color = _beautify.frameColor.value;
        color.a = enabled ? .04f : 0;
        _bloodScreenSeq = DOTween.Sequence();
        _bloodScreenSeq.Append(DOTween.To(() => _beautify.frameColor.value,
            x => _beautify.frameColor.value = x, color, duration));
    }
}