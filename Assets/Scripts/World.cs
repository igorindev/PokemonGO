using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

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

    public GameObject psShow;
    public Image image;
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
        //yield return Addressables.LoadContentCatalogAsync(r.Result.ToString());
        yield return null;

        //StartCoroutine(LoadAddressables("WorldFile"));

        for (int i = 0; i < pokestops.Length; i++)
        {
            //if ((playerTransform.position - pokestops[i].location).sqrMagnitude <= pokestopRange * pokestopRange)
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
    public void OpenPokestop()
    {
        psShow.SetActive(true);
    }

    public IEnumerator LoadAddressablesPokemons(string key, Vector3 pos, int id)
    {
        AsyncOperationHandle<long> ds = Addressables.GetDownloadSizeAsync(key);
        yield return ds;
        Debug.Log("KEY: " + key + " | Size: " + ds.Result);

        //Load a GameObject
        AsyncOperationHandle<GameObject> handler = Addressables.InstantiateAsync(key, pos, Quaternion.identity);
        yield return handler;
        pokemons[id].pokemon = handler.Result;
        pokemons[id].pokemon.GetComponent<Pokemon>().id = id;
        pokemons[id].pokemon.GetComponent<Pokemon>().handler = handler;
    }
    public IEnumerator LoadAddressablesPokestop(string key, Vector3 pos, string spriteKey)
    {
        AsyncOperationHandle<long> ds = Addressables.GetDownloadSizeAsync(key);
        yield return ds;
        Debug.Log("KEY: " + key + " | Size: " + ds.Result);
        //Load a GameObject
        AsyncOperationHandle<GameObject> handler = Addressables.LoadAssetAsync<GameObject>(key);
        yield return handler;
        if (handler.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject obj = Instantiate(handler.Result, pos, Quaternion.identity);
            obj.GetComponent<Pokestop>().spriteKey = spriteKey;
        }
    }
    public AsyncOperationHandle<Sprite> currentOpenPokestop;
    public IEnumerator LoadAddressablesPokestopSprite(Pokestop ps, string key)
    {
        AsyncOperationHandle<long> ds = Addressables.GetDownloadSizeAsync(key);
        yield return ds;
        Debug.Log("KEY: " + key + " | Size: " + ds.Result);

        //Load a GameObject
        AsyncOperationHandle<Sprite> handler = Addressables.LoadAssetAsync<Sprite>(key);
        yield return handler;
        if (handler.Status == AsyncOperationStatus.Succeeded)
        {
            image.sprite = handler.Result;
        }
        currentOpenPokestop = handler;
    }
    public void ClosePokestop()
    {
        psShow.SetActive(false);
        image.sprite = null;
        Addressables.Release(currentOpenPokestop);
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        StartCoroutine(ClearC());
    }
    public IEnumerator ClearC()
    {
        yield return Resources.UnloadUnusedAssets();
        Debug.LogWarning("Clear");
    }
}
