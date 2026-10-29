using System;
using System.IO;
using System.Collections.Generic;

public static class ImportadorCsv
{
    public static (TablaHashLibros hash, TrieLibros trie, GrafoSedes grafo, UnionFindSedes uf) Cargar(string carpeta)
    {
        TablaHashLibros hash = new();
        TrieLibros trie = new();
        GrafoSedes grafo = new();
        UnionFindSedes uf = new();
        foreach (string[] fila in Leer(Path.Combine(carpeta, "sedes.csv"), 1))
        {
            if (grafo.ExisteSede(fila[0])) throw new FormatException("Sede repetida: " + fila[0]);
            grafo.AgregarSede(fila[0]);
            uf.RegistrarSede(fila[0]);
        }
        foreach (string[] fila in Leer(Path.Combine(carpeta, "rutas.csv"), 4))
        {
            if (!grafo.ExisteSede(fila[0]) || !grafo.ExisteSede(fila[1]))
                throw new FormatException("La ruta menciona una sede inexistente.");
            if (!int.TryParse(fila[2], out int minutos) || minutos < 0)
                throw new FormatException("Minutos de ruta inválidos.");
            string via = fila[3].Trim().ToLowerInvariant();
            if (via != "true" && via != "false" && via != "si" && via != "no" && via != "1" && via != "0")
                throw new FormatException("DobleVia debe ser true/false, si/no o 1/0.");
            grafo.AgregarRuta(fila[0], fila[1], minutos, via == "true" || via == "si" || via == "1");
            uf.Unir(fila[0], fila[1]);
        }
        foreach (string[] fila in Leer(Path.Combine(carpeta, "libros.csv"), 4))
        {
            if (!grafo.ExisteSede(fila[3])) throw new FormatException("Sede del libro inexistente.");
            if (hash.Buscar(fila[0]) != null) throw new FormatException("Código repetido: " + fila[0]);
            Libro libro = new(fila[0], fila[1], fila[2], fila[3]);
            hash.Insertar(libro.Codigo, libro);
            trie.Insertar(libro.Titulo);
        }
        return (hash, trie, grafo, uf);
    }

    private static IEnumerable<string[]> Leer(string archivo, int columnas)
    {
        if (!File.Exists(archivo)) throw new FileNotFoundException("Falta " + Path.GetFileName(archivo));
        int numero = 0;
        foreach (string linea in File.ReadLines(archivo))
        {
            numero++;
            if (string.IsNullOrWhiteSpace(linea)) continue;
            if (numero == 1) continue; // Primera línea: encabezados.
            string[] campos = Separar(linea);
            if (campos.Length != columnas) throw new FormatException("Columnas incorrectas en " + archivo + ", línea " + numero);
            for (int i = 0; i < campos.Length; i++)
            {
                campos[i] = campos[i].Trim();
                if (campos[i].Length == 0) throw new FormatException("Campo vacío en " + archivo + ", línea " + numero);
            }
            yield return campos;
        }
    }

    private static string[] Separar(string linea)
    {
        List<string> resultado = new();
        System.Text.StringBuilder campo = new();
        bool comillas = false;
        for (int i = 0; i < linea.Length; i++)
        {
            char c = linea[i];
            if (c == '"')
            {
                if (comillas && i + 1 < linea.Length && linea[i + 1] == '"') { campo.Append('"'); i++; }
                else comillas = !comillas;
            }
            else if (c == ',' && !comillas) { resultado.Add(campo.ToString()); campo.Clear(); }
            else campo.Append(c);
        }
        if (comillas) throw new FormatException("Comillas CSV sin cerrar.");
        resultado.Add(campo.ToString());
        return resultado.ToArray();
    }
}
