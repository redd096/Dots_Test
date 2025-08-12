using UnityEngine;

public class UnitSelectionManagerUI : MonoBehaviour
{
    [SerializeField] private RectTransform selectionAreaRt;
    [SerializeField] private Canvas selectionAreaCanvas;

    private void Start()
    {
        UnitSelectionManager.Instance.OnSelectionAreaStart += OnSelectionAreaStart;
        UnitSelectionManager.Instance.OnSelectionAreaEnd += OnSelectionAreaEnd;

        //by default hide selection area
        selectionAreaRt.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        UnitSelectionManager.Instance.OnSelectionAreaStart -= OnSelectionAreaStart;
        UnitSelectionManager.Instance.OnSelectionAreaEnd -= OnSelectionAreaEnd;
    }

    private void Update()
    {
        //continue update selection area
        if (selectionAreaRt.gameObject.activeSelf)
            UpdateVisual();
    }

    private void OnSelectionAreaStart()
    {
        //show selection area
        UpdateVisual();
        selectionAreaRt.gameObject.SetActive(true);
    }

    private void OnSelectionAreaEnd()
    {
        //hide selection area
        selectionAreaRt.gameObject.SetActive(false);
    }

    private void UpdateVisual()
    {
        //set selection area position and size
        Rect selectionAreaRect = UnitSelectionManager.Instance.GetSelectionAreaRect();
        selectionAreaRt.anchoredPosition = selectionAreaRect.position / selectionAreaCanvas.scaleFactor;
        selectionAreaRt.sizeDelta = selectionAreaRect.size / selectionAreaCanvas.scaleFactor;
    }
}
