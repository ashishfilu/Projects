using UnityEngine;

public class ColorGenerator : MonoBehaviour
{
    public Gradient _gradient;
    public Material _terrainMaterial;
    public float _normalOffsetWeight;
    public float _boundY;

    private Texture2D _texture;
    private static int _textureResolution = 50;
    
    // Start is called before the first frame update
    void Start()
    {
        if (_texture == null)
        {
            _texture = new Texture2D(_textureResolution,1,TextureFormat.RGBA32 ,false);
        }
        UpdateTexture();
    }

    // Update is called once per frame
    void Update()
    {
        _terrainMaterial.SetFloat ("boundsY", _boundY);
        _terrainMaterial.SetFloat ("normalOffsetWeight", _normalOffsetWeight);
        _terrainMaterial.SetTexture ("_MainTex", _texture);
    }
    
    void UpdateTexture () 
    {
        if (_gradient != null) 
        {
            Color[] colours = new Color[_texture.width];
            for (int i = 0; i < _textureResolution; i++) 
            {
                Color gradientCol = _gradient.Evaluate (i / (_textureResolution - 1f));
                colours[i] = gradientCol;
            }
            _texture.SetPixels (colours);
            _texture.Apply ();
        }
    }
}
