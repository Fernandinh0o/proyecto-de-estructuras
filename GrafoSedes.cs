using System;
using System.Collections.Generic;

public class AristaRuta
{
    public string Destino;
    public int CostoMinutos; // Peso de la arista (tiempo/costo de traslado)

    public AristaRuta(string destino, int costoMinutos)
    {
        Destino = destino.Trim();
        CostoMinutos = costoMinutos;
    }
}

public class GrafoSedes
{
    private Dictionary<string, List<AristaRuta>> listaAdyacencia;

    public GrafoSedes()
    {
        listaAdyacencia = new Dictionary<string, List<AristaRuta>>();
    }

    public void AgregarSede(string nombreSede)
    {
        nombreSede = nombreSede.Trim();
        if (string.IsNullOrEmpty(nombreSede)) return;

        if (!listaAdyacencia.ContainsKey(nombreSede))
        {
            listaAdyacencia[nombreSede] = new List<AristaRuta>();
        }
    }

    public void AgregarRuta(string origen, string destino, int costoMinutos, bool dobleVia)
    {
        origen = origen.Trim();
        destino = destino.Trim();

        AgregarSede(origen);
        AgregarSede(destino);

        // Evitar duplicados directos
        if (!ExisteArista(origen, destino))
        {
            listaAdyacencia[origen].Add(new AristaRuta(destino, costoMinutos));
        }

        if (dobleVia && !ExisteArista(destino, origen))
        {
            listaAdyacencia[destino].Add(new AristaRuta(origen, costoMinutos));
        }
    }

    private bool ExisteArista(string origen, string destino)
    {
        foreach (var arista in listaAdyacencia[origen])
        {
            if (arista.Destino == destino) return true;
        }
        return false;
    }

    public bool ExisteSede(string nombre)
    {
        return listaAdyacencia.ContainsKey(nombre.Trim());
    }

    // =========================================================================
    // RECORRIDO BFS (Búsqueda en Anchura)
    // =========================================================================
    public void RecorridoBFS(string origen, string destino)
    {
        Queue<string> cola = new Queue<string>();
        HashSet<string> visitados = new HashSet<string>();
        Dictionary<string, string> previo = new Dictionary<string, string>();
        List<string> ordenVisita = new List<string>();

        cola.Enqueue(origen);
        visitados.Add(origen);

        while (cola.Count > 0)
        {
            string actual = cola.Dequeue();
            ordenVisita.Add(actual);

            foreach (var arista in listaAdyacencia[actual])
            {
                if (!visitados.Contains(arista.Destino))
                {
                    visitados.Add(arista.Destino);
                    previo[arista.Destino] = actual;
                    cola.Enqueue(arista.Destino);
                }
            }
        }

        Console.WriteLine($"\n--- RECORRIDO BFS DESDE SEDE '{origen}' ---");
        Console.WriteLine(string.Join(" -> ", ordenVisita));

        if (!visitados.Contains(destino))
        {
            Console.WriteLine($"No existe conexión en la red de sedes desde {origen} hasta {destino}.");
            return;
        }

        List<string> camino = ConstruirCamino(previo, origen, destino);
        Console.WriteLine($"Ruta con menor cantidad de saltos: {string.Join(" -> ", camino)} ({camino.Count - 1} tramos)");
    }

    // =========================================================================
    // RECORRIDO DFS (Búsqueda en Profundidad)
    // =========================================================================
    public void RecorridoDFS(string origen)
    {
        HashSet<string> visitados = new HashSet<string>();
        List<string> ordenVisita = new List<string>();

        DFSRecursivo(origen, visitados, ordenVisita);

        Console.WriteLine($"\n--- RECORRIDO DFS DESDE SEDE '{origen}' ---");
        Console.WriteLine(string.Join(" -> ", ordenVisita));
        Console.WriteLine($"Sedes totales alcanzables desde {origen}: {ordenVisita.Count} de {listaAdyacencia.Count}");
    }

    private void DFSRecursivo(string actual, HashSet<string> visitados, List<string> ordenVisita)
    {
        visitados.Add(actual);
        ordenVisita.Add(actual);

        foreach (var arista in listaAdyacencia[actual])
        {
            if (!visitados.Contains(arista.Destino))
            {
                DFSRecursivo(arista.Destino, visitados, ordenVisita);
            }
        }
    }

    // =========================================================================
    // ALGORITMO DE DIJKSTRA (Ruta más rápida / económica)
    // =========================================================================
    public void RutaMasRapida(string origen, string destino)
    {
        Dictionary<string, int> distancia = new Dictionary<string, int>();
        Dictionary<string, string> previo = new Dictionary<string, string>();
        HashSet<string> visitados = new HashSet<string>();

        foreach (var sede in listaAdyacencia.Keys)
        {
            distancia[sede] = int.MaxValue;
        }
        distancia[origen] = 0;

        while (true)
        {
            string actual = null;
            int menorDistancia = int.MaxValue;

            foreach (var sede in listaAdyacencia.Keys)
            {
                if (!visitados.Contains(sede) && distancia[sede] < menorDistancia)
                {
                    menorDistancia = distancia[sede];
                    actual = sede;
                }
            }

            if (actual == null || menorDistancia == int.MaxValue) break;
            visitados.Add(actual);

            foreach (var arista in listaAdyacencia[actual])
            {
                int nuevaDistancia = distancia[actual] + arista.CostoMinutos;
                if (nuevaDistancia < distancia[arista.Destino])
                {
                    distancia[arista.Destino] = nuevaDistancia;
                    previo[arista.Destino] = actual;
                }
            }
        }

        Console.WriteLine($"\n--- RUTA MÁS RÁPIDA (DIJKSTRA) DE {origen} A {destino} ---");
        if (distancia[destino] == int.MaxValue)
        {
            Console.WriteLine("No hay una ruta de traslado disponible.");
            return;
        }

        List<string> camino = ConstruirCamino(previo, origen, destino);
        Console.WriteLine($"Camino óptimo: {string.Join(" -> ", camino)}");
        Console.WriteLine($"Tiempo total estimado de traslado: {distancia[destino]} minutos.");
    }

    private List<string> ConstruirCamino(Dictionary<string, string> previo, string origen, string destino)
    {
        List<string> camino = new List<string>();
        string actual = destino;

        while (actual != origen)
        {
            camino.Insert(0, actual);
            actual = previo[actual];
        }
        camino.Insert(0, origen);
        return camino;
    }
}