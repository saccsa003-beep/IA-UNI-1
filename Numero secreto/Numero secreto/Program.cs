using System;

class Program
{
    static void Main(string[] args)
    {
        // 1. Generar el número aleatorio entre 1 y 100
        Random random = new Random();
        int numeroSecreto = random.Next(1, 101);
        int contadorIntentos = 0;
        int intento = 0;

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("=========================================");
        Console.WriteLine("✨ ¡BIENVENIDO AL NÚMERO SECRETO! ✨");
        Console.WriteLine("=========================================");
        Console.ResetColor();
        Console.WriteLine("La computadora ha elegido un número entre 1 y 100.");
        Console.WriteLine("¿Podrás adivinarlo en la menor cantidad de intentos?\n");

        // Loop del juego
        while (intento != numeroSecreto)
        {
            Console.Write("Introduce tu intento: ");
            string input = Console.ReadLine();

            // Validación de entrada segura
            if (!int.TryParse(input, out intento) || intento < 1 || intento > 100)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("⚠️ Por favor, introduce un número válido entre 1 y 100.");
                Console.ResetColor();
                continue;
            }

            contadorIntentos++;

            if (intento < numeroSecreto)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("📈 El número secreto es MAYOR.\n");
            }
            else if (intento > numeroSecreto)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("📉 El número secreto es MENOR.\n");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n🎉 ¡Felicidades! Has adivinado el número secreto ({numeroSecreto}).");
            }
            Console.ResetColor();
        }

        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"📊 Total de intentos: {contadorIntentos}");
        Console.WriteLine("=========================================");
        Console.ResetColor();
    }
}
