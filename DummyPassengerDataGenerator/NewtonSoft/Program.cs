using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NewtonSoft
{
    class Program
    {
        static void Main(string[] args)
        {
            List<PassengerInfo> passengerInfoList = new List<PassengerInfo>();
            Console.WriteLine("Enter No. Of random passengers to be generated for each city ?");
            int reqCount = Int32.Parse(Console.ReadLine());
            PassengerInfo passenger = null;
            
            string[] destination = ConfigurationManager.AppSettings["Destinations"].Split(",".ToCharArray());
           // destination = { "Mumbai", "Delhi", "Bengaluru", "Kolkata", "Chennai", "Ahmedabad", "Hyderabad", "Pune", "Surat", "Kanpur", "Jaipur", "Lucknow", "Nagpur", "Indore", "Patna", "Bhopal", "Ludhiana", "Tirunelveli", "Agra", "Mumbai", "Delhi", "Bengaluru", "Kolkata", "Chennai" };
            List<string> destList = destination.ToList<string>();

            Parallel.ForEach(destList, desti =>
             {
                 Parallel.For(0, reqCount, i =>
                  {
                      passenger = new PassengerInfo();
                      Random randomNumber = new Random();
                      var age = GetRandomDate(1945);
                      var travelDate = GetRandomDate(2017).Item1.Date;
                      passenger.Gender = randomNumber.Next(1, 2);
                      passenger.StringDateOfBirth = 
                      (age.Item1 < DateTime.MinValue) ? String.Format("{0:MM/dd/yyyy}", DateTime.Today.AddMonths(-4).Date) : String.Format("{0:MM/dd/yyyy}",age.Item1.Date);
                      passenger.Age = (DateTime.Today.Year - passenger.DateOfBirth.Year);
                      passenger.Mode = (TravelMode)(new Random(1)).Next(1, 3);
                      passenger.StringTravelDate = 
                      (travelDate < DateTime.MinValue) ? String.Format("{0:MM/dd/yyyy}", DateTime.Today.AddDays(-30)) : String.Format("{0:MM/dd/yyyy}", travelDate);
                      passenger.Origin = GetRandomPlaces().Item1; 
                      passenger.Destination = desti;
                      Console.WriteLine(passenger.DateOfBirth + "--" + passenger.Origin + "--" + passenger.Destination + "--"+passenger.Age + "--"+passenger.TravelDate);
                      passengerInfoList.Add(passenger);
                  });
             });
            string fileNameSuffix = DateTime.Now.ToString().Replace("/","-").Replace(":","-") + ".csv";
           CreateCSV<PassengerInfo>(passengerInfoList, "travelInfoSampe_"+ fileNameSuffix);
        }

        private static Tuple<string> GetRandomPlaces()
        {
            string[] origin = ConfigurationManager.AppSettings["Origins"].Split(",".ToCharArray());
          //  origin = { "Mumbai", "Delhi", "Bengaluru", "Kolkata", "Chennai", "Ahmedabad", "Hyderabad", "Pune", "Surat", "Kanpur", "Jaipur", "Lucknow", "Nagpur", "Indore", "Patna", "Bhopal", "Ludhiana", "Tirunelveli", "Agra", "Vadodara", "Gorakhpur", "Nashik", "Pimpri", "Kalyan", "Thane", "Meerut", "Nowrangapur", "Faridabad", "Ghaziabad", "Dombivli", "Rajkot", "Varanasi", "Amritsar", "Allahabad", "Visakhapatnam", "Teni", "Jabalpur", "Haora", "Aurangabad", "Shivaji Nagar", "Solapur", "Srinagar", "Chandigarh", "Coimbatore", "Jodhpur", "Madurai", "Guwahati", "Gwalior", "Vijayawada", "Mysore", "Ranchi", "Hubli", "Jalandhar", "Thiruvananthapuram", "Salem", "Tiruchirappalli" };
            int y = ((new Random()).Next(10000, 560000)) / 10000;
            return new Tuple<string>(origin[y]);
        }
        private static Tuple<DateTime, int> GetRandomDate(int fromYear)
        {
            DateTime start = new DateTime(fromYear, 1, 1);
            int range = (DateTime.Today - start).Days;
            DateTime randomDate = start.AddDays((new Random()).Next(range));
            return new Tuple<DateTime, int>(randomDate, DateTime.Today.Year - randomDate.Year);
        }

        public static void CreateCSV<T>(List<T> list, string filePath)
        {
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                CreateHeader(list, sw);
                CreateRows(list, sw);
            }
        }
        private static void CreateHeader<T>(List<T> list, StreamWriter sw)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            for (int i = 0; i < properties.Length - 1; i++)
            {
                sw.Write(properties[i].Name + ",");
            }
            var lastProp = properties[properties.Length - 1].Name;
            sw.Write(lastProp + sw.NewLine);
        }
        private static void CreateRows<T>(List<T> list, StreamWriter sw)
        {
            foreach (var item in list)
            {
                if (item != null)
                {
                    PropertyInfo[] properties = typeof(T).GetProperties();
                    for (int i = 0; i < properties.Length - 1; i++)
                    {
                        var prop = properties[i];
                        sw.Write(prop.GetValue(item) + ",");
                    }
                    var lastProp = properties[properties.Length - 1];
                    sw.Write(lastProp.GetValue(item) + sw.NewLine);
                }
            }
        }
    }

    public class PassengerInfo
    {
        public string StringDateOfBirth { get; set; }
        public string StringTravelDate { get; set; }
        public DateTime DateOfBirth { get; set; } = DateTime.Today.AddYears(-30);
        public string Origin { get; set; } = "Ahmedabad";
        public string Destination { get; set; } = "Mumbai";
        public TravelMode Mode { get; set; } = TravelMode.Train;
        public DateTime TravelDate { get; set; } = DateTime.Today.AddMonths(-3);
        public int Age { get; set; } = 30;
        public int Gender { get; set; } = 1;
    }

    public enum TravelMode
    {
        Bus=1,
        Train,
        Air
    }
}
