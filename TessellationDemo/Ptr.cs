namespace TessellationDemo;

public class Ptr<T>
{
    public T Get { get; set; }

    public Ptr(T t)
    {
        Get = t;
    }
}