using System;

namespace MidnightCommander
{
    class Program
    {
        static void Main(string[] args)
        {
            /* TODO:
             *          Nebrat si věci osobně ¯\_(ツ)_/¯
             *          Cute kód
             *          Editec texťákec
             */

            //Dafualtní nastavení
            Console.WindowHeight = 30;
            Console.WindowWidth = 120;
            Console.Title = "Midnight Commnader";

            Application main = new();

            while(true)
            {
                Console.CursorVisible = false;
                main.HandleKey(Console.ReadKey(true));
            }
        }
    }
}
