using UnityEngine;
using UnityEngine.UI;

public class Progressbar : MonoBehaviour
{ 
    public Image _glowImage;
    
    private RawImage _image;
    private float _initialWidth;
    private bool _glowActivated;
    private float _time;
    private float _glowSpeed = 10.0f;

    private void Start()
    {
        _image = GetComponentInChildren<RawImage>();
        _initialWidth = _image.rectTransform.rect.width;
        if (_glowImage != null)
        {
            _glowImage.color = new Color(1,1,1,0);   
        }
        _time = 0.0f;
    }

    public void SetRatio(float ratio)
    {
        Rect uv = _image.uvRect;
        RectTransform rectTransform = _image.rectTransform;
        rectTransform.sizeDelta = new Vector2(_initialWidth*ratio,rectTransform.rect.height);
        uv.width = ratio;
        _image.uvRect = uv;
    }

    public void StartGlow()
    {
        if (_glowImage != null)
        {
            _glowActivated = true;
            _time = 0.0f;
        }
    }
    
    void Update()
    {
        if (_glowActivated && _glowImage != null)
        {
            _time += Time.deltaTime * _glowSpeed;
            float alpha = Mathf.Sin(_time) * 0.5f + 0.5f ;
            _glowImage.color = new Color(1,1,1,alpha);

            if (_time > 3.0f * _glowSpeed )
            {
                _glowActivated = false;
                _glowImage.color = new Color(1,1,1,0);
            }
        }
    }
    
}
