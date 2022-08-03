using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Pokestop : MonoBehaviour
{
    public string spriteKey;
    public SpriteRenderer spriteRenderer;

    public void Interact()
    {
        if (World.instance.psShow.activeSelf) return;
        World.instance.OpenPokestop();
        StartCoroutine(World.instance.LoadAddressablesPokestopSprite(this, spriteKey));
    }
}
