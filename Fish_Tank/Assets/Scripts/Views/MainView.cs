using R3;
using UniRx;
using UnityEngine;

public class MainView : MonoBehaviour
{
    DisposableBag disposable;

    private MainViewModel _viewModel;

    void Start()
    {
        var model = new MainModel(transform.position);
        _viewModel = new MainViewModel(model);

        // ViewModel의 Position 변경 시 View 업데이트
        _viewModel.Position.Subscribe(pos => transform.position = pos).AddTo(ref disposable);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _viewModel.MoveUp();
        }
    }
}