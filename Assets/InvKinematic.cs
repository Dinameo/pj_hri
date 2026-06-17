using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InvKinematic : MonoBehaviour
{
    public DenHavitParams DH_Parameters = new DenHavitParams();
    public UI uiGameObject;
    public ForwardKinematic forwardKinematic;
    public GameObject Robot;

    float l1, l2, l3, l4, d1, d2;
    float[] thetas = new float[6];
    public float[,] T01, T12, T23, T34, T45, T56, T06;
    float x, y, z, roll, pitch, yaw;
    float[,] solutions;
    float[] solutions_index;
    public Mpu mpu;
    public MouseTracker mouseTracker;
    public int autoSelectIndex = 1;

    void Start()
    {
        l1 = DH_Parameters.l1;
        l2 = DH_Parameters.l2;
        l3 = DH_Parameters.l3;
        l4 = DH_Parameters.l4;
        d1 = DH_Parameters.d1;
        d2 = DH_Parameters.d2;

        x = uiGameObject.InvPanel.transform.Find("X_input").GetComponent<InputField>().text == "" ? 0f : float.Parse(uiGameObject.InvPanel.transform.Find("X_input").GetComponent<InputField>().text);
        y = uiGameObject.InvPanel.transform.Find("Y_input").GetComponent<InputField>().text == "" ? 0f : float.Parse(uiGameObject.InvPanel.transform.Find("Y_input").GetComponent<InputField>().text);
        z = uiGameObject.InvPanel.transform.Find("Z_input").GetComponent<InputField>().text == "" ? 0f : float.Parse(uiGameObject.InvPanel.transform.Find("Z_input").GetComponent<InputField>().text);
        roll = uiGameObject.InvPanel.transform.Find("Roll_input").GetComponent<InputField>().text == "" ? 0f : float.Parse(uiGameObject.InvPanel.transform.Find("Roll_input").GetComponent<InputField>().text);
        pitch = uiGameObject.InvPanel.transform.Find("Pitch_input").GetComponent<InputField>().text == "" ? 0f : float.Parse(uiGameObject.InvPanel.transform.Find("Pitch_input").GetComponent<InputField>().text);
        yaw = uiGameObject.InvPanel.transform.Find("Yaw_input").GetComponent<InputField>().text == "" ? 0f : float.Parse(uiGameObject.InvPanel.transform.Find("Yaw_input").GetComponent<InputField>().text);

        uiGameObject.InvPanel.transform.Find("CalcButton").GetComponent<Button>().onClick.AddListener(() => {
            CalcTheta();
        });
        uiGameObject.SolutionsDropdown.onValueChanged.AddListener((value) => {
            if(value > 0)
            {

                uiGameObject.ControlRobotScript.InstantMove = false;
                autoSelectIndex = value;
                int index = (int)solutions_index[value - 1];
                for(int j = 0; j < 6; j++)
                {
                    uiGameObject.sliders[j].value = solutions[index, j];
                }
            }
        });

        
    }
    
    void Update()
    {
        bool auto = uiGameObject.InvPanel.transform.Find("Auto").GetComponent<Toggle>().isOn;
        if(!auto)
        {
            uiGameObject.InputDropdown.value = 0;
            return;
        }
        if(uiGameObject.InputDropdown.value == 0)
        {
            mouseTracker.UseMouseTracker = false;
            mpu.EnableMpu = false;
        }
        else if(uiGameObject.InputDropdown.value == 1)
        {
            mouseTracker.UseMouseTracker = false;
            mpu.EnableMpu = true;
        }
        else if(uiGameObject.InputDropdown.value == 2)
        {
            mouseTracker.UseMouseTracker = true;
            mpu.EnableMpu = false;
        }
        
        if(mouseTracker.UseMouseTracker)
        {

            x = mouseTracker.x;
            y = mouseTracker.y;
            z = mouseTracker.z;
            roll = mouseTracker.roll;
            pitch = mouseTracker.pitch;
            yaw = mouseTracker.yaw;
        }
        else if(mpu.EnableMpu)
        {
            x = mpu.x;
            y = mpu.y;
            z = mpu.z;
            roll = mpu.roll;
            pitch = mpu.pitch;
            yaw = mpu.yaw;
        }



        uiGameObject.UpdateInvPanel(x, y, z, roll, pitch, yaw);

        CalcTheta();

        uiGameObject.SolutionsDropdown.value = autoSelectIndex;
    }
    void CalcPose()
    {
        string xStr = uiGameObject.InvPanel.transform.Find("X_input").GetComponent<InputField>().text;
        string yStr = uiGameObject.InvPanel.transform.Find("Y_input").GetComponent<InputField>().text;
        string zStr = uiGameObject.InvPanel.transform.Find("Z_input").GetComponent<InputField>().text;
        string rollStr = uiGameObject.InvPanel.transform.Find("Roll_input").GetComponent<InputField>().text;
        string pitchStr = uiGameObject.InvPanel.transform.Find("Pitch_input").GetComponent<InputField>().text;
        string yawStr = uiGameObject.InvPanel.transform.Find("Yaw_input").GetComponent<InputField>().text;
        if(!float.TryParse(xStr, out x) || !float.TryParse(yStr, out y) || !float.TryParse(zStr, out z) ||
           !float.TryParse(rollStr, out roll) || !float.TryParse(pitchStr, out pitch) || !float.TryParse(yawStr, out yaw))
        {
            return;
        }

        x = (Mathf.Abs(x) < 0.001f) ? 0f : x;
        y = (Mathf.Abs(y) < 0.001f) ? 0f : y;
        z = (Mathf.Abs(z) < 0.001f) ? 0f : z;

        roll = (Mathf.Abs(roll) < 0.001f) ? 0f : roll;
        pitch = (Mathf.Abs(pitch) < 0.001f) ? 0f : pitch;
        yaw = (Mathf.Abs(yaw) < 0.001f) ? 0f : yaw;

        float[,] R = RPYToRotationMatrix(roll, pitch, yaw);
        T06 = new float[4, 4]
        {
            {R[0, 0], R[0, 1], R[0, 2], x},
            {R[1, 0], R[1, 1], R[1, 2], y},
            {R[2, 0], R[2, 1], R[2, 2], z},
            {0, 0, 0, 1}
        };
    }
    float[,] CalcT36(float theta1, float theta2, float theta3)
    {
        T01 = Matrix.TransformMatrix(DH_Parameters.DH_Table[0, 0], DH_Parameters.DH_Table[0, 1], DH_Parameters.DH_Table[0, 2], theta1 * Mathf.Rad2Deg);
        T12 = Matrix.TransformMatrix(DH_Parameters.DH_Table[1, 0], DH_Parameters.DH_Table[1, 1], DH_Parameters.DH_Table[1, 2], theta2 * Mathf.Rad2Deg);
        T23 = Matrix.TransformMatrix(DH_Parameters.DH_Table[2, 0], DH_Parameters.DH_Table[2, 1], DH_Parameters.DH_Table[2, 2], theta3 * Mathf.Rad2Deg);

        float[,] T03 = Matrix.Multiply(Matrix.Multiply(T01, T12), T23);

        float[,] T36 = Matrix.Multiply(Matrix.InverseHomogeneous(T03), T06);

        return T36;
    }
    float[,] SolveWrist(float theta1, float theta2, float theta3)
    {
        float[,] T36 = CalcT36(theta1, theta2, theta3);
        float a24 = (Mathf.Abs(T36[1, 3]) < 0.001f) ? 0f : T36[1, 3];
        float a14 = (Mathf.Abs(T36[0, 3]) < 0.001f) ? 0f : T36[0, 3];
        float a33 = (Mathf.Abs(T36[2, 2]) < 0.001f) ? 0f : T36[2, 2];
        float a32 = (Mathf.Abs(T36[2, 1]) < 0.001f) ? 0f : T36[2, 1];
        float a31 = (Mathf.Abs(T36[2, 0]) < 0.001f) ? 0f : T36[2, 0];

        a33 = Mathf.Clamp(a33, -1f, 1f);

        float s5 = Mathf.Sqrt(Mathf.Max(0f, 1f - a33 * a33));

        float theta5Rad1_1 = Mathf.Atan2(s5, a33);
        float theta5Rad1_2 = Mathf.Atan2(-s5, a33);

        // nghiệm 1: s5 dương
        float theta4Rad1 = Mathf.Atan2(-a24, -a14);
        float theta6Rad1 = Mathf.Atan2(-a32, a31);

        // nghiệm 2: s5 âm
        float theta4Rad2 = Mathf.Atan2(a24, a14);
        float theta6Rad2 = Mathf.Atan2(a32, -a31);


        return new float[,]
        {
            { theta4Rad1, theta5Rad1_1, theta6Rad1 },
            { theta4Rad2, theta5Rad1_2, theta6Rad2 }
        };
    }
    float NormalizeDegrees(float angle)
    {
        angle = (angle + 180f) % 360f;
        if (angle < 0) angle += 360f;
        return angle - 180f;
    }
    void CalcTheta()
    {
        CalcPose();
        Vector3 P = new Vector3(x, y, z);
        Vector3 dB = new Vector3(0f, 0f, l4);

        Vector3 RdB = new Vector3(
            T06[0,0] * dB.x + T06[0,1] * dB.y + T06[0,2] * dB.z,
            T06[1,0] * dB.x + T06[1,1] * dB.y + T06[1,2] * dB.z,
            T06[2,0] * dB.x + T06[2,1] * dB.y + T06[2,2] * dB.z
        );

        Vector3 K = P - RdB;


        float theta1Rad_1 = Mathf.Atan2(K.y, K.x);
        float theta1Rad_2 = Mathf.Atan2(-K.y, -K.x); // Nghiệm đối xứng (vai trái/phải)

        float[] theta1Sols = { theta1Rad_1, theta1Rad_2 };

        // Gom tất cả nghiệm theta1-2-3
        float[,] Sols = new float[8, 3];
        int solRow = 0;

        for (int t1 = 0; t1 < 2; t1++)
        {
            float theta1Rad = theta1Sols[t1];

            float A = K.x * Mathf.Cos(theta1Rad) - d1 + K.y * Mathf.Sin(theta1Rad);
            float B = -(K.z - l1);
            float C = (d2 * d2 + l3 * l3 - A * A - B * B - l2 * l2) / (-2f * l2);
            float temp = Mathf.Sqrt(Mathf.Max(0f, A * A + B * B - C * C));

            float _theta2Rad1 = Mathf.Atan2(C, temp) - Mathf.Atan2(A, B);
            float _theta2Rad2 = Mathf.Atan2(C, -temp) - Mathf.Atan2(A, B);

            float a1 = d2;
            float b1 = -l3;

            float C1_1 = Mathf.Cos(_theta2Rad1) * A - l2 - Mathf.Sin(_theta2Rad1) * (K.z - l1);
            float C1_2 = Mathf.Cos(_theta2Rad2) * A - l2 - Mathf.Sin(_theta2Rad2) * (K.z - l1);

            float temp31 = Mathf.Sqrt(Mathf.Max(0f, a1 * a1 + b1 * b1 - C1_1 * C1_1));
            float temp32 = Mathf.Sqrt(Mathf.Max(0f, a1 * a1 + b1 * b1 - C1_2 * C1_2));

            float theta3Rad1_1 = Mathf.Atan2(C1_1, temp31) - Mathf.Atan2(a1, b1);
            float theta3Rad1_2 = Mathf.Atan2(C1_1, -temp31) - Mathf.Atan2(a1, b1);
            float theta3Rad2_1 = Mathf.Atan2(C1_2, temp32) - Mathf.Atan2(a1, b1);
            float theta3Rad2_2 = Mathf.Atan2(C1_2, -temp32) - Mathf.Atan2(a1, b1);

            Sols[solRow, 0] = theta1Rad; Sols[solRow, 1] = _theta2Rad1; Sols[solRow, 2] = theta3Rad1_1; solRow++;
            Sols[solRow, 0] = theta1Rad; Sols[solRow, 1] = _theta2Rad1; Sols[solRow, 2] = theta3Rad1_2; solRow++;
            Sols[solRow, 0] = theta1Rad; Sols[solRow, 1] = _theta2Rad2; Sols[solRow, 2] = theta3Rad2_1; solRow++;
            Sols[solRow, 0] = theta1Rad; Sols[solRow, 1] = _theta2Rad2; Sols[solRow, 2] = theta3Rad2_2; solRow++;
        }
        solutions = new float[16, 6]; // 8 bộ (theta1-2-3) × 2 cổ tay = 16 nghiệm
        int solIndex = 0;
        for(int i = 0; i < Sols.GetLength(0); i++)
        {
            float[] sol13 = { Sols[i, 0], Sols[i, 1], Sols[i, 2] };
            float[,] sol46 = SolveWrist(sol13[0], sol13[1], sol13[2]);
            float[] slol16_1 = { sol13[0], sol13[1] + Mathf.PI/2f, sol13[2], sol46[0, 0], sol46[0, 1], sol46[0, 2] };
            float[] slol16_2 = { sol13[0], sol13[1] + Mathf.PI/2f, sol13[2], sol46[1, 0], sol46[1, 1], sol46[1, 2] };
            
            for(int j = 0; j < 6; j++)
            {
                solutions[solIndex, j] = slol16_1[j];
                
            }
            solIndex++;
            for(int j = 0; j < 6; j++)
            {
                solutions[solIndex, j] = slol16_2[j];
            }
            solIndex++; 
        }

        for(int i = 0; i < solutions.GetLength(0); i++)
        {
            for(int j = 0; j < solutions.GetLength(1); j++)
            {
                solutions[i, j] = solutions[i, j] * Mathf.Rad2Deg; // Convert to degrees for better readability
                solutions[i, j] = NormalizeDegrees(solutions[i, j]);
            
            }
        }
        UpdateSolutionDropdown();
    }
    void UpdateSolutionDropdown()
    {
        int idx = 0;
        solutions_index = new float[solutions.GetLength(0)];
        uiGameObject.SolutionsDropdown.options.Clear();
        uiGameObject.SolutionsDropdown.options.Add(new Dropdown.OptionData("Select Solution"));
        
        for(int i = 0; i < solutions.GetLength(0); i++)
        {
            bool continueAdding = true;
            float[,] t06;
            float[] thetas_vector = new float[6];
            for(int j = 0; j < 6; j++)
            {
                thetas_vector[j] = solutions[i, j];
                if(thetas_vector[j] < DH_Parameters.jointLimit[j, 0] || thetas_vector[j] > DH_Parameters.jointLimit[j, 1])
                {
                    continueAdding = false;
                    break;
                }
            }
            if(!continueAdding)
            {
                continue;
            }
            forwardKinematic.Forward(thetas_vector, out t06);
            float x_f, y_f, z_f;
            x_f = t06[0, 3];
            y_f = t06[1, 3];
            z_f = t06[2, 3];
            if(Mathf.Abs(x_f - x) > 1f || Mathf.Abs(y_f - y) > 1f || Mathf.Abs(z_f - z) > 1f)
            {
                continue;
            }
            solutions_index[idx++] = i;
            string optionText = $"Solution {i+1}: ";
            for(int j = 0; j < solutions.GetLength(1); j++)
            {
                optionText += $"θ{j+1}={solutions[i, j]:F1}° ";
            }
            uiGameObject.SolutionsDropdown.options.Add(new Dropdown.OptionData(optionText));
        }
        uiGameObject.SolutionsDropdown.value = 0;
    }
    public static float[,] RPYToRotationMatrix(float rollDeg,
                                           float pitchDeg,
                                           float yawDeg)
    {
        float roll = rollDeg * Mathf.Deg2Rad;
        float pitch = pitchDeg * Mathf.Deg2Rad;
        float yaw = yawDeg * Mathf.Deg2Rad;

        float cr = Mathf.Cos(roll);
        float sr = Mathf.Sin(roll);

        float cp = Mathf.Cos(pitch);
        float sp = Mathf.Sin(pitch);

        float cy = Mathf.Cos(yaw);
        float sy = Mathf.Sin(yaw);

        return new float[,]
        {
            {
                cy * cp,
                cy * sp * sr - sy * cr,
                cy * sp * cr + sy * sr
            },
            {
                sy * cp,
                sy * sp * sr + cy * cr,
                sy * sp * cr - cy * sr
            },
            {
                -sp,
                cp * sr,
                cp * cr
            }
        };
    }
}