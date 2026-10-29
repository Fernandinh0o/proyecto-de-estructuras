using System;

// Nodo para el encadenamiento de colisiones en la Tabla Hash
class NodoHash
{
    public string Clave; // Código del libro
    public Libro Valor;  // Objeto Libro
    public NodoHash Siguiente;

    public NodoHash(string clave, Libro valor)
    {
        Clave = clave;
        Valor = valor;
        Siguiente = null;
    }
}

public class TablaHashLibros
{
    private NodoHash[] tabla;
    private int capacidad;
    private int elementos;

    public TablaHashLibros(int capacidadInicial = 53)
    {
        capacidad = capacidadInicial;
        tabla = new NodoHash[capacidad];
        elementos = 0;
    }

    // Función Hash simple basada en los caracteres de la cadena
    private int ObtenerHash(string clave)
    {
        int hash = 0;
        foreach (char c in clave)
        {
            hash = (hash * 31 + c) % capacidad;
        }
        return Math.Abs(hash);
    }

    public void Insertar(string clave, Libro libro)
    {
        int indice = ObtenerHash(clave);
        NodoHash nuevoNodo = new NodoHash(clave, libro);

        if (tabla[indice] == null)
        {
            tabla[indice] = nuevoNodo;
        }
        else
        {
            // Si hay colisión, recorremos la cubeta o actualizamos si la clave ya existe
            NodoHash actual = tabla[indice];
            while (true)
            {
                if (actual.Clave == clave)
                {
                    actual.Valor = libro; // Actualiza si ya existe
                    return;
                }
                if (actual.Siguiente == null) break;
                actual = actual.Siguiente;
            }
            actual.Siguiente = nuevoNodo;
        }
        elementos++;
    }

    public Libro Buscar(string clave)
    {
        int indice = ObtenerHash(clave);
        NodoHash actual = tabla[indice];

        while (actual != null)
        {
            if (actual.Clave == clave)
            {
                return actual.Valor;
            }
            actual = actual.Siguiente;
        }
        return null; // No encontrado
    }

    public void MostrarTodos()
    {
        bool hayRegistros = false;
        for (int i = 0; i < capacidad; i++)
        {
            NodoHash actual = tabla[i];
            while (actual != null)
            {
                Console.WriteLine(actual.Valor);
                hayRegistros = true;
                actual = actual.Siguiente;
            }
        }
        if (!hayRegistros)
        {
            Console.WriteLine("No hay libros registrados en la tabla hash.");
        }
    }
}