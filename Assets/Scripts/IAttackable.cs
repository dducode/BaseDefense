public interface IAttackable
{
    ///<summary>Вызывается для нанесения повреждений сущности</summary>
    ///<param name="damage">Количество нанесённых повреждений</param>
    public void Hit(float damage);
}
