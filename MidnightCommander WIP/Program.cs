using System;
using MidnightCommander.Editor;

namespace MidnightCommander
{
    class Program
    {
        static void Main(string[] args)
        {
            /* TODO:
             *          Nebrat si věci osobně ¯\_(ツ)_/¯
             *          Cute kód
             *          
             *          Předělat barvy aby fungovali všude
             */

            //Dafualtní nastavení
            Console.WindowHeight = 30;
            Console.WindowWidth = 120;
            Console.Title = "Midnight Commnader";

            Application main = new();

            while(true)
            {
                main.HandleKey(Console.ReadKey(true));
            }
        }
    }
}
