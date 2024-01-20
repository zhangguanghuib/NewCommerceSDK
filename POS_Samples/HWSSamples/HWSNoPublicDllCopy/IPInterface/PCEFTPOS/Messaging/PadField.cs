using System;
using System.Collections;
using System.Collections.Generic;

namespace PCEFTPOS.EFTClient.IPInterface
{
    public class PadTag
    {
        public string Name { get; set; }
        public string Data { get; set; }

        public PadTag()
        {
        }

        public PadTag(PadTag t)
        {
            Name = t.Name;
            Data = t.Data;
        }

        public PadTag(string name, string data)
        {
            Name = name;
            Data = data;
        }
    }

    public class PadField: IList<PadTag>
    {
        List<PadTag> tags = new List<PadTag>();

        public int Count => ((IList<PadTag>)tags).Count;

        public bool IsReadOnly => ((IList<PadTag>)tags).IsReadOnly;

        public PadTag this[int index] { get { return ((IList<PadTag>)tags)[index]; } set { ((IList<PadTag>)tags)[index] = value;  } }

        public PadField() : this(null)
        {
        }

        public PadField(string data)
        {
            if (data == null || data.Length == 0)
                return;

            if (!SetFromString(data, true))
                SetFromString(data, false);
        }

        int AsInt(string s, int defaultValue)
        {
            int r;
            if (!Int32.TryParse(s, out r))
                r = defaultValue;
            return r;
        }

        string Substring(string s, int startIndex, int length)
        {
            if (s == null || s.Length <= startIndex)
                return "";

            if ((startIndex + length) > s.Length)
                length = s.Length - startIndex;

            return s.Substring(startIndex, length);
        }

        bool SetFromString(string data, bool lenhdr)
        {
            int i, len, n, r;

            tags.Clear();

            if (lenhdr)
            {

                len = AsInt(Substring(data, 0, 3), -1) + 3;

                if (data.Length < len)
                    len = data.Length;
                if (len < 3)
                    return false;
                i = 3;
            }
            else
            {
                len = data.Length;
                i = 0;
            }
            while (i < len)
            {
                r = len - i;
                if (r < 6)
                {
                    // bad data - but caller may still process what has been parsed
                    return false;
                }
                n = AsInt(Substring(data, i + 3, 3), -1);
                if (n < 0 || (i + 6 + n) > len)
                {
                    // bad length info
                    return false;
                }
                tags.Add(new PadTag(Substring(data, i, 3), Substring(data, i + 6, n)));
                i += n + 6;
            }
            // ended nicely
            return true;
        }

        public string GetAsString(bool lenhdr)
        {
            String name, result = "";
            int i = 0;

            while (i < tags.Count)
            {
                name = tags[i].Name + "   ";

                result += Substring(name, 0, 3);
                result += tags[i].Data.Length.ToString().PadLeft(3, '0');
                result += tags[i].Data;
                i++;
            }
            if (lenhdr)
                return result.Length.ToString().PadLeft(3, '0') + result;
            return result;
        }

        public int FindTag(string name)
        {
            int i = 0;
            while (i < tags.Count)
            {
                if (tags[i].Name == name)
                    return i;
                i++;
            }
            return -1;
        }

        public bool HasTag(string name)
        {
            return (FindTag(name) >= 0);
        }

        public void RemoveTag(string name)
        {
            int i = FindTag(name);
            if (i >= 0)
                tags.RemoveAt(i);
        }


        public PadTag GetTag(string name)
        {
            int i = FindTag(name);
            if (i >= 0)
                return tags[i];

            var t = new PadTag(name, "");
            tags.Add(t);
            return t;
        }

        public PadTag SetTag(string name, string data)
        {
            int i = FindTag(name);
            if (i >= 0)
            {
                tags[i].Data = data;
                return tags[i];
            }

            var t = new PadTag(name, data);
            tags.Add(t);
            return t;
        }

        public override string ToString()
        {
            return GetAsString(false);
        }

        public static string GetTagInPad(string pad, string name)
        {
            return (new PadField(pad)).GetTag(name).Data;
        }

        public static string UpdateTagInPad(string pad, string name, string data)
        {
            var pf = new PadField(pad);
            pf.SetTag(name, data);
            return pf.GetAsString(true);
        }

        public int IndexOf(PadTag item)
        {
            return ((IList<PadTag>)tags).IndexOf(item);
        }

        public void Insert(int index, PadTag item)
        {
            ((IList<PadTag>)tags).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ((IList<PadTag>)tags).RemoveAt(index);
        }

        public void Add(PadTag item)
        {
            ((IList<PadTag>)tags).Add(item);
        }

        public void Clear()
        {
            ((IList<PadTag>)tags).Clear();
        }

        public bool Contains(PadTag item)
        {
            return ((IList<PadTag>)tags).Contains(item);
        }

        public void CopyTo(PadTag[] array, int arrayIndex)
        {
            ((IList<PadTag>)tags).CopyTo(array, arrayIndex);
        }

        public bool Remove(PadTag item)
        {
            return ((IList<PadTag>)tags).Remove(item);
        }

        public IEnumerator<PadTag> GetEnumerator()
        {
            return ((IList<PadTag>)tags).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<PadTag>)tags).GetEnumerator();
        }
    }
}
