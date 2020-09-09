using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace FakerLib
{
    public class FakerConfig
    {
        private Dictionary<Type, Dictionary<Tuple<Type, Expression<Func<object, object>>>, Func<Object>>> configExpressionDelegate;

        public void Add<ParentType, ChildType>(Func<Object> del, Expression<Func<ParentType, ChildType>> specifiedField = null)
        {
            Dictionary<Tuple<Type, Expression<Func<object, object>>>, Func<Object>> targetDictionary;
            if (!configExpressionDelegate.TryGetValue(typeof(ParentType), out targetDictionary))
            {
                targetDictionary = new Dictionary<Tuple<Type, Expression<Func<object, object>>>, Func<Object>>();
                configExpressionDelegate.Add(typeof(ParentType), targetDictionary);
            }

            targetDictionary.Add(new Tuple<Type, Expression<Func<object, object>>>(typeof(ChildType), ConvertFunction<ParentType, ChildType>(specifiedField)), del);
        }

        private Expression<Func<object, object>> ConvertFunction<TInput, TOutput>(Expression<Func<TInput, TOutput>> expression)
        {
            Expression converted = Expression.Convert(expression.Body, typeof(object));
            ParameterExpression p = Expression.Parameter(typeof(object));
            var expr = Expression.Lambda<Func<object, object>>(converted, p);
            return expr;
        }

        public Func<Object> GetExpressionDelegate(Type ParentType, Type ChildType, string ChildName)
        {
            Dictionary<Tuple<Type, Expression<Func<object, object>>>, Func<Object>> childDictionary;
            Func<Object> del = null;

            if (configExpressionDelegate.TryGetValue(ParentType, out childDictionary))
            {
                del = searchForDelegate(ChildType, childDictionary, ChildName);
            }
            else
            {
                if (configExpressionDelegate.TryGetValue(typeof(object), out childDictionary))
                {
                    del = searchForDelegate(ChildType, childDictionary, ChildName);
                }
                else
                {
                    throw new Exception("Error while trying to receive standart Expression<Func<object, object>>. Config not setup correctly");
                }
            }
            return del;
        }

        public Func<Object> searchForDelegate(Type ChildType, Dictionary<Tuple<Type, Expression<Func<object, object>>>, Func<Object>> childDictionary, string ChildName)
        {
            Func<Object> del = null;
            foreach (var keyPair in childDictionary.Keys)
            {
                if (keyPair.Item1 == ChildType)
                {
                    childDictionary.TryGetValue(keyPair, out del);

                    var op = ((UnaryExpression)keyPair.Item2.Body).Operand;
                    if (((MemberExpression)op).Member.Name == ChildName)
                    {
                        break;
                    }
                }
            }
            return del;
            
        }

        public FakerConfig()
        {
            configExpressionDelegate = new Dictionary<Type, Dictionary<Tuple<Type, Expression<Func<object, object>>>, Func<Object>>>();

            var defaultConfig = new Dictionary<Tuple<Type, Expression<Func<object, object>>>, Func<Object>>();

            defaultConfig.Add(new Tuple<Type, Expression<Func<object, object>>>(typeof(int), null), PropertyFactory.GenerateInt);

            configExpressionDelegate.Add(typeof(object), defaultConfig);
        }
    }
}
