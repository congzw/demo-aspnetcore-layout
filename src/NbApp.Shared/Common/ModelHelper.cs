using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace Common
{
    public class ModelHelper
    {
        private static IList<Type> _notProcessPropertyBaseTypes = new List<Type>()
        {
            typeof (DynamicObject),
            typeof (object),
            //typeof (BaseViewModel),
            //typeof (BaseViewModel<>),
            //typeof (Expando)
        };

        /// <summary>
        /// 在这些类型中声明的属性不处理
        /// </summary>
        public static IList<Type> NotProcessPropertyBaseTypes
        {
            get => _notProcessPropertyBaseTypes;
            set => _notProcessPropertyBaseTypes = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static void TryCopyProperties(object updatingObj, object collectedObj, string[] excludeProperties = null)
        {
            if (collectedObj != null && updatingObj != null)
            {
                //获取类型信息
                Type updatingObjType = updatingObj.GetType();
                PropertyInfo[] updatingObjPropertyInfos = updatingObjType.GetProperties();

                Type collectedObjType = collectedObj.GetType();
                PropertyInfo[] collectedObjPropertyInfos = collectedObjType.GetProperties();

                string[] fixedExPropertites = excludeProperties ?? new string[] { };

                foreach (PropertyInfo updatingObjPropertyInfo in updatingObjPropertyInfos)
                {
                    foreach (PropertyInfo collectedObjPropertyInfo in collectedObjPropertyInfos)
                    {
                        if (updatingObjPropertyInfo.Name.Equals(collectedObjPropertyInfo.Name,StringComparison.OrdinalIgnoreCase))
                        {
                            if (fixedExPropertites.Contains(updatingObjPropertyInfo.Name, StringComparer.OrdinalIgnoreCase))
                            {
                                continue;
                            }

                            //do not process complex property
                            var isSimpleType = IsSimpleType(collectedObjPropertyInfo.PropertyType);
                            if (!isSimpleType)
                            {
                                continue;
                            }

                            //fix dynamic problems: System.Reflection.TargetParameterCountException
                            var declaringType = collectedObjPropertyInfo.DeclaringType;
                            if (declaringType != null && declaringType != collectedObjType)
                            {
                                //do not process base class dynamic property
                                if (NotProcessPropertyBaseTypes.Contains(declaringType))
                                {
                                    continue;
                                }
                            }

                            object value = collectedObjPropertyInfo.GetValue(collectedObj, null);
                            if (updatingObjPropertyInfo.CanWrite)
                            {
                                //do not process read only property
                                updatingObjPropertyInfo.SetValue(updatingObj, value, null);
                            }
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 是否是简单类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsSimpleType(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // nullable type, check if the nested type is simple.
                return IsSimpleType(typeInfo.GetGenericArguments()[0]);
            }
            return typeInfo.IsPrimitive
                   || typeInfo.IsEnum
                   || type == typeof(string)
                   || type == typeof(decimal)
                   //|| type == typeof(Guid)
                   //|| type == typeof(DateTime)
                   || type.IsSubclassOf(typeof(ValueType)); //Guid, Datetime, etc...
        }

        public static IList<string> GetPropertyNames<T>()
        {
            return GetPropertyNames(typeof(T));
        }

        public static IList<string> GetPropertyNames(Type theType)
        {
            var result = new List<string>();
            var propertyInfos = theType.GetProperties();
            foreach (var var in propertyInfos)
            {
                result.Add(var.Name);
            }
            return result;
        }

        public static IDictionary<string, object> GetKeyValueDictionary(object model, string[] excludeProperties = null)
        {
            var result = new Dictionary<string, object>();
            if (model == null)
            {
                return result;
            }

            var theType = model.GetType();
            var propertyInfos = theType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo var in propertyInfos)
            {
                if (excludeProperties != null && excludeProperties.Contains(var.Name, StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

                result.Add(var.Name, var.GetValue(model, null));
            }
            return result;
        }

        public static IDictionary<string, object> GetKeyValueDictionary(object model, bool onlySimpleType, string[] excludeProperties = null)
        {
            var result = new Dictionary<string, object>();
            if (model == null)
            {
                return result;
            }

            var theType = model.GetType();
            var propertyInfos = theType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo propInfo in propertyInfos)
            {
                if (onlySimpleType)
                {
                    if (!IsSimpleType(propInfo.PropertyType))
                    {
                        continue;
                    }
                }

                if (excludeProperties != null && excludeProperties.Contains(propInfo.Name, StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

                result.Add(propInfo.Name, propInfo.GetValue(model, null));
            }
            return result;
        }

        public static void SetPropertiesWithDictionary(IDictionary<string, object> items, object toBeUpdated, string keyPrefix = null)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }
            if (toBeUpdated == null)
            {
                throw new ArgumentNullException(nameof(toBeUpdated));
            }

            var theType = toBeUpdated.GetType();
            var propertyInfos = theType.GetProperties();
            foreach (var propertyInfo in propertyInfos)
            {
                foreach (var theKey in items.Keys)
                {
                    if (!theKey.Equals(keyPrefix + propertyInfo.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var theValue = items[theKey];
                    if (theValue == null)
                    {
                        continue;
                    }

                    if (theValue.GetType() != propertyInfo.PropertyType)
                    {
                        theValue = Convert.ChangeType(theValue, propertyInfo.PropertyType);
                    }
                    propertyInfo.SetValue(toBeUpdated, theValue, null);
                }
            }

        }

        public static void SetProperties(object toBeUpdated, object getFrom, string[] excludeProperties = null)
        {
            if (toBeUpdated == null)
            {
                throw new ArgumentNullException(nameof(toBeUpdated));
            }
            if (getFrom == null)
            {
                throw new ArgumentNullException(nameof(getFrom));
            }

            //获取类型信息
            Type toBeUpdatedType = toBeUpdated.GetType();
            PropertyInfo[] propertyInfos = toBeUpdatedType.GetProperties();
            IList<string> fixedProperties = excludeProperties ?? new string[] { };

            var propertyValues = GetKeyValueDictionary(getFrom);
            foreach (var excludeProperty in fixedProperties)
            {
                if (propertyValues.ContainsKey(excludeProperty))
                {
                    propertyValues.Remove(excludeProperty);
                }
            }


            foreach (var propertyValue in propertyValues)
            {
                var propertyInfo = propertyInfos.SingleOrDefault(x => propertyValue.Key.Equals(x.Name, StringComparison.OrdinalIgnoreCase));
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(toBeUpdated, propertyValue.Value, null);
                }
            }
        }

        public static bool SetProperty(object model, string key, object value)
        {
            var result = false;
            if (model != null && !string.IsNullOrEmpty(key) && value != null)
            {
                //获取类型信息
                var theType = model.GetType();
                var propertyInfos = theType.GetProperties();

                foreach (var propertyInfo in propertyInfos)
                {
                    if (propertyInfo.Name.Equals(key, StringComparison.OrdinalIgnoreCase))
                    {
                        var theValue = value;
                        if (value.GetType() != propertyInfo.PropertyType)
                        {
                            theValue = Convert.ChangeType(value, propertyInfo.PropertyType);
                        }
                        propertyInfo.SetValue(model, theValue, null);
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }
        
        public static object TryGetValue(object model, string name, object defaultValue)
        {
            if (model == null || string.IsNullOrWhiteSpace(name))
            {
                return defaultValue;
            }

            var theType = model.GetType();
            var propertyInfos = theType.GetProperties(BindingFlags.Instance);

            var propertyInfo = theType.GetProperty(name, BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (propertyInfo != null)
            {
                return propertyInfo.GetValue(model, null);
            }
            
            var filedInfo = theType.GetField(name, BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (filedInfo != null)
            {
                return filedInfo.GetValue(model);
            }

            return defaultValue;
        }

    }
}
