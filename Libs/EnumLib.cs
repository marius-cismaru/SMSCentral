using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel;

namespace SMSCentral.Libs
{
    public static class EnumLib
    {
        public static string GetDescriptionFromEnum(Enum en)
        {
            string result = string.Empty;
            if (en != null)
            {
                result = en.ToString();
                Type type = en.GetType();
                MemberInfo[] memInfo = type.GetMember(en.ToString());
                if (memInfo != null && memInfo.Length > 0)
                {
                    object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (attrs != null && attrs.Length > 0)
                    {
                        result = ((DescriptionAttribute)attrs[0]).Description;
                    }
                }
            }
            return result;
        }

        public static IEnumerable<KeyValuePair<Enum, string>> GetAllEnumsAndDescriptions<T>()
        {
            var t = typeof(T);
            if (!t.IsEnum)
                throw new ArgumentException("t must be an enum type");

            return Enum.GetValues(t).Cast<Enum>().Select((e) => new KeyValuePair<Enum, string>(e, GetDescriptionFromEnum(e))).ToList();
        }

        public static IEnumerable<KeyValuePair<Enum, string>> GetAllEnumsAndDescriptions(Type t)
        {
            if (!t.IsEnum)
                throw new ArgumentException("t must be an enum type");

            return Enum.GetValues(t).Cast<Enum>().Select((e) => new KeyValuePair<Enum, string>(e, GetDescriptionFromEnum(e))).ToList();
        }

        public static IEnumerable<KeyValuePair<int, string>> GetAllValuesAndDescriptions(Type t)
        {
            if (!t.IsEnum)
                throw new ArgumentException("t must be an enum type");

            return Enum.GetValues(t).Cast<Enum>().Select((e) => new KeyValuePair<int, string>(Convert.ToInt32(e), GetDescriptionFromEnum(e))).ToList();
        }

        public static T GetEnumFromDescription<T>(string description)
        {
            T result = default(T);

            var type = typeof(T);
            if ((type.IsEnum) && (!string.IsNullOrEmpty(description)))
            {
                foreach (var field in type.GetFields())
                {
                    var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attribute != null)
                    {
                        if (attribute.Description == description)
                        {
                            result = (T)field.GetValue(null);
                        }
                    }
                    else
                    {
                        if (field.Name == description)
                        {
                            result = (T)field.GetValue(null);
                        }
                    }
                }
            }

            return result;
        }
    }
}
