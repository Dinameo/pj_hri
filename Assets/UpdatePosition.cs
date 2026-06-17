using UnityEngine;
using UnityEngine.UI;

public class UpdatePosition : MonoBehaviour
{
    public UI uiGameObject;
    float x, y, z;
    float roll, pitch, yaw;
    GameObject EndEffector;
    GameObject PosePanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EndEffector = uiGameObject.EndEffector;
        PosePanel = uiGameObject.PosePanel;
    }
    public void GetPositionEffector(GameObject endEffector, out float x, out float y, out float z)
    {
        Vector3 position = endEffector.transform.position;
        x = position.x * 1000; // Chuyển đổi từ mét sang milimét
        z = position.y * 1000; // Chuyển đổi từ mét sang milimét
        y = position.z * 1000; // Chuyển đổi từ mét sang milimét
    }
    // Update is called once per frame
    void Update()
    {
        GetPositionEffector(EndEffector, out x, out y, out z);
        uiGameObject.UpdatePosePanelPosition(x, y, z);
    }
}
