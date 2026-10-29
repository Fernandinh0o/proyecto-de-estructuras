using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

public class BibliotecaForm : Form
{
    private TablaHashLibros hash = new();
    private TrieLibros trie = new();
    private GrafoSedes grafo = new();
    private UnionFindSedes uf = new();
    private bool cargado;
    private readonly Panel cuerpo = new() { Dock = DockStyle.Fill, Padding = new Padding(20) };
    private readonly Label titulo = new() { Dock = DockStyle.Top, Height = 66, Font = new Font("Segoe UI", 19, FontStyle.Bold), ForeColor = Color.FromArgb(35, 65, 110), TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.White };

    public BibliotecaForm()
    {
        Text = "Biblioteca - Red de sedes";
        Size = new Size(1100, 720);
        MinimumSize = new Size(800, 560);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(245, 248, 252);
        Font = new Font("Segoe UI", 10);
        Panel menu = new() { Dock = DockStyle.Left, Width = 225, BackColor = Color.FromArgb(228, 237, 249), Padding = new Padding(10) };
        Controls.Add(cuerpo);
        Controls.Add(titulo);
        Controls.Add(menu);
        var opciones = new (string, Action)[] {
            ("1. Cargar datos CSV", Cargar),
            ("2. Mostrar libros", Libros),
            ("3. Buscar libros", Buscar),
            ("4. Red de sedes", Red),
            ("5. Ruta Dijkstra", Dijkstra),
            ("6. Conectividad", Conectividad),
            ("7. BFS y DFS", Recorridos),
            ("8. Registrar libro", Registrar)
        };
        for (int i = opciones.Length - 1; i >= 0; i--)
        {
            var opcion = opciones[i];
            Button boton = new() { Text = opcion.Item1, Dock = DockStyle.Top, Height = 48, FlatStyle = FlatStyle.Flat, BackColor = Color.White, ForeColor = Color.FromArgb(30, 55, 95), TextAlign = ContentAlignment.MiddleLeft };
            boton.FlatAppearance.BorderSize = 0;
            boton.Click += (_, _) => Seguro(opcion.Item2);
            menu.Controls.Add(boton);
        }
        Button salir = new()
        {
            Text = "Salir del programa",
            Dock = DockStyle.Bottom,
            Height = 50,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(178, 49, 58),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        salir.FlatAppearance.BorderSize = 0;
        salir.Click += (_, _) => Application.Exit();
        menu.Controls.Add(salir);
        salir.BringToFront();
        Pantalla("Sistema de gestión de biblioteca");
        cuerpo.Controls.Add(new Label { Dock = DockStyle.Fill, Text = "Selecciona 'Cargar datos' para comenzar.", TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 14) });
    }

