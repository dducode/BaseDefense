///<summary>Значение с плавающей запятой, ограниченное определённым диапазоном</summary>
[System.Serializable]
public class MinMaxSliderFloat
{
    public float minLimit;
    public float maxLimit;

    public float minValue;
    public float maxValue;

    ///<param name="minLimit">Минимально возможное значение для слайдера</param>
    ///<param name="maxLimit">Максимально возможное значение для слайдера</param>
    public MinMaxSliderFloat(float minLimit, float maxLimit)
    {
        minLimit = minLimit < 0 ? 0 : minLimit;
        maxLimit = maxLimit < 0 ? 0 : maxLimit;
        if (minLimit > maxLimit)
        {
            float temp = minLimit;
            minLimit = maxLimit;
            maxLimit = temp;
        }
        this.minLimit = minLimit;
        this.maxLimit = maxLimit;
    }
}

///<summary>Целочисленное значение, ограниченное определённым диапазоном</summary>
[System.Serializable]
public class MinMaxSliderInt
{
    public int minLimit;
    public int maxLimit;

    public int minValue;
    public int maxValue;

    ///<param name="minLimit">Минимально возможное значение для слайдера</param>
    ///<param name="maxLimit">Максимально возможное значение для слайдера</param>
    public MinMaxSliderInt(int minLimit, int maxLimit)
    {
        minLimit = minLimit < 0 ? 0 : minLimit;
        maxLimit = maxLimit < 0 ? 0 : maxLimit;
        if (minLimit > maxLimit)
        {
            int temp = minLimit;
            minLimit = maxLimit;
            maxLimit = temp;
        }
        this.minLimit = minLimit;
        this.maxLimit = maxLimit;
    }
}
