using UnityEngine;
// Cần khai báo thêm thư viện EventSystems để check va chạm với UI Panel
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
    public float zeroRoll = 0;
    public float zeroPitch = 0;
    public float zeroYaw = 0;

    [Header("Final Values")]
    public float x, y, z = 1159, roll = 0, pitch = -90, yaw = -180; 

    private float rawMouseAccumulatedX = 0f;
    private float rawMouseAccumulatedY = 0f;

    void Start()
    {
        if (UseMouseTracker)
        {
            LockCursor();
        }
        else
        {
            UnlockCursor();
        }
    }

    void Update()
    {
        float mouseX = 0f;
        float mouseY = 0f;

        if (UseMouseTracker)
        {
            // KIỂM TRA CLICK VÀO VÙNG TRỐNG (KHÔNG PHẢI UI PANEL)
            if (Cursor.lockState == CursorLockMode.None && Mouse.current.leftButton.wasPressedThisFrame)
            {
                // Kiểm tra xem con trỏ chuột hiện tại có đang đè lên bất kỳ UI Object nào không
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                {
                    // Nếu đang bấm vào UI (ví dụ: Dropdown, Panel, Button), bỏ qua không lock chuột
                    // Debug.Log("Clicked on UI - Ignore Locking");
                }
                else
                {
                    // Nếu bấm vào vùng trống, tiến hành khóa chuột như bình thường
                    LockCursor();
                }
            }

            if (Mouse.current != null)
            {
                Vector2 mouseDelta = Mouse.current.delta.ReadValue();
                mouseX = mouseDelta.x * 0.1f;
                mouseY = mouseDelta.y * 0.1f;
            }

            rawMouseAccumulatedX += mouseX * scaleFactor;
            rawMouseAccumulatedY += mouseY * scaleFactor;
        }

        if (OffsetMouse)
        {
            OffsetMouse = false;
            ZeroOffsetFromLatest();
        }

        if (UseMouseTracker)
        {
            x = rawMouseAccumulatedX - zeroX;
            y = rawMouseAccumulatedY - zeroY;
        }

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            UnlockCursor();
        }
    }

    void ZeroOffsetFromLatest()
    {
        // Khi offset, đặt lại giá trị tích lũy hiện tại làm điểm mốc zero để x, y thực sự về 0
        zeroX = rawMouseAccumulatedX;
        zeroY = rawMouseAccumulatedY;

        x = y = 0;
        roll = 0;
        pitch = -90;
        yaw = -180;

        isUpdate = true;
        Debug.Log("ZERO OFFSET DONE");
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