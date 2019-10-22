using UnityEngine;
using UnityEngine.UI;

public class ImageCrossfade : MonoBehaviour
{
    public float minimumTimeForTextDisplay = 30.0f;

    public float transitionTime = 2.0f;

    public float _textDisplayProgress = 0.0f;

    private float _transitionProgress = 0.0f;

    private bool _isTransitioning = false;

    private Sprite _currentText = null;

    private Sprite _nextSprite = null;

    private Image _image = null;

    public void SetSprite(Sprite sprite)
    {
        if (_textDisplayProgress > minimumTimeForTextDisplay)
        {
            _nextSprite = sprite;
            _textDisplayProgress = 0.0f;
            _transitionProgress = 0.0f;
            _isTransitioning = true;
        }
    }

    private void Awake()
    {
        _image = GetComponent<Image>();
        if (!_image)
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
            _currentText = _nextSprite;
            _nextSprite = null;
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 1.0f);
            _isTransitioning = false;
        }
        else if (progress > 1.0f)
        {
            progress -= 1.0f;
            _image.sprite = _nextSprite;
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, Mathf.SmoothStep(0.0f, 1.0f, progress));
        }
        else
        {
            _image.sprite = _currentText;
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, Mathf.SmoothStep(1.0f, 0.0f, progress));
        }
    }
}
