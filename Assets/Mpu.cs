using UnityEngine;

public class Mpu : MonoBehaviour
{
    public bool OffsetMpu = false;
    public bool isUpdate = false;

    public UDPConnect UDPConnectObject;

    public float zeroX = 0;
    public float zeroY = 0;
    public float zeroZ = 0;
    public float zeroRoll = 0;
    public float zeroPitch = 0;
    public float zeroYaw = 0;

    public float x, y, z, roll, pitch, yaw;

    void Update()
    {
        if (UDPConnectObject == null) return;

        UDPConnectObject.GetMsg();

        if (UDPConnectObject.hasNewData)
        {
            UDPConnectObject.hasNewData = false;

            UDPConnectObject.DataProcessor(UDPConnectObject.latestMessage);

            if (OffsetMpu)
            {
                OffsetMpu = false;
                ZeroOffsetFromLatest();
            }

            x = UDPConnectObject.latestX - zeroX;
            y = UDPConnectObject.latestY - zeroY;
            z = UDPConnectObject.latestZ - zeroZ;

            roll  = UDPConnectObject.latestRoll  - zeroRoll;
            pitch = UDPConnectObject.latestPitch - zeroPitch;
            yaw   = UDPConnectObject.latestYaw   - zeroYaw;

            isUpdate = true;
        }
    }

    void ZeroOffsetFromLatest()
    {
        zeroX = UDPConnectObject.latestX;
        zeroY = UDPConnectObject.latestY;
        zeroZ = UDPConnectObject.latestZ;

        zeroRoll  = UDPConnectObject.latestRoll;
        zeroPitch = UDPConnectObject.latestPitch;
        zeroYaw   = UDPConnectObject.latestYaw;

        x = y = z = 0;
        roll = pitch = yaw = 0;

        isUpdate = true;

        Debug.Log("MPU ZERO OFFSET DONE");
    }
}