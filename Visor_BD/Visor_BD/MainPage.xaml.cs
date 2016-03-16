using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using SQLite.Net.Attributes;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;

// La plantilla de elemento Página en blanco está documentada en http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Visor_BD
{
    /// <summary>
    /// Página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>

    public sealed partial class MainPage : Page
    {
        string URLImageDefault = "http://assets.pokemon.com/assets/cms2/img/championship/card_not_found.png";
        string UrlImagen = "http://assets.pokemon.com/assets/cms2-es-es/img/cards/web/";
        string path;
        SQLite.Net.SQLiteConnection conn;
        public MainPage()
        {
            this.InitializeComponent();

            /**
            Conexión a la base de datos
            */
            path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "DBCartas.db");
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);


            /**
            Carga de la imagen por defecto al inicio del programa.
            */
            Uri Defuri = new Uri(URLImageDefault, UriKind.Absolute);
            ImageSource imgSourceDef = new BitmapImage(Defuri);
            Imagen_Carta.Source = imgSourceDef;

            /**
            Relleno del ComboBox NombreEdicion.
            */
            var queryEdicion = conn.Table<Edicion>();
            foreach (var item in queryEdicion)
            {
                NombreEdicion.Items.Add(item.Nombre);
            }
            NombreEdicion.SelectedIndex = 0;

        }

        /** 
        Declaro las diversas tablas de la BD
        */
        public class Edicion
        {
            [PrimaryKey]
            public string Id { get; set; }

            [NotNull]
            public string Nombre { get; set; }

            [NotNull]
            public int Cantidad { get; set; }

            [NotNull]
            public int Meta { get; set; }
        }

        public class Cartas
        {
            [PrimaryKey, AutoIncrement]
            public int ID { get; set; }

            [NotNull]
            public int NumCarta { get; set; }

            [NotNull]
            public string Nombre { get; set; }

            public string Tipo { get; set; }

            public string Fase { get; set; }

            public int PS { get; set; }

            public string Habilidad { get; set; }

            public string Atk1 { get; set; }

            public string Atk2 { get; set; }

            public string Atk3 { get; set; }

            public string Debilidad { get; set; }

            public string Resistencia { get; set; }

            public string Retirada { get; set; }

            [NotNull]
            public string Edicion { get; set; }

            public string IURL { get; set; }

            [NotNull]
            public int Tiene { get; set; }
        }

        //Se ejecuta el cambio de los numeros segun edicion
        private void Cambio_Edicion(object sender, SelectionChangedEventArgs e)
        {
            NumCarta.Items.Clear();
            String EdicAct = NombreEdicion.SelectedItem.ToString();
            var queryEdicion = conn.Table<Edicion>();
            int CantCart = 0;
            foreach (var item in queryEdicion)
            {
                if (item.Nombre == EdicAct) { CantCart = item.Cantidad; }
            }
            for (int i = 1; i <= CantCart; i++) { NumCarta.Items.Add(i); }
            NumCarta.SelectedIndex = 0;
        }

        //Se ejecuta el cambio de imagen segun el numero y edicion
        private void Cambio_Carta(object sender, SelectionChangedEventArgs e)
        {
            string IdEdicion = string.Empty;

            string NumCar = "1";
            if (NumCarta.SelectedIndex >= 0) { NumCar = NumCarta.SelectedItem.ToString(); }
            var queryEdicion = conn.Table<Edicion>();
            foreach (var item in queryEdicion)
            {
                if (item.Nombre == NombreEdicion.SelectedItem.ToString()) { IdEdicion = item.Id; }
            }
            string UrlImgDef = UrlImagen + IdEdicion + "/" + IdEdicion + "_ES_" + NumCar + ".png";
            Uri uri = new Uri(UrlImgDef, UriKind.Absolute);
            ImageSource imgSource = new BitmapImage(uri);
            Imagen_Carta.Source = imgSource;
            Cargar_carta(NumCar);
        }

        private void Cargar_carta(string n)
        {
            string EdiAct = string.Empty;
            string NumAct = n;
            string Nombre = string.Empty;
            string Fase = string.Empty;
            int PS = 0;
            string Tipo = string.Empty;
            string Habilidad = string.Empty;
            string Atk_1 = string.Empty;
            string Atk_2 = string.Empty;
            string Atk_3 = string.Empty;
            string Debilidad = string.Empty;
            string Resistencia = string.Empty;
            string Retirada = string.Empty;
            //var queryEdicion = conn.Table<Edicion>();
            var queryCartas = conn.Table<Cartas>();
            var queryEdicion = conn.Table<Edicion>();

            foreach (var itemE in queryEdicion)
            {
                if (NombreEdicion.SelectedItem.ToString() == itemE.Nombre) { EdiAct = itemE.Id; break; }
            }

            foreach (var item in queryCartas)
            {
                if (EdiAct == item.Edicion & NumAct == item.NumCarta.ToString())
                {
                    Nombre = item.Nombre;
                    Fase = item.Fase;
                    PS = item.PS;
                    Tipo = item.Tipo;
                    Habilidad = item.Habilidad;
                    Atk_1 = item.Atk1;
                    Atk_2 = item.Atk2;
                    Atk_3 = item.Atk3;
                    Debilidad = item.Debilidad;
                    Resistencia = item.Resistencia;
                    Retirada = item.Retirada;
                    break;
                }
            }
            //if (Habilidad != null) { this.Habilidad.Text = Habilidad; } else { this.Habilidad.Text = string.Empty; }
            if (Nombre != null) { NombreCar.Text = Nombre; } else { NombreCar.Text = string.Empty; }

            if (Fase == Basico.Name)
            {
                Basico.IsSelected = true;
            }
            else if (Fase == Fase_1.Name)
            {
                Fase_1.IsSelected = true;
            }
            else if (Fase == Fase_2.Name)
            {
                Fase_2.IsSelected = true;
            }
            else if (Fase == EX.Name)
            {
                EX.IsSelected = true;
            } else if (Fase == Mega.Name)
            {
                Mega.IsSelected = true;
            } else if (Fase == Turbo.Name)
            {
                Turbo.IsSelected = true;
            } else if (Fase == Obeto.Name)
            {
                Obeto.IsSelected = true;
            } else if (Fase == Partidario.Name)
            {
                Partidario.IsSelected = true;
            } else if (Fase == Energia.Name)
            {
                Energia.IsSelected = true;
            }
            else { Basico.IsSelected = true; }

            this.PS.Text = PS.ToString();

            if (Tipo == Incolora.Name)
            {
                Incolora.IsSelected = true;
            } else if (Tipo == Fuego.Name)
            {
                Fuego.IsSelected = true;
            } else if (Tipo == Agua.Name)
            {
                Agua.IsSelected = true;
            } else if (Tipo == Hoja.Name)
            {
                Hoja.IsSelected = true;
            } else if (Tipo == Electrico.Name)
            {
                Electrico.IsSelected = true;
            } else if (Tipo == Lucha.Name)
            {
                Lucha.IsSelected = true;
            } else if (Tipo == Psiquico.Name)
            {
                Psiquico.IsSelected = true;
            } else if (Tipo == Siniestro.Name)
            {
                Siniestro.IsSelected = true;
            } else if (Tipo == Dragon.Name)
            {
                Dragon.IsSelected = true;
            } else if (Tipo == Acero.Name)
            {
                Acero.IsSelected = true;
            } else if (Tipo == Hada.Name)
            {
                Hada.IsSelected = true;
            }
            else { Incolora.IsSelected = true; }

            if (Habilidad != null) { this.Habilidad.Text = Habilidad; } else { this.Habilidad.Text = string.Empty; }
            if (Atk_1 != null) { Atk1.Text = Atk_1; } else { this.Atk1.Text = string.Empty; }
            if (Atk_2 != null) { Atk2.Text = Atk_2; } else { this.Atk2.Text = string.Empty; }
            if (Atk_3 != null) { Atk3.Text = Atk_3; } else { this.Atk3.Text = string.Empty; }
            if (Debilidad != null) { this.Debilidad.Text = Debilidad; } else { this.Debilidad.Text = string.Empty; }
            if (Resistencia != null) { this.Resistencia.Text = Resistencia; } else { this.Resistencia.Text = string.Empty; }
            if (Retirada != null) { this.Retirada.Text = Retirada;} else { this.Retirada.Text = string.Empty; }
        }

        private void Click_Aceptar(object sender, RoutedEventArgs e)
        {

        }

        private void Sincronizar(object sender, RoutedEventArgs e)
        {

        }
    }
}