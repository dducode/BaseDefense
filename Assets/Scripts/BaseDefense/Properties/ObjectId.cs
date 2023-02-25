namespace BaseDefense.Properties
{
    /// <summary>Идентификатор объекта является уникальным только для объектов разных видов.
    /// Объекты одного вида (напр. LowEnemy) имеют одинаковый id</summary>
    [System.Serializable]
    public struct ObjectId
    {
        public int id;
    }
}
