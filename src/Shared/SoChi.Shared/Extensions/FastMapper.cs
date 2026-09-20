using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace SoChi.Shared.Extensions;

public static class FastMapper
{
    private static readonly ConcurrentDictionary<string, object> _cache = new();

    public static TDestination Map<TSource, TDestination>(TSource source)
    {
        if (source == null) return default!;

        var key = GetKey<TSource, TDestination>();

        var func = (Func<TSource, TDestination>)_cache.GetOrAdd(key, _ =>
        {
            return CreateMapFunc<TSource, TDestination>();
        });

        return func(source);
    }

    public static List<TDestination> MapList<TSource, TDestination>(IEnumerable<TSource> source)
    {
        if (source == null) return null!;

        return source.Select(Map<TSource, TDestination>).ToList();
    }

    private static string GetKey<TSource, TDestination>()
        => $"{typeof(TSource).FullName}_{typeof(TDestination).FullName}";

    private static Func<TSource, TDestination> CreateMapFunc<TSource, TDestination>()
    {
        var sourceParam = Expression.Parameter(typeof(TSource), "src");

        var bindings = new List<MemberBinding>();

        var sourceProps = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var destProps = typeof(TDestination).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var config = MapperConfig.GetConfig<TSource, TDestination>();

#if DEBUG
        var unmapped = destProps
            .Where(d => d.CanWrite)
            .Where(d => sourceProps.All(s => s.Name != d.Name))
            .Where(d => config?.CustomMaps.ContainsKey(d.Name) != true)
            .Select(d => d.Name)
            .ToList();

        if (unmapped.Count > 0)
            System.Diagnostics.Debug.WriteLine(
                $"[FastMapper] {typeof(TSource).Name} -> {typeof(TDestination).Name}: " +
                $"không map được {string.Join(", ", unmapped)}");
#endif

        foreach (var destProp in destProps)
        {
            if (!destProp.CanWrite) continue;

            Expression valueExpr = null!;

            // 1. Custom mapping
            if (config != null && config.CustomMaps.TryGetValue(destProp.Name, out var customMap))
            {
                valueExpr = ReplaceParameter(customMap.Body, customMap.Parameters[0], sourceParam);
            }
            else
            {
                var sourceProp = sourceProps.FirstOrDefault(p => p.Name == destProp.Name);
                if (sourceProp == null) continue;

                var sourceAccess = Expression.Property(sourceParam, sourceProp);

                // 2. Same type
                if (sourceProp.PropertyType == destProp.PropertyType)
                {
                    valueExpr = sourceAccess;
                }
                else
                {
                    // 3. Try convert
                    valueExpr = Expression.Convert(sourceAccess, destProp.PropertyType);
                }
            }

            if (valueExpr == null) continue;

            bindings.Add(Expression.Bind(destProp, valueExpr));
        }

        var body = Expression.MemberInit(
            Expression.New(typeof(TDestination)),
            bindings
        );

        var lambda = Expression.Lambda<Func<TSource, TDestination>>(body, sourceParam);

        return lambda.Compile();
    }

    private static Expression ReplaceParameter(Expression body, ParameterExpression oldParam, Expression newParam)
    {
        return new ReplaceVisitor(oldParam, newParam).Visit(body);
    }
    private class ReplaceVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _oldParam;
        private readonly Expression _newParam;

        public ReplaceVisitor(ParameterExpression oldParam, Expression newParam)
        {
            _oldParam = oldParam;
            _newParam = newParam;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _oldParam ? _newParam : base.VisitParameter(node);
        }
    }
}
public class MapConfig<TSource, TDestination>
{
    public Dictionary<string, LambdaExpression> CustomMaps { get; } = new();

    public MapConfig<TSource, TDestination> ForMember<TMember>(
        Expression<Func<TDestination, TMember>> dest,
        Expression<Func<TSource, TMember>> map)
    {
        var name = ((MemberExpression)dest.Body).Member.Name;
        CustomMaps[name] = map;
        return this;
    }
}

public static class MapperConfig
{
    private static readonly ConcurrentDictionary<string, object> _configs = new();

    public static MapConfig<TSource, TDestination> CreateMap<TSource, TDestination>()
    {
        var key = $"{typeof(TSource).FullName}_{typeof(TDestination).FullName}";
        var config = new MapConfig<TSource, TDestination>();
        _configs[key] = config;
        return config;
    }

    public static MapConfig<TSource, TDestination> GetConfig<TSource, TDestination>()
    {
        var key = $"{typeof(TSource).FullName}_{typeof(TDestination).FullName}";
        return _configs.TryGetValue(key, out var config)
            ? (MapConfig<TSource, TDestination>)config
            : null!;
    }
}