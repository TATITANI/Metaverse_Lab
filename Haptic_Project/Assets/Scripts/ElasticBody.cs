using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class ElasticBody : Body
{
    private Mesh mesh;
    private MeshCollider meshCollider;

    private Vector3[] initVertices, vertices, velocities, normals;

    // 탄성
    [Range(1,99)] [SerializeField] int elasticity = 5;

    // 누르는 압력
    [Min(0)] [SerializeField] float power = 5f;

    // 감쇠
    [Min(0)] [SerializeField] float damping = 5f;

    // 압력지점 거리에 따른 감쇠
    [Min(0)] [SerializeField] float attenuation = 15f;

    private bool isDeformed = false;

    private float initVertexSqrMag;

    protected override void Start()
    {
        base.Start();

        mesh = GetComponent<MeshFilter>().mesh;
        meshCollider = GetComponent<MeshCollider>();

        velocities = Enumerable.Repeat(Vector3.zero, mesh.vertices.Length).ToArray();
        vertices = (Vector3[])mesh.vertices.Clone();
        initVertices = (Vector3[])mesh.vertices.Clone();
        normals = mesh.normals;

        // 구체이므로 정점거리 한 곳만 측정
        initVertexSqrMag = initVertices[0].sqrMagnitude;
    }

    protected override void Update()
    {
#if UNITY_EDITOR
        ProcessInput();
#endif
        if (elasticity > 0)
        {
            Restore();
            Damping();
            UpdateVertex();
            if (isDeformed)
            {
                UpdateMesh();
            }
        }
    }

    public override void Press(int fingerId, Vector3 pos)
    {
        // world -> local
        Vector3 contactLocalPos = transform.InverseTransformPoint(pos);
        // Debug.Log($" contactPos : {contactPos}{{");

        int pressingVertexID = 0;
        float minDistance = float.MaxValue;
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 diff = (vertices[i] - contactLocalPos);
            float distance = diff.sqrMagnitude;
            if (distance < minDistance)
            {
                minDistance = distance;
                pressingVertexID = i;
            }

            Vector3 direction = diff.normalized;
            float velocity = power / Mathf.Pow(1 + distance * attenuation, 2);
            velocities[i] += direction * velocity;
        }

        float currentGap = (vertices[pressingVertexID] - initVertices[pressingVertexID]).sqrMagnitude;
        float maxGap = ((velocities[pressingVertexID] + power * (vertices[pressingVertexID] - contactLocalPos))
                        / elasticity).sqrMagnitude; // 최대한으로 눌렸을 때 초기위치와 떨어진 거리

        float pressure = currentGap / maxGap;
        pressure = Mathf.Clamp01(pressure);
        controllerSO.SetFingerPressure(fingerId, pressure);
    }

    void Restore()
    {
        for (int i = 0; i < velocities.Length; i++)
        {
            velocities[i] -= (vertices[i] - initVertices[i]) * elasticity;
        }
    }

    void Damping()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            velocities[i] *= damping;
        }
    }

    void UpdateVertex()
    {
        float diff = 0;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] += velocities[i] * Time.deltaTime;
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

    protected override void OnGrabBegin()
    {
        controllerSO.SetGrab(true, elasticity);
    }

    protected override void OnGrabEnd()
    {
        controllerSO.SetGrab(false);
        // controllerSO.ResetFingerPressure();
    }


}