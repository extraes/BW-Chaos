using UnityEngine;
using UnityEngine.Rendering;

// Attach this script to a Camera and pick a mesh to render.
// When you enter Play mode, a command buffer renders a green mesh at
// origin position.
//[RequireComponent(typeof(Camera))]
public class ExampleScript : MonoBehaviour
{
    public Mesh mesh;

    void Start()
    {
        var material = new Material(Shader.Find("Hidden/Internal-Colored"));
        material.SetColor("_Color", Color.green);

        var tr = transform;
        var camera = Camera.main;

        // Code below does the same as what camera.worldToCameraMatrix would do. Doing
        // it "manually" here to illustrate how a view matrix is constructed.
        //
        // Matrix that looks from camera's position, along the forward axis.
        var lookMatrix = Matrix4x4.LookAt(tr.position, tr.position + tr.forward, tr.up);
        // Matrix that mirrors along Z axis, to match the camera space convention.
        var scaleMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1, 1, -1));
        // Final view matrix is inverse of the LookAt matrix, and then mirrored along Z.
        var viewMatrix = scaleMatrix * lookMatrix.inverse;

        var buffer = new CommandBuffer();
        //buffer.SetViewMatrix(camera.worldToCameraMatrix);
        buffer.SetProjectionMatrix(camera.projectionMatrix);
        buffer.DrawMesh(mesh, Matrix4x4.identity, material);

        camera.AddCommandBuffer(CameraEvent.BeforeSkybox, buffer);
    }
}