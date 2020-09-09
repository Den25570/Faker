﻿using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FakerLib
{
    public class Faker
    {
        private FakerConfig config;

        public Faker(FakerConfig fakerConfig)
        {
            this.config = fakerConfig;
        }
        public T Create<T>() where T : new()
        {
            ItemFactory itemFactory = new ItemFactory();
            T item = itemFactory.CreateItem<T>();

            FillFields<T>(item);

            return item;
        }

        private void FillFields<T>(T item)
        {
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo propertyInfo in properties)
            {
           /*     // If not writable then cannot null it; if not readable then cannot check it's value
                if (!propertyInfo.CanWrite || !propertyInfo.CanRead) { continue; }

                MethodInfo mget = propertyInfo.GetGetMethod(false);
                MethodInfo mset = propertyInfo.GetSetMethod(false);

                // Get and set methods have to be public
                if (mget == null) { continue; }
                if (mset == null) { continue; }

                var entityType = propertyInfo.DeclaringType;
                var parameter = Expression.Parameter(entityType, "entity");
                var property = Expression.Property(parameter, propertyInfo);
                var funcType = typeof(Func<,>).MakeGenericType(entityType, propertyInfo.PropertyType);
                var lambda = (Func<Type, Type>)Expression.Lambda(funcType, property, parameter).Compile();

                var del = config.GetDelegate(typeof(T), propertyInfo.PropertyType, lambda);

                propertyInfo.SetValue(item, del(), null);*/
            }

            foreach (FieldInfo filedInfo in fields)
            {
                var entityType = filedInfo.DeclaringType;
                var parameter = Expression.Parameter(entityType, "entity");
                var property = Expression.Field(parameter, filedInfo);
                var funcType = typeof(Func<, >).MakeGenericType(entityType, filedInfo.FieldType);
                var lambda = Expression.Lambda(funcType, property, parameter);

                var del = config.GetExpressionDelegate(typeof(T), filedInfo.FieldType, filedInfo.Name);

                filedInfo.SetValue(item, del());
            }
        }
    }
}
