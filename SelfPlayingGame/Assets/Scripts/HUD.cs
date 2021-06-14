using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Button CapturePhotoButton;

    private WebCamTexture m_WebcamTexture;

    void Start()
    {
        CapturePhotoButton.onClick.AddListener(() => { StartCoroutine(CapturePhoto()); });
        m_WebcamTexture = new WebCamTexture();
        WebCamDevice[] allCameras = WebCamTexture.devices;
        for( int i = 0; i < allCameras.Length; i++ )
        {
            if( allCameras[i].isFrontFacing )
            {
                m_WebcamTexture.deviceName = allCameras[i].name;
                Debug.Log($"Front camera : {allCameras[i].name}");
                break;
            }
        }
        m_WebcamTexture.Play();
    }

    IEnumerator CapturePhoto()
    {
        yield return new WaitForEndOfFrame();

        Texture2D photo = new Texture2D(m_WebcamTexture.width, m_WebcamTexture.height);
        photo.SetPixels(m_WebcamTexture.GetPixels());
        photo.Apply();

        byte[] bytes = photo.EncodeToPNG();
        string path = Application.persistentDataPath + "/CameraCapture.png";
        Debug.Log($"Data Path : {path}");
        File.WriteAllBytes(path, bytes);

        m_WebcamTexture.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
