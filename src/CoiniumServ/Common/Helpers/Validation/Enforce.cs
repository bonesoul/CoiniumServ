using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Coinium.Common.Helpers.Validation
{
    /// <summary>
    /// The enforce.
    /// </summary>
    public static class Enforce
    {
        #region Methods

        /// <summary>
        /// Arguments the is valid.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="message">The message.</param>
        /// <param name="predicate">The predicate.</param>
        /// <exception cref="System.ArgumentException"></exception>
        public static void ArgumentIsValid<T>(T value, string message, Func<T, bool> predicate)
        {
            if (!predicate(value))
            {
                throw new ArgumentException(message);
            }
        }

        /// <summary>
        /// Arguments the not null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="argument">The argument.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static void ArgumentNotNull<T>(Expression<Func<T>> argument) where T : class
        {
            string name = GetName(argument);
            ArgumentNotNull((argument.Compile())(), name);
        }

        /// <summary>
        /// Arguments the not null.
        /// </summary>
        /// <param name="value"> The value. </param>
        /// <param name="name"> The name. </param>
        public static void ArgumentNotNull(object value, string name)
        {
            if (value == null)
            {
                throw new ArgumentNullException(string.Format("{0} cannot be passed as null. Check the DI configuration or consuming class.", name));
            }
        }

        /// <summary>
        /// Determines whether the specified collection has items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="name">The name.</param>
        /// <exception cref="System.ArgumentException"></exception>
        public static void HasItems<T>(ICollection<T> collection, string name)
        {
            if (collection == null || collection.Count == 0)
            {
                throw new ArgumentException(string.Format("{0} does not contain elements and elements are required. Check the consuming class.", name));
            }
        }

        /// <summary>
        /// Enums the value is defined.
        /// </summary>
        /// <typeparam name="TEnum"> The type of the enum. </typeparam>
        /// <param name="value"> The value. </param>
        /// <param name="name"> The name. </param>
        public static void EnumValueIsDefined<TEnum>(TEnum value, string name)
        {
            if (!Enum.IsDefined(typeof(TEnum), value))
            {
                throw new ArgumentException(string.Format("The enum value {0} does not exist in {1}.", value, typeof(TEnum).Name));
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="argument">The argument.</param>
        /// <returns></returns>
        private static string GetName<T>(Func<T> argument)
        {
            // get IL code behind the delegate
            var methodBody = argument.Method.GetMethodBody();
            if (methodBody != null)
            {
                var il = methodBody.GetILAsByteArray();

                // bytes 2-6 represent the field handle
                var fieldHandle = BitConverter.ToInt32(il, 2);

                // resolve the handle
                var field = argument.Target.GetType().Module.ResolveField(fieldHandle);

                return field.Name;
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="argument">The argument.</param>
        /// <returns></returns>
        private static string GetName<T>(Expression<Func<T>> argument)
        {
            var mbody = argument.Body as MemberExpression;

            if (mbody == null)
            {
                //This will handle Nullable<T> properties.
                var ubody = argument.Body as UnaryExpression;

                if (ubody != null)
                {
                    mbody = ubody.Operand as MemberExpression;
                }
            }

            return mbody != null ? mbody.Member.Name : string.Empty;
        }

        #endregion Methods
    }
}