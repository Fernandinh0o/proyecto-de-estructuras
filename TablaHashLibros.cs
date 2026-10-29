using System;
using System.Collections.Generic;
using System.Linq;

class NodoHash
{
    public string Clave;
    public Libro Valor;
    public NodoHash? Siguiente;
    public NodoHash(string clave, Libro valor) { Clave = clave; Valor = valor; }
}
public class TablaHashLibros
{
    private readonly NodoHash?[] tabla;
    private readonly int capacidad;
    public TablaHashLibros(int capacidadInicial = 53)
    {
        capacidad = capacidadInicial;
        tabla = new NodoHash?[capacidad];
    }
    private int ObtenerHash(string clave)
    {
        uint hash = 0;
        foreach (char c in clave) hash = (hash * 31 + c) % (uint)capacidad;
        return (int)hash;
    }
    public void Insertar(string clave, Libro libro)
    {
        int indice = ObtenerHash(clave);
        NodoHash? actual = tabla[indice];
        if (actual == null) { tabla[indice] = new NodoHash(clave, libro); return; }
        while (true)
        {
            if (actual.Clave == clave) { actual.Valor = libro; return; }
            if (actual.Siguiente == null) break;
            actual = actual.Siguiente;
        }
        actual.Siguiente = new NodoHash(clave, libro);
    }
    public Libro? Buscar(string clave)
    {
        NodoHash? actual = tabla[ObtenerHash(clave)];
        while (actual != null)
        {
            if (actual.Clave == clave) return actual.Valor;
            actual = actual.Siguiente;
        }
        return null;
    }
    public List<Libro> ObtenerTodos()
    {
        List<Libro> libros = new();
        foreach (NodoHash? nodo in tabla)
        {
            NodoHash? actual = nodo;
            while (actual != null) { libros.Add(actual.Valor); actual = actual.Siguiente; }
        }
        return libros.OrderBy(l => l.Codigo).ToList();
    }
    public void MostrarTodos()
    {
        var libros = ObtenerTodos();
        if (libros.Count == 0) Console.WriteLine("No hay libros registrados en la tabla hash.");
        foreach (var libro in libros) Console.WriteLine(libro);
    }
}