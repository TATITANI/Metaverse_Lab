using UnityEngine.UI;
using TMPro;
using UnityEngine;


public class GaugeUI : MonoBehaviour
{
   private Image img;
   private TextMeshProUGUI text;

   private void Awake()
   {
      img = GetComponentInChildren<Image>();
      text = GetComponentInChildren<TextMeshProUGUI>();
   }

   /// <param name="pressure"> 0~1 </param>
   public void SetState(float pressure)
   {
        
            img.fillAmount = pressure;
            text.text = $"{pressure*100:0.0}";
   }
}
