using System.Reflection;

namespace CodeCrafters.Interpreter.Attributes
{
    internal class AttributeHelper
    {
        /// <summary></summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <returns></returns>
        public static List<Type> GetAttributeTypes<TAttribute>()
            where TAttribute : Attribute
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes());

            return FilterTypes<TAttribute>(assemblies);
        }

        /// <summary></summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TAttribute"></typeparam>
        /// <returns></returns>
        public static List<Type> GetAttributeTypes<TInterface, TAttribute>()
            where TAttribute : Attribute
        {
            var type = typeof(TInterface);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(type.IsAssignableFrom);

            return FilterTypes<TAttribute>(assemblies);
        }

        /// <summary></summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static List<Type> GetAttributeTypes<TAttribute>(Func<TAttribute, bool> predicate)
            where TAttribute : Attribute
        {
            var filtered = GetAttributeTypes<TAttribute>().ConvertAll(x =>
                x.GetCustomAttributes<TAttribute>().Any(predicate) ? x : null);

            filtered.RemoveAll(x => x is null);
            return filtered!;
        }

        /// <summary></summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static List<Type> GetAttributeTypes<TInterface, TAttribute>(Func<TAttribute, bool> predicate)
            where TAttribute : Attribute
        {
            var filtered = GetAttributeTypes<TInterface, TAttribute>().ConvertAll(x =>
                x.GetCustomAttributes<TAttribute>().Any(predicate) ? x : null);

            filtered.RemoveAll(x => x is null);
            return filtered!;
        }

        /// <summary></summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <returns></returns>
        private static List<Type> FilterTypes<TAttribute>(IEnumerable<Type> assemblies)
            where TAttribute : Attribute
        {
            var types = from assembly in assemblies
                where !assembly.IsInterface
                let attr = assembly.GetCustomAttributes<TAttribute>()
                where attr.Any<TAttribute>() != false
                select assembly;

            return [.. types];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TAttribute"></typeparam>
        /// <returns></returns>
        public static List<TAttribute> GetAttributes<TInterface, TAttribute>()
            where TAttribute : Attribute
            => GetAttributeTypes<TAttribute>().
                SelectMany(x => x.GetCustomAttributes<TAttribute>()).
                ToList();

        /// <summary></summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <returns></returns>
        public static List<TAttribute> GetAttributes<TAttribute>()
            where TAttribute : Attribute
            => GetAttributeTypes<TAttribute>().
                SelectMany(x => x.GetCustomAttributes<TAttribute>()).
                ToList();

        /// <summary></summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static List<TAttribute> GetAttributes<TInterface, TAttribute>(Func<TAttribute, bool> predicate)
            where TAttribute : Attribute => GetAttributes<TInterface, TAttribute>().Where(predicate).ToList();

        /// <summary></summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static List<TAttribute> GetAttributes<TAttribute>(Func<TAttribute, bool> predicate)
            where TAttribute : Attribute => GetAttributes<TAttribute>().Where(predicate).ToList();
    }
}
