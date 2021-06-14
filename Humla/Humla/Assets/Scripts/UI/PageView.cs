using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum PageViewType
{
    Horizontal,
    Vertical,
}

public delegate void OnPageUpdatedDelegate(int pageIndex);

public class PageView : MonoBehaviour , IDragHandler , IEndDragHandler , IBeginDragHandler
{
    public GameObject _viewRoot;
    public PageViewType _viewType;

    private float _shiftOffset;
    private Vector3 _targetPosition;
    private Vector3 _previousDragPosition;
    private float _percentage;
    private int _index , _maxIndex;

    public int CurrentIndex => _index;
    public event OnPageUpdatedDelegate OnPageUpdated;

    public void InitializePageView()
    {
        GridLayoutGroup grid = _viewRoot.GetComponent<GridLayoutGroup>();
        _shiftOffset = _viewType == PageViewType.Horizontal ? grid.cellSize.x : grid.cellSize.y;
        _percentage = 0.1f;
        _maxIndex = _viewRoot.transform.childCount-1;
        _index = 0;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        _previousDragPosition = eventData.pressPosition;
        _targetPosition = _viewRoot.transform.localPosition;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        float delta = eventData.position.x - _previousDragPosition.x;
        if (_viewType == PageViewType.Vertical)
        {
            delta = eventData.position.y - _previousDragPosition.y;
        }
        _viewRoot.gameObject.transform.localPosition += _viewType == PageViewType.Horizontal?
                                                        new Vector3(delta,0,0):new Vector3(0,delta,0);
        _previousDragPosition = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float ratio = (eventData.pressPosition.x - eventData.position.x) / Screen.width;
        if (_viewType == PageViewType.Vertical)
        {
            ratio = (eventData.pressPosition.y - eventData.position.y) / Screen.height;
        }
        
        if ( Mathf.Abs(ratio) >= _percentage )
        {
            if (ratio > 0 && CanMove(MoveDirection.Left) )
            {
                _targetPosition -= new Vector3(_shiftOffset, 0, 0);
                _index++;
                OnPageUpdated?.Invoke(_index);
            }
            else if( ratio < 0 && CanMove(MoveDirection.Right))
            {
                _targetPosition += new Vector3(_shiftOffset, 0, 0);
                _index--;
                OnPageUpdated?.Invoke(_index);
            }
        }
        StartCoroutine(TweenPosition(_viewRoot.gameObject.transform.localPosition, _targetPosition, 0.2f));
    }

    private bool CanMove( MoveDirection direction)
    {
        if (direction == MoveDirection.Left && _index < _maxIndex)
        {
            return true;
        }
        
        if (direction == MoveDirection.Right && _index > 0 )
        {
            return true;
        }

        return false;
    }
    
    IEnumerator TweenPosition(Vector3 startPosition , Vector3 endPosition , float time)
    {
        float deltaTime = 0.0f;

        while (deltaTime <= 1.0f)
        {
            deltaTime += Time.deltaTime / time;
            _viewRoot.transform.localPosition = Vector3.Lerp(startPosition,endPosition,Mathf.SmoothStep(0f,1f,deltaTime));
            yield return null;
        }
    }
}
