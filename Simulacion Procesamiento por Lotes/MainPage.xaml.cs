using Simulacion_Procesamiento_por_Lotes.Models;
using System.Collections.ObjectModel;


namespace Simulacion_Procesamiento_por_Lotes
{
    public partial class MainPage : ContentPage
    {
        //generate path to export results
        private readonly static string _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Resultados Simulacion");
        private string resultados;
        private bool ticking = true;

        private TimeOnly RelojGlobal = new();
        private List<Lote> lotes = new();
        public ObservableCollection<Proceso> procesosTerminados = new();
        public ObservableCollection<Proceso> procesosPendientes = new();

        private List<string> programadores = new()
        {
            "Kevin",
            "Lucas",
            "Pepe",
            "Edgar",
            "Carmen",
            "Ninfa",
            "Caliope"
        };
        

        public MainPage()
        {
            InitializeComponent();
#if ANDROID
            Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.RequestedOrientation = Android.Content.PM.ScreenOrientation.Locked;
            Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;
#endif
            LabelSizeLote.Text = string.Format("Tamaño del lote: {0}", StepperSizeLote.Value);
            LabelMinTme.Text = string.Format("TME minimo: {0}", StepperMinTme.Value);
            LabelMaxTme.Text = string.Format("TME maximo: {0}", StepperMaxTme.Value);
            LabelTotalProcesos.Text = string.Format("Total de Procesos: {0}", StepperTotalProcesos.Value);
            ListPendings.ItemsSource = procesosPendientes;
            ListFinished.ItemsSource = procesosTerminados;
            
        }


        private void BtnEjecutar_Clicked(object sender, EventArgs e)
        {
            StepperSizeLote.IsEnabled = false;
            StepperMinTme.IsEnabled = false;
            StepperMaxTme.IsEnabled = false;
            StepperTotalProcesos.IsEnabled = false;
            BtnEjecutar.IsEnabled = false;
            BtnStop.IsEnabled = true;
            ticking = true;
            Run();
        }

        private void StepperSizeLote_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            LabelSizeLote.Text = string.Format("Tamaño del lote: {0}", e.NewValue);
        }

