using UnityEngine;
using R3;

public class CubeView : MonoBehaviour
{
    private CubeViewModel _vm;
    private DisposableBag _bag = new();

    // ViewModel을 연결하는 메서드
    public void Bind(CubeViewModel vm)
    {
        // 기존 바인딩 해제 (재사용 시 안전)
        _bag.Dispose();
        _bag = new();

        _vm = vm;

        // ViewModel.Position 값이 바뀔 때마다 Transform.position 업데이트
        _vm.Position
           .DistinctUntilChanged()
           .Subscribe(pos => transform.position = pos)
           .AddTo(ref _bag);
    }

    private void OnDestroy()
    {
        // 오브젝트 삭제 시 구독 해제
        _bag.Dispose();
    }
}