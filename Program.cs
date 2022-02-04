
var service = DependencyBuilder.BuildDependencyTree<A>();

Console.WriteLine(service.Message);

public static class DependencyBuilder
{
    public static T BuildDependencyTree<T>()
    {
        return (T)Build(typeof(T));
    }

    private static object Build(Type type)
    {
        var ctorParamInfo = type.GetConstructors().Single().GetParameters();

        if (ctorParamInfo is { Length: > 0 })
        {
            var paramInstances = new object[ctorParamInfo.Length];

            for (int i = 0; i < ctorParamInfo.Length; i++)
            {
                paramInstances[i] = Build(ctorParamInfo[i].ParameterType);
            }
            return CreateInstance(type, paramInstances);
        }
        return CreateInstance(type);
    }

    private static object CreateInstance(Type type, object[]? paramInstances = null)
    {
        var objectInstance = paramInstances == null
            ? Activator.CreateInstance(type)
            : Activator.CreateInstance(type, paramInstances);

        _ = objectInstance ?? throw new Exception($"Could not create instance of {nameof(type)}");

        return objectInstance;
    }
}

public class A
{
    private readonly B _b;
    public A(B b) => _b = b;
    public string Message => $"A depends on B\n{_b.Message}";
}

public class B
{
    private readonly C _c;
    public B(C c) => _c = c;
    public string Message => $"B depends on C\n{_c.Message}";
}

public class C
{
    public string Message => $"C has no Dependencies";
}

