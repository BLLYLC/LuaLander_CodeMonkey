using Unity.Cinemachine;
using UnityEngine;

public class CinemachineZoom2D : MonoBehaviour
{
    public static CinemachineZoom2D Instance { get; private set; }
    [SerializeField] private CinemachineCamera cinemachineCamera;
    private float targetOrthographicSize=10f;
    private const float NORMAL_ORTHOGRAPHIC_SIZE = 10f;

    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        float zoomSpeed = 2f;
        cinemachineCamera.Lens.OrthographicSize =
            Mathf.Lerp(cinemachineCamera.Lens.OrthographicSize, targetOrthographicSize, Time.deltaTime * zoomSpeed);
    }
    public void SetTargetOrthographicSize(float targetOrthographicSize)
    {
        this.targetOrthographicSize = targetOrthographicSize;
    }
    public void SetNormalOrthographicSize()
    {
        SetTargetOrthographicSize(NORMAL_ORTHOGRAPHIC_SIZE);
    }
}
