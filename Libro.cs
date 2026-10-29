using System;

public class Libro
{
    public string Codigo { get; set; }      // Llave para la Tabla Hash y el Trie
    public string Titulo { get; set; }      // Usado para el autocompletado en el Trie
    public string Autor { get; set; }
    public string SedeActual { get; set; }  // Ubicación física actual de la sede

    public Libro(string codigo, string titulo, string autor, string sedeActual)
    {
        Codigo = codigo.Trim();
        Titulo = titulo.Trim();
        Autor = autor.Trim();
        SedeActual = sedeActual.Trim();
    }

    public override string ToString()
    {
        return $"[Código: {Codigo}] '{Titulo}' - Autor: {Autor} | Ubicación: Sede {SedeActual}";
    }
}