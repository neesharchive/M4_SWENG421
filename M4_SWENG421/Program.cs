using System;
using System.Collections.Generic;


namespace p2
{
    // --- ProductIF interface ---
    public interface ProductIF : IComparable<ProductIF>
    {
        int Id { get; }             // immutable
        string Name { get; set; }
        decimal Price { get; set; }
    }

    // --- Marker interface for sort strategies (for future extensibility) ---
    public interface ISortStrategyMarker { }

    // --- Desk class implementing ProductIF ---
    public class Desk : ProductIF
    {
        public int Id { get; }      // immutable once set
        public string Name { get; set; }
        public decimal Price { get; set; }

        public Desk(int id, decimal price, string name)
        {
            Id = id;
            Price = price;
            Name = name;
            Price = price;
        }

        // enable IComparable for List.Sort quicksort path
        public int CompareTo(ProductIF other)
        {
            if (other == null) return 1;
            return this.Price.CompareTo(other.Price);
        }
    }

    // --- SortUtility base class ---
    public class SortUtility<T> where T : ProductIF
    {
        private string sortName = "bubblesort";

        public SortUtility(string sortName)
        {
            this.sortName = sortName;
        }

        public string getName() { return sortName; }
        public void setName(string sortName) { this.sortName = sortName; }

        // Default bubble sort implementation
        public virtual List<T> sort(List<T> data)
        {
            if (data == null) return data;

            for (int i = 0; i < data.Count - 1; i++)
            {
                for (int j = 0; j < data.Count - 1 - i; j++)
                {
                    if (data[j].Price > data[j + 1].Price)
                    {
                        var tmp = data[j];
                        data[j] = data[j + 1];
                        data[j + 1] = tmp;
                    }
                }
            }
            return data;
        }
    }

    // --- BubblesortUtility subclass ---
    public class BubblesortUtility<T> : SortUtility<T>, ISortStrategyMarker where T : ProductIF
    {
        public BubblesortUtility(string sortName) : base(sortName) { }

        // Printing: ID, Name and Price
        public void Print(List<T> data)
        {
            foreach (var p in data)
            {
                Console.WriteLine($"{p.Id}, {p.Name} and {p.Price:F2}");
            }
        }
    }

    // --- QuicksortUtility subclass ---
    public class QuicksortUtility<T> : SortUtility<T>, ISortStrategyMarker where T : ProductIF
    {
        public QuicksortUtility(string sortName) : base(sortName) { }

        // Override sort to use built-in quicksort (List.Sort)
        public override List<T> sort(List<T> data)
        {
            data?.Sort(); // uses ProductIF.CompareTo by Price
            return data;
        }

        // Printing: Price, Name, and ID
        public void Print(List<T> data)
        {
            foreach (var p in data)
            {
                Console.WriteLine($"{p.Price:F2}, {p.Name}, and {p.Id}");
            }
        }
    }
}

namespace p1
{
    using p2;

    // --- Company class (unchanged, proxy to sortUtility) ---
    public class Company
    {
        public SortUtility<ProductIF> sortUtility;
    }

    // --- MyProg demonstration ---
    public class MyProg
    {
        public static void Main(string[] args)
        {
            Company xyz = new Company();

            // Scenario 1: bubble sort
            xyz.sortUtility = new BubblesortUtility<ProductIF>("bubblesort");
            var desks1 = new MyProg().GetProducts();
            var sorted1 = xyz.sortUtility.sort(desks1);
            ((BubblesortUtility<ProductIF>)xyz.sortUtility).Print(sorted1);

            Console.WriteLine();

            // Scenario 2: quicksort
            xyz.sortUtility = new QuicksortUtility<ProductIF>("quicksort");
            var desks2 = new MyProg().GetProducts();
            var sorted2 = xyz.sortUtility.sort(desks2);
            ((QuicksortUtility<ProductIF>)xyz.sortUtility).Print(sorted2);
        }

        public List<ProductIF> GetProducts()
        {
            return new List<ProductIF>
            {
                new Desk(1, 20.30m, "Writing Desk"),
                new Desk(2, 15.25m, "Corner Desk"),
                new Desk(3, 25.13m, "Lap Desk"),
                new Desk(4, 15.85m, "Standing Desk"),
                new Desk(5, 22.56m, "Floating Desk"),
            };
        }
    }
}
