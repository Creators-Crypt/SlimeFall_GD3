using UnityEngine;
using UnityEngine.UI;

public class UIGradientFade : BaseMeshEffect
{
    public enum FadeDirection
    {
        FadeAtTop,
        FadeAtBottom
    }

    [SerializeField] private FadeDirection fadeDirection = FadeDirection.FadeAtTop;

    public override void ModifyMesh(VertexHelper vertexHelper)
    {
        if (!IsActive())
            return;

        UIVertex vertex = new UIVertex();

        float minY = float.MaxValue;
        float maxY = float.MinValue;

        for (int i = 0; i < vertexHelper.currentVertCount; i++)
        {
            vertexHelper.PopulateUIVertex(ref vertex, i);
            minY = Mathf.Min(minY, vertex.position.y);
            maxY = Mathf.Max(maxY, vertex.position.y);
        }

        float height = maxY - minY;

        if (height <= 0f)
            return;

        for (int i = 0; i < vertexHelper.currentVertCount; i++)
        {
            vertexHelper.PopulateUIVertex(ref vertex, i);

            float normalizedY = (vertex.position.y - minY) / height;

            float fade = fadeDirection == FadeDirection.FadeAtTop ? normalizedY : 1f - normalizedY;

            fade = Mathf.InverseLerp(0f, 0.9f, fade);

            float alpha = Mathf.Pow(fade, 0.35f);

            Color color = vertex.color;
            color.a *= alpha;
            vertex.color = color;

            vertexHelper.SetUIVertex(vertex, i);
        }
    }
}
