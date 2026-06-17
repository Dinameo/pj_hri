using UnityEngine;

public class Mpu : MonoBehaviour
{
    // THÊM CHECKBOX BẬT/TẮT TÍNH NĂNG Ở ĐÂY
    public bool EnableMpu = false; 

    public bool OffsetMpu = false;
    public bool isUpdate = false;

    public UDPConnect UDPConnectObject;

    [Header("Offsets")]
    public float zeroX = 0;
    public float zeroY = 0;
    public float zeroZ = 0;
    public float zeroRoll = 0;
    public float zeroPitch = 0;
    public float zeroYaw = 0;

    [Header("Final Values")]
    // Cài đặt các giá trị mặc định ban đầu của Robot
    public float x = 915f, y = 0f, z = 1159f, roll = 0f, pitch = 0f, yaw = 0f;

    void Update()
    {
        if (!EnableMpu) return; 

        if (UDPConnectObject == null) return;

        UDPConnectObject.GetMsg();

        if (UDPConnectObject.hasNewData)
        {
            UDPConnectObject.hasNewData = false;

            UDPConnectObject.DataProcessor(UDPConnectObject.latestMessage);

            // 1. XỬ LÝ LỆNH OFFSET TRƯỚC ĐỂ CẬP NHẬT MỐC ZERO
            if (OffsetMpu)
            {
                OffsetMpu = false;
                ZeroOffsetFromLatest();
            }

            // 2. TOÁN HỌC CHUẨN XÁC: Đã sửa từ "+=" thành "=" và đưa hằng số mặc định vào công thức
            // Khi vừa bấm Offset xong, các hiệu số (latest - zero) đều bằng 0, giá trị sẽ trả về chuẩn mốc mặc định.
            x = (UDPConnectObject.latestX - zeroX) + 915f; 
            y = UDPConnectObject.latestY - zeroY;
            z = (UDPConnectObject.latestZ - zeroZ) + 1159f;

            roll  = UDPConnectObject.latestRoll  - zeroRoll;
            pitch = UDPConnectObject.latestPitch - zeroPitch;
            yaw   = UDPConnectObject.latestYaw   - zeroYaw;

            isUpdate = true;
        }
    }

    public void ZeroOffsetFromLatest()
    {
        if (UDPConnectObject == null) return;

        // Chụp lại toàn bộ dữ liệu thô từ ESP32 tại thời điểm bấm nút
        zeroX = UDPConnectObject.latestX;
        zeroY = UDPConnectObject.latestY;
        zeroZ = UDPConnectObject.latestZ;

        zeroRoll  = UDPConnectObject.latestRoll;
        zeroPitch = UDPConnectObject.latestPitch;
        zeroYaw   = UDPConnectObject.latestYaw;

        // Ép ngay lập tức trạng thái hiển thị của Robot về đúng mốc tọa độ mong muốn
        x = 915f; 
        y = 0f; 
        z = 1159f;
        
        roll = pitch = yaw = 0f;

        isUpdate = true;

        Debug.Log("MPU OFFSET DONE: RESET TO DEFAULT (915, 0, 1159)");
    }
}