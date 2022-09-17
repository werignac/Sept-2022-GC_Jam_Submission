using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Button_Click : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Image _img;
    [SerializeField] private Sprite _default, _pressed;
    [SerializeField] private AudioClip _compressClip, _uncompressClip;
    [SerializeField] private AudioSource _source;
    [SerializeField] private string sceneName;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log(_source.isPlaying);
        _img.sprite = _pressed;
        _source.PlayOneShot(_compressClip);
        while(_source.isPlaying)
        {
            Debug.Log("Playing");
        }
        Debug.Log("Stopped");

        //LoadScene(sceneName);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _img.sprite = _default;
        _source.PlayOneShot(_uncompressClip);
    }

    public void LoadScene(string sceneName)
    {
        // optional - _img.sprite = _presed;
        //_source.PlayOneShot(_compressClip);
        // optional - _img.sprite = _default;
        _source.PlayOneShot(_compressClip);
        SceneManager.LoadScene(sceneName);
    }
}
