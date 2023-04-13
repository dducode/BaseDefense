using System;
using System.IO;
using UnityEngine;

namespace BaseDefense.SaveSystem {

    /// <summary>
    /// Адаптер к классу <see cref="BinaryReader"/> для упрощения чтения данных
    /// </summary>
    public class UnityReader {

        private readonly BinaryReader m_reader;


        public UnityReader (BinaryReader reader) => m_reader = reader;


        public Vector3 ReadPosition () {
            return new Vector3 {
                x = m_reader.ReadSingle(),
                y = m_reader.ReadSingle(),
                z = m_reader.ReadSingle()
            };
        }


        public Quaternion ReadRotation () {
            return new Quaternion {
                x = m_reader.ReadSingle(),
                y = m_reader.ReadSingle(),
                z = m_reader.ReadSingle(),
                w = m_reader.ReadSingle()
            };
        }


        public Color ReadColor () {
            return new Color {
                r = m_reader.ReadSingle(),
                g = m_reader.ReadSingle(),
                b = m_reader.ReadSingle(),
                a = m_reader.ReadSingle()
            };
        }


        public T ReadObject<T> () {
            return JsonUtility.FromJson<T>(m_reader.ReadString());
        }


        public int ReadInt () {
            return m_reader.ReadInt32();
        }


        public string ReadString () {
            return m_reader.ReadString();
        }


        public float ReadFloat () {
            return m_reader.ReadSingle();
        }


        public bool ReadBool () {
            return m_reader.ReadBoolean();
        }

    }

}