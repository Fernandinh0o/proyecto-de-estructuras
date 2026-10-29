using System;

// Nodo individual de cada letra en el Trie
class NodoTrie
{
    public char Caracter;
    public NodoTrie[] Hijos;
    public bool EsFinDePalabra;
    public string TituloCompleto; // Guarda el título completo al llegar al final

    public NodoTrie(char caracter)
    {
        Caracter = caracter;
        Hijos = new NodoTrie[256]; // Cobertura para caracteres ASCII extendidos
        EsFinDePalabra = false;
        TituloCompleto = null;
    }
}

public class TrieLibros
{
    private NodoTrie raiz;

    public TrieLibros()
    {
        raiz = new NodoTrie('\0');
    }

    // Insertar un título en el Trie
    public void Insertar(string titulo)
    {
        if (string.IsNullOrEmpty(titulo)) return;
        string tituloLower = titulo.Trim().ToLower(); // Normalizamos a minúsculas para búsqueda uniforme
        NodoTrie actual = raiz;

        foreach (char c in tituloLower)
        {
            int index = (int)c;
            if (index < 0 || index >= 256) continue; // Seguridad de índice

            if (actual.Hijos[index] == null)
            {
                actual.Hijos[index] = new NodoTrie(c);
            }
            actual = actual.Hijos[index];
        }
        actual.EsFinDePalabra = true;
        actual.TituloCompleto = titulo.Trim(); // Guardamos el formato original del título
    }


    public System.Collections.Generic.List<string> ObtenerSugerencias(string prefijo)
    {
        var resultados = new System.Collections.Generic.List<string>();
        if (string.IsNullOrWhiteSpace(prefijo)) return resultados;
        NodoTrie actual = raiz;
        foreach (char c in prefijo.Trim().ToLowerInvariant())
        {
            int indice = (int)c;
            if (indice >= 256 || actual.Hijos[indice] == null) return resultados;
            actual = actual.Hijos[indice];
        }
        void Recolectar(NodoTrie nodo)
        {
            if (nodo.EsFinDePalabra && nodo.TituloCompleto != null)
                resultados.Add(nodo.TituloCompleto);
            for (int i = 0; i < nodo.Hijos.Length; i++)
                if (nodo.Hijos[i] != null) Recolectar(nodo.Hijos[i]);
        }
        Recolectar(actual);
        return resultados;
    }

    // Buscar sugerencias de títulos que comiencen con un prefijo
    public void BuscarPorPrefijo(string prefijo)
    {
        if (string.IsNullOrEmpty(prefijo))
        {
            Console.WriteLine("El prefijo no puede estar vacío.");
            return;
        }

        string prefijoLower = prefijo.Trim().ToLower();
        NodoTrie actual = raiz;

        // Navegamos hasta el último nodo del prefijo
        foreach (char c in prefijoLower)
        {
            int index = (int)c;
            if (index < 0 || index >= 256 || actual.Hijos[index] == null)
            {
                Console.WriteLine("No se encontraron títulos con ese prefijo.");
                return;
            }
            actual = actual.Hijos[index];
        }

        Console.WriteLine($"\n--- SUCESORAS / SUGERENCIAS PARA '{prefijo}': ---");
        int contador = 0;
        RecaudarSugerencias(actual, ref contador);
        if (contador == 0)
        {
            Console.WriteLine("No hay más títulos registrados bajo este prefijo.");
        }
    }

    // Método recursivo auxiliar para recolectar todas las palabras hijas a partir del prefijo
    private void RecaudarSugerencias(NodoTrie nodo, ref int contador)
    {
        if (nodo == null) return;

        if (nodo.EsFinDePalabra)
        {
            contador++;
            Console.WriteLine($"  {contador}. {nodo.TituloCompleto}");
        }

        for (int i = 0; i < 256; i++)
        {
            if (nodo.Hijos[i] != null)
            {
                RecaudarSugerencias(nodo.Hijos[i], ref contador);
            }
        }
    }
}