        private void StepperMinTme_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if(e.NewValue < StepperMaxTme.Value)
            {
                LabelMinTme.Text = string.Format("TME minimo: {0}", e.NewValue);
            }
            else
            {
                StepperMinTme.Value = StepperMaxTme.Value - 1;
            }
        }

        private void StepperMaxTme_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if(e.NewValue > StepperMinTme.Value)
            {
                LabelMaxTme.Text = string.Format("TME maximo: {0}", e.NewValue);
            }
            else
            { 
                StepperMaxTme.Value = StepperMinTme.Value + 1;
            }
        }
        private void StepperTotalProcesos_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            LabelTotalProcesos.Text = string.Format("Total de Procesos: {0}", e.NewValue);
        }

        private async void Run()
        {
            FrameGlock.IsVisible = true;
            //initialize every lote needed basing on settings
            lotes.Add(new Lote(((int)StepperSizeLote.Value)));
            int indexLote = 0;
            for(int j = 0; j < StepperTotalProcesos.Value; j++)
            {
                if(!lotes[indexLote].Add(new Proceso(j + 1, (int)StepperMinTme.Value, (int)StepperMaxTme.Value, programadores[Randomizer(7)])))
                {
                    indexLote ++;
                    lotes.Add(new Lote((int)StepperSizeLote.Value));
                    lotes[indexLote].Add(new Proceso(j + 1, (int)StepperMinTme.Value, (int)StepperMaxTme.Value, programadores[Randomizer(7)]));
                }
            }
            int totallotes = lotes.Count - 1;
            LblLotesFaltantes.Text = "Lotes Faltantes: " + totallotes;
            foreach (var lote in lotes)
            {
                
                
                while (lote)
                {
                    
                    Proceso chamba = lote.TakeFirst();
                    foreach (Proceso proceso in lote.Procesos)
                    {
                        procesosPendientes.Add(proceso);
                    }
                    while (chamba.Tme > 0 && ticking)
                    {
                        if (chamba.Tme >= 0 && ticking)
                            Ejecucion(chamba);
                        RelojGlobal = RelojGlobal.Add(TimeSpan.FromSeconds(1));
                        await Task.Delay(TimeSpan.FromSeconds(1));
                    }
                    if (!ticking)
                        return;
                    Finalizados(chamba);
                    procesosPendientes.Clear();
                    
                }
                if(totallotes != 0)
                {
                    LblLotesFaltantes.Text = "Lotes Faltantes: " + --totallotes;
                }
                else
                {
                    LblId.Text = "-";
                    LblInstruccion.Text = "-";
                    LblProgramador.Text = "-";
                    LblTME.Text = "-";
                }
            }
            resultados += $"\n\nTiempo total de ejecucion: {RelojGlobal.Minute}:{RelojGlobal.Second:00}\n";
            BtnStop.IsEnabled = false;
            EnableButtons(true);

        }

        private void EnableButtons(bool x)
        {
            BtnExportResults.IsEnabled = x;
            BtnRerun.IsEnabled = x;
        }

        private void Ejecucion(Proceso chamba)
        {
            LblRelojGlobal.Text = $"Reloj Global:{RelojGlobal.Minute}:{RelojGlobal.Second:00}";
            LblId.Text = "Proceso: " + chamba.Id.ToString();
            LblInstruccion.Text = "Instruccion: " + chamba.Instruccion;
            LblProgramador.Text = "Programador: " + chamba.Programador;
            chamba.Tme--;
            LblTME.Text = "TME restante: " + chamba.Tme.ToString();
            if(chamba.Tme == 0)
                resultados += @$"
Reloj Golbal: {RelojGlobal.Minute}:{RelojGlobal.Second:00}
Proceso: {chamba.Id}
Instruccion: {chamba.Instruccion}
Resultado: {chamba.Resultado}
Programador: {chamba.Programador}
TME: {chamba.Tme}
";
        }
        private void Finalizados(Proceso chamba)
        {
            procesosTerminados.Add(chamba);
        }

        //evento de timer
        


        //generate more random stuff
        private int Randomizer(int x)
        {
            //it is max+1 bcs includes min and excludes max
            var rand = new Random().Next(x);
            return rand;
        }

        private void ListPendings_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ListPendings.SelectedItem = null;
        }

        private void ListPendings_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

        }

        private void ListFinished_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

        }

        private void ListFinished_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ListFinished.SelectedItem = null;
        }

        private async  void BtnExportResults_Clicked(object sender, EventArgs e)
        {
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
            string ruta = _path + @"\Resultados.txt";
            try
            {
                using (StreamWriter writer = new StreamWriter(ruta, append : true))
                {
                    // Se escribe el contenido en el archivo
                    writer.WriteLine($"{DateTime.Now:G}\n" + resultados);
                }

                await DisplayAlert("Exito", "Se han exportado los Resultados.", "Ok");
                BtnExportResults.IsEnabled = false;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Se han exportado los Resultados:\n{ex}", "Ok");
                BtnExportResults.IsEnabled = false;
            }
        }

        private void BtnRerun_Clicked(object sender, EventArgs e)
        {
            StepperSizeLote.IsEnabled = true;
            StepperMinTme.IsEnabled = true;
            StepperMaxTme.IsEnabled = true;
            StepperTotalProcesos.IsEnabled = true;
            BtnEjecutar.IsEnabled = true;
            EnableButtons(false);
            procesosPendientes.Clear();
            procesosTerminados.Clear();
            LblRelojGlobal.Text = string.Empty;
            FrameGlock.IsVisible = false;
            BtnStop.IsEnabled = false;
            LblId.Text = "-";
            LblInstruccion.Text = "-";
            LblProgramador.Text = "-";
            LblTME.Text = "-";
        }

        private void BtnStop_Clicked(object sender, EventArgs e)
        {
            ticking = false;
            BtnStop.IsEnabled = false;
            EnableButtons(true);
        }
    }
}
