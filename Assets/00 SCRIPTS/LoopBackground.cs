using UnityEngine;
using System.Collections.Generic;

public class LoopBackground : MonoBehaviour
{
    [Tooltip("Move speed to the left (world units per second).")]
    public float speed = 2f;

    [Tooltip("Small overlap to hide seams (world units). Use 0 if images tile perfectly.")]
    public float overlapMargin = 0.01f;

    private class Layer
    {
        public Transform t;
        public float width; // world units
    }

    private List<Layer> layers;
    private Camera mainCam;
    private float camDistanceForProjection;

    void Start()
    {
        mainCam = Camera.main;
        if (mainCam == null)
        {
            Debug.LogError("LoopBackground: No Camera.main found.");
            enabled = false;
            return;
        }

        // compute z distance used for ViewportToWorldPoint
        camDistanceForProjection = Mathf.Abs(mainCam.transform.position.z - transform.position.z);

        // collect children that have SpriteRenderer
        layers = new List<Layer>();
        foreach (Transform child in transform)
        {
            SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
            if (sr == null)
            {
                Debug.LogWarning("LoopBackground: child " + child.name + " has no SpriteRenderer, skipping.");
                continue;
            }

            // world width of the sprite (takes scale into account)
            float w = sr.bounds.size.x;
            layers.Add(new Layer { t = child, width = w });
        }

        if (layers.Count < 2)
        {
            Debug.LogWarning("LoopBackground: Need at least 2 child sprites to loop smoothly.");
        }
    }

    void Update()
    {
        if (layers == null || layers.Count == 0) return;

        // move all layers left
        float dx = speed * Time.deltaTime;
        for (int i = 0; i < layers.Count; i++)
        {
            var l = layers[i];
            l.t.position += Vector3.left * dx;
        }

        // compute camera left bound (world x) at same Z as the layers' parent
        float leftBound = mainCam.ViewportToWorldPoint(new Vector3(0f, 0.5f, camDistanceForProjection)).x;

        // Reposition any layer that has fully moved off the left side (its right edge < leftBound)
        // Place it immediately to the right of the current rightmost edge (no gaps).
        for (int i = 0; i < layers.Count; i++)
        {
            var layer = layers[i];
            float layerRightEdge = layer.t.position.x + (layer.width / 2f);

            if (layerRightEdge < leftBound)
            {
                // find current rightmost edge among all layers (use latest positions)
                float maxRight = float.MinValue;
                foreach (var other in layers)
                {
                    if (other == layer) continue;
                    float otherRight = other.t.position.x + (other.width / 2f);
                    if (otherRight > maxRight) maxRight = otherRight;
                }

                // if none found (degenerate) use leftBound as base
                if (maxRight == float.MinValue) maxRight = leftBound;

                // compute new center x so that this layer's left edge touches maxRight (no gap)
                float newCenterX = maxRight + (layer.width / 2f) - overlapMargin;

                layer.t.position = new Vector3(newCenterX, layer.t.position.y, layer.t.position.z);
            }
        }
    }
}
