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

   public void SetState(float pressure)
   {
      // img.fillAmount = percent * 0.01f;
      text.text = $"{pressure}";
   }
}
