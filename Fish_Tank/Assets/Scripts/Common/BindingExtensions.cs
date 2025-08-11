// Unity uGUI/TMP <-> R3 ReactiveProperty 바인딩 헬퍼
// 사용 예시는 파일 하단 참고
using R3;
using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public static class BindingExtensions
{
    // 내부: Dispose 시 콜백 호출용
    private sealed class CallbackDisposable : IDisposable
    {
        private Action _onDispose;
        public CallbackDisposable(Action onDispose) => _onDispose = onDispose;
        public void Dispose() { _onDispose?.Invoke(); _onDispose = null; }
    }

    // ============ 공통 ============

    // ReadOnly RP -> Action 바인딩 (ObserveOnMainThread를 호출하고 싶다면 이 라인에서 체인해도 됨)
    public static IDisposable Bind<T>(
        this IReadOnlyReactiveProperty<T> source,
        Action<T> onNext)
        => source.Subscribe(onNext);

    // IDisposable -> DisposableBag 등록
    public static void AddTo(this IDisposable d, ref DisposableBag bag) => bag.Add(d);

    // ============ Text / Label (단방향: VM -> View) ============

    public static IDisposable BindText(this TextMeshProUGUI label,
        IReadOnlyReactiveProperty<string> source,
        bool distinctUntilChanged = true)
    {
        var stream = distinctUntilChanged ? source.DistinctUntilChanged() : source;
        return stream.Subscribe(x => label.text = x ?? string.Empty);
    }

    public static IDisposable BindText(this Text uiText,
        IReadOnlyReactiveProperty<string> source,
        bool distinctUntilChanged = true)
    {
        var stream = distinctUntilChanged ? source.DistinctUntilChanged() : source;
        return stream.Subscribe(x => uiText.text = x ?? string.Empty);
    }

    // ============ TMP_InputField (양방향) ============

    public static void BindTwoWay(this TMP_InputField input,
        R3.ReactiveProperty<string> prop,
        ref DisposableBag bag,
        bool distinctUntilChanged = true)
    {
        // VM -> View
        (distinctUntilChanged ? prop.DistinctUntilChanged() : prop)
            .Subscribe(v =>
            {
                if (input.text != v) input.SetTextWithoutNotify(v ?? string.Empty);
            })
            .AddTo(ref bag);

        // View -> VM
        void OnChanged(string v)
        {
            var val = v ?? string.Empty;
            if (!EqualityComparer<string>.Default.Equals(prop.Value, val))
                prop.Value = val;
        }
        input.onValueChanged.AddListener(OnChanged);
        bag.Add(new CallbackDisposable(() => input.onValueChanged.RemoveListener(OnChanged)));
    }

    // ============ Slider (양방향 float) ============

    public static void BindTwoWay(this Slider slider,
        R3.ReactiveProperty<float> prop,
        ref DisposableBag bag,
        bool distinctUntilChanged = true)
    {
        // VM -> View
        (distinctUntilChanged ? prop.DistinctUntilChanged() : prop)
            .Subscribe(v =>
            {
                if (!Mathf.Approximately(slider.value, v))
                    slider.SetValueWithoutNotify(v);
            })
            .AddTo(ref bag);

        // View -> VM
        void OnChanged(float v)
        {
            if (!Mathf.Approximately(prop.Value, v))
                prop.Value = v;
        }
        slider.onValueChanged.AddListener(OnChanged);
        bag.Add(new CallbackDisposable(() => slider.onValueChanged.RemoveListener(OnChanged)));
    }

    // ============ Toggle (양방향 bool) ============

    public static void BindTwoWay(this Toggle toggle,
        R3.ReactiveProperty<bool> prop,
        ref DisposableBag bag,
        bool distinctUntilChanged = true)
    {
        // VM -> View
        (distinctUntilChanged ? prop.DistinctUntilChanged() : prop)
            .Subscribe(v =>
            {
                if (toggle.isOn != v)
                    toggle.SetIsOnWithoutNotify(v);
            })
            .AddTo(ref bag);

        // View -> VM
        void OnChanged(bool v)
        {
            if (prop.Value != v) prop.Value = v;
        }
        toggle.onValueChanged.AddListener(OnChanged);
        bag.Add(new CallbackDisposable(() => toggle.onValueChanged.RemoveListener(OnChanged)));
    }

    // ============ TMP_Dropdown (양방향 int SelectedIndex) ============

    public static void BindTwoWay(this TMP_Dropdown dropdown,
        R3.ReactiveProperty<int> selectedIndex,
        ref DisposableBag bag,
        bool distinctUntilChanged = true)
    {
        // VM -> View
        (distinctUntilChanged ? selectedIndex.DistinctUntilChanged() : selectedIndex)
            .Subscribe(i =>
            {
                if (dropdown.value != i)
                    dropdown.SetValueWithoutNotify(i);
            })
            .AddTo(ref bag);

        // View -> VM
        void OnChanged(int i)
        {
            if (selectedIndex.Value != i) selectedIndex.Value = i;
        }
        dropdown.onValueChanged.AddListener(OnChanged);
        bag.Add(new CallbackDisposable(() => dropdown.onValueChanged.RemoveListener(OnChanged)));
    }

    // ============ Button (Command 스타일: View -> VM) ============

    public static void BindClick(this Button button, Action onClick, ref DisposableBag bag)
    {
        void Handler() => onClick?.Invoke();
        button.onClick.AddListener(Handler);
        bag.Add(new CallbackDisposable(() => button.onClick.RemoveListener(Handler)));
    }
}