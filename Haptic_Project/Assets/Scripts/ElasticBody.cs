using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class ElasticBody : MonoBehaviour
{
    private Camera cam;
    [SerializeField] private HandControllerSO controllerSO;
    private Mesh mesh;
    private MeshCollider meshCollider;

    private Vector3[] initVertices, vertices, velocities, normals;

    [SerializeField] private ElasticBodySO data;

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
#if UNITY_EDITOR
        ProcessInput();
#endif

        Restore();
        Damping();
        UpdateVertex();
        if (isDeformed)
        {
            UpdateMesh();
        }

        if (controllerSO.isGrab)
        {
            UpdateFingerPressure();
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
                    Press(new GrabPos() { pos = point });
                }
            }
        }
    }


    public void Press(GrabPos grabPos)
    {
        // world -> local
        Vector3 contactLocalPos = transform.InverseTransformPoint(grabPos.pos);
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
            float velocity = data.power / Mathf.Pow(1 + distance * data.attenuation, 2);
            velocities[i] += direction * velocity * Time.deltaTime;
        }

        controllerSO.SetFingerPressingVertex(grabPos.isLeft, grabPos.fingerID, pressingVertexID);
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

    void UpdateFingerPressure()
    {
        for (int i = 0; i < controllerSO.pressureRight.Length; i++)
        {
            int vertexID = controllerSO.pressureRight[i].vertexID;
            // Debug.Log($"? {vertexID}");
            float diff = (vertices[vertexID] - initVertices[vertexID]).sqrMagnitude;

            // todo : 퍼센트로 변경
            diff *= 1000000; // 임시값 

            bool isPress = controllerSO.pressureRight[i].isPress;
            controllerSO.SetFingerPressure(false, i, isPress ? (int)diff : 0);
        }
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