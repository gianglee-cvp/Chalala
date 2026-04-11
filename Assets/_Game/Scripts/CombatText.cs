using UnityEngine;

public class CombatText : MonoBehaviour
{
    [SerializeField]  private TMPro.TextMeshProUGUI text;

    public void OnInit(float damage)
    {
        text.text = damage.ToString();
        Invoke(nameof(Despawn), 1f);
    }

    public void Despawn()
    {
        Destroy(gameObject);
    }   
}
