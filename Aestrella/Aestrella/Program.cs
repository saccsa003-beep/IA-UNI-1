using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    // 1. Grafo del mapa: Ciudad de origen -> Lista de (Ciudad destino, Distancia real g)
    static Dictionary<string, List<(string Destino, int Distancia)>> grafo = new Dictionary<string, List<(string, int)>>()
    {
        { "Oradea", new List<(string, int)> { ("Zerind", 71), ("Sibiu", 151) } },
        { "Zerind", new List<(string, int)> { ("Oradea", 71), ("Arad", 75) } },
        { "Arad", new List<(string, int)> { ("Zerind", 75), ("Sibiu", 140), ("Timisoara", 118) } },
        { "Timisoara", new List<(string, int)> { ("Arad", 118), ("Lugoj", 111) } },
        { "Lugoj", new List<(string, int)> { ("Timisoara", 111), ("Mehadia", 70) } },
        { "Mehadia", new List<(string, int)> { ("Lugoj", 70), ("Drobeta", 75) } },
        { "Drobeta", new List<(string, int)> { ("Mehadia", 75), ("Craiova", 120) } },
        { "Craiova", new List<(string, int)> { ("Drobeta", 120), ("Rimnicu Vilcea", 146), ("Pitesti", 138) } },
        { "Sibiu", new List<(string, int)> { ("Oradea", 151), ("Arad", 140), ("Fagaras", 99), ("Rimnicu Vilcea", 80) } },
        { "Rimnicu Vilcea", new List<(string, int)> { ("Sibiu", 80), ("Craiova", 146), ("Pitesti", 97) } },
        { "Fagaras", new List<(string, int)> { ("Sibiu", 99), ("Bucharest", 211) } },
        { "Pitesti", new List<(string, int)> { ("Rimnicu Vilcea", 97), ("Craiova", 138), ("Bucharest", 101) } },
        { "Bucharest", new List<(string, int)> { ("Fagaras", 211), ("Pitesti", 101), ("Giurgiu", 90), ("Urziceni", 85) } },
        { "Giurgiu", new List<(string, int)> { ("Bucharest", 90) } },
        { "Urziceni", new List<(string, int)> { ("Bucharest", 85), ("Vaslui", 142), ("Hirsova", 98) } },
        { "Neamt", new List<(string, int)> { ("Iasi", 87) } },
        { "Iasi", new List<(string, int)> { ("Neamt", 87), ("Vaslui", 92) } },
        { "Vaslui", new List<(string, int)> { ("Iasi", 92), ("Urziceni", 142) } },
        { "Hirsova", new List<(string, int)> { ("Urziceni", 98), ("Eforie", 86) } },
        { "Eforie", new List<(string, int)> { ("Hirsova", 86) } }
    };

    // 2. Tabla Heurística: Distancia estimada en línea recta aérea HACIA Bucharest h(n)
    static Dictionary<string, int> heuristicaBucharest = new Dictionary<string, int>()
    {
        { "Arad", 366 }, { "Bucharest", 0 }, { "Craiova", 160 }, { "Drobeta", 242 },
        { "Eforie", 161 }, { "Fagaras", 176 }, { "Giurgiu", 77 }, { "Hirsova", 151 },
        { "Iasi", 226 }, { "Lugoj", 244 }, { "Mehadia", 241 }, { "Neamt", 234 },
        { "Oradea", 380 }, { "Pitesti", 100 }, { "Rimnicu Vilcea", 193 },
        { "Sibiu", 253 }, { "Timisoara", 329 }, { "Urziceni", 80 },
        { "Vaslui", 199 }, { "Zerind", 374 }
    };

    static void Main()
    {
        string inicio = "Arad";
        string destino = "Bucharest";

        Console.WriteLine($"--- Buscando el camino más corto con A* de {inicio} a {destino} ---");
        BusquedaAEstrella(inicio, destino);
    }

    static void BusquedaAEstrella(string inicio, string destino)
    {
        if (!grafo.ContainsKey(inicio) || !grafo.ContainsKey(destino))
        {
            Console.WriteLine("Las ciudades indicadas no existen en el mapa.");
            return;
        }

        // gCosto: Costo real acumulado desde el inicio hasta la ciudad actual
        var gCosto = new Dictionary<string, int>();
        // fCosto: Costo total estimado g(n) + h(n)
        var fCosto = new Dictionary<string, int>();
        var previos = new Dictionary<string, string>();

        // Lista abierta (nodos por evaluar descubiertos)
        var listaAbierta = new HashSet<string>();

        // Inicialización del mapa
        foreach (var ciudad in grafo.Keys)
        {
            gCosto[ciudad] = int.MaxValue;
            fCosto[ciudad] = int.MaxValue;
            previos[ciudad] = null;
        }

        gCosto[inicio] = 0;
        fCosto[inicio] = heuristicaBucharest[inicio]; // Para Arad es 0 + 366 = 366
        listaAbierta.Add(inicio);

        while (listaAbierta.Count > 0)
        {
            // A* expande siempre el nodo con el menor valor f(n) absoluto
            string actual = listaAbierta.OrderBy(node => fCosto[node]).First();

            // Si llegamos a la meta, detenemos la búsqueda inmediatamente
            if (actual == destino) break;

            listaAbierta.Remove(actual);

            foreach (var vecino in grafo[actual])
            {
                // g(n) provisional = distancia hasta el nodo actual + distancia al vecino por carretera
                int gProvisional = gCosto[actual] + vecino.Distancia;

                // Si encontramos un camino más corto hacia este vecino
                if (gProvisional < gCosto[vecino.Destino])
                {
                    previos[vecino.Destino] = actual;
                    gCosto[vecino.Destino] = gProvisional;

                    // f(n) = g(n) + h(n)
                    fCosto[vecino.Destino] = gCosto[vecino.Destino] + heuristicaBucharest[vecino.Destino];

                    if (!listaAbierta.Contains(vecino.Destino))
                    {
                        listaAbierta.Add(vecino.Destino);
                    }
                }
            }
        }

        // Reconstrucción de la ruta óptima
        if (gCosto[destino] == int.MaxValue)
        {
            Console.WriteLine($"No fue posible trazar una ruta entre {inicio} y {destino}.");
            return;
        }

        var camino = new List<string>();
        string paso = destino;

        while (paso != null)
        {
            camino.Add(paso);
            paso = previos[paso];
        }
        camino.Reverse();

        // Imprimir resultados obtenidos
        Console.WriteLine("\n¡Ruta Óptima encontrada por A*!");
        Console.WriteLine("Camino: " + string.Join(" -> ", camino));
        Console.WriteLine($"Distancia total recorrida (g): {gCosto[destino]} km");
    }
}
