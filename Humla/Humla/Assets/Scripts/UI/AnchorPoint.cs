using UnityEngine;

public enum AnchorType
{
    Center,
    Top,
    TopLeft,
    TopRight,
    Bottom,
    BottomLeft,
    BottomRight,
    Left,
    Right
}

public class AnchorPoint : MonoBehaviour
{
    public AnchorType _type;
   
    
    void Start()
    {
        ClampToAnchor();
    }

    private void ClampToAnchor()
    {
        float worldScreenHeight = Camera.main.orthographicSize * 2f;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        switch (_type)
        {
            case AnchorType.Center:
                gameObject.transform.position = Vector3.zero;
                break;
            case AnchorType.Top:
                gameObject.transform.position = new Vector3(0,worldScreenHeight*0.5f,0.0f);
                break;
            case AnchorType.TopLeft:
                gameObject.transform.position = new Vector3(-0.5f*worldScreenWidth,worldScreenHeight*0.5f,0.0f);
                break;
            case AnchorType.TopRight:
                gameObject.transform.position = new Vector3(0.5f*worldScreenWidth,worldScreenHeight*0.5f,0.0f);
                break;
            case AnchorType.Bottom:
                gameObject.transform.position = new Vector3(0,-0.5f*worldScreenHeight,0.0f);
                break;
            case AnchorType.BottomLeft:
                gameObject.transform.position = new Vector3(-0.5f*worldScreenWidth,-0.5f*worldScreenHeight,0.0f);
                break;
            case AnchorType.BottomRight:
                gameObject.transform.position = new Vector3(0.5f*worldScreenWidth,-0.5f*worldScreenHeight,0.0f);
                break;
            case AnchorType.Left:
                gameObject.transform.position = new Vector3(-0.5f*worldScreenWidth,0.0f,0.0f);
                break;
            case AnchorType.Right:
                gameObject.transform.position = new Vector3(0.5f*worldScreenWidth,0.0f,0.0f);
                break;
        }
    }
}
