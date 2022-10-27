using System;
using System.Linq;
using Leap.Unity.Interaction;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class ElasticBody : MonoBehaviour
{
    private Mesh mesh;
    private Camera cam;
    private MeshCollider meshCollider;

    private Vector3[] initVertices, vertices, velocities, normals;

    [SerializeField] private ElasticBodyData data;

    private bool isDeformed = false;

    private void Start()
    {
        cam = Camera.main;
        mesh = GetComponent<MeshFilter>().mesh;
        meshCollider = GetComponent<MeshCollider>();

        velocities = Enumerable.Repeat(Vector3.zero, mesh.vertices.Length).ToArray();
        vertices = (Vector3[])mesh.vertices.Clone();
        initVertices = (Vector3[])mesh.vertices.Clone();
        normals = mesh.normals;
    }

    private void Update()
    {
        ProcessInput();
        Restore();
        Damping();
        UpdateVertex();
        if (isDeformed)
        {
            UpdateMesh();
        }
    }

    void ProcessInput()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    // 직접 닿은 표면에 압력을 주기 위해서
                    // 충돌점으로부터 법선벡터 쪽으로 약간 올라간 좌표를 입력. 
                    const float hitPointOffset = 0.1f;
                    Vector3 point = hit.point + hit.normal * hitPointOffset;
                    Press(point);
                }
            }
        }
    }

    public void Press(GrabPos grabPos)
    {
        Press(grabPos.pos, grabPos.id);
    }

    void Press(Vector3 contactPos, int fingerID = -1)
    {
        // world -> local
        Vector3 contactLocalPos = transform.InverseTransformPoint(contactPos);
        // Debug.Log($" contactPos : {contactPos}{{");

        int vertexID = 0;
        float minDistance = float.MaxValue;
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 diff = (vertices[i] - contactLocalPos);
            float distance = diff.sqrMagnitude;
            if (distance < minDistance)
            {
                minDistance = distance;
                vertexID = i;
            }

            Vector3 direction = diff.normalized;
            float velocity = data.power / Mathf.Pow(1 + distance * data.attenuation, 2);
            velocities[i] += direction * velocity * Time.deltaTime;
        }
    }

    void Restore()
    {
        for (int i = 0; i < velocities.Length; i++)
        {
            velocities[i] -= (vertices[i] - initVertices[i]) * data.elasticity * Time.deltaTime;
        }
    }

    void Damping()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            velocities[i] *= data.damping * Time.deltaTime;
        }
    }

    void UpdateVertex()
    {
        float diff = 0;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] += velocities[i];
            diff += velocities[i].sqrMagnitude;
        }
        diff *= Mathf.Pow(10, 8);
        // Debug.Log($"diff : {diff}");
        isDeformed = diff > 0.1f;
    }

    void UpdateMesh()
    {
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        meshCollider.sharedMesh = mesh;
    }
}