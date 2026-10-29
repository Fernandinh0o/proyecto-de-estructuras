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

    public List<string> ObtenerSedes() => listaAdyacencia.Keys.OrderBy(s => s).ToList();

    public List<(string Origen, string Destino, int Minutos)> ObtenerRutas()
    {
        var rutas = new List<(string, string, int)>();
        foreach (var par in listaAdyacencia)
            foreach (var arista in par.Value)
                rutas.Add((par.Key, arista.Destino, arista.CostoMinutos));
        return rutas;
    }


    // Resultados consumibles directamente por la interfaz gráfica.
    public string DescribirDijkstra(string origen, string destino)
    {
        if (!ExisteSede(origen) || !ExisteSede(destino)) return "Una o ambas sedes no existen.";
        var distancias = listaAdyacencia.Keys.ToDictionary(x => x, _ => int.MaxValue);
        var previos = new Dictionary<string, string>();
        var pendientes = new HashSet<string>(listaAdyacencia.Keys);
        distancias[origen] = 0;
        while (pendientes.Count > 0)
        {
            string actual = pendientes.OrderBy(x => distancias[x]).First();
            if (distancias[actual] == int.MaxValue) break;
            pendientes.Remove(actual);
            foreach (var ruta in listaAdyacencia[actual])
            {
                if (ruta.CostoMinutos < 0) throw new InvalidOperationException("Dijkstra requiere costos no negativos.");
                int propuesta = distancias[actual] + ruta.CostoMinutos;
                if (propuesta < distancias[ruta.Destino])
                {
                    distancias[ruta.Destino] = propuesta;
                    previos[ruta.Destino] = actual;
                }
            }
        }
        if (distancias[destino] == int.MaxValue)
            return $"No existe una ruta disponible entre {origen} y {destino}.";
        var camino = ConstruirCamino(previos, origen, destino);
        return $"RUTA MÁS RÁPIDA (DIJKSTRA)\r\n\r\nOrigen: {origen}\r\nDestino: {destino}\r\n\r\nCamino óptimo: {string.Join(" -> ", camino)}\r\nTiempo total: {distancias[destino]} minutos.";
    }

    public string DescribirBFSyDFS(string origen, string destino)
    {
        if (!ExisteSede(origen) || !ExisteSede(destino)) return "Una o ambas sedes no existen.";
        var visitados = new HashSet<string> { origen };
        var cola = new Queue<string>();
        var previos = new Dictionary<string, string>();
        var ordenBfs = new List<string>();
        cola.Enqueue(origen);
        while (cola.Count > 0)
        {
            string actual = cola.Dequeue();
            ordenBfs.Add(actual);
            foreach (var ruta in listaAdyacencia[actual])
                if (visitados.Add(ruta.Destino))
                {
                    previos[ruta.Destino] = actual;
                    cola.Enqueue(ruta.Destino);
                }
        }
        var ordenDfs = new List<string>();
        var dfsVisitados = new HashSet<string>();
        void Visitar(string sede)
        {
            dfsVisitados.Add(sede);
            ordenDfs.Add(sede);
            foreach (var ruta in listaAdyacencia[sede])
                if (!dfsVisitados.Contains(ruta.Destino)) Visitar(ruta.Destino);
        }
        Visitar(origen);
        string rutaBfs = visitados.Contains(destino)
            ? $"{string.Join(" -> ", ConstruirCamino(previos, origen, destino))} ({ConstruirCamino(previos, origen, destino).Count - 1} tramos)"
            : "No hay conexión entre las sedes seleccionadas.";
        return $"BFS - BÚSQUEDA EN ANCHURA\r\n{string.Join(" -> ", ordenBfs)}\r\n\r\nCamino con menos tramos hacia {destino}:\r\n{rutaBfs}\r\n\r\nDFS - BÚSQUEDA EN PROFUNDIDAD\r\n{string.Join(" -> ", ordenDfs)}\r\n\r\nSedes alcanzables: {ordenDfs.Count} de {listaAdyacencia.Count}.";
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