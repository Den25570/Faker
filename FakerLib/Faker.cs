using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;

namespace FakerLib
{
    public class Faker
    {
        private FakerConfig config;
        private Stack<object> DTOCallStack;

        public Faker(FakerConfig fakerConfig)
        {
            this.config = fakerConfig;
            this.DTOCallStack = new Stack<object>();

            this.config.Configure(this);
        }

        public object Create(Type objectType)
        {
            object item = null;

            if (!TryGetObjectInStack(objectType, out item))
            {                           
                item = CreateItem(objectType);         
                DTOCallStack.Push(item);
                FillFields(item);
                DTOCallStack.Pop();
            }  

            return item;
        }

        private bool TryGetObjectInStack(Type type, out object item)
        {
            foreach(object obj in DTOCallStack)
            {
                if (obj.GetType() == type)
                {
                    item = obj;
                    return true;
                }
                    
            }
            item = null;
            return false;
        }

        private void FillFields(object item)
        {
            if (item == null) return;

            PropertyInfo[] properties = item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            FieldInfo[] fields = item.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);


            foreach (PropertyInfo propertyInfo in properties)
            {
                if ((!propertyInfo.CanWrite || !propertyInfo.CanRead) || (propertyInfo.GetGetMethod(false) == null || propertyInfo.GetSetMethod(false) == null)) 
                { 
                    continue; 
                }

                propertyInfo.SetValue(item, GetValue(propertyInfo.PropertyType, item.GetType(), propertyInfo.Name));
            }

            foreach (FieldInfo fieledInfo in fields)
            {
                fieledInfo.SetValue(item, GetValue(fieledInfo.FieldType, item.GetType(), fieledInfo.Name));
            }
        }

        private object CreateItem(Type objectType)
        {
            object instance;
            ConstructorInfo[] constructorsInfo = objectType.GetConstructors();

            if (constructorsInfo.Length == 0)
            {
                instance = null;
            }
            else
            {
                ConstructorInfo constructorInfo = constructorsInfo.First();
                ParameterInfo[] parametersInfo = constructorInfo.GetParameters();
                Object[] objectParams = new Object[parametersInfo.Length];

                for (int i = 0; i < objectParams.Length; i++)
                {
                    objectParams[i] = GetValue(parametersInfo[i].ParameterType, objectType, parametersInfo[i].Name, false);
                }

                try
                {
                    instance = constructorInfo.Invoke(objectParams);
                }
                catch
                {
                    throw new Exception("Error occured while trying to create object via constructor with parameters");
                }
            }
            return instance;
        }

        public object GetValue(Type valueType, Type parentType, string valueName, bool caseSensetive = true)
        {
            object item;
            var del = config.GetExpressionDelegate(parentType, valueType, valueName, caseSensetive);

            if (valueType.IsClass && !valueType.FullName.StartsWith("System."))
            {
                if (del != null)
                {
                    item = del(getGenericArgs(valueType));
                }
                else
                {
                    item = Create(valueType);
                }               
            }
            else
            {             
                if (del != null)
                {
                    item = del(getGenericArgs(valueType));
                }
                else
                {
                    try
                    {
                        item = Activator.CreateInstance(valueType);
                    }
                    catch
                    {
                        item = null;
                    }                   
                }
            }
            return item;
        }

        private Type[] getGenericArgs(Type type)
        {
            Type[] args;
            if (type.IsArray)
            {
                args = new Type[] { type.GetElementType() };
            }
            else
            {
                args = type.GetGenericArguments();
            }
            return args;
        }
    }
}
