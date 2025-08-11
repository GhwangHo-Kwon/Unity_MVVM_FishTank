using UnityEngine;
using R3;

public class CubeBindingExample : MonoBehaviour
{
    [Header("정적 Cube (씬에 존재 시 드래그, 없으면 자동 생성)")]
    [SerializeField] private CubeView staticCubeView;

    [Header("동적 Cube 프리팹 (비워두면 런타임에 기본 Cube 생성)")]
    [SerializeField] private CubeView cubePrefab;

    [Header("스폰 루트(없으면 this.transform)")]
    [SerializeField] private Transform spawnRoot;

    private CubeViewModel _staticVM;
    private CubeViewModel _dynamicVM;

    void Start()
    {
        if (spawnRoot == null) spawnRoot = this.transform;

        // === 정적 바인딩: 없으면 자동 생성 ===
        if (staticCubeView == null)
        {
            // 씬에 이미 CubeView가 있으면 찾아서 사용
            staticCubeView = FindFirstObjectByType<CubeView>();
            if (staticCubeView == null)
            {
                // 없으면 새로 만든다
                var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.name = "StaticCube_Auto";
                staticCubeView = go.AddComponent<CubeView>();
                go.transform.position = Vector3.zero;
            }
        }
        _staticVM = new CubeViewModel(staticCubeView.transform.position);
        staticCubeView.Bind(_staticVM);

        // === 동적 바인딩: 프리팹 없으면 즉석 생성 ===
        if (cubePrefab != null)
        {
            var dyn = Instantiate(cubePrefab, spawnRoot);
            _dynamicVM = new CubeViewModel(new Vector3(2, 0, 0));
            dyn.Bind(_dynamicVM);
        }
        else
        {
            // 프리팹이 없으면 기본 Cube를 만들어 바인딩
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "DynamicCube_Auto";
            go.transform.SetParent(spawnRoot, false);
            go.transform.position = new Vector3(2, 0, 0);
            var dynView = go.AddComponent<CubeView>();

            _dynamicVM = new CubeViewModel(go.transform.position);
            dynView.Bind(_dynamicVM);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) _staticVM?.MoveUp();
        if (Input.GetKeyDown(KeyCode.Return)) _dynamicVM?.MoveUp();
    }
}