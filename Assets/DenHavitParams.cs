using UnityEngine;
public class DenHavitParams
{
    public float d1 = 150f;
    public float d2 = 40f;
    public float l1 = 544f;
    public float l2 = 575f;
    public float l3 = 675f;
    public float l4 = 90f;
    public float[,] DH_Table;
    public float[,] jointLimit = new float[6, 2]
    {
        {-180f, 180f},
        {-95f, 155f},
        {-210f, 69f},
        {-230f, 230f},
        {-130f, 130f},
        {-400f, 400f}
    };
    public DenHavitParams()
    {
        DH_Table = new float[6, 3]
        {
            { d1, -90f, l1 },
            { l2, 0f, 0f },
            { d2, -90f, 0f },
            { 0f, 90f, l3 },
            { 0f, -90f, 0f },
            { 0f, 0f, l4 }
        };
    }
}