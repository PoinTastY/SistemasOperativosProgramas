using Simulacion_Procesamiento_por_Lotes.Models;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

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
        private bool skip = false;
        private bool wait4Blocked = false;
        private int nuevos;
        private int nextLote;
        Proceso chamba;


        //Reloj
        private TimeOnly RelojGlobal = new();

        //lotes array
        private List<Lote> lotes = new();

        //observable collections
        public ObservableCollection<Proceso> procesosTerminados = new();
        public ObservableCollection<Proceso> procesosPendientes = new();
        public ObservableCollection<Proceso> procesosBloqueados = new();
        public ObservableCollection<Proceso> procesosNuevos = new();

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
            //LabelSizeLote.Text = string.Format("Tamaño del lote: {0}", StepperSizeLote.Value); not used 4 dis one
            LabelMinTme.Text = string.Format("TME minimo: {0}", StepperMinTme.Value);
            LabelMaxTme.Text = string.Format("TME maximo: {0}", StepperMaxTme.Value);
            LabelTotalProcesos.Text = string.Format("Total de Procesos: {0}", StepperTotalProcesos.Value);
            ListPendings.ItemsSource = procesosPendientes;
            ListFinished.ItemsSource = procesosTerminados;
            ListBlocked.ItemsSource = procesosBloqueados;
            
        }


        //arrancamos la simulacion
        private async void Run()
        {
            //reloj en 0
            RelojGlobal = new();
            int tmetotal= 0;

            //initialize every lote needed basing on settings
            lotes.Add(new Lote(5));
            int indexLote = 0;
            for(int j = 0; j < StepperTotalProcesos.Value; j++)
            {
                if(!lotes[indexLote].Add(new Proceso(j + 1, (int)StepperMinTme.Value, (int)StepperMaxTme.Value, programadores[Randomizer(10)])))
                {
                    indexLote ++;
                    lotes.Add(new Lote(5));
                    lotes[indexLote].Add(new Proceso(j + 1, (int)StepperMinTme.Value, (int)StepperMaxTme.Value, programadores[Randomizer(1)]));
                }
            }

            //writing generated datos.txt
            string datos = $"-------------------------------------------------------\nCantidad de Procesos: {StepperTotalProcesos.Value} TME minimo: {StepperMinTme.Value} TME maximo: {StepperMaxTme.Value} Total de Procesos: {StepperTotalProcesos.Value}\n\n";
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
            //int totallotes = lotes.Count - 1;
            nuevos = ((int)(StepperTotalProcesos.Value - 5));
            LblProcesosPendientes.Text = "Procesos Nuevos: " + nuevos;
            nextLote = 1;
            foreach (var lote in lotes)
            {
                //resultados += $"Lote: {++cuentalotes}\n";
                int x = 0;
                foreach(Proceso process in lote.Procesos) {
                    lote.Procesos[x++].Llegada = RelojGlobal.Second + (RelojGlobal.Minute * 60);
                }

                while (lote || wait4Blocked)
                {
                    ListBlocked.ItemsSource = procesosBloqueados;

                    if (!skip && lote)
                    {
                        chamba = lote.TakeFirst();//tomamos los procesos en orden
                        int j = 0;
                        foreach (Proceso proceso in lote.Procesos)//se llena la lista de pendientes (sin el primero que tomamos, xq se pasa directo a ejecucion)
                        {
                            procesosPendientes.Add(proceso);
                            lote.Procesos[j++].Espera++;
                        }
                        if(procesosBloqueados.Count != 0)
                            skip = true;

                    }
                    else 
                    { 
                        skip = false;
                        if (procesosBloqueados.Count != 0)
                        {
                            int i = 0;
                            foreach (Proceso proceso in lote.Procesos)//se llena la lista de pendientes (sin el primero que tomamos, xq se pasa directo a ejecucion)
                            {
                                procesosPendientes.Add(proceso);
                                lote.Procesos[i++].Espera++;
                            }
                            chamba = procesosBloqueados[0];
                            procesosBloqueados.RemoveAt(0);
                        }
                    }
                    
                    
                    if(lotes.Count > 1)
                    {
                        if (lotes[nextLote] && procesosBloqueados.Count + procesosPendientes.Count < 5)
                        {
                            //si la memoria esta disponible, tomamos un proceso de los siguientes
                            if (lotes[nextLote])
                            {
                                lote.Add(lotes[nextLote].TakeFirst());
                                lote.Procesos.Last().Llegada = RelojGlobal.Second + (RelojGlobal.Minute * 60);
                            }
                        }
                    }
                    do
                    {
                        
                        ListBlocked.ItemsSource = procesosBloqueados;
                        //si la chamba sigue siendo valida, se mantiene en ejecucion(valida if tme >= 0)
                        //logs
                        if (chamba.Respuesta == 0)
                            chamba.Respuesta = RelojGlobal.Second + (RelojGlobal.Minute * 60);



                        if (chamba.Tme >= 0 && ticking)
                        {
                            Ejecucion(chamba);
                            chamba.Servicio += 1;
                        }
                        await Task.Delay(TimeSpan.FromSeconds(1));
                        RelojGlobal = RelojGlobal.Add(TimeSpan.FromSeconds(1));
                        if(interrupt)
                        {
                            if(chamba.Tme != 0)
                                procesosBloqueados.Add(chamba);
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
                            chamba.Resultado = "ERROR";
                            procesosTerminados.Add(chamba);
                            if (nuevos != 0)
                                LblProcesosPendientes.Text = "Procesos Nuevos: " + --nuevos;
                            chamba.Finalizacion = RelojGlobal.Second + (RelojGlobal.Minute * 60);
                            break;
                        }
                        ListBlocked.ItemsSource = null;
                        if (procesosBloqueados.Count != 0)
                        {
                            int f = 0;
                            foreach (var pro in procesosBloqueados)
                            {
                                procesosBloqueados[f].Bloqueado++;
                                procesosBloqueados[f++].Espera++;
                            }
                        }

                    } while (chamba.Tme > 0 && ticking);//recuerda, ticking puede tronar el proceso si queremos, si no, hasta que yano haya chamba
                    //log
                    chamba.Finalizacion = RelojGlobal.Second + (RelojGlobal.Minute * 60);

                    if (!lote && procesosBloqueados.Count !=0)
                    {
                        //int y = 0;
                        //foreach(var pro in procesosBloqueados)
                        //{
                        //    procesosBloqueados[y].Bloqueado++;
                        //    procesosBloqueados[y++].Espera++;
                        //}
                        wait4Blocked = true;
                    }
                    else
                    {
                        int n = 0;
                        foreach (var pro in procesosBloqueados)
                        {
                            procesosBloqueados[n].Bloqueado++;
                            procesosBloqueados[n++].Espera++;
                        }
                        wait4Blocked = false;
                    }


                    if (!ticking)//si abortamos, pasa aqui y termina run();
                        return;

                    if(chamba.Tme == 0)
                    {
                        chamba.Retorno = RelojGlobal.Second + (RelojGlobal.Minute * 60);
                        Finalizados(chamba);//SI UNA Chamba termina, sale del while, y lo arrojamos a la lista de terminados
                    }
                    procesosPendientes.Clear();//se limpian los procesos pendientes para actulaizar la vista de los elmentos de nuevo al ciclar
                    ListBlocked.ItemsSource = null;
                }//cuando termina el lote, sale de while y vamos al siguiente
                nextLote++;
            }
            lotes.Clear();//al terminar, limpiamos el changarro, terminamos la variable de resultados, y estamos listos para exportar los resultados, cuando el usuario quiera
            resultados += $"\n\nTiempo total de ejecucion: {RelojGlobal.Minute}:{RelojGlobal.Second:00}\n";
            LblRelojGlobal.Text = $"Reloj Global: {RelojGlobal.Minute}:{RelojGlobal.Second:00}";
            LblId.Text = "-";
            LblInstruccion.Text = "-";
            LblProgramador.Text = "-";
            LblTME.Text = "-";
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
            LblTME.Text = "TME restante: " + chamba.Tme.ToString() + "/" + chamba.TmeOriginal.ToString();

            if (chamba.Tme == 0)//si se termina la chamba, se manda a la variable que guarda los resultados
                resultados += @$"
{chamba.Id}. {chamba.Programador}
{chamba.Instruccion} = {chamba.Resultado:0.00}
";
        }

        private void Finalizados(Proceso chamba)//cuando se termina una chamba, traemos la chamba finalizada aqui
        {
            procesosTerminados.Add(chamba);
            if(nuevos != 0)
                LblProcesosPendientes.Text = "Procesos Nuevos: " + --nuevos;
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
                using (StreamWriter writer = new StreamWriter(ruta, append : false))
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
            //StepperSizeLote.IsEnabled = false;
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
        //private void StepperSizeLote_ValueChanged(object sender, ValueChangedEventArgs e)
        //{
        //    LabelSizeLote.Text = string.Format("Tamaño del lote: {0}", e.NewValue);
        //}

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
                using (StreamWriter writer = new StreamWriter(ruta, append: false))
                {
                    // Se escribe el contenido en el archivo
                    writer.WriteLine($"---------------------------------------------------------------------------\n" + resultados + "\n----------------------------------------------------------");
                    //List<Proceso> procesos = new();
                    //foreach(Proceso pro in procesosTerminados)
                    //{
                    //    procesos.Add(pro);
                    //}
                    writer.WriteLine(GenLogTable(procesosTerminados));
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
            //StepperSizeLote.IsEnabled = true;
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

        private string GenLogTable(ObservableCollection<Proceso> data)
        {
            string tabla = "ID  Llegada  Finalizacion  Retorno  Respuesta  Espera  Bloqueado  Servicio\n";
            foreach (Proceso proceso in data.OrderBy(p => p.Id))
            {
                tabla += $"{proceso.Id,-4} {proceso.Llegada,-9} {proceso.Finalizacion,-13} {proceso.Retorno,-8} {proceso.Respuesta,-10} {proceso.Espera,-7} {proceso.Bloqueado,-10} {proceso.Servicio}\n";
            }
            return tabla;
        }
    }
}