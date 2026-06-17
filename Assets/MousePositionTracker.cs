using UnityEngine;
using UnityEngine.EventSystems; 
using UnityEngine.InputSystem; 

public class MouseTracker : MonoBehaviour
{
    [Header("Toggle Mouse Feature")]
    public bool UseMouseTracker = false; 

    [Header("Controls")]
    public bool OffsetMouse = false; 
    public bool isUpdate = false;

    [Header("Mouse Configuration")]
    public float scaleFactor = 1.0f; 

    [Header("Offsets")]
    public float zeroX = 0;
    public float zeroY = 0;

    [Header("Final Values")]
    // Đặt mặc định ban đầu khi vừa vào game là x = 915
    public float x = 915, y = 0, z = 1159, roll = 0, pitch = -90, yaw = -180; 

    private float rawMouseAccumulatedX = 0f;
    private float rawMouseAccumulatedY = 0f;

    void Start()
    {
        if (UseMouseTracker) LockCursor();
        else UnlockCursor();
    }

    void Update()
    {
        float mouseX = 0f;
        float mouseY = 0f;

        if (UseMouseTracker)
        {
            // 1. XỬ LÝ KHÓA CHUỘT
            if (Cursor.lockState == CursorLockMode.None && Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                {
                    // Click trúng UI -> Bỏ qua
                }
                else
                {
                    LockCursor();
                }
            }

            // 2. CẬP NHẬT TỌA ĐỘ KHI CHUỘT ĐANG KHÓA
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                if (Mouse.current != null)
                {
                    Vector2 mouseDelta = Mouse.current.delta.ReadValue();
                    mouseX = mouseDelta.x * 0.1f;
                    mouseY = mouseDelta.y * 0.1f;
                }

                rawMouseAccumulatedX += mouseX * scaleFactor;
                rawMouseAccumulatedY += mouseY * scaleFactor;
                
                isUpdate = true; 
            }
            else
            {
                isUpdate = false; 
            }

            // ĐỔI LOGIC TOÁN HỌC: Cộng thêm 915 vào trục X 
            // Như vậy khi vừa bấm Offset (rawMouseAccumulatedX - zeroX = 0), thì x sẽ bằng đúng 915
            x = (rawMouseAccumulatedX - zeroX) + 915f;
            y = rawMouseAccumulatedY - zeroY;
        }

        // 3. XỬ LÝ LỆNH OFFSET
        if (OffsetMouse)
        {
            OffsetMouse = false;
            ZeroOffsetFromLatest();
        }

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            UnlockCursor();
        }
    }

    public void ZeroOffsetFromLatest()
    {
        // Chụp lại vị trí chuột thô hiện tại
        zeroX = rawMouseAccumulatedX;
        zeroY = rawMouseAccumulatedY;

        // Ép vị trí robot về đúng mốc mong muốn ngay tại frame này
        x = 915f;
        y = 0f;

        isUpdate = true; 
        Debug.Log("LOGIC FIX: MOUSE RESET TO DEFAULT (915, 0) SUCCESSFUL");
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}