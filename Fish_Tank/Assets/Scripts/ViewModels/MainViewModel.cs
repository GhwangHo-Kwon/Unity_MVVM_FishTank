using R3;
using UnityEngine;

public class MainViewModel
{
    public ReadOnlyReactiveProperty<Vector3> Position => _model.Position;

    private MainModel _model;

    public MainViewModel(MainModel model)
    {
        _model = model;
    }

    public void MoveUp()
    {
        _model.SetPosition(_model.Position.Value + Vector3.up);
    }
}