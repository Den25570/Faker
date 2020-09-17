﻿using System;
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
        private ItemFactory itemFactory;
        private Stack<Type> DTOCallStack;

        public Faker(FakerConfig fakerConfig)
        {
            this.config = fakerConfig;
            this.itemFactory = new ItemFactory();
            this.DTOCallStack = new Stack<Type>();
        }

        public object Create(Type objectType)
        {
            object item = null;

            if (!IsTypeInStack(objectType))
            {             
                DTOCallStack.Push(objectType);
                item = CreateItem(objectType);
                FillFields(item);
                DTOCallStack.Pop();
            }         

            return item;
        }

        private bool IsTypeInStack(Type type)
        {
            foreach(Type stackType in DTOCallStack)
            {
                if (stackType == type)
                    return true;
            }
            return false;
        }

        private void FillFields(object item)
        {
            if (item == null) return;

            PropertyInfo[] properties = item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            FieldInfo[] fields = item.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

            Random rand = new Random();

            foreach (PropertyInfo propertyInfo in properties)
            {
                if ((!propertyInfo.CanWrite || !propertyInfo.CanRead) || (propertyInfo.GetGetMethod(false) == null || propertyInfo.GetSetMethod(false) == null)) 
                { 
                    continue; 
                }

                propertyInfo.SetValue(item, GetValue(propertyInfo.PropertyType, item.GetType(), propertyInfo.Name, rand, propertyInfo.PropertyType.GetGenericArguments()));
            }

            foreach (FieldInfo fieledInfo in fields)
            {
                fieledInfo.SetValue(item, GetValue(fieledInfo.FieldType, item.GetType(), fieledInfo.Name, rand));
            }
        }

        private object CreateItem(Type objectType)
        {
            object instance;
            ConstructorInfo[] constructorsInfo = objectType.GetConstructors();

            if (constructorsInfo.Length == 0)
            {
                instance = Activator.CreateInstance(objectType);
            }
            else
            {
                Random rand = new Random();
                ConstructorInfo constructorInfo = constructorsInfo.First();
                ParameterInfo[] parametersInfo = constructorInfo.GetParameters();
                Object[] objectParams = new Object[parametersInfo.Length];

                for (int i = 0; i < objectParams.Length; i++)
                {
                    objectParams[i] = GetValue(parametersInfo[i].ParameterType, objectType, parametersInfo[i].Name, rand);
                }

                try
                {
                    instance = constructorInfo.Invoke(objectParams);
                }
                catch (Exception e)
                {
                    throw new Exception("Error occured while trying to create object via constructor with parameters");
                }

            }

            return instance;
        }

        public object GetValue(Type valueType, Type parentType, string valueName, Random rand, Type[] genericParams)
        {
            if (valueType.IsClass && !valueType.FullName.StartsWith("System."))
            {
                return Create(valueType);
            }
            else
            {
                var del = config.GetExpressionDelegate(parentType, valueType, valueName);
                return del != null ? del(rand, genericParams) : valueType.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
            }
        }
    }
}
