namespace BaseDefense.SaveSystem {

    public interface IPersistentObject {

        public void Save (UnityWriter writer);
        public void Load (UnityReader reader);

    }

}