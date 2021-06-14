using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public delegate void OnButtonPressed(PointerEventData data);
public delegate void OnButtonReleased(PointerEventData data);

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class CustomButton : MonoBehaviour,IPointerDownHandler, IPointerUpHandler
{
    public event OnButtonPressed onButtonPressed;
    public event OnButtonReleased onButtonReleased;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        onButtonPressed?.Invoke(eventData);
    }
 
    public void OnPointerUp(PointerEventData eventData)
    {
        onButtonReleased?.Invoke(eventData);
    }
}
