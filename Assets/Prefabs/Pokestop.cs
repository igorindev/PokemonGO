using UnityEngine;

public class Pokestop : MonoBehaviour
{
    public string spriteKey;
    public SpriteRenderer spriteRenderer;

    public void Interact()
    {
        StartCoroutine(World.instance.LoadAddressablesPokestopSprite(this, spriteKey));
    }
}
