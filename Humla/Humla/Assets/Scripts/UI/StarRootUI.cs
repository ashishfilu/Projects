using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StarRootUI : MonoBehaviour
{
    private List<Star> _stars;
    private float _percentage;
    
    // Start is called before the first frame update
    void Start()
    {
        _stars = gameObject.GetComponentsInChildren<Star>().ToList();
    }
    
    void Update()
    {
        for (int i = 0; i < _stars.Count; i++)
        {
            _stars[i].ActivateForeground(false);
        }
        
        float percentage = 100.0f / _stars.Count;
        float totalPercentage = _percentage;
        
        for (int i = 0; i < _stars.Count && totalPercentage > 0 ; i++)
        {
            _stars[i].ActivateForeground(true);
            totalPercentage -= percentage;
        }
    }
    
    public void SetPercentage(float percentage)
    {
        _percentage = percentage;
    }
}
