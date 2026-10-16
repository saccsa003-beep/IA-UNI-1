using System;
using System.Collections.Generic;
using System.Linq;

class ProblemaArriero
{
    // Estructura para representar el estado del sistema
    // true = Izquierda (Inicio), false = Derecha (Destino)
    struct Estado
    {
        public bool Arriero;
        public bool Puma;
        public bool Cabra;
        public bool Lechuga;

        public Estado(bool a, bool l, bool cb, bool cl)
        {
            Arriero = a; Puma = l; Cabra = cb; Lechuga = cl;
        }

        // Verifica si el estado es válido según las reglas del acertijo
        public bool EsValido()
        {
            // Si la cabra está con el lobo y el arriero está en la otra orilla
            if (Cabra == Puma && Arriero != Cabra) return false;
            // Si la cabra está con la col y el arriero está en la otra orilla
            if (Cabra == Lechuga && Arriero != Cabra) return false;
            return true;
        }

        // Verifica si se llegó al estado objetivo (todos en la derecha / false)
        public bool EsObjetivo()
        {
            return !Arriero && !Puma && !Cabra && !Lechuga;
        }

        // Métodos necesarios para poder usar el Estado en un HashSet y Dictionary
        public override bool Equals(object obj)
        {
            if (!(obj is Estado)) return false;
            Estado otro = (Estado)obj;
            return Arriero == otro.Arriero && Puma == otro.Puma && Cabra == otro.Cabra && Lechuga == otro.Lechuga;
        }

        public override int GetHashCode()
        {
            return (Arriero ? 1 : 0) + (Puma ? 2 : 0) + (Cabra ? 4 : 0) + (Lechuga ? 8 : 0);
        }

        public override string ToString()
        {
            string izq = "", der = "";
            if (Arriero) izq += "Arriero "; else der += "Arriero ";
            if (Puma) izq += "Puma "; else der += "Puma ";
            if (Cabra) izq += "Cabra "; else der += "Cabra ";
            if (Lechuga) izq += "Lechuga "; else der += "Lechuga ";
            return $"[{izq.Trim()}] |===RÍO===| [{der.Trim()}]";
        }
    }

    static void Main()
    {
        // Estado inicial: Todos en la izquierda (true)
        Estado estadoInicial = new Estado(true, true, true, true);

        // Cola para BFS
        Queue<Estado> cola = new Queue<Estado>();
        // HashSet para evitar repetir estados ya visitados
        HashSet<Estado> visitados = new HashSet<Estado>();
        // Diccionario para rastrear el camino (Estado hijo -> Estado padre)
        Dictionary<Estado, Estado> padres = new Dictionary<Estado, Estado>();

        cola.Enqueue(estadoInicial);
        visitados.Add(estadoInicial);

        bool solucionEncontrada = false;
        Estado estadoFinal = new Estado();

        while (cola.Count > 0)
        {
            Estado actual = cola.Dequeue();

            if (actual.EsObjetivo())
            {
                solucionEncontrada = true;
                estadoFinal = actual;
                break;
            }

            // Generar todos los movimientos posibles del arriero
            List<Estado> proximosEstados = GenerarSiguientesEstados(actual);

            foreach (Estado siguiente in proximosEstados)
            {
                if (siguiente.EsValido() && !visitados.Contains(siguiente))
                {
                    visitados.Add(siguiente);
                    padres[siguiente] = actual;
                    cola.Enqueue(siguiente);
                }
            }
        }

        if (solucionEncontrada)
        {
            ImprimirCamino(padres, estadoFinal);
        }
        else
        {
            Console.WriteLine("No se encontró una solución.");
        }
    }

    // Genera las transiciones de estado válidas (el arriero cruza solo o con un objeto)
    static List<Estado> GenerarSiguientesEstados(Estado actual)
    {
        List<Estado> siguientes = new List<Estado>();
        bool nuevaPosArriero = !actual.Arriero;

        // Opción 1: El arriero cruza solo
        siguientes.Add(new Estado(nuevaPosArriero, actual.Puma, actual.Cabra, actual.Lechuga));

        // Opción 2: El arriero se lleva al lobo (deben estar en la misma orilla)
        if (actual.Arriero == actual.Puma)
            siguientes.Add(new Estado(nuevaPosArriero, !actual.Puma, actual.Cabra, actual.Lechuga));

        // Opción 3: El arriero se lleva a la cabra
        if (actual.Arriero == actual.Cabra)
            siguientes.Add(new Estado(nuevaPosArriero, actual.Puma, !actual.Cabra, actual.Lechuga));

        // Opción 4: El arriero se lleva la col
        if (actual.Arriero == actual.Lechuga)
            siguientes.Add(new Estado(nuevaPosArriero, actual.Puma, actual.Cabra, !actual.Lechuga));

        return siguientes;
    }

    // Reconstruye el camino desde el final hasta el inicio usando el diccionario de padres
    static void ImprimirCamino(Dictionary<Estado, Estado> padres, Estado final)
    {
        List<Estado> camino = new List<Estado>();
        Estado actual = final;

        while (padres.ContainsKey(actual))
        {
            camino.Add(actual);
            actual = padres[actual];
        }
        camino.Add(actual); // Agregar el estado inicial
        camino.Reverse();

        Console.WriteLine("=== PASOS PARA RESOLVER EL ACERTIJO ===");
        int paso = 0;
        foreach (var estado in camino)
        {
            Console.WriteLine($"Paso {paso++}: {estado}");
        }
    }
}
