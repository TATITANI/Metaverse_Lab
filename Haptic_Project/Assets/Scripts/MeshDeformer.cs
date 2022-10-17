using System;
using System.Linq;
using Leap.Unity.Interaction;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshDeformer : MonoBehaviour
{
    private Mesh mesh;
    private Camera cam;
    private MeshCollider meshCollider;

    private Vector3[] initVertices, vertices, velocities, normals;

    // 탄성
    [SerializeField] [Min(0)] private float elasticity = 1f;

    private HandController controller;
    private InteractionBehaviour interaction;

    private void Start()
    {
        interaction = GetComponent<InteractionBehaviour>();
        controller = AppManager.Instance.handController;

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
        Flex();
        UpdateVertex();
        UpdateMesh();
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
                    const float power = 20f;
                    Press(point, power);
                }
            }
        }
    }

    void Press(Vector3 contactPos, float power)
    {
        // world -> local
        Vector3 contactLocalPos = transform.InverseTransformPoint(contactPos);
        // Debug.Log($" contactPos : {contactPos}");
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 diff = (vertices[i] - contactLocalPos);
            Vector3 direction = diff.normalized;
            float distance = diff.sqrMagnitude;
            float velocity = power / Mathf.Pow(1 + distance, 2);
            velocities[i] += direction * velocity * Time.deltaTime;
        }
    }

    void Flex()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            velocities[i] *= elasticity * Time.deltaTime;
        }
    }

    void Restore()
    {
        for (int i = 0; i < velocities.Length; i++)
        {
            velocities[i] -= (vertices[i] - initVertices[i]) * elasticity * Time.deltaTime;
        }
    }

    void UpdateVertex()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] += velocities[i];
        }
    }

    void UpdateMesh()
    {
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        // meshCollider.sharedMesh = mesh;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Debug.Log($" collision - impulse :  {collision.impulse}," +
        //           $" relativeVelocity : {collision.relativeVelocity}");
    }

    public void OnGrab()
    {
        foreach (var hand in interaction.graspingHands)
        {
            bool isleft = hand.isLeft;
        }
    }
}