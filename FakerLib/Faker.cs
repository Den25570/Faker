using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        }

        public object Create(Type objectType)
        {
            object item = null;

            if (!IsTypeInStack(objectType))
            {
                item = itemFactory.CreateItem(objectType);
                DTOCallStack.Push(objectType);
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
            PropertyInfo[] properties = item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            FieldInfo[] fields = item.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

            Random rand = new Random();

            foreach (PropertyInfo propertyInfo in properties)
            {
                if ((!propertyInfo.CanWrite || !propertyInfo.CanRead) || (propertyInfo.GetGetMethod(false) == null || propertyInfo.GetSetMethod(false) == null)) 
                { 
                    continue; 
                }

                object valueToSet;
                if (propertyInfo.PropertyType.IsClass && !propertyInfo.PropertyType.FullName.StartsWith("System."))
                {
                    valueToSet = Create(propertyInfo.PropertyType);
                }
                else
                {
                    var del = config.GetExpressionDelegate(item.GetType(), propertyInfo.PropertyType, propertyInfo.Name);
                    valueToSet = del(rand);
                }
                            
                propertyInfo.SetValue(item, valueToSet, null);
            }

            foreach (FieldInfo filedInfo in fields)
            {
                object valueToSet;
                if (filedInfo.FieldType.IsClass && !filedInfo.FieldType.FullName.StartsWith("System."))
                {
                    valueToSet = Create(filedInfo.FieldType);
                }
                else
                {
                    var del = config.GetExpressionDelegate(item.GetType(), filedInfo.FieldType, filedInfo.Name);
                    valueToSet = del(rand);
                }
            
                filedInfo.SetValue(item, valueToSet);
            }
        }
    }
}
