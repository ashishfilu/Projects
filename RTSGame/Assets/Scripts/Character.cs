using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    private GameObject m_SelectionHighlight;
    // Start is called before the first frame update
    void Start()
    {
        m_SelectionHighlight.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateHighlighter( bool activate )
    {
        m_SelectionHighlight.SetActive(activate);
    }
}
