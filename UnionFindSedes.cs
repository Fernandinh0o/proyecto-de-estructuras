using System;
using System.Collections.Generic;

public class UnionFindSedes
{
    private Dictionary<string, string> padre;
    private Dictionary<string, int> rango;

    public UnionFindSedes()
    {
        padre = new Dictionary<string, string>();
        rango = new Dictionary<string, int>();
    }

    public void RegistrarSede(string sede)
    {
        sede = sede.Trim();
        if (!padre.ContainsKey(sede))
        {
            padre[sede] = sede;
            rango[sede] = 0;
        }
    }

    // Buscar con compresión de camino
    public string Buscar(string sede)
    {
        sede = sede.Trim();
        if (!padre.ContainsKey(sede)) return null;

        if (padre[sede] != sede)
        {
            padre[sede] = Buscar(padre[sede]);
        }
        return padre[sede];
    }

    // Verificar si dos sedes pertenecen a la misma red conectada
    public bool EstanConectadas(string sede1, string sede2)
    {
        string raiz1 = Buscar(sede1);
        string raiz2 = Buscar(sede2);

        if (raiz1 == null || raiz2 == null) return false;
        return raiz1 == raiz2;
    }

    // Unir dos redes de sedes mediante unión por rango
    public void Unir(string sede1, string sede2)
    {
        string raiz1 = Buscar(sede1);
        string raiz2 = Buscar(sede2);

        if (raiz1 == null || raiz2 == null) return;

        if (raiz1 != raiz2)
        {
            if (rango[raiz1] < rango[raiz2])
            {
                padre[raiz1] = raiz2;
            }
            else if (rango[raiz1] > rango[raiz2])
            {
                padre[raiz2] = raiz1;
            }
            else
            {
                padre[raiz2] = raiz1;
                rango[raiz1]++;
            }
        }
    }
}