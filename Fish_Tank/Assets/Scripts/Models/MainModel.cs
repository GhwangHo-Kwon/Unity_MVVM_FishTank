using R3;
using UnityEngine;

public class MainModel
{
    public ReactiveProperty<Vector3> Position { get; private set; }

    public MainModel(Vector3 startPos)
    {
        Position = new ReactiveProperty<Vector3>(startPos);
    }

    public void SetPosition(Vector3 newPos)
    {
        Position.Value = newPos;
    }
}