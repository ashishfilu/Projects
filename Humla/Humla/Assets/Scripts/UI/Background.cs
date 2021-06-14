using UnityEngine;

public class Background : MonoBehaviour
{
    
    void Start()
    {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Vector3 ambientColor = MissionController.Instance.Mission.Data.GetAmbientColor();
            spriteRenderer.color = new Color(ambientColor.x/255.0f,ambientColor.y/255.0f,ambientColor.z/255.0f);
        }
    }
}
