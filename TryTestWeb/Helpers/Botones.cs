using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TryTestWeb.Helpers
{
    public static class Botones
    {
        
        public static MvcHtmlString BtnSeleccionar(this HtmlHelper helper, string Id, string name, string clase, string href) {

            string html;


            html = "<button class= 'btn btn-primary "+clase+"' id=" + Id + 
                " name=" + name + "><a href=" + href + 
                "><span style='color:white'>Seleccionar</span></a></button>";

            return new MvcHtmlString(html);
        }

        public static MvcHtmlString BtnBuscar(this HtmlHelper helper, string Id, string name, string clase, string onclick) {

            string html="";

            if (onclick == "")
            {

                html = "<button type = 'submit' class = 'btn btn-success " + clase + "' id = '" + Id + "' name = '" + name + "'><span class = 'btn-label '>" +
                "<i class = 'glyphicon glyphicon-search'></i></span>Buscar</button>";

            }
            else {

                html = "<button type = 'submit' class = 'btn btn-success " + clase + "' onclick='"+onclick+"'><span class = 'btn-label '>" +
                   "<i class = 'glyphicon glyphicon-search'></i></span>Buscar</button>";
            }

            

            return new MvcHtmlString(html);
        }

        public static MvcHtmlString BtnGuardar(this HtmlHelper helper, string Id, string name, string clase, string onclick)
        {

            string html;

            html = "<button class = 'btn btn-primary waves-effect waves-light " + clase +
                "' id = '" + Id + "' name = '" + name + "' onclick = '" + onclick +
                "' type = 'submit'><span class='btn-label'><i class='glyphicon glyphicon-floppy-disk'>" +
                "</i></span>Guardar</button>";

            return new MvcHtmlString(html);
        }

        public static MvcHtmlString BtnEditar(this HtmlHelper helper, string Id, string name, string clase, string href, string onclick, string nombre)
        {
            string html="";

            if (onclick == "")
            {

                html = "<div class='table-detail' name ='" + name + "' id='" + Id + "'>" +
                    "<a class = 'btn btn-primary " + clase + "' data-toggle='tooltip' data-placement='bottom' " +
                    "href='" + href + "' title='Editar "+nombre+" ' data-original-title='Editar "+nombre+"'>" +
                    "<span class='glyphicon glyphicon-edit'></span></a></div>";
            }
            else {

                html = "<div class='table-detail' name ='" + name + "' id='" + Id + "'>" +
                   "<a class = 'btn btn-primary " + clase + "' onclick='"+onclick+"' data-toggle='tooltip' data-placement='bottom' " +
                   "title='Editar " + nombre + " ' data-original-title='Editar " + nombre + "'>" +
                   "<span class='glyphicon glyphicon-edit'></span></a></div>";

            }

            

            return new MvcHtmlString(html);
        }
        public static MvcHtmlString BtnBorrar(this HtmlHelper helper,string Id, string name, string href, string onclick, string clases)
        {

            string html;

            html = "<div class='table-detail'>" +
                "<a href='"+href+"' id='"+Id+"' name='"+name+"' onclick='"+onclick+
                "' class='btn btn-danger "+clases+"'><span class='glyphicon glyphicon-trash'></span></a></div>";
            

            return new MvcHtmlString(html);
        }
        public static MvcHtmlString BtnAgregar(this HtmlHelper helper, string href, string onclick)
        {
            string html="";
            if (onclick == "")
            {

                html = "<a href='" + href + "' class='btn btn-success waves-effect waves-light pull-right'" +
                    "data-animation='fadein' data-plugin='custommodal' data-overlaySpeed = '200' data-overlayColor = '#36404a'>" +
                    "<span class='btn-label'><i class= 'ion-android-add'></i></span>Agregar</a>";
            }
            else {

                html = "<a onclick = '"+onclick+"' class='btn btn-success waves-effect waves-light pull-right'" +
                    "data-animation='fadein' data-plugin='custommodal' data-overlaySpeed = '200' data-overlayColor = '#36404a'>" +
                    "<span class='btn-label'><i class= 'ion-android-add'></i></span>Agregar</a>";

            }
            
           
            return new MvcHtmlString(html);

        }

        public static MvcHtmlString BtnExportar(this HtmlHelper helper, string model, string name) {
            
            string html;

            html = "<button "+model+" class='btn btn-primary waves-effect waves-light pull-right' " +
                "type='submit' name='"+name+"' data-animation='fadein' data-plugin='custommodal' " +
                "data-overlaySpeed='200' data-overlayColor='#36404a'><span class='btn-label'>" +
                "<i class='fa fa-file-excel-o'></i></span>Exportar</button>";

            return new MvcHtmlString(html);
        }

        public static MvcHtmlString BtnVolver(this HtmlHelper helper, string href) {

            string html;

            html = "<a href = '"+href+"' class= 'btn btn-primary waves-effect waves-light'><span class='btn-label'>" +
                "<i class='ion-android-arrow-back'></i></span>Volver</a>";

            return new MvcHtmlString(html);
        }

        public static MvcHtmlString BtnExportarPDF(this HtmlHelper helper, string onclick) {

            string html;

            html = "<button class='btn btn-default' onclick='"+onclick+
                "'><span class='btn-label'><i class='fa fa-file-pdf-o'></i></span>ExportarPDF</button>";


            return new MvcHtmlString(html);
        }
    }
}