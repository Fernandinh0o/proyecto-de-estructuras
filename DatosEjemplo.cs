public static class DatosEjemplo
{
    public static void Cargar(TablaHashLibros hash, TrieLibros trie, GrafoSedes grafo, UnionFindSedes uf)
    {
        string[] sedes = { "Sede Central", "Sede Norte", "Sede Sur", "Sede Este", "Sede Oeste" };
        foreach (string sede in sedes) { grafo.AgregarSede(sede); uf.RegistrarSede(sede); }
        grafo.AgregarRuta("Sede Central", "Sede Norte", 15, true);
        uf.Unir("Sede Central", "Sede Norte");
        grafo.AgregarRuta("Sede Norte", "Sede Este", 25, true);
        uf.Unir("Sede Norte", "Sede Este");
        grafo.AgregarRuta("Sede Central", "Sede Sur", 20, true);
        uf.Unir("Sede Central", "Sede Sur");
        Libro[] libros = {
            new("L001", "Estructuras de Datos Avanzadas", "Cormen", "Sede Central"),
            new("L002", "Algoritmos en C Sharp", "John Smith", "Sede Norte"),
            new("L003", "Base de Datos I", "Elmasri", "Sede Sur"),
            new("L004", "Ingenieria del Software", "Pressman", "Sede Este"),
            new("L005", "Redes de Computadoras", "Tanenbaum", "Sede Central")
        };
        foreach (Libro libro in libros) { hash.Insertar(libro.Codigo, libro); trie.Insertar(libro.Titulo); }
    }
}