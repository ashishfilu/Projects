using UnityEngine;

public class ResizeToScreen : MonoBehaviour
{
    public bool _scaleHorizontal;
    public bool _scaleVertical;
    void Start () 
    {
        Resize();
    }

    void Resize()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;

        float width = sr.sprite.bounds.size.x;
        float height = sr.sprite.bounds.size.y;

        float worldScreenHeight = Camera.main.orthographicSize * 2f;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        if (_scaleHorizontal)
        {
            Vector3 xWidth = transform.localScale;
            xWidth.x = worldScreenWidth / width;
            transform.localScale = xWidth;   
        }

        if (_scaleVertical)
        {
            Vector3 yHeight = transform.localScale;
            yHeight.y = worldScreenHeight / height;
            transform.localScale = yHeight;
        }
    }
}
