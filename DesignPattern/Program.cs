using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace DotNetDesignPatternDemos
{

    // ---------------------------Builders--------------------------------- //


    public class CodeElement
    {
        public string Name, Type;
        public List<CodeElement> Elements = new List<CodeElement>();
        private const int indentSize = 2;

        public CodeElement()
        {

        }

        public CodeElement(string name, string type)
        {
            Name = name;
            Type = type;
        }
        // "{"
        private string ToStringImpl(int indent)
        {
            var sb = new StringBuilder();
            var i = new string(' ', indentSize * indent);
            if (!string.IsNullOrWhiteSpace(Type))
            {
                sb.Append($"\n{i}public {Type} {Name};");
            }
            else
            {
                sb.Append($"{i}public class {Name} \n {{");
            }

            foreach (var e in Elements)
                sb.Append(e.ToStringImpl(indent + 1));
            if (string.IsNullOrWhiteSpace(Type))
                sb.Append($"\n{i} }}\n");
            return sb.ToString();
        }

        public override string ToString()
        {
            return ToStringImpl(0);
        }
    }

    public class CodeBuilder
    {
        private readonly string rootName;

        public CodeBuilder(string rootName)
        {
            this.rootName = rootName;
            root.Name = rootName;
        }

        // not fluent
        public void AddField(string childName, string childText)
        {
            var e = new CodeElement(childName, childText);
            root.Elements.Add(e);
        }

        public CodeBuilder AddFieldFluent(string childName, string childText)
        {
            var e = new CodeElement(childName, childText);
            root.Elements.Add(e);
            return this;
        }

        public override string ToString()
        {
            return root.ToString();
        }

        public void Clear()
        {
            root = new CodeElement { Name = rootName };
        }

        CodeElement root = new CodeElement();
    }

    // ---------------------------Factory Method----------------------------------------- //

    public class Point
    {
        private readonly double x;
        private readonly double y;
        //Factory Method Design Pattern

        //public static Point NewCartesianPoint(double x, double y)
        //{
        //    return new Point(x, y);
        //}

        //public static Point NewPolarPoint(double rho, double theta)
        //{
        //    return new Point(rho * Math.Cos(theta), rho * Math.Sin(theta));
        //}

        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            var sB = new StringBuilder();
            sB.AppendLine("X: " + x);
            sB.AppendLine("Y: " + y);
            return sB.ToString();
        }
    }


    // ------------------ Asynchronous Factory Methods -------------------//

    public class Foo
    {
        private Foo()
        {
            // May be load a web page.
        }

        private async Task<Foo> InitAsync()
        {
            await Task.Delay(1000);
            return this;
        }

        public static Task<Foo> CreateAsync()
        {
            var result = new Foo();
            return result.InitAsync();
        }
    }

    //--------------------------- Factory  ---------------------------//

    //How about having Point Factory
    //We will face bunch of problems as we have the point constructor private.


    public class PointFactory
    {
        public static Point NewCartesianPoint(double x, double y)
        {
            return new Point(x, y);
        }

        public static Point NewPolarPoint(double rho, double theta)
        {
            return new Point(rho * Math.Cos(theta), rho * Math.Sin(theta));
        }
    }

    //--------------------------- Object Tracking and Bulk Replacement ---------------------------//

    public interface ITheme
    {
       string TextColor { get; }
       string  BgrColor { get; }
    }

    public class LightTheme : ITheme
    {
        public string TextColor => "black";

        public string BgrColor => "white";
    }

    public class DarkTheme : ITheme
    {
        public string TextColor => "white";

        public string BgrColor => "black";
    }

    public class TrackingThemeFactory
    {
        private readonly List<WeakReference<ITheme>> themes = new List<WeakReference<ITheme>>();
        public ITheme CreateTheme(bool dark) {
            ITheme theme = dark ? new DarkTheme() : new LightTheme();
            themes.Add(new WeakReference<ITheme>(theme));
            return theme;
        }

        public string FactoryInfo
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var weakRef in themes)
                {
                    if(weakRef.TryGetTarget(out var theme))
                    {
                        bool dark = theme is DarkTheme;
                        sb.Append(dark ? "Dark" : "Light")
                          .AppendLine(" : theme");
                    }
                }
                return sb.ToString();
            }
        }
    }

    //---------------------Bulk Replacing-----------------//

    public class ReplaceableThemeFactory
    {
        private readonly List<WeakReference<Ref<ITheme>>> themes = new();

        public ITheme createThemeImpl(bool dark)
        {
            return dark ? new DarkTheme() : new LightTheme(); 
        }

        public Ref<ITheme> CreateTheme(bool dark)
        {
            var r = new Ref<ITheme>(createThemeImpl(dark));
            themes.Add(new(r));
            return r;
        }

        public void ReplaceTheme(bool dark)
        {
            foreach (var weakRef in themes)
            {
                if(weakRef.TryGetTarget(out var reference)) { 
                    reference.Value = createThemeImpl(dark);
                }
            }
        }
    }



    public class Ref<T> where T : class
    {
        public T Value;

        public Ref(T value)
        {
            Value = value; 
        }
    }

    //--------------------------- --------------------------- --------------------------- //

    public class Demo
    {
        static void Main(string[] args)
        {
            // -------------------- Builder ----------------------------//

            //// if you want to build a simple HTML paragraph using StringBuilder
            //var hello = "hello";
            //var sb = new StringBuilder();
            //sb.Append("<p>");
            //sb.Append(hello);
            //sb.Append("</p>");
            //WriteLine(sb);

            //// now I want an HTML list with 2 words in it
            //var words = new[] { "hello", "world" };
            //sb.Clear();
            //sb.Append("<ul>");
            //foreach (var word in words)
            //{
            //    sb.AppendFormat("<li>{0}</li>", word);
            //}
            //sb.Append("</ul>");
            //WriteLine(sb);

            // ordinary non-fluent builder
            //var builder = new CodeBuilder("Person");
            //builder.AddField("Name", "string");
            //builder.AddField("Age", "int");
            //WriteLine(builder.ToString());

            //// fluent builder
            ////sb.Clear();
            //builder.Clear(); // disengage builder from the object it's building, then...
            //builder.AddFieldFluent("Name", "String").AddFieldFluent("Age", "int");
            //WriteLine(builder);

            // -------------------------------Factory Method-------------------------------------//

            //var pointCartesian = Point.NewCartesianPoint(4, 5);
            //var pointPolar = Point.NewPolarPoint(1.0, Math.PI/2);
            //WriteLine(pointCartesian);
            //WriteLine(pointPolar);

            //--------------------------- Asynchronous Factory Method ---------------------------//

            //Foo x = await Foo.CreateAsync();


            //--------------------------- Factory  ---------------------------//
            //var pointCartesian = PointFactory.NewCartesianPoint(4, 5);
            //var pointPolar = PointFactory.NewPolarPoint(1.0, Math.PI / 2);
            //WriteLine(pointCartesian);
            //WriteLine(pointPolar);

            //--------------------------- Object Tracking and Bulk Replacement ---------------------------//

            var factory = new TrackingThemeFactory();
            var theme1 = factory.CreateTheme(true);
            var theme2 = factory.CreateTheme(false);

            WriteLine(factory.FactoryInfo);

            var factory2 = new ReplaceableThemeFactory();
            var theme3 = factory2.CreateTheme(true);
            var theme4 = factory2.CreateTheme(false);

            WriteLine(theme3.Value.BgrColor);
            WriteLine(theme4.Value.BgrColor);

            factory2.ReplaceTheme(true);

            WriteLine(theme3.Value.BgrColor);
            WriteLine(theme4.Value.BgrColor);
        }
    }
}
