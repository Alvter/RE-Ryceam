using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Pool;

public class Global : MonoBehaviour
{
    // 实例化
    private static Global _instance;
    public static Global Instance { get { return _instance; } }

    public List<List<SpriteRenderer>> TrackSprites = new();
    public List<SpriteRenderer> TrackSprite = new();

    public List<List<Image>> TrackImages = new();
    public List<Image> TrackImage = new();

    public List<List<Transform>> TrackChildTsforms = new();
    public List<Transform> TrackTsforms = new();

    // 对象池
    public GameObject tap;
    public GameObject drag;
    public GameObject flick;
    public GameObject dFlick;
    public GameObject hold;
    public GameObject holdBody;

    public ObjectPool<GameObject> TapPool { get; private set; }
    public ObjectPool<GameObject> DragPool { get; private set; }
    public ObjectPool<GameObject> FlickPool { get; private set; }
    public ObjectPool<GameObject> DFlickPool { get; private set; }
    public ObjectPool<GameObject> HoldPool { get; private set; }

    public float holdHeight;
    

    void Awake()
    {
        _instance = this;

        TapPool = CreatePool(tap);
        DragPool = CreatePool(drag);
        FlickPool = CreatePool(flick);
        DFlickPool = CreatePool(dFlick);
        HoldPool = CreatePool(hold);

        SpriteRenderer holdSprite = holdBody.GetComponent<SpriteRenderer>();
        holdHeight = holdSprite.sprite.bounds.size.y;//hold身所占y方向单位长度
        // holdHeight = hold.GetComponent<RectTransform>().rect.height;
    }

    private ObjectPool<GameObject> CreatePool(GameObject prefab)
    {
        return new ObjectPool<GameObject>(
            createFunc: () => Instantiate(prefab),
            actionOnGet: obj => obj.SetActive(true),
            actionOnRelease: obj => obj.SetActive(false),
            actionOnDestroy: obj => Destroy(obj),
            collectionCheck: true,
            defaultCapacity: 10,
            maxSize: 20
        );
    }
    
}
