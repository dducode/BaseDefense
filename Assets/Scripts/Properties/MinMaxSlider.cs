[System.Serializable]
public class MinMaxSliderFloat
{
    public float minLimit;
    public float maxLimit;

    public float minValue;
    public float maxValue;

    public MinMaxSliderFloat(float _minLimit, float _maxLimit)
    {
        minLimit = _minLimit;
        maxLimit = _maxLimit;
    }
}

[System.Serializable]
public class MinMaxSliderInt
{
    public int minLimit;
    public int maxLimit;

    public int minValue;
    public int maxValue;

    public MinMaxSliderInt(int _minLimit, int _maxLimit)
    {
        minLimit = _minLimit;
        maxLimit = _maxLimit;
    }
}
