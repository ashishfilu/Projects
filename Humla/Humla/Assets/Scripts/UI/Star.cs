using UnityEngine;

public class Star : MonoBehaviour
{
    public GameObject _background;
    public GameObject _foreground;


    public void ActivateForeground(bool status)
    {
        _foreground.SetActive(status);
    }
}
