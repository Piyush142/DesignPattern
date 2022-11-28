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

    //public class Point
    //{
    //    private readonly double x;
    //    private readonly double y;
    //    //Factory Method Design Pattern

    //    //public static Point NewCartesianPoint(double x, double y)
    //    //{
    //    //    return new Point(x, y);
    //    //}

    //    //public static Point NewPolarPoint(double rho, double theta)
    //    //{
    //    //    return new Point(rho * Math.Cos(theta), rho * Math.Sin(theta));
    //    //}

    //    public Point(double x, double y)
    //    {
    //        this.x = x;
    //        this.y = y;
    //    }

    //    public override string ToString()
    //    {
    //        var sB = new StringBuilder();
    //        sB.AppendLine("X: " + x);
    //        sB.AppendLine("Y: " + y);
    //        return sB.ToString();
    //    }
    //}


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


    //public class PointFactory
    //{
    //    public static Point NewCartesianPoint(double x, double y)
    //    {
    //        return new Point(x, y);
    //    }

    //    public static Point NewPolarPoint(double rho, double theta)
    //    {
    //        return new Point(rho * Math.Cos(theta), rho * Math.Sin(theta));
    //    }
    //}

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
                if (weakRef.TryGetTarget(out var reference))
                {
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


    //---------------------Inner Factory-----------------//

    public class Point
    {
        private readonly double x;
        private readonly double y;

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

        public static class Factory
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
    }


    //--------------------Abstract Factory-----------------//

    public interface IHotDrink
    {
        void Consume();
    }

    internal class Tea : IHotDrink
    {
        public void Consume()
        {
            WriteLine("Tea with milk is delicious you should try it.");
        }
    }

    internal class Coffee : IHotDrink
    {
        public void Consume()
        {
            WriteLine("Coffee can keep you awake for hours");
        }
    }

    public interface IHotDrinkFactory
    {
        IHotDrink Prepare(int amount);
    }

    internal class TeaFactory : IHotDrinkFactory
    {
        public IHotDrink Prepare(int amount)
        {
            WriteLine($"Add Tea leaves with {amount} ml Milk and add sugar");
            return new Tea();
        }
    }

    internal class CoffeeFactory : IHotDrinkFactory
    {
        public IHotDrink Prepare(int amount)
        {
            WriteLine($"Add Beans, boil water and add {amount} of milk then add cream and sugar");
            return new Coffee();
        }
    }

    //public class HotDrinksMachine
    //{

    //    //Here we are voilating the open closed principle. What if you want to introduce a new drink in future? You will have to change the Enum
    //    // values and that is the voliation of the openclosed principle. To work around this issue we use reflection.
    //    public enum AvailableDrinks
    //    {
    //        Coffee,
    //        Tea             
    //    }

    //    private Dictionary<AvailableDrinks, IHotDrinkFactory> factories = new Dictionary<AvailableDrinks, IHotDrinkFactory>();

    //    public HotDrinksMachine()
    //    {
    //        foreach (AvailableDrinks drink in Enum.GetValues(typeof(AvailableDrinks)))
    //        {
    //            var factory = (IHotDrinkFactory)Activator.CreateInstance(Type.GetType("DotNetDesignPatternDemos."
    //                + Enum.GetName(typeof(AvailableDrinks), drink) + "Factory"));
    //            factories.Add(drink, factory);
    //        }
    //    }

    //    public IHotDrink MakeDrink(AvailableDrinks drink, int amount){
    //        return factories[drink].Prepare(amount);
    //    }
    //}

    //--------------------Abstract Factory and OCP-----------------//

    public class HotDrinkMachine
    {
        List<Tuple<string, IHotDrinkFactory>> factories = new List<Tuple<string, IHotDrinkFactory>>();
        public HotDrinkMachine()
        {
            foreach (var t in typeof(HotDrinkMachine).Assembly.GetTypes())
            {
                if (typeof(IHotDrinkFactory).IsAssignableFrom(t) && !t.IsInterface)
                {
                    factories.Add(Tuple.Create(t.Name.Replace("Factory", String.Empty), (IHotDrinkFactory)Activator.CreateInstance(t)));
                }
            }
        }

        public IHotDrink MakeDrink
        {
            get
            {
                WriteLine("Available Drinks");
                for (int index = 0; index < factories.Count; index++)
                {
                    Tuple<string, IHotDrinkFactory>? tuple = factories[index];
                    WriteLine($"{index}: {tuple.Item1}");
                }
                while (true)
                {
                    string s;
                    if ((s = Console.ReadLine()) != null
                        && int.TryParse(s, out int i)
                        && i >= 0
                        && i < factories.Count()
                        )
                    {
                        WriteLine("Specify Amount: ");
                        s = ReadLine();
                        if (s != null
                            && int.TryParse(s, out int amount)
                            && amount > 0)
                        {
                            return factories[i].Item2.Prepare(amount);
                        }
                    }
                    WriteLine("Incorrect Selection, try again!");
                }
            }
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

            //var factory = new TrackingThemeFactory();
            //var theme1 = factory.CreateTheme(true);
            //var theme2 = factory.CreateTheme(false);

            //WriteLine(factory.FactoryInfo);

            //var factory2 = new ReplaceableThemeFactory();
            //var theme3 = factory2.CreateTheme(true);
            //var theme4 = factory2.CreateTheme(false);

            //WriteLine(theme3.Value.BgrColor);
            //WriteLine(theme4.Value.BgrColor);

            //factory2.ReplaceTheme(true);

            //WriteLine(theme3.Value.BgrColor);
            //WriteLine(theme4.Value.BgrColor);

            //---------------------Inner Factory-----------------//

            //var point = Point.Factory.NewCartesianPoint(3, 5);
            //WriteLine(point);


            //--------------------Abstract Factory-----------------//
            //var hotDrink = new HotDrinksMachine();
            //var Tea = hotDrink.MakeDrink(HotDrinksMachine.AvailableDrinks.Tea, 200);
            ////var Coffee = hotDrink.MakeDrink(HotDrinksMachine.AvailableDrinks.Coffee, 100);

            //Tea.Consume();
            //Coffee.Consume();

            //--------------------Abstract Factory and OCP-----------------//

            var hotdrinkMachine = new HotDrinkMachine();
            var drink = hotdrinkMachine.MakeDrink;
            drink.Consume();
        }
    }
}
