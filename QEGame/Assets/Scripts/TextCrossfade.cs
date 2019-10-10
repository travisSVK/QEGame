using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextCrossfade : MonoBehaviour
{
    public float minimumTimeForTextDisplay = 30.0f;

    public float transitionTime = 2.0f;

    public float _textDisplayProgress = 0.0f;

    private float _transitionProgress = 0.0f;

    private bool _isTransitioning = false;

    private string _currentText = "";

    private string _nextText = "";

    private Text _text = null;

    public void SetText(string text)
    {
        if (_textDisplayProgress > minimumTimeForTextDisplay)
        {
            _nextText = text;
            _textDisplayProgress = 0.0f;
            _transitionProgress = 0.0f;
            _isTransitioning = true;
        }
    }

    private void Awake()
    {
        _text = GetComponent<Text>();
        if (!_text)
        {
            Debug.LogError("Missing Text Component.");
        }
    }

    private void Update()
    {
        _textDisplayProgress += Time.deltaTime;

        if (!_isTransitioning)
        {
            return;
        }

        _transitionProgress += Time.deltaTime;
        float progress = (_transitionProgress / transitionTime) * 2.0f;
        if (progress >= 2.0f)
        {
            _currentText = _nextText;
            _nextText = "";
            _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, 1.0f);
            _isTransitioning = false;
        }
        else if (progress > 1.0f)
        {
            progress -= 1.0f;
            _text.text = _nextText;
            _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, Mathf.SmoothStep(0.0f, 1.0f, progress));
        }
        else
        {
            _text.text = _currentText;
            _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, Mathf.SmoothStep(1.0f, 0.0f, progress));
        }
    }
}
