using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MyCore
{
    public class MyDictionary<TKey, TValue> : IXmlSerializable, IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private readonly Dictionary<TKey, TValue> dictionary;

        public int Count { get { return dictionary.Count; } }
        public Dictionary<TKey, TValue>.KeyCollection Keys { get { return dictionary.Keys; } }
        public Dictionary<TKey, TValue>.ValueCollection Values { get { return dictionary.Values; } }

        public TValue this[TKey key]
        {
            get
            {
                return !dictionary.ContainsKey(key) ? default(TValue) : dictionary[key];
            }
            set
            {
                Add(key, value);
            }
        }

        public MyDictionary()
        {
            dictionary = new Dictionary<TKey, TValue>();
        } 

        public void Add(TKey key, TValue value)
        {
            if (EqualityComparer<TValue>.Default.Equals(default(TValue), value))
            {
                dictionary.Remove(key);
            }
            else if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
            }
            else
            {
                dictionary[key] = value;
            }
        }

        public void Clear()
        {
            dictionary.Clear();
        }

        public bool ContainsKey(TKey key)
        {
            return dictionary.ContainsKey(key);
        }

        public bool ContainsValue(TValue value)
        {
            return dictionary.ContainsValue(value);
        }

        public bool Remove(TKey key)
        {
            return dictionary.Remove(key);
        }

        public XmlSchema GetSchema()
        {
            return (null);
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement)
                return;

            if (reader.ReadToDescendant("Item"))
            {
                do
                {
                    reader.ReadStartElement("Item");

                    var otherSer = new XmlSerializer(typeof(TKey));
                    var key = (TKey)otherSer.Deserialize(reader);
                    otherSer = new XmlSerializer(typeof(TValue));
                    var value = (TValue)otherSer.Deserialize(reader);

                    dictionary.Add(key, value);

                } while (reader.ReadToNextSibling("Item"));
            }

            reader.ReadEndElement();// read the header
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (var key in dictionary.Keys)
            {
                writer.WriteStartElement("Item");
                var otherSerKey = new XmlSerializer(typeof(TKey));
                otherSerKey.Serialize(writer, key);
                otherSerKey = new XmlSerializer(typeof(TValue));
                otherSerKey.Serialize(writer, dictionary[key]);
                writer.WriteEndElement();
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        public List<KeyValuePair<TKey, TValue>> ToList()
        {
            var result = new List<KeyValuePair<TKey, TValue>>(dictionary.Count);

            foreach (var key in dictionary.Keys)
            {
                result.Add(new KeyValuePair<TKey, TValue>(key, dictionary[key]));
            }

            return result;
        }
    }
}
