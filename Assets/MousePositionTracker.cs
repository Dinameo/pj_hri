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
    public float zScrollStep = 100.0f; // Mỗi nấc lăn chuột tăng/giảm Z bao nhiêu mm

    [Header("Offsets")]
    public float zeroX = 0;
    public float zeroY = 0;
    public float zeroZ = 0;

    [Header("Final Values")]
    public float x = 915, y = 0, z = 1159, roll = 0, pitch = -90, yaw = -180; 

    private float rawMouseAccumulatedX = 0f;
    private float rawMouseAccumulatedY = 0f;
    private float rawMouseAccumulatedZ = 0f;

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
                    // Click trúng UI -> bỏ qua
                }
                else
                {
                    LockCursor();
                }
            }

            // 2. CẬP NHẬT X, Y KHI CHUỘT ĐANG KHÓA
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                if (Mouse.current != null)
                {
                    Vector2 mouseDelta = Mouse.current.delta.ReadValue();
                    mouseX = mouseDelta.x * 0.1f;
                    mouseY = mouseDelta.y * 0.1f;

                    rawMouseAccumulatedX += mouseX * scaleFactor;
                    rawMouseAccumulatedY += mouseY * scaleFactor;

                    // Lăn chuột để tăng/giảm Z
                    float scrollY = Mouse.current.scroll.ReadValue().y;

                    if (scrollY != 0)
                    {
                        rawMouseAccumulatedZ += Mathf.Sign(scrollY) * zScrollStep;
                    }
                }

                isUpdate = true; 
            }
            else
            {
                isUpdate = false; 
            }

            // 3. TÍNH GIÁ TRỊ CUỐI
            x = (rawMouseAccumulatedX - zeroX) + 915f;
            y = rawMouseAccumulatedY - zeroY;
            z = (rawMouseAccumulatedZ - zeroZ) + 1159f;
        }

        // 4. XỬ LÝ LỆNH OFFSET
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
        zeroX = rawMouseAccumulatedX;
        zeroY = rawMouseAccumulatedY;
        zeroZ = rawMouseAccumulatedZ;

        x = 915f;
        y = 0f;
        z = 1159f;

        isUpdate = true; 
        Debug.Log("MOUSE RESET TO DEFAULT (915, 0, 1159) SUCCESSFUL");
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