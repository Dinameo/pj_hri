using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ForwardKinematic : MonoBehaviour
{
    public DenHavitParams DH_Parameters = new DenHavitParams();
    public UI uiGameObject;
    public GameObject Robot;
    

    float[] thetas = new float[6];
    public float[,] T01, T12, T23, T34, T45, T56, T06;
    float x, y, z, roll, pitch, yaw;
    void GetTheta(out float[] thetas_vector)
    {
        thetas_vector = new float[6];
        for(int i = 0; i < 6; i++)
        {
            thetas_vector[i] = uiGameObject.GetComponent<UI>().sliders[i].value;
        }
        
    }
    void Start()
    {
        for(int idx = 0; idx < 6; idx++)
        {
            int index = idx;
            uiGameObject.sliders[index].onValueChanged.AddListener((value) => {
                uiGameObject.OnChangeSliderValue(index, value);
                ForwardUpdate();
            });
            uiGameObject.valueDisplay[index].onEndEdit.AddListener((value) => {
                uiGameObject.OnChangeInputFieldValue(index, value);
                ForwardUpdate();
            });
        }
        ForwardUpdate();
        
        
        
    }
    void ForwardUpdate()
    {
        bool ani = !uiGameObject.ManualControlPanel.transform.Find("AnimationCheck").GetComponent<Toggle>().isOn;
        GetTheta(out thetas);
        Forward(thetas, out T06);
        x = T06[0, 3];
        y = T06[1, 3];
        z = T06[2, 3];
        CalcPose(T06, out roll, out pitch, out yaw);
        uiGameObject.UpdatePosePanelOrientation(roll, pitch, yaw);

        uiGameObject.ControlRobotScript.InstantMove = ani;

    }
    public void Forward(float[] thetas_vector, out float[,] t06)
    {
        float[] q = new float[6];

        for(int i = 0; i < 6; i++)
        {
            q[i] = thetas_vector[i];
        }

        q[1] -= 90f;
        float[,] t01, t12, t23, t34, t45, t56;
        t01 = Matrix.TransformMatrix(DH_Parameters.DH_Table[0, 0], DH_Parameters.DH_Table[0, 1], DH_Parameters.DH_Table[0, 2], q[0]);
        t12 = Matrix.TransformMatrix(DH_Parameters.DH_Table[1, 0], DH_Parameters.DH_Table[1, 1], DH_Parameters.DH_Table[1, 2], q[1]);
        t23 = Matrix.TransformMatrix(DH_Parameters.DH_Table[2, 0], DH_Parameters.DH_Table[2, 1], DH_Parameters.DH_Table[2, 2], q[2]);
        t34 = Matrix.TransformMatrix(DH_Parameters.DH_Table[3, 0], DH_Parameters.DH_Table[3, 1], DH_Parameters.DH_Table[3, 2], q[3]);
        t45 = Matrix.TransformMatrix(DH_Parameters.DH_Table[4, 0], DH_Parameters.DH_Table[4, 1], DH_Parameters.DH_Table[4, 2], q[4]);
        t56 = Matrix.TransformMatrix(DH_Parameters.DH_Table[5, 0], DH_Parameters.DH_Table[5, 1], DH_Parameters.DH_Table[5, 2], q[5]);
        t06 = Matrix.Multiply(
            Matrix.Multiply(
                Matrix.Multiply(
                    Matrix.Multiply(t01, t12), 
                    t23
                ), 
                t34
            ), 
            Matrix.Multiply(t45, t56)
        );    
    }
    void CalcPose(float[,] t06, out float roll, out float pitch, out float yaw)
    {
        float r11 = Mathf.Abs(t06[0, 0]) < 1e-6f ? 0f : t06[0, 0];
        float r21 = Mathf.Abs(t06[1, 0]) < 1e-6f ? 0f : t06[1, 0];
        float r31 = Mathf.Abs(t06[2, 0]) < 1e-6f ? 0f : t06[2, 0];
        float r32 = Mathf.Abs(t06[2, 1]) < 1e-6f ? 0f : t06[2, 1];
        float r33 = Mathf.Abs(t06[2, 2]) < 1e-6f ? 0f : t06[2, 2];

        float r13 = Mathf.Abs(t06[0, 2]) < 1e-6f ? 0f : t06[0, 2];
        float r23 = Mathf.Abs(t06[1, 2]) < 1e-6f ? 0f : t06[1, 2];

        pitch = Mathf.Asin(-r31);

        if (Mathf.Abs(Mathf.Cos(pitch)) < 1e-4f)
        {
            roll = 0f;

            if (r31 < 0) // pitch = +90
                yaw = Mathf.Atan2(r23, r13);
            else         // pitch = -90
                yaw = Mathf.Atan2(-r23, -r13);
        }
        else
        {
            roll = Mathf.Atan2(r32, r33);
            yaw = Mathf.Atan2(r21, r11);
        }

        roll *= Mathf.Rad2Deg;
        pitch *= Mathf.Rad2Deg;
        yaw *= Mathf.Rad2Deg;
    }

}