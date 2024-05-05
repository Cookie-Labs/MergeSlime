using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Redcode.Pools;

public class SpawnManager : Singleton<SpawnManager>
{
    public CameraBound camBound;

    protected override void Awake()
    {
        base.Awake();

        camBound = new CameraBound();
        camBound.SetCameraBound();
    }

    private void Start()
    {
        StartCoroutine(CloudSpawnRoutine());
        StartCoroutine(StarSpawnRoutine());
    }

    #region Cloud
    private void SpawnCloud()
    {
        string cloudName = $"Cloud_{Random.Range(0, 3)}";
        Transform cloudTrans = PoolManager.Instance.GetFromPool<Transform>(cloudName);
        cloudTrans.name = cloudName;

        float x = camBound.Left - cloudTrans.localScale.x;
        float y = Random.Range(camBound.Bottom + cloudTrans.localScale.y, camBound.Top - cloudTrans.localScale.y);

        cloudTrans.position = new Vector2(x, y);

        cloudTrans.DOLocalMoveX(camBound.Right + cloudTrans.localScale.x, 5f).SetUpdate(true).SetEase(Ease.Linear).OnComplete(() => DeSpawnCloud(cloudName, cloudTrans));
    }

    private void DeSpawnCloud(string name,Transform trans)
    {
        PoolManager.Instance.TakeToPool<Transform>(name, trans);
    }

    private IEnumerator CloudSpawnRoutine()
    {
        SpawnCloud();

        yield return new WaitForSeconds(Random.Range(0.5f, 1.2f));

        StartCoroutine(CloudSpawnRoutine());
    }
    #endregion

    #region Star
    private void SpawnStar()
    {
        Transform starTrans = PoolManager.Instance.GetFromPool<Transform>("Star");

        float x = camBound.RanWidthBoundary();
        float y = camBound.RanHeightBoudnary();
        float scale = Random.Range(0.1f, 0.5f);

        starTrans.position = new Vector2(x, y);
        starTrans.rotation = Quaternion.Euler(0, 0, Random.Range(-180f, 180f));
        starTrans.localScale = Vector3.zero;

        Sequence starSeq = DOTween.Sequence().SetUpdate(true);
        starSeq.Append(starTrans.DOScale(scale, 1f).SetEase(Ease.Linear))
            .AppendInterval(1f)
            .Append(starTrans.DOScale(0f, 1f).SetEase(Ease.Linear))
            .AppendCallback(() => DeSpawnStar(starTrans));
    }

    private void DeSpawnStar(Transform trans)
    {
        PoolManager.Instance.TakeToPool<Transform>("Star", trans);
    }

    private IEnumerator StarSpawnRoutine()
    {
        SpawnStar();

        yield return new WaitForSeconds(Random.Range(0.5f, 1.2f));

        StartCoroutine(StarSpawnRoutine());
    }
    #endregion

    #region Slime
    public Slime SpawnSlime(int level, Vector2 vec2)
    {
        Slime slime = PoolManager.Instance.GetFromPool<Slime>("Slime");

        slime.transform.position = vec2;
        slime.SetSlime(level);
        SpawnPop(slime.transform);

        return slime;
    }
    #endregion

    #region VFX
    public VFX_Pop SpawnPop(Transform parent)
    {
        VFX_Pop pop = PoolManager.Instance.GetFromPool<VFX_Pop>("VFX_Pop");

        pop.transform.position = parent.transform.position;
        Vector3 scale = parent.transform.localScale;
        pop.transform.localScale = new Vector3(scale.x + 0.3f, scale.y + 0.3f, scale.z + 0.3f);

        return pop;
    }

    public void DeSpawnPop(VFX_Pop pop)
    {
        PoolManager.Instance.TakeToPool<VFX_Pop>("VFX_Pop", pop);
    }
    #endregion
}

public class CameraBound
{
    public RectTransform BGCanvasRect;

    private Camera camera;
    private float size_x, size_y;

    public void SetCameraBound()
    {
        camera = Camera.main;

        size_y = camera.orthographicSize;
        size_x = camera.orthographicSize * Screen.width / Screen.height;
    }

    public float RanWidthBoundary()
    {
        return Random.Range(Left + 1, Right - 1);
    }

    public float RanHeightBoudnary()
    {
        return Random.Range(Bottom + 1, Top - 1);
    }

    public float Bottom
    {
        get
        {
            return size_y * -1 + camera.gameObject.transform.position.y;
        }
    }

    public float Top
    {
        get
        {
            return size_y + camera.gameObject.transform.position.y;
        }
    }

    public float Left
    {
        get
        {
            return size_x * -1 + camera.gameObject.transform.position.x;
        }
    }

    public float Right
    {
        get
        {
            return size_x + camera.gameObject.transform.position.x;
        }
    }

    public float Height
    {
        get
        {
            return size_y * 2;
        }
    }

    public float Width
    {
        get
        {
            return size_x * 2;
        }
    }
}