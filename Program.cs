using System;
using System.Collections.Generic;

class Program
{
    static void CargarDatosBase(TablaHashLibros tablaHash, TrieLibros trie, GrafoSedes grafo, UnionFindSedes uf)
    {
        // 1. Registro de Sedes en el Grafo y Union-Find
        string[] sedes = { "Sede Central", "Sede Norte", "Sede Sur", "Sede Este", "Sede Oeste" };
        foreach (var s in sedes)
        {
            grafo.AgregarSede(s);
            uf.RegistrarSede(s);
        }

        // 2. Creación de rutas de traslado (Grafo) y unión inicial de redes (Union-Find)
        grafo.AgregarRuta("Sede Central", "Sede Norte", 15, true);
        uf.Unir("Sede Central", "Sede Norte");

        grafo.AgregarRuta("Sede Norte", "Sede Este", 25, true);
        uf.Unir("Sede Norte", "Sede Este");

        grafo.AgregarRuta("Sede Central", "Sede Sur", 20, true);
        uf.Unir("Sede Central", "Sede Sur");

        // Nota: Sede Oeste se mantiene separada inicialmente para probar Union-Find

        // 3. Carga de Libros (Tabla Hash + Trie)
        Libro[] libros = {
            new Libro("L001", "Estructuras de Datos Avanzadas", "Cormen", "Sede Central"),
            new Libro("L002", "Algoritmos en C Sharp", "John Smith", "Sede Norte"),
            new Libro("L003", "Base de Datos I", "Elmasri", "Sede Sur"),
            new Libro("L004", "Ingenieria del Software", "Pressman", "Sede Este"),
            new Libro("L005", "Redes de Computadoras", "Tanenbaum", "Sede Central")
        };

        foreach (var libro in libros)
        {
            tablaHash.Insertar(libro.Codigo, libro);
            trie.Insertar(libro.Titulo);
        }

        Console.WriteLine("Datos base cargados exitosamente en las cuatro estructuras.");
    }

    static string PedirDato(string mensaje)
    {
        Console.Write(mensaje);
        return (Console.ReadLine() ?? "").Trim();
    }

    static void Pausar()
    {
        Console.WriteLine("\nPresione Enter para continuar...");
        Console.ReadLine();
    }

