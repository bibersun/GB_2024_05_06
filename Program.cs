using System.Reflection;
using System.Text;

namespace lesson_7_hw;

class Program
{
    [AttributeUsage(AttributeTargets.Field)]
    public class CustomNameAttribute(string customName) : Attribute
    {
        public string CustomName { get; private set; } = customName;
    }

    static object? StringToObject(string s)
    {
        string[] arrayInfo = s.Split("\n");

        Console.WriteLine($"{arrayInfo[0]} - {arrayInfo[1]}");

        var t4 = Activator.CreateInstance(null, arrayInfo[1])?.Unwrap();

        if (t4 != null && arrayInfo.Length > 2)
        {
            Type type = t4.GetType();

            for (int i = 2; i < arrayInfo.Length; i++)
            {
                string[] arrayInfo2 = arrayInfo[i].Split("=");
                var prop = type.GetProperty(arrayInfo2[0]);

                if (prop == null) continue;
                if (prop.PropertyType == typeof(int))
                {
                    prop.SetValue(t4, int.Parse(arrayInfo2[1]));
                }
                else if (prop.PropertyType == typeof(string))
                {
                    prop.SetValue(t4, arrayInfo2[1]);
                }
                else if (prop.PropertyType == typeof(char[]))
                {
                    prop.SetValue(t4, arrayInfo2[1].ToCharArray());
                }
                else if (prop.PropertyType == typeof(decimal))
                {
                    prop.SetValue(t4, decimal.Parse(arrayInfo2[1]));
                }
            }
        }

        return t4;
    }

    static string ObjectToString(object o)
    {
        StringBuilder sb = new StringBuilder();

        Type type = o.GetType();

        sb.Append(type.Assembly + "\n");
        sb.Append(type.FullName + "\n");

        var properties = type.GetProperties();
        foreach (var prop in properties)
        {
            sb.Append(prop.Name + "=");
            var val = prop.GetValue(o);

            if (prop.PropertyType == typeof(char[]))
            {
                sb.Append(new string(val as char[]) + "\n");
            }
            else
            {
                sb.Append(val + "\n");
            }
        }

        var fields = type.GetFields();
        foreach (var field in fields)
        {
            var attribute = field.GetCustomAttribute<CustomNameAttribute>();
            if (attribute != null)
            {
                sb.Append(attribute.CustomName + "=");
                var fieldVal = field.GetValue(o);
                sb.Append(fieldVal + "\n");
            }
        }

        return sb.ToString();
    }

    static void Main(string[] args)
    {
        Task2();
    }

    private static void Task2()
    {
        Type type = typeof(TestClass);

        var t3 = Activator.CreateInstance(type, [10, new[] { 'A', 'B', 'C' }, "Hii", 10.01m, "Hi"]);

        if (t3 != null)
        {
            string objectToString = ObjectToString(t3);
            Console.WriteLine(objectToString);
            // var obj = StringToObject(objectToString);
        }
    }
}

internal class TestClass
{
    public int I { get; set; }
    public char[]? C { get; set; }
    public string? S { get; set; }
    public decimal D { get; set; }

    [Program.CustomName("CustomFieldName")]
    public string F = "Hi";

    private TestClass(int i)
    {
        I = i;
    }

    public TestClass()
    {
    }

    public TestClass(int i, char[]? c, string? s, decimal d, string f) : this(i)
    {
        C = c;
        S = s;
        D = d;
        F = f;
    }
}