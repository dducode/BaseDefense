namespace BaseDefense.AttackImplemention {

    ///<summary>Интерфейс для реализации жизненного цикла атакуемого объекта</summary>
    public interface IAttackable {

        ///<summary>Вызывается для нанесения повреждений объекту</summary>
        ///<param name="damage">Количество нанесённых повреждений</param>
        public void Hit (float damage);


        ///<summary>Текущий показатель здоровья объекта</summary>
        public float CurrentHealthPoints { get; }

    }

}