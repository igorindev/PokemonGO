using System.Collections;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
//using Firebase.Storage;

[Serializable]
public struct PokestopsRef
{
    public Vector3 location;
    public string key;
}
[Serializable]
public struct PokemonsRef
{
    public Vector3 pos;
    public string key;
    public bool loaded;
    public GameObject pokemon;
}
[Serializable]
public struct Pokestops
{
    public Sprite location;
}
[Serializable]
public struct Pokemons
{
    public GameObject pokemon;
}

public class World : MonoBehaviour
{
    public static World instance;

    public float pokestopRange = 10;
    public float interactionRange = 3;
    public float despawnRange = 5;

    public PokestopsRef[] pokestops;
    public PokemonsRef[] pokemons;

    public Transform playerTransform;

    public float worldLoadDelay = 3;
    float delay;

    [SerializeField] string loadResult;

    private void Awake()
    {
        instance = this;
    }

    IEnumerator Start()
    {
        //Load the file containing the positions
        // StorageReference reference = FirebaseStorage.DefaultInstance.GetReferenceFromUrl("gs://learnaddressables.appspot.com/");
        // var r = reference.GetDownloadUrlAsync();
        // yield return r;
        //
        // yield return Addressables.LoadContentCatalogAsync(r.Result.ToString());
        yield return null;

        //StartCoroutine(LoadAddressables("WorldFile"));

        for (int i = 0; i < pokestops.Length; i++)
        {
            if ((playerTransform.position - pokestops[i].location).sqrMagnitude <= pokestopRange * pokestopRange)
            {
                StartCoroutine(LoadAddressablesPokestop("Pokestop", pokestops[i].location, pokestops[i].key));
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 10000))
            {
                Debug.DrawRay(ray.origin, ray.direction * 10000);
                //Select stage    
                if (hit.transform.TryGetComponent(out Pokestop pokestop))
                {
                    pokestop.Interact();
                }
            }
        }

        if (Time.time < delay) { return; }
        delay = Time.time + worldLoadDelay - (Time.time - delay);

        Debug.Log("Area Search Update");

        for (int i = 0; i < pokemons.Length; i++)
        {
            if (pokemons[i].loaded == false && (playerTransform.position - pokemons[i].pos).sqrMagnitude <= interactionRange * interactionRange)
            {
                pokemons[i].loaded = true;
                StartCoroutine(LoadAddressablesPokemons(pokemons[i].key, pokemons[i].pos, i));
            }
            else if ((playerTransform.position - pokemons[i].pos).sqrMagnitude > despawnRange * despawnRange)
            {
                Destroy(pokemons[i].pokemon);
            }
        }
    }
    public IEnumerator LoadAddressables(string key)
    {
        //Debug.Log("KEY: " + key + " | Size: " + Addressables.GetDownloadSizeAsync(key));

        //Load a GameObject
        AsyncOperationHandle<TextAsset> handler = Addressables.LoadAssetAsync<TextAsset>(key);
        yield return handler;
        if (handler.Status == AsyncOperationStatus.Succeeded)
        {
            loadResult = handler.Result.text;
        }

        Addressables.Release(handler);
    }
    public IEnumerator LoadAddressablesPokemons(string key, Vector3 pos, int id)
    {
        Debug.Log("KEY: " + key + " | Size: " + Addressables.GetDownloadSizeAsync(key).Result);

        //Load a GameObject
        AsyncOperationHandle<GameObject> handler = Addressables.LoadAssetAsync<GameObject>(key);
        yield return handler;
        if (handler.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject obj = Instantiate(handler.Result, pos, Quaternion.identity);
            obj.GetComponent<Pokemon>().id = id;
            pokemons[id].pokemon = obj;
        }

        Addressables.Release(handler);
    }
    public IEnumerator LoadAddressablesPokestop(string key, Vector3 pos, string spriteKey)
    {
        Debug.Log("KEY: " + key + " | Size: " + Addressables.GetDownloadSizeAsync(key).Result);
        //Load a GameObject
        AsyncOperationHandle<GameObject> handler = Addressables.LoadAssetAsync<GameObject>(key);
        yield return handler;
        if (handler.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject obj = Instantiate(handler.Result, pos, Quaternion.identity);
            obj.GetComponent<Pokestop>().spriteKey = spriteKey;
        }

        Addressables.Release(handler);
    }
    public IEnumerator LoadAddressablesPokestopSprite(Pokestop ps, string key)
    {
        Debug.Log("KEY: " + key + " | Size: " + Addressables.GetDownloadSizeAsync(key).Result);

        //Load a GameObject
        AsyncOperationHandle<Sprite> handler = Addressables.LoadAssetAsync<Sprite>(key);
        yield return handler;
        if (handler.Status == AsyncOperationStatus.Succeeded)
        {
            ps.spriteRenderer.sprite = handler.Result;
        }

        Addressables.Release(handler);
    }
}
