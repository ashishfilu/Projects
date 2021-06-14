using UnityEngine;
using System.Collections.Generic;

public class RTSController : MonoBehaviour
{
    [SerializeField] private Transform m_SelectionAreaTransform;

    private Vector3 m_StartPositionOfMouseInScreenSpace;
    private Vector3 m_StartPositionOfMouseInWorldSpace;
    private Vector3 m_EndPositionOfMouseInWorldSpace;

    private bool m_DrawDebugBox;
    private List<Character> m_SelectedCharacters;

    void Start()
    {
        m_SelectedCharacters = new List<Character>();
        m_DrawDebugBox = false;
        m_SelectionAreaTransform.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_StartPositionOfMouseInScreenSpace = Input.mousePosition;
            m_StartPositionOfMouseInWorldSpace = Utility.GetMouseWorldPosition();
            m_SelectionAreaTransform.gameObject.SetActive(true); ;
        }

        if( Input.GetMouseButton(0))
        {
            Vector3 currentMousePosition = Input.mousePosition;
            Vector3 minPosition = new Vector3(Mathf.Min(m_StartPositionOfMouseInScreenSpace.x,currentMousePosition.x), 
                                                Mathf.Min(m_StartPositionOfMouseInScreenSpace.y,currentMousePosition.y));
            Vector3 maxPosition = new Vector3(Mathf.Max(m_StartPositionOfMouseInScreenSpace.x, currentMousePosition.x),
                                                Mathf.Max(m_StartPositionOfMouseInScreenSpace.y, currentMousePosition.y));

            m_SelectionAreaTransform.position = minPosition;
            m_SelectionAreaTransform.localScale = maxPosition - minPosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            m_SelectionAreaTransform.gameObject.SetActive(false);
            m_EndPositionOfMouseInWorldSpace = Utility.GetMouseWorldPosition();

            Vector3 center = (m_StartPositionOfMouseInWorldSpace + m_EndPositionOfMouseInWorldSpace) * 0.5f;
            Vector3 extent = (m_EndPositionOfMouseInWorldSpace - m_StartPositionOfMouseInWorldSpace) *0.5f ;
            extent.x = Mathf.Abs(extent.x);
            extent.y = 1.0f;
            extent.z = Mathf.Abs(extent.z);
            Collider[] colliders = Physics.OverlapBox(center, extent);

            GameObject[] characters = GameObject.FindGameObjectsWithTag("Character");
            if( characters.Length > 0 )
            {
                for( int i = 0; i < characters.Length; i++ )
                {
                    Character script = characters[i].GetComponent<Character>();
                    script?.ActivateHighlighter(false);
                }
            }

            if (colliders.Length > 0)
            {
                m_SelectedCharacters.Clear();
                for (int i = 0; i < colliders.Length; i++)
                {
                    Character selectedUnit = colliders[i].gameObject.GetComponent<Character>();
                    if( selectedUnit != null )
                    {
                        selectedUnit.ActivateHighlighter(true);
                        m_SelectedCharacters.Add(selectedUnit);
                    }
                }
            }
            
        }
    }

    // Update is called once per frame
    void OnDrawGizmos()
    {
        if (m_DrawDebugBox)
        {
            Vector3 center = (m_StartPositionOfMouseInWorldSpace + m_EndPositionOfMouseInWorldSpace) * 0.5f;
            Vector3 extent = (m_EndPositionOfMouseInWorldSpace - m_StartPositionOfMouseInWorldSpace) ;
            extent.x = Mathf.Abs(extent.x);
            extent.y = 1.0f;
            extent.z = Mathf.Abs(extent.z);
            Gizmos.DrawCube(center, extent);
        }
    }

}
