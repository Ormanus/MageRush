using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
[RequireComponent(typeof(CanvasRenderer))]
public class RoundedRectangleRenderer : Graphic
{
    public new Color color = Color.white;
    public int CornerVertices = 16;
    public float CornerRadius = 40f;
    public bool RoundedEnd = false;
    public bool RoundedTop = false;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        Rect rect = rectTransform.rect;

        int vi = 0; // Vertex Index

        if (CornerVertices < 2)
        {
            RenderRectangle(rect.min, rect.max, vh, ref vi);

            return;
        }

        float radius = CornerRadius;

        if (RoundedEnd)
            radius = rect.height / 2f;
        if (RoundedTop)
            radius = rect.width / 2f;

        float radiusX = Mathf.Min(radius, rect.width / 2f);
        float radiusY = Mathf.Min(radius, rect.height / 2f);
        Vector2 radii = new Vector2(radiusX, radiusY);

        Vector2 neOrigo = new (rect.xMax - radiusX, rect.yMax - radiusY);
        Vector2 nwOrigo = new (rect.xMin + radiusX, rect.yMax - radiusY);
        Vector2 swOrigo = new (rect.xMin + radiusX, rect.yMin + radiusY);
        Vector2 seOrigo = new (rect.xMax - radiusX, rect.yMin + radiusY);

        UIVertex vert = UIVertex.simpleVert;
        vert.color = color;

        RenderQuarterCircle(vh, neOrigo, radii, 0f, ref vi);
        RenderQuarterCircle(vh, nwOrigo, radii, Mathf.PI / 2f, ref vi);
        RenderQuarterCircle(vh, swOrigo, radii, Mathf.PI, ref vi);
        RenderQuarterCircle(vh, seOrigo, radii, Mathf.PI / 2f * 3f, ref vi);

        if (RoundedEnd)
        {
            RenderRectangle(new Vector2(rect.xMin + radii.x, rect.yMin), new Vector2(rect.xMax - radii.x, rect.yMax), vh, ref vi);
            return;
        }

        if (RoundedTop)
        {
            RenderRectangle(new Vector2(rect.xMin, rect.yMin + radii.y), new Vector2(rect.xMax, rect.yMax - radii.y), vh, ref vi);
            return;
        }

        RenderRectangle(new Vector2(rect.xMin, rect.yMin + radii.y), new Vector2(rect.xMax, rect.yMax - radii.y), vh, ref vi);
        RenderRectangle(nwOrigo, neOrigo + new Vector2(0f, radii.y), vh, ref vi);
        RenderRectangle(swOrigo + new Vector2(0f, -radii.y), seOrigo, vh, ref vi);
    }

    void RenderRectangle(Vector2 min, Vector2 max, VertexHelper vh, ref int vi)
    {
        UIVertex vert = UIVertex.simpleVert;
        vert.color = color;

        vert.position = new Vector2(min.x, max.y);
        vh.AddVert(vert);
        vert.position = new Vector2(min.x, min.y);
        vh.AddVert(vert);
        vert.position = new Vector2(max.x, max.y);
        vh.AddVert(vert);
        vert.position = new Vector2(max.x, min.y);
        vh.AddVert(vert);

        vh.AddTriangle(vi + 0, vi + 1, vi + 2);
        vh.AddTriangle(vi + 1, vi + 3, vi + 2);
        vi += 4;
    }

    void RenderQuarterCircle(VertexHelper vh, Vector2 origo, Vector2 radii, float angle, ref int vi)
    {
        float step = Mathf.PI / 2f / (CornerVertices - 1);
        UIVertex vert = UIVertex.simpleVert;
        vert.color = color;
        vert.position = origo;
        vh.AddVert(vert);
        int origoindex = vi;
        vi++;
        for (int i = 0; i < CornerVertices; i++)
        {
            vert.position = new Vector2(origo.x + Mathf.Cos(i * step + angle) * radii.x, origo.y + Mathf.Sin(i * step + angle) * radii.y);
            vh.AddVert(vert);
            vi++;
            if (i > 0)
            {
                vh.AddTriangle(origoindex, vi - 2, vi - 1);
            }
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(RoundedRectangleRenderer))]
[CanEditMultipleObjects]
public class RoundedButtonRendererEditor : Editor
{
    SerializedProperty color;
    SerializedProperty vertices;
    SerializedProperty radius;
    SerializedProperty roundedEnd;
    SerializedProperty roundedTop;

    void OnEnable()
    {
        vertices = serializedObject.FindProperty("CornerVertices");
        radius = serializedObject.FindProperty("CornerRadius");
        roundedEnd = serializedObject.FindProperty("RoundedEnd");
        roundedTop = serializedObject.FindProperty("RoundedTop");
        color = serializedObject.FindProperty("color");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(color);
        EditorGUILayout.PropertyField(vertices);
        EditorGUILayout.LabelField("Semi circle ends");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(roundedEnd, new GUIContent("X", "Rounded ends horizontally"));
        if (roundedEnd.boolValue)
            roundedTop.boolValue = false;
        EditorGUILayout.PropertyField(roundedTop, new GUIContent("Y", "Rounded ends vertically"));
        if (roundedTop.boolValue)
            roundedEnd.boolValue = false;
        EditorGUILayout.EndHorizontal();
        if (!roundedEnd.boolValue && !roundedTop.boolValue)
            EditorGUILayout.PropertyField(radius);
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
