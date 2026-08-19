using UnityEngine;
using TMPro;

public class Tile : MonoBehaviour
{
    public TextMeshPro symbolText;
    public enum SymbolType { Cross, Circle, Square, Triangle }
    public SymbolType currentSymbol;

    private Renderer tileRenderer;
    private Color originalColor;
    private bool isCollapsed = false;
    private Vector3 originalScale;
    private Vector3 originalPosition;
    private Collider tileCollider;

    void Awake()
    {
        tileRenderer = GetComponent<Renderer>();
        if (tileRenderer != null) originalColor = tileRenderer.material.color;
        originalScale = transform.localScale;
        originalPosition = transform.position;
        tileCollider = GetComponent<Collider>();
    }

    public void SetSymbol(SymbolType type)
    {
        currentSymbol = type;
        string symbolStr = "";
        switch (type)
        {
            case SymbolType.Cross: symbolStr = "×"; break;
            case SymbolType.Circle: symbolStr = "●"; break;
            case SymbolType.Square: symbolStr = "■"; break;
            case SymbolType.Triangle: symbolStr = "▲"; break;
        }
        if (symbolText != null) symbolText.text = symbolStr;
    }

    public void SetHighlight(bool highlight, Color? color = null)
    {
        if (isCollapsed) return;
        if (tileRenderer == null) return;
        if (highlight) tileRenderer.material.color = color ?? Color.yellow;
        else tileRenderer.material.color = originalColor;
    }

    public void Collapse()
    {
        if (isCollapsed) return;
        isCollapsed = true;
        if (tileCollider != null) tileCollider.enabled = false;
        StartCoroutine(AnimateCollapse());
    }

    private System.Collections.IEnumerator AnimateCollapse()
    {
        float duration = 0.5f;
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = new Vector3(startPos.x, -1f, startPos.z);
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.zero;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
        transform.position = endPos;
        transform.localScale = endScale;
    }

    public void Recover()
    {
        if (!isCollapsed) return;
        isCollapsed = false;
        if (tileCollider != null) tileCollider.enabled = true;
        transform.position = originalPosition;
        transform.localScale = originalScale;
        if (tileRenderer != null) tileRenderer.material.color = originalColor;
        StopAllCoroutines();
    }

    public bool IsCollapsed() { return isCollapsed; }
}