    private void Seguro(Action accion)
    {
        try { accion(); }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
    private void Pantalla(string nombre) { titulo.Text = nombre; cuerpo.Controls.Clear(); }
    private bool Listo()
    {
        if (cargado) return true;
        MessageBox.Show("Primero carga los archivos CSV.");
        return false;
    }
    private void Cargar()
    {
        using FolderBrowserDialog selector = new()
        {
            Description = "Selecciona la carpeta que contiene sedes.csv, rutas.csv y libros.csv",
            UseDescriptionForTitle = true
        };
        if (selector.ShowDialog(this) != DialogResult.OK) return;
        var datos = ImportadorCsv.Cargar(selector.SelectedPath);
        hash = datos.hash;
        trie = datos.trie;
        grafo = datos.grafo;
        uf = datos.uf;
        cargado = true;
        Libros();
        MessageBox.Show(this, "Archivos CSV cargados correctamente.", "Importación completa",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
    private void Libros()
    {
        if (!Listo()) return;
        Pantalla("Catálogo general de libros");
        DataGridView tabla = new() { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, RowHeadersVisible = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect, BackgroundColor = Color.White };
        tabla.DataSource = hash.ObtenerTodos();
        cuerpo.Controls.Add(tabla);
    }
    private static TextBox Campo(string texto) => new() { PlaceholderText = texto, Width = 350, Font = new Font("Segoe UI", 11) };
    private static Button Accion(string texto, Action accion)
    {
        Button b = new() { Text = texto, Width = 185, Height = 38, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(55, 105, 180), ForeColor = Color.White };
        b.FlatAppearance.BorderSize = 0;
        b.Click += (_, _) => accion();
        return b;
    }
    private static FlowLayoutPanel Fila() => new() { Dock = DockStyle.Top, Height = 115, Padding = new Padding(8), AutoScroll = true, WrapContents = true };
    private static string Capturar(Action accion)
    {
        TextWriter original = Console.Out;
        using StringWriter salida = new();
        try { Console.SetOut(salida); accion(); return salida.ToString(); }
        finally { Console.SetOut(original); }
    }
    private TextBox Salida(string nombre)
    {
        Pantalla(nombre);
        TextBox t = new() { Multiline = true, ReadOnly = true, Dock = DockStyle.Fill, ScrollBars = ScrollBars.Vertical, Font = new Font("Consolas", 11), BackColor = Color.White };
        cuerpo.Controls.Add(t);
        return t;
    }
    private void Buscar()
    {
        if (!Listo()) return;
        Pantalla("Buscar libros - Hash y Trie");
        FlowLayoutPanel panel = new() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true, Padding = new Padding(12) };
        TextBox codigo = Campo("Código, por ejemplo L001");
        TextBox prefijo = Campo("Prefijo del título");
        ListBox lista = new() { Width = 460, Height = 250 };
        panel.Controls.Add(codigo);
        panel.Controls.Add(Accion("Buscar por código", () => MessageBox.Show(hash.Buscar(codigo.Text.Trim())?.ToString() ?? "Libro no encontrado.")));
        panel.Controls.Add(prefijo);
        panel.Controls.Add(lista);
        Label detalle = new() { AutoSize = true, MaximumSize = new Size(650, 0), Font = new Font("Segoe UI", 11), Text = "Selecciona un título para consultar sus datos." };
        panel.Controls.Add(detalle);
        prefijo.TextChanged += (_, _) =>
        {
            lista.Items.Clear();
            if (string.IsNullOrWhiteSpace(prefijo.Text)) return;
            foreach (string sugerencia in trie.ObtenerSugerencias(prefijo.Text))
                lista.Items.Add(sugerencia);
            detalle.Text = lista.Items.Count == 0 ? "No se encontraron títulos con ese prefijo." : "Selecciona un título para consultar sus datos.";
        };
        void MostrarDetalle()
        {
            if (lista.SelectedItem == null) return;
            string seleccionado = lista.SelectedItem.ToString() ?? "";
            Libro? libro = hash.ObtenerTodos().FirstOrDefault(l => l.Titulo.Equals(seleccionado, StringComparison.OrdinalIgnoreCase));
            detalle.Text = libro?.ToString() ?? "No se encontró información para este título.";
        }
        lista.SelectedIndexChanged += (_, _) => MostrarDetalle();
        lista.DoubleClick += (_, _) => MostrarDetalle();
        cuerpo.Controls.Add(panel);
    }
    private void Red()
    {
        if (!Listo()) return;
        TextBox salida = Salida("Modelo de la red de sedes");
        salida.Text = "SEDES:" + Environment.NewLine + string.Join(Environment.NewLine, grafo.ObtenerSedes())
            + Environment.NewLine + Environment.NewLine + "RUTAS DIRIGIDAS:" + Environment.NewLine
            + string.Join(Environment.NewLine, grafo.ObtenerRutas().Select(r => $"{r.Origen} -> {r.Destino} ({r.Minutos} minutos)"));
    }
    private void Operacion(string nombre, string texto, Action<string, string, TextBox> ejecutar, bool limitarDestinos = false)
    {
        if (!Listo()) return;
        TextBox salida = Salida(nombre);
        FlowLayoutPanel panel = Fila();
        ComboBox origen = new() { Width = 195, DropDownStyle = ComboBoxStyle.DropDownList };
        ComboBox destino = new() { Width = 195, DropDownStyle = ComboBoxStyle.DropDownList };
        var sedes = grafo.ObtenerSedes().ToArray();
        origen.Items.AddRange(sedes);
        destino.Items.AddRange(sedes);
        Label aviso = new()
        {
            AutoSize = true,
            ForeColor = Color.FromArgb(85, 95, 110),
            Text = limitarDestinos
                ? "Elige un origen: solo aparecerán destinos con una ruta existente."
                : "Selecciona dos sedes."
        };
        Button ejecutarBoton = Accion(texto, () => Seguro(() =>
        {
            if (origen.SelectedItem == null || destino.SelectedItem == null)
            {
                salida.Text = "No hay un traslado disponible para la selección actual.";
                return;
            }
            string desde = origen.SelectedItem.ToString()!;
            string hasta = destino.SelectedItem.ToString()!;
            if (limitarDestinos && !grafo.ObtenerDestinosAlcanzables(desde).Contains(hasta))
            {
                salida.Text = "No existe una ruta entre las sedes seleccionadas.";
                return;
            }
            ejecutar(desde, hasta, salida);
        }));
        void ActualizarDestinos()
        {
            destino.Items.Clear();
            if (origen.SelectedItem == null) return;
            string desde = origen.SelectedItem.ToString()!;
            string[] disponibles = limitarDestinos
                ? grafo.ObtenerDestinosAlcanzables(desde).ToArray()
                : sedes;
            destino.Items.AddRange(disponibles);
            if (disponibles.Length > 0) destino.SelectedIndex = 0;
            ejecutarBoton.Enabled = disponibles.Length > 0;
            if (limitarDestinos)
            {
                aviso.Text = disponibles.Length > 0
                    ? $"Destinos disponibles desde {desde}: {disponibles.Length}."
                    : $"No hay rutas de traslado desde {desde}.";
                salida.Text = disponibles.Length > 0
                    ? "Selecciona un destino y ejecuta el recorrido."
                    : $"La sede {desde} no tiene destinos alcanzables en la red actual.";
            }
        }
        origen.SelectedIndexChanged += (_, _) => ActualizarDestinos();
        if (sedes.Length > 0) origen.SelectedIndex = 0;
        panel.Controls.Add(new Label { Text = "Origen:", AutoSize = true });
        panel.Controls.Add(origen);
        panel.Controls.Add(new Label { Text = "Destino:", AutoSize = true });
        panel.Controls.Add(destino);
        panel.Controls.Add(ejecutarBoton);
        panel.Controls.Add(aviso);
        cuerpo.Controls.Add(panel);
        panel.BringToFront();
    }
    private void Dijkstra() => Operacion("Ruta más rápida - Dijkstra", "Calcular ruta", (a, b, t) => t.Text = grafo.DescribirDijkstra(a, b), limitarDestinos: true);
    private void Recorridos()
    {
        if (!Listo()) return;
        Pantalla("BFS y DFS - Comparación de rutas");

        FlowLayoutPanel controles = Fila();
        controles.Height = 75;
        ComboBox origen = new() { Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
        ComboBox destino = new() { Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
        string[] sedes = grafo.ObtenerSedes().ToArray();
        origen.Items.AddRange(sedes);
        destino.Items.AddRange(sedes);

        TableLayoutPanel comparacion = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 2,
            Padding = new Padding(5),
            BackColor = Color.White
        };
        comparacion.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        comparacion.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        comparacion.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
        comparacion.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        Label etiquetaBfs = new()
        {
            Text = "BFS - CAMINO MÁS CORTO",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 85, 155)
        };
        Label etiquetaDfs = new()
        {
            Text = "DFS - CAMINO EN PROFUNDIDAD",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 110, 85)
        };
        TextBox resultadoBfs = new()
        {
            Multiline = true, ReadOnly = true, Dock = DockStyle.Fill,
            ScrollBars = ScrollBars.Vertical, Font = new Font("Consolas", 11),
            BackColor = Color.FromArgb(241, 247, 255)
        };
        TextBox resultadoDfs = new()
        {
            Multiline = true, ReadOnly = true, Dock = DockStyle.Fill,
            ScrollBars = ScrollBars.Vertical, Font = new Font("Consolas", 11),
            BackColor = Color.FromArgb(241, 250, 246)
        };
        comparacion.Controls.Add(etiquetaBfs, 0, 0);
        comparacion.Controls.Add(etiquetaDfs, 1, 0);
        comparacion.Controls.Add(resultadoBfs, 0, 1);
        comparacion.Controls.Add(resultadoDfs, 1, 1);

        void MostrarAmbos()
        {
            if (origen.SelectedItem == null || destino.SelectedItem == null)
            {
                resultadoBfs.Text = "Selecciona origen y destino.";
                resultadoDfs.Text = "Selecciona origen y destino.";
                return;
            }
            string desde = origen.SelectedItem.ToString()!;
            string hasta = destino.SelectedItem.ToString()!;
            string texto = grafo.DescribirBFSyDFS(desde, hasta);
            const string marcaBfs = "BFS - CAMINO MÁS CORTO (MENOS TRAMOS)";
            const string marcaDfs = "DFS - CAMINO POR PROFUNDIDAD";
            int posicionBfs = texto.IndexOf(marcaBfs, StringComparison.Ordinal);
            int posicionDfs = texto.IndexOf(marcaDfs, StringComparison.Ordinal);
            if (posicionBfs < 0 || posicionDfs < 0 || posicionDfs <= posicionBfs)
            {
                resultadoBfs.Text = texto;
                resultadoDfs.Text = "No se pudo separar el resultado DFS.";
                return;
            }
            string cabecera = $"Origen: {desde}{Environment.NewLine}Destino: {hasta}{Environment.NewLine}{Environment.NewLine}";
            resultadoBfs.Text = cabecera + texto.Substring(posicionBfs, posicionDfs - posicionBfs).Trim();
            resultadoDfs.Text = cabecera + texto.Substring(posicionDfs).Trim();
            resultadoBfs.SelectionStart = 0;
            resultadoDfs.SelectionStart = 0;
        }

        origen.SelectedIndexChanged += (_, _) => Seguro(MostrarAmbos);
        destino.SelectedIndexChanged += (_, _) => Seguro(MostrarAmbos);
        controles.Controls.Add(new Label { Text = "Origen:", AutoSize = true });
        controles.Controls.Add(origen);
        controles.Controls.Add(new Label { Text = "Destino:", AutoSize = true });
        controles.Controls.Add(destino);
        controles.Controls.Add(Accion("Mostrar ambos", () => Seguro(MostrarAmbos)));

        cuerpo.Controls.Add(comparacion);
        cuerpo.Controls.Add(controles);
        controles.BringToFront();
        if (sedes.Length > 0)
        {
            origen.SelectedIndex = 0;
            destino.SelectedIndex = 0;
        }
    }
    private void Conectividad() => Operacion("Conectividad - Union Find", "Verificar / Convenio", (a, b, t) =>
    {
        if (a == b)
        {
            string mensaje = "Has seleccionado la misma sede como origen y destino. Selecciona dos sedes diferentes.";
            t.Text = mensaje;
            MessageBox.Show(this, mensaje, "Selección de sedes", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (uf.EstanConectadas(a, b))
        {
            string mensaje = $"Las sedes {a} y {b} ya están conectadas en la misma red. No es necesario crear un convenio.";
            t.Text = mensaje;
            MessageBox.Show(this, mensaje, "Sedes conectadas", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        string pregunta = $"Las sedes {a} y {b} no están conectadas.\n\n¿Deseas establecer un convenio y agregar una ruta bidireccional de 30 minutos?";
        t.Text = $"SIN CONEXIÓN\r\n\r\n{a} y {b} pertenecen a redes separadas.";
        DialogResult respuesta = MessageBox.Show(this, pregunta, "Nuevo convenio", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (respuesta != DialogResult.Yes)
        {
            t.Text += "\r\n\r\nNo se creó ningún convenio.";
            return;
        }

        grafo.AgregarRuta(a, b, 30, true);
        uf.Unir(a, b);
        string confirmacion = $"Convenio registrado correctamente.\n\n{a} <-> {b}\nTiempo de traslado: 30 minutos.";
        t.Text = confirmacion.Replace("\n", "\r\n");
        MessageBox.Show(this, confirmacion, "Convenio registrado", MessageBoxButtons.OK, MessageBoxIcon.Information);
    });
    private void Registrar()
    {
        if (!Listo()) return;
        Pantalla("Registrar nuevo libro");
        FlowLayoutPanel panel = new() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true, Padding = new Padding(15) };
        TextBox codigo = Campo("Código");
        TextBox nombre = Campo("Título");
        TextBox autor = Campo("Autor");
        ComboBox sede = new() { Width = 350, DropDownStyle = ComboBoxStyle.DropDownList };
        sede.Items.AddRange(grafo.ObtenerSedes().ToArray());
        if (sede.Items.Count > 0) sede.SelectedIndex = 0;
        panel.Controls.Add(codigo); panel.Controls.Add(nombre); panel.Controls.Add(autor); panel.Controls.Add(sede);
        panel.Controls.Add(Accion("Guardar libro", () => Seguro(() =>
        {
            string cod = codigo.Text.Trim(), tit = nombre.Text.Trim(), aut = autor.Text.Trim();
            string sed = sede.SelectedItem?.ToString() ?? "";
            if (new[] { cod, tit, aut, sed }.Any(string.IsNullOrWhiteSpace)) { MessageBox.Show("Completa todos los campos."); return; }
            if (hash.Buscar(cod) != null) { MessageBox.Show("El código ya existe."); return; }
            Libro libro = new(cod, tit, aut, sed);
            hash.Insertar(cod, libro);
            trie.Insertar(tit);
            MessageBox.Show("Libro registrado correctamente.");
            Libros();
        })));
        cuerpo.Controls.Add(panel);
    }
}