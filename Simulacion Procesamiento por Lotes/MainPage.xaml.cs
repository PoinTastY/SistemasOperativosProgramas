using Microsoft.Maui.Controls.PlatformConfiguration;
using Simulacion_Procesamiento_por_Lotes.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace Simulacion_Procesamiento_por_Lotes
{
    public partial class MainPage : ContentPage
    {
        //generate path to export results
        private readonly static string _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Resultados Simulacion");

        //string where we write the results
        private string resultados;

        //controlls and allows the execution of the program, no ticking no working
        private bool ticking = true;
        private bool interrupt = false;
        private bool error = false;
        Proceso chamba;

        //Reloj
        private TimeOnly RelojGlobal = new();

        //lotes array
        private List<Lote> lotes = new();

        //observable collections
        public ObservableCollection<ViewInfo> procesosTerminados = new();
        public ObservableCollection<ViewInfo> procesosPendientes = new();

        //programadores xd
        private List<string> programadores = new()
        {
            "Kevin",
            "Lucas",
            "Pepe",
            "Edgar",
            "Carmen",
            "Ninfa",
            "Caliope",
            "Carlos",
            "Collette",
            "Magdalena",
            "AMLO"
        };
        
        //main
        public MainPage()
        {
            InitializeComponent();

            //display and link data (not Relevant 4 teacher)
            LabelSizeLote.Text = string.Format("Tamaño del lote: {0}", StepperSizeLote.Value);
            LabelMinTme.Text = string.Format("TME minimo: {0}", StepperMinTme.Value);
            LabelMaxTme.Text = string.Format("TME maximo: {0}", StepperMaxTme.Value);
            LabelTotalProcesos.Text = string.Format("Total de Procesos: {0}", StepperTotalProcesos.Value);
            ListPendings.ItemsSource = procesosPendientes;
            ListFinished.ItemsSource = procesosTerminados;
            
        }


        //arrancamos la simulacion
        private async void Run()
        {
            //reloj en 0
            RelojGlobal = new();
            int tmetotal= 0;

            //initialize every lote needed basing on settings
            lotes.Add(new Lote(((int)StepperSizeLote.Value)));
            int indexLote = 0;
            for(int j = 0; j < StepperTotalProcesos.Value; j++)
            {
                if(!lotes[indexLote].Add(new Proceso(j + 1, (int)StepperMinTme.Value, (int)StepperMaxTme.Value, programadores[Randomizer(10)])))
                {
                    indexLote ++;
                    lotes.Add(new Lote((int)StepperSizeLote.Value));
                    lotes[indexLote].Add(new Proceso(j + 1, (int)StepperMinTme.Value, (int)StepperMaxTme.Value, programadores[Randomizer(1)]));
                }
            }

            //writing generated datos.txt
            string datos = $"-------------------------------------------------------\nTamaño del lote: {StepperSizeLote.Value} TME minimo: {StepperMinTme.Value} TME maximo: {StepperMaxTme.Value} Total de Procesos: {StepperTotalProcesos.Value}\n\n";
            int cantidadlotes = 0;// pa imprimir que lote es
            foreach (var lote in lotes)
            {
                datos += "Lote: " + ++cantidadlotes;
                foreach (var proceso in lote.Procesos)
                {
                    datos += @$"
{proceso.Id}. {proceso.Programador}
{proceso.Instruccion}
TME: {proceso.TmeOriginal}


";
                    tmetotal += proceso.TmeOriginal;
                }
            }

            //despues de que "datos" tenga la info generada, la mandamos a escribir
            ExportDatos(datos,tmetotal);

            //running the simulation
            int totallotes = lotes.Count - 1;
            int cuentalotes = 0;
            LblLotesFaltantes.Text = "Lotes Faltantes: " + totallotes;
            foreach (var lote in lotes)
            {
                resultados += $"Lote: {++cuentalotes}\n";
                while (lote)
                {
                    chamba = lote.TakeFirst();//tomamos los procesos en orden

                    foreach (Proceso proceso in lote.Procesos)//se llena la lista de pendientes (sin el primero que tomamos, xq se pasa directo a ejecucion)
                    {
                        ViewInfo viewinfo = new(proceso.Id, proceso.Instruccion, proceso.Programador, proceso.Tme);//usamos nuestro objeto para data binding (para xaml)
                        procesosPendientes.Add(viewinfo);
                    }
                    do
                    {
                        //si la chamba sigue siendo valida, se mantiene en ejecucion(valida if tme >= 0)
                        if (chamba.Tme >= 0 && ticking)
                            Ejecucion(chamba);
                        await Task.Delay(TimeSpan.FromSeconds(1));
                        RelojGlobal = RelojGlobal.Add(TimeSpan.FromSeconds(1));
                        if(interrupt)
                        {
                            chamba.TmeOriginal = chamba.Tme;
                            lote.Add(chamba);
                            interrupt = false;
                            break;
                        }
                        if(error)
                        {
                            resultados += @$"
{chamba.Id}. {chamba.Programador}
{chamba.Instruccion} !ERROR
";
                            error = false;
                            ViewInfo viewinfo = new(chamba.Id, chamba.Instruccion + " !ERROR", chamba.Programador, chamba.TmeOriginal);
                            procesosTerminados.Add(viewinfo);
                            break;
                        }
                    } while (chamba.Tme > 0 && ticking);//recuerda, ticking puede tronar el proceso si queremos, si no, hasta que yano haya chamba

                        if (!ticking)//si abortamos, pasa aqui y termina run();
                            return;
                    if(chamba.Tme == 0) 
                        Finalizados(chamba);//SI UNA Chamba termina, sale del while, y lo arrojamos a la lista de terminados
                    procesosPendientes.Clear();//se limpian los procesos pendientes para actulaizar la vista de los elmentos de nuevo al ciclar
                    
                }//cuando termina el lote, sale de while y vamos al siguiente
                if(totallotes != 0)
                {
                    LblLotesFaltantes.Text = "Lotes Faltantes: " + --totallotes;
                }
                else
                {//seccion de ejecucion vacia
                 //el segundo que falta, al cerrar el ultimo proceso, actualizamos el final del cronometro
                    LblRelojGlobal.Text = $"Reloj Global: {RelojGlobal.Minute}:{RelojGlobal.Second:00}";
                    LblId.Text = "-";
                    LblInstruccion.Text = "-";
                    LblProgramador.Text = "-";
                    LblTME.Text = "-";
                }
            }
            lotes.Clear();//al terminar, limpiamos el changarro, terminamos la variable de resultados, y estamos listos para exportar los resultados, cuando el usuario quiera
            resultados += $"\n\nTiempo total de ejecucion: {RelojGlobal.Minute}:{RelojGlobal.Second:00}\n";
            BtnStop.IsEnabled = false;
            EnableButtons(true);
            BtnError.IsEnabled = false;
            BtnInterrupt.IsEnabled = false;
            BtnRerun.IsEnabled = true;
        }

        private void Ejecucion(Proceso chamba)
        {
            //llenado de lbls para mostrar
            LblRelojGlobal.Text = $"Reloj Global: {RelojGlobal.Minute}:{RelojGlobal.Second:00}";
            LblId.Text = "Proceso: " + chamba.Id.ToString();
            LblInstruccion.Text = "Instruccion: " + chamba.Instruccion;
            LblProgramador.Text = "Programador: " + chamba.Programador;
            chamba.Tme--;
            LblTME.Text = "TME restante: " + chamba.Tme.ToString();

            if (chamba.Tme == 0)//si se termina la chamba, se manda a la variable que guarda los resultados
                resultados += @$"
{chamba.Id}. {chamba.Programador}
{chamba.Instruccion} = {chamba.Resultado:0.00}
";
        }

        private void Finalizados(Proceso chamba)//cuando se termina una chamba, traemos la chamba finalizada aqui
        {
            ViewInfo viewinfo = new(chamba.Id, chamba.Instruccion, chamba.Programador, chamba.TmeOriginal);
            procesosTerminados.Add(viewinfo);
        }


        //function to export generated data
        private void ExportDatos(string dato, int tme)
        {
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
            string ruta = _path + @"\Datos.txt";
            try
            {
                using (StreamWriter writer = new StreamWriter(ruta, append : true))
                {
                    // Se escribe el contenido en el archivo
                    writer.WriteLine($"\n" + dato + "-------------------------------------------------------------------------\n" + "TME total: " + tme);
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", $"No se han exportado los Datos:\n{ex}", "Ok");
            }
        }


        //event handlers

        //Ejecutar
        private void BtnEjecutar_Clicked(object sender, EventArgs e)
        {
            StepperSizeLote.IsEnabled = false;
            StepperMinTme.IsEnabled = false;
            StepperMaxTme.IsEnabled = false;
            StepperTotalProcesos.IsEnabled = false;
            BtnEjecutar.IsEnabled = false;
            BtnStop.IsEnabled = true;
            BtnInterrupt.IsEnabled = true;
            BtnError.IsEnabled = true;
            ticking = true;
            Run();// running controll function
        }

        //Settings Steppers

        //size lote
        private void StepperSizeLote_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            LabelSizeLote.Text = string.Format("Tamaño del lote: {0}", e.NewValue);
        }

        //TME min
        private void StepperMinTme_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (e.NewValue < StepperMaxTme.Value)
            {
                LabelMinTme.Text = string.Format("TME minimo: {0}", e.NewValue);
            }
            else
            {
                StepperMinTme.Value = StepperMaxTme.Value - 1;
            }
        }

        //TME max
        private void StepperMaxTme_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (e.NewValue > StepperMinTme.Value)
            {
                LabelMaxTme.Text = string.Format("TME maximo: {0}", e.NewValue);
            }
            else
            {
                StepperMaxTme.Value = StepperMinTme.Value + 1;
            }
        }

        //Total processses
        private void StepperTotalProcesos_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            LabelTotalProcesos.Text = string.Format("Total de Procesos: {0}", e.NewValue);
        }

        //export results btn
        private async void BtnExportResults_Clicked(object sender, EventArgs e)
        {
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
            string ruta = _path + @"\Resultados.txt";
            try
            {
                using (StreamWriter writer = new StreamWriter(ruta, append: true))
                {
                    // Se escribe el contenido en el archivo
                    writer.WriteLine($"---------------------------------------------------------------------------\n" + resultados + "\n----------------------------------------------------------");
                }

                await DisplayAlert("Exito", "Se han exportado los Resultados.", "Ok");
                BtnExportResults.IsEnabled = false;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se han exportado los resuldatos:\n{ex}", "Ok");
                BtnExportResults.IsEnabled = false;
            }
        }

        //Reiniciar btn
        private void BtnRerun_Clicked(object sender, EventArgs e)
        {
            StepperSizeLote.IsEnabled = true;
            StepperMinTme.IsEnabled = true;
            StepperMaxTme.IsEnabled = true;
            StepperTotalProcesos.IsEnabled = true;
            BtnEjecutar.IsEnabled = true;
            EnableButtons(false);
            BtnRerun.IsEnabled = false;
            BtnError.IsEnabled = true;
            BtnInterrupt.IsEnabled = true;
            procesosPendientes.Clear();
            procesosTerminados.Clear();
            LblRelojGlobal.Text = "Esperando...";
            BtnStop.IsEnabled = false;
            LblId.Text = "-";
            LblInstruccion.Text = "-";
            LblProgramador.Text = "-";
            LblTME.Text = "-";
        }

        //btn abort
        private void BtnStop_Clicked(object sender, EventArgs e)
        {
            ticking = false;
            BtnStop.IsEnabled = false;
            EnableButtons(true);
            BtnError.IsEnabled = false;
            BtnInterrupt.IsEnabled = false;
        }

        //lists tapping, basically dont do anything
        private void ListPendings_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ListPendings.SelectedItem = null;
        }

        private void ListPendings_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            //emptty, but if dont exist, we get crash bcs we need event handler
        }

        private void ListFinished_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            //emptty, but if dont exist, we get crash bcs we need event handler
        }

        private void ListFinished_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ListFinished.SelectedItem = null;
        }

        //private functions
        //private method 4 random stuff
        private int Randomizer(int x)
        {
            //it is max+1 bcs includes min and excludes max
            var rand = new Random().Next(x);
            return rand;
        }
        //private unneded but added method to enable/disable buttons lol
        private void EnableButtons(bool x)
        {
            BtnExportResults.IsEnabled = x;
            BtnInterrupt.IsEnabled = x;
        }

        private void BtnInterrupt_Clicked(object sender, EventArgs e)
        {
            interrupt = true;
        }

        private void BtnError_Clicked(object sender, EventArgs e)
        {
            chamba.Resultado = null;
            error = true;

        }
    }
}