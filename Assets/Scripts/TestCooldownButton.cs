using UnityEngine;

public class TestCooldownButton : MonoBehaviour
{
    public CooldownUI cooldownUI;
    public float cooldownTime = 3f;

    public void ActivateCooldown()
    {
        cooldownUI.StartCooldown(cooldownTime);
    }
}
