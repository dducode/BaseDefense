using System.IO;

namespace BaseDefense.SaveSystem
{
    public static class GameDataStorage
    {
        public static GameDataReader GetDataReader(string path)
        {
            if (!File.Exists(path))
                return null;

            var data = File.ReadAllBytes(path);
            var binaryReader = new BinaryReader(new MemoryStream(data));
            var reader = new GameDataReader(binaryReader);
            return reader;
        }
    }
}