using System;
using System.ComponentModel;
using System.Reflection;

namespace ZuegerAdressbook.Extensions
{
    public static class ObjectExtensions
    {
        public static T DynamicAccess<T>(this object data, string propertyName)
        {
            PropertyInfo[] propinfo = data.GetType().GetProperties();
            PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(data.GetType());
            PropertyDescriptor descriptor = descriptors[propertyName];

            if (descriptor == null)
            {
                throw new InvalidOperationException();
            }

            T value = (T)descriptor.GetValue(data);

            return value;
        }
    }
}
