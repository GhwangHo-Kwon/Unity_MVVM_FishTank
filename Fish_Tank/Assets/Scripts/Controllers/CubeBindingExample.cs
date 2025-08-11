using UnityEngine;
using R3;

public class CubeBindingExample : MonoBehaviour
{
    [Header("정적 Cube")]
    [SerializeField] private CubeView staticCubeView; // 씬에 미리 배치된 Cube

    [Header("동적 Cube 프리팹")]
    [SerializeField] private CubeView cubePrefab;     // Project에 저장된 Cube Prefab
    [SerializeField] private Transform spawnRoot;     // 동적 생성 위치 부모

    private CubeViewModel _staticVM;
    private CubeViewModel _dynamicVM;
    private DisposableBag _bag = new();

    void Start()
    {
        // === 정적 바인딩 ===
        // 씬에 있는 Cube의 현재 위치를 기반으로 ViewModel 생성
        _staticVM = new CubeViewModel(staticCubeView.transform.position);
        staticCubeView.Bind(_staticVM);

        // === 동적 바인딩 ===
        // Cube 프리팹을 Instantiate 후 ViewModel 연결
        var dynamicCube = Instantiate(cubePrefab, spawnRoot);
        _dynamicVM = new CubeViewModel(new Vector3(2, 0, 0)); // 시작 위치 (2,0,0)
        dynamicCube.Bind(_dynamicVM);
    }

    void Update()
    {
        // Space → 정적 Cube 이동
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _staticVM.MoveUp();
        }

        // Enter → 동적 Cube 이동
        if (Input.GetKeyDown(KeyCode.Return))
        {
            _dynamicVM.MoveUp();
        }
    }

    private void OnDestroy()
    {
        _bag.Dispose();
    }
}