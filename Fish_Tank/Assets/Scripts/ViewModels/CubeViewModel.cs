using R3;
using UnityEngine;

public class CubeViewModel
{
    // 위치 값 (View와 양방향 바인딩 가능)
    public ReactiveProperty<Vector3> Position { get; } = new();

    public CubeViewModel(Vector3 startPos)
    {
        Position.Value = startPos;
    }

    // Cube를 위로 올리는 메서드
    public void MoveUp()
    {
        Position.Value += Vector3.up;
    }
}