﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;


public class PaginadorGenerico<T> where T : class
{
    public int PaginaActual { get; set; }
    public int RegistrosPorPagina { get; set; }
    public int TotalRegistros { get; set; }
    public int TotalPaginas { get; set; }
    public IEnumerable<T> Resultado { get; set; }
}   
