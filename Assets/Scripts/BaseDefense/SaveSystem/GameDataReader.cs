using System.IO;
using UnityEngine;

namespace BaseDefense.SaveSystem
{
    public class GameDataReader
    {
        private readonly BinaryReader m_reader;

        public GameDataReader(BinaryReader reader)
        {
            m_reader = reader;
        }

        public Vector3 ReadPosition()
        {
            var position = new Vector3
            {
                x = m_reader.ReadSingle(),
                y = m_reader.ReadSingle(),
                z = m_reader.ReadSingle()
            };

            return position;
        }

        public Quaternion ReadRotation()
        {
            var rotation = new Quaternion
            {
                x = m_reader.ReadSingle(),
                y = m_reader.ReadSingle(),
                z = m_reader.ReadSingle(),
                w = m_reader.ReadSingle()
            };

            return rotation;
        }

        public int ReadInteger()
        {
            return m_reader.ReadInt32();
        }
    }
}