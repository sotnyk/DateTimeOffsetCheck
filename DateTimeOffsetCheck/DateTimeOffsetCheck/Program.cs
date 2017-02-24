using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DateTimeOffsetCheck
{
    class Program
    {
        static void Main(string[] args)
        {            
            CheckDTOBehaviour();
            CheckDtoInDB();
            Console.ReadLine();
        }

        private static void CheckDtoInDB()
        {
            var db = new Database();
        }

        private static void CheckDTOBehaviour()
        {
            var now = DateTimeOffset.Now;
            var utcnow = DateTimeOffset.UtcNow;
            DateTime dtnow = now.DateTime;
            DateTime dtutcnow = utcnow.DateTime;

            Console.WriteLine($"now={now}, utcnow={utcnow}");
            Console.WriteLine($"now==utcnow: {now == utcnow}");
            Console.WriteLine($"now.UtcDateTime==utcnow.UtcDateTime: {now.UtcDateTime == utcnow.UtcDateTime}");
            Console.WriteLine($"now.UtcTicks={now.UtcTicks}, utcnow.UtcTicks={utcnow.UtcTicks}");
            Console.WriteLine();
            Console.WriteLine($"dtnow={dtnow}, dtutcnow={dtutcnow}");
            Console.WriteLine($"dtnow==dtutcnow: {dtnow == dtutcnow}");
            Console.WriteLine($"dtnow.Kind={dtnow.Kind}");
            Console.WriteLine($"dtutcnow.Kind={dtutcnow.Kind}");
            Console.WriteLine();
            now = new DateTimeOffset(2017, 2, 24, 12, 38, 0, TimeSpan.FromHours(2));
            utcnow = new DateTimeOffset(2017, 2, 24, 10, 38, 0, TimeSpan.FromHours(0));
            Console.WriteLine($"now={now}, utcnow={utcnow}");
            Console.WriteLine($"now==utcnow: {now == utcnow}");
            Console.WriteLine($"now.UtcDateTime==utcnow.UtcDateTime: {now.UtcDateTime == utcnow.UtcDateTime}");
        }
    }
}
