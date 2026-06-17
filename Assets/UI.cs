using UnityEngine;
using UnityEngine.UI;
public class UI : MonoBehaviour
{
    public ControlRobot ControlRobotScript;
    public GameObject ManualControlPanel;
    public DenHavitParams DenHavitParams = new DenHavitParams();
    [HideInInspector]
    public Slider[] sliders = new Slider[6];
    [HideInInspector]
    public InputField[] valueDisplay = new InputField[6];
    public Text DebugText;
    public GameObject PosePanel;
    public GameObject InvPanel;
    public Dropdown SolutionsDropdown;

    public GameObject Robot;
    public GameObject EndEffector;

    void Start()
    {

        for (int i = 0; i < 6; i++)
        {
            sliders[i] = ManualControlPanel.transform
                .Find("Slider" + (i + 1))
                .GetComponent<Slider>();
            valueDisplay[i] = ManualControlPanel.transform
                .Find("displayTheta" + (i + 1))
                .GetComponent<InputField>();
        }

        InvPanel.transform.Find("GetButton").GetComponent<Button>().onClick.AddListener(OnClickButtonGet);

        for(int idx = 0; idx < 6; idx++)
        {
            int index = idx;
            sliders[index].minValue = DenHavitParams.jointLimit[index, 0];
            sliders[index].maxValue = DenHavitParams.jointLimit[index, 1];
            sliders[index].value = 0f;
            sliders[index].value = 0f; 
            valueDisplay[index].text = 0f.ToString();
        }

        SolutionsDropdown.options.Clear();
        SolutionsDropdown.options.Add(new Dropdown.OptionData("Select Solution"));

        InvPanel.transform.Find("X_input").GetComponent<InputField>().text = "915.0";
        InvPanel.transform.Find("Y_input").GetComponent<InputField>().text = "0";
        InvPanel.transform.Find("Z_input").GetComponent<InputField>().text = "1159.000";

        InvPanel.transform.Find("Roll_input").GetComponent<InputField>().text = "0";
        InvPanel.transform.Find("Pitch_input").GetComponent<InputField>().text = "-90";
        InvPanel.transform.Find("Yaw_input").GetComponent<InputField>().text = "0";

        ControlRobotScript.FirstStart(Robot);
    }
    public void OnChangeSliderValue(int index, float value)
    {
        valueDisplay[index].text = value.ToString("F3");
        ControlRobotScript.SetTargetAngles(new float[] {
            sliders[0].value,
            sliders[1].value,
            sliders[2].value,
            sliders[3].value,
            sliders[4].value,
            sliders[5].value
        });
    }
    public void OnChangeInputFieldValue(int index, string value)
    {
        if(float.TryParse(value, out float parsedValue))
        {
            if(parsedValue < sliders[index].minValue)
            {
                parsedValue = sliders[index].minValue;
            }
            else if(parsedValue > sliders[index].maxValue)
            {
                parsedValue = sliders[index].maxValue;
            }
            sliders[index].value = parsedValue;
        }
        else
        {
            valueDisplay[index].text = sliders[index].value.ToString("F3");
        }
        ControlRobotScript.SetTargetAngles(new float[] {
            sliders[0].value,
            sliders[1].value,
            sliders[2].value,
            sliders[3].value,
            sliders[4].value,
            sliders[5].value
        });
    }
    public void GetPose(out float x, out float y, out float z, out float roll, out float pitch, out float yaw)
    {
        x = float.Parse(PosePanel.transform.Find("X_pos").GetComponent<InputField>().text);
        y = float.Parse(PosePanel.transform.Find("Y_pos").GetComponent<InputField>().text);
        z = float.Parse(PosePanel.transform.Find("Z_pos").GetComponent<InputField>().text);
        roll = float.Parse(PosePanel.transform.Find("Roll_pos").GetComponent<InputField>().text);
        pitch = float.Parse(PosePanel.transform.Find("Pitch_pos").GetComponent<InputField>().text);
        yaw = float.Parse(PosePanel.transform.Find("Yaw_pos").GetComponent<InputField>().text);

        
    }
    public void UpdateInvPanel(float x, float y, float z, float roll, float pitch, float yaw)
    {
        InvPanel.transform.Find("X_input").GetComponent<InputField>().text = x.ToString("F3");
        InvPanel.transform.Find("Y_input").GetComponent<InputField>().text = y.ToString("F3");
        InvPanel.transform.Find("Z_input").GetComponent<InputField>().text = z.ToString("F3");
        InvPanel.transform.Find("Roll_input").GetComponent<InputField>().text = roll.ToString("F3");
        InvPanel.transform.Find("Pitch_input").GetComponent<InputField>().text = pitch.ToString("F3");
        InvPanel.transform.Find("Yaw_input").GetComponent<InputField>().text = yaw.ToString("F3");
    }
    public void UpdatePosePanel(float x, float y, float z, float roll, float pitch, float yaw)
    {
        UpdatePosePanelPosition(x, y, z);
        UpdatePosePanelOrientation(roll, pitch, yaw);
    }
    public void UpdatePosePanelPosition(float x, float y, float z)
    {
        PosePanel.transform.Find("X_pos").GetComponent<InputField>().text = x.ToString("F3");
        PosePanel.transform.Find("Y_pos").GetComponent<InputField>().text = y.ToString("F3");
        PosePanel.transform.Find("Z_pos").GetComponent<InputField>().text = z.ToString("F3");
    }
    public void UpdatePosePanelOrientation(float roll, float pitch, float yaw)
    {
        PosePanel.transform.Find("Roll_pos").GetComponent<InputField>().text = roll.ToString("F3");
        PosePanel.transform.Find("Pitch_pos").GetComponent<InputField>().text = pitch.ToString("F3");
        PosePanel.transform.Find("Yaw_pos").GetComponent<InputField>().text = yaw.ToString("F3");
    }
    public void OnClickButtonGet()
    {
        float x, y, z, roll, pitch, yaw;
        GetPose(out x, out y, out z, out roll, out pitch, out yaw);
        UpdateInvPanel(x, y, z, roll, pitch, yaw);
    }

}