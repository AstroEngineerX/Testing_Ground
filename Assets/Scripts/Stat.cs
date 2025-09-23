using UnityEngine;

public abstract class Stat
{
    private int _currentValue;
    private int _maxValue;
    private int _minValue;

    public int CurrentValue => _currentValue;
    public int MaxValue => _maxValue;
    public int MinValue => _minValue;

    protected Stat(int minValue, int maxValue, bool initWithMax = false)
    {
        _minValue = Mathf.Max(0, minValue);
        _maxValue = Mathf.Max(0, maxValue);

        _currentValue = initWithMax ? _maxValue : _minValue;
    }

    public virtual void AffectValue(int value)
    {
        _currentValue = Mathf.Clamp(_currentValue + value, _minValue, _maxValue);
    }
}