using UnityEngine;

public class GameLevel : MonoBehaviour
{
    [SerializeField] private int levelNumber;
    [SerializeField] private Transform landerStartPositionTransform;
    [SerializeField] private Transform cameraStartTargetTransform;
    [SerializeField] private float zoomedOutOrthoGraphicSize;

    public int GetLevelNumber()
    {
        return levelNumber;
    }
    public Vector3 GetLanderStartPosition()
    {
        return landerStartPositionTransform.position;
    }
    public Transform getCameraStartTargetTransform()
    {
        return cameraStartTargetTransform;
    }
    public float GetZoomedOutOrthoGraphicSize()
    {
        return zoomedOutOrthoGraphicSize;
    }
}