    static void Main()
    {
        TablaHashLibros tablaHash = new TablaHashLibros();
        TrieLibros trie = new TrieLibros();
        GrafoSedes grafo = new GrafoSedes();
        UnionFindSedes uf = new UnionFindSedes();

        int opcion = 0;

        while (opcion != 9)
        {
            Console.Clear();
            Console.WriteLine("SISTEMA DE GESTION DE BIBLIOTECA - PRESTAMOS ENTRE SEDES");
            Console.WriteLine("1. Cargar datos de ejemplo");
            Console.WriteLine("2. Mostrar todos los libros (Tabla Hash)");
            Console.WriteLine("3. Buscar libro por codigo (Tabla Hash) o autocompletar titulo (Trie)");
            Console.WriteLine("4. Mostrar modelo de la red de sedes (Grafo)");
            Console.WriteLine("5. Calcular ruta mas rapida de traslado (Dijkstra)");
            Console.WriteLine("6. Verificar conectividad de redes antes de nuevo convenio (Union-Find)");
            Console.WriteLine("7. Ejecutar recorridos BFS y DFS en la red");
            Console.WriteLine("8. Registrar nuevo libro manualmente");
            Console.WriteLine("9. Salir");

            if (!int.TryParse(PedirDato("\nSeleccione una opcion: "), out opcion))
            {
                opcion = 0;
            }

            Console.WriteLine();

            switch (opcion)
            {
                case 1:
                    CargarDatosBase(tablaHash, trie, grafo, uf);
                    Pausar();
                    break;

                case 2:
                    Console.WriteLine("--- CATALOGO GENERAL DE LIBROS ---");
                    tablaHash.MostrarTodos();
                    Pausar();
                    break;

                case 3:
                    Console.WriteLine("1. Buscar libro exacto por Codigo");
                    Console.WriteLine("2. Autocompletar / Buscar por prefijo en el Título");
                    string subOp = PedirDato("Elija opcion de busqueda: ");
                    if (subOp == "1")
                    {
                        string codigo = PedirDato("Ingrese el codigo del libro (ej. L001): ");
                        Libro encontrada = tablaHash.Buscar(codigo);
                        if (encontrada != null)
                        {
                            Console.WriteLine("Libro encontrado: " + encontrada);
                        }
                        else
                        {
                            Console.WriteLine("No se encontro ningun libro con ese codigo.");
                        }
                    }
                    else if (subOp == "2")
                    {
                        string prefijo = PedirDato("Ingrese el prefijo o letras iniciales del titulo: ");
                        trie.BuscarPorPrefijo(prefijo);
                    }
                    else
                    {
                        Console.WriteLine("Opcion no valida.");
                    }
                    Pausar();
                    break;

                case 4:
                    grafo.RecorridoDFS("Sede Central"); // Muestra estado base del grafo
                    Pausar();
                    break;

                case 5:
                    string origen = PedirDato("Sede de origen: ");
                    string destino = PedirDato("Sede de destino: ");
                    if (grafo.ExisteSede(origen) && grafo.ExisteSede(destino))
                    {
                        grafo.RutaMasRapida(origen, destino);
                    }
                    else
                    {
                        Console.WriteLine("Una o ambas sedes no existen en el sistema.");
                    }
                    Pausar();
                    break;

                case 6:
                    string sedeA = PedirDato("Primera sede del nuevo convenio: ");
                    string sedeB = PedirDato("Segunda sede del nuevo convenio: ");
                    
                    uf.RegistrarSede(sedeA);
                    uf.RegistrarSede(sedeB);

                    if (uf.EstanConectadas(sedeA, sedeB))
                    {
                        Console.WriteLine($"Aviso: Las sedes '{sedeA}' y '{sedeB}' ya pertenecen a la misma red conectada.");
                    }
                    else
                    {
                        Console.WriteLine($"Permiso concedido: Las sedes estan en redes separadas. Uniendo redes para habilitar convenio...");
                        uf.Unir(sedeA, sedeB);
                        grafo.AgregarRuta(sedeA, sedeB, 30, true); // Ruta física de respaldo
                        Console.WriteLine("Convenio y nueva ruta de traslado registrados con exito.");
                    }
                    Pausar();
                    break;

                case 7:
                    string sedeInicio = PedirDato("Sede inicial para recorridos: ");
                    string sedeFin = PedirDato("Sede objetivo para BFS: ");
                    if (grafo.ExisteSede(sedeInicio) && grafo.ExisteSede(sedeFin))
                    {
                        grafo.RecorridoBFS(sedeInicio, sedeFin);
                        grafo.RecorridoDFS(sedeInicio);
                    }
                    else
                    {
                        Console.WriteLine("Sede inicial o destino no encontrada.");
                    }
                    Pausar();
                    break;

                case 8:
                    string cod = PedirDato("Codigo del libro: ");
                    string tit = PedirDato("Titulo del libro: ");
                    string aut = PedirDato("Autor: ");
                    string sed = PedirDato("Sede actual: ");

                    Libro nuevoLibro = new Libro(cod, tit, aut, sed);
                    tablaHash.Insertar(cod, nuevoLibro);
                    trie.Insertar(tit);
                    grafo.AgregarSede(sed);
                    uf.RegistrarSede(sed);

                    Console.WriteLine("Libro registrado e indexado correctamente en la Tabla Hash y el Trie.");
                    Pausar();
                    break;

                case 9:
                    Console.WriteLine("Finalizando aplicacion...");
                    break;

                default:
                    Console.WriteLine("Opcion invalida.");
                    Pausar();
                    break;
            }
        }
    }
}