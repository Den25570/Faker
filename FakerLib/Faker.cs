using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace FakerLib
{
    public class Faker
    {
        private FakerConfig config;
        private ItemFactory itemFactory;

        public Faker(FakerConfig fakerConfig)
        {
            this.config = fakerConfig;
            this.itemFactory = new ItemFactory();
        }
        public object Create(Type objectType)
        {
            object item = itemFactory.CreateItem(objectType);

            FillFields(item);

            return item;
        }

        private void FillFields(object item)
        {
            PropertyInfo[] properties = item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            FieldInfo[] fields = item.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo propertyInfo in properties)
            {
                if ((!propertyInfo.CanWrite || !propertyInfo.CanRead) || (propertyInfo.GetGetMethod(false) == null || propertyInfo.GetSetMethod(false) == null)) 
                { 
                    continue; 
                }

                object valueToSet;
                if (propertyInfo.PropertyType.IsClass)
                {
                    valueToSet = Create(propertyInfo.PropertyType);
                }
                else
                {
                    var del = config.GetExpressionDelegate(item.GetType(), propertyInfo.PropertyType, propertyInfo.Name);
                    valueToSet = del();
                }
                            
                propertyInfo.SetValue(item, valueToSet, null);
            }

            foreach (FieldInfo filedInfo in fields)
            {
                object valueToSet;
                if (filedInfo.FieldType.IsClass)
                {
                    valueToSet = Create(filedInfo.FieldType);
                }
                else
                {
                    var del = config.GetExpressionDelegate(item.GetType(), filedInfo.FieldType, filedInfo.Name);
                    valueToSet = del();
                }
            
                filedInfo.SetValue(item, valueToSet);
            }
        }
    }
}
