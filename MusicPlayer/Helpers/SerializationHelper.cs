using System;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace MusicPlayer.Helpers
{
    /// <summary>
    /// 枚举类型，其包括Xml、Soap和Binary串行化编码方式
    /// </summary>
    public enum SerializationFormatterType
    {
        /// <summary>
        /// Xml消息格式编码
        /// </summary>
        Xml,
        /// <summary>
        /// 二进制消息格式编码
        /// </summary>
        Binary
    }

    /// <summary>
    /// 帮助对象实现序列化和反序列化
    /// </summary>
    /// <remarks>
    /// 对象序列化是把对象序列化转化为string类型，对象反序列化是把对象从string类型反序列化转化为其源类型。
    /// </remarks>
    public static class SerializationHelper
    {
        private static IRemotingFormatter GetFormatter(SerializationFormatterType formatterType)
        {
            switch (formatterType)
            {
                case SerializationFormatterType.Binary:
                    return new BinaryFormatter();
                default:
                    throw new NotSupportedException();
            }
        }

        private static string XmlSerializeObject(object graph)
        {
            using (StringWriter stringWriter = new StringWriter())
            {
                XmlSerializer xmlSerializer = new XmlSerializer(graph.GetType());
                xmlSerializer.Serialize(stringWriter, graph);
                stringWriter.Flush();
                return stringWriter.ToString();
            }
        }

        private static bool XmlSerializeObjectToFile(object graph, string path)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Create))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(graph.GetType());
                xmlSerializer.Serialize(fileStream, graph);
                fileStream.Flush();
                return true;
            }
        }

        private static T XmlDeserializeObject<T>(string serializedGraph)
        {
            using (StreamReader streamReader = new StreamReader(serializedGraph))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                return (T)xmlSerializer.Deserialize(streamReader);
            }
        }

        private static T XmlDeserializeObjectFromFile<T>(string path)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                return (T)xmlSerializer.Deserialize(fileStream);
            }
        }

        public static string SerializeObject(object graph, SerializationFormatterType formatterType)
        {
            if (graph == null)
            {
                throw new ArgumentNullException("graph");
            }

            if (SerializationFormatterType.Xml == formatterType)
            {
                return XmlSerializeObject(graph);
            }

            using (MemoryStream memoryStream = new MemoryStream())
            {
                IRemotingFormatter formatter = GetFormatter(formatterType);
                formatter.Serialize(memoryStream, graph);
                byte[] arrGraph = memoryStream.ToArray();
                return Convert.ToBase64String(arrGraph);
            }
        }

        public static bool SerializeObjectToFile(object graph, SerializationFormatterType formatterType, string path)
        {
            if (graph == null)
            {
                throw new ArgumentNullException("graph");
            }

            if (SerializationFormatterType.Xml == formatterType)
            {
                return XmlSerializeObjectToFile(graph, path);
            }

            using (FileStream fileStream = new FileStream(path, FileMode.Create))
            {
                IRemotingFormatter formatter = GetFormatter(formatterType);
                formatter.Serialize(fileStream, graph);
                fileStream.Flush();
                return true;
            }
        }

        public static T DeserializeObject<T>(string serializedGraph, SerializationFormatterType formatterType, SerializationBinder binder = null)
        {
            if (string.IsNullOrEmpty(serializedGraph.Trim()))
            {
                throw new ArgumentNullException("serializedGraph");
            }

            if (SerializationFormatterType.Xml == formatterType)
            {
                return XmlDeserializeObject<T>(serializedGraph);
            }

            byte[] arrGraph = Convert.FromBase64String(serializedGraph);
            using (MemoryStream memoryStream = new MemoryStream(arrGraph))
            {
                IRemotingFormatter formatter = GetFormatter(formatterType);
                if (binder != null) formatter.Binder = binder;
                return (T)formatter.Deserialize(memoryStream);
            }
        }

        public static T DeserializeObjectFromFile<T>(string path, SerializationFormatterType formatterType, SerializationBinder binder = null)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("path");
            }

            if (SerializationFormatterType.Xml == formatterType)
            {
                return XmlDeserializeObjectFromFile<T>(path);
            }

            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                IRemotingFormatter formatter = GetFormatter(formatterType);
                if (binder != null) formatter.Binder = binder;
                return (T)formatter.Deserialize(fileStream);
            }
        }

        /// <summary>
        /// 通过序列化复制对象
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        public static T CloneObject<T>(T graph)
        {
            if (graph == null || string.IsNullOrEmpty(graph.ToString()))
            {
                throw new ArgumentNullException("graph");
            }

            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, graph);

                memoryStream.Position = 0;
                return (T)formatter.Deserialize(memoryStream);
            }
        }
    }
}