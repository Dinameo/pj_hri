using UnityEngine;

public class ControlRobot : MonoBehaviour
{
    private Transform[] Joints = new Transform[6];
    private Quaternion[] JointsStart = new Quaternion[6];

    private float[] currentAngles = new float[6];
    private float[] targetAngles = new float[6];

    public bool InstantMove = false;

    // public float[] jointSpeeds = new float[6]
    // {
    //     230f, 120f, 110f, 480f, 390f, 535f
    // };
    public float[] jointSpeeds =
    {
        90f,
        90f,
        90f,
        90f,
        150f,
        200f
    };


    public void FirstStart(GameObject Robot)
    {
        GameObject parent = Robot;

        for (int i = 0; i < 6; i++)
        {
            Joints[i] = parent.transform.Find("axis" + i);

            if (Joints[i] == null)
            {
                Debug.LogError("Không tìm thấy axis" + i);
                return;
            }

            JointsStart[i] = Joints[i].localRotation;
            currentAngles[i] = 0f;
            targetAngles[i] = 0f;

            parent = Joints[i].gameObject;
        }
    }

    public void SetTargetAngles(float[] thetas_vector)
    {
        for (int i = 0; i < 6; i++)
        {
            targetAngles[i] = thetas_vector[i];
        }
    }

    void Update()
    {
        for (int i = 0; i < 6; i++)
        {
            MoveJoint(i, targetAngles[i]);
        }
    }

    void MoveJoint(int jointIndex, float targetAngle)
    {
        currentAngles[jointIndex] = InstantMove
            ? targetAngle
            : Mathf.MoveTowards(
                currentAngles[jointIndex],
                targetAngle,
                jointSpeeds[jointIndex] * Time.deltaTime
            );

        if (Joints[jointIndex] == null) return;

        Joints[jointIndex].localRotation =
            JointsStart[jointIndex] *
            Quaternion.Euler(0, 0, -currentAngles[jointIndex]);
    }
}