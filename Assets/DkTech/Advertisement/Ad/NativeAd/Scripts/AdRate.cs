using UnityEngine;
using UnityEngine.UI;

public class AdRate : MonoBehaviour
{
    [SerializeField] Image[] star;
    public void SetRate(float rate)
    {
        for(int i=0;i<star.Length; i++)
        {
            if (rate >= i + 1)
            {
                star[i].fillAmount = 1;
            }else if (rate > i && rate < i + 1)
            {
                star[i].fillAmount = rate - i;
            }
            else
            {
                star[i].fillAmount = 0;
            }
        }
    }
}
