using System.IO;
using UnityEngine;

namespace BaseDefense.SaveSystem {

    /// <summary>
    /// Адаптер к классу <see cref="BinaryWriter"/> для упрощения записи данных
    /// </summary>
    public class UnityWriter {

        private readonly BinaryWriter m_writer;


        public UnityWriter (BinaryWriter writer) => m_writer = writer;


        public void Write (Vector3 position) {
            m_writer.Write(position.x);
            m_writer.Write(position.y);
            m_writer.Write(position.z);
        }


        public void Write (Quaternion rotation) {
            m_writer.Write(rotation.x);
            m_writer.Write(rotation.y);
            m_writer.Write(rotation.z);
            m_writer.Write(rotation.w);
        }


        public void Write (Color color) {
            m_writer.Write(color.r);
            m_writer.Write(color.g);
            m_writer.Write(color.b);
            m_writer.Write(color.a);
        }


        public void Write<T> (T obj) {
            m_writer.Write(JsonUtility.ToJson(obj));
        }


        public void Write (int value) {
            m_writer.Write(value);
        }


        public void Write (string value) {
            m_writer.Write(value);
        }


        public void Write (float value) {
            m_writer.Write(value);
        }


        public void Write (bool value) {
            m_writer.Write(value);
        }

    }

}