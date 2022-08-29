/*
 * 2022 WraithWinterly
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIHitMarker : MonoBehaviour
{
    private AudioSource _hitSound;
    private Image _texture;

    [SerializeField] private GameManager game;

    private void Start()
    {
        _texture = GetComponent<Image>();
        _hitSound = GetComponent<AudioSource>();
        Color color = _texture.color;
        color.a = 0;
        _texture.color = color;
        OnEnable();
    }

    private void OnEnable()
    {
        Refs.Inst.EventManager?.playerShotEnemy.AddListener(OnShot);
        Refs.Inst.EventManager?.playerShotFriendly.AddListener(OnShotFriendly);
        Refs.Inst.EventManager?.playerKilledEnemy.AddListener(OnKill);
    }

    // Update is called once per frame
    private void Update()
    {
        Color color = _texture.color;
        color.a = Utils.Damp(color.a, 0, 0.5f, Time.deltaTime);
        _texture.color = color;
    }


    public void OnShot()
    {
        ShowHitMarker(Color.white, 1);
    }

    public void OnShotFriendly()
    {
        ShowHitMarker(Color.blue, 0.9f);
    }

    public void OnKill()
    {
        ShowHitMarker(Color.red, 1.1f);
    }

    private void ShowHitMarker(Color color, float pitch = 1)
    {
        _texture.color = Color.red;
        _hitSound.pitch = pitch;
        _texture.color = new(color.r, color.b, color.b, color.a);
        StartCoroutine(PlayShotSound());
    }

    private IEnumerator PlayShotSound()
    {
        yield return new WaitForSeconds(0.055f);
        _hitSound.Play();
    }
}
