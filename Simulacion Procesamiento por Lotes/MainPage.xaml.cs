using Simulacion_Procesamiento_por_Lotes.Models;
using System.Collections.ObjectModel;


namespace Simulacion_Procesamiento_por_Lotes
{
    public partial class MainPage : ContentPage
    {

        private TimeOnly RelojGlobal = new();
        private List<Lote> lotes = new();
        ObservableCollection<Proceso> procesosTerminados = new();
        ObservableCollection<Proceso> procesosPendientes = new();

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

            LabelSizeLote.Text = string.Format("Tamaño del lote: {0}", StepperSizeLote.Value);
            LabelMinTme.Text = string.Format("TME minimo: {0}", StepperMinTme.Value);
            LabelMaxTme.Text = string.Format("TME maximo: {0}", StepperMaxTme.Value);
            LabelTotalProcesos.Text = string.Format("Total de Procesos: {0}", StepperTotalProcesos.Value);
            ListFinished.ItemsSource = procesosTerminados;
            ListPendings.ItemsSource = procesosPendientes;
        }

        private void BtnEjecutar_Clicked(object sender, EventArgs e)
        {
            StepperSizeLote.IsEnabled = false;
            StepperMinTme.IsEnabled = false;
            StepperMaxTme.IsEnabled = false;
            StepperTotalProcesos.IsEnabled = false;
            BtnEjecutar.IsEnabled = false;
            BtnEjecutar.IsVisible = false;
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
            //start the timer
            foreach (var lote in lotes)
            {
                while (lote)
                {
                    Proceso chamba = lote.TakeFirst();
                    procesosPendientes.Remove(chamba);
                    while (chamba.Tme >= 0)
                    {
                        RelojGlobal = RelojGlobal.Add(TimeSpan.FromSeconds(1));
                        if(chamba != null)
                            Ejecucion(chamba);
                        await Task.Delay(TimeSpan.FromSeconds(1));
                    }
                }
            }
        }

        private void Ejecucion(Proceso chamba)
        {
            LblRelojGlobal.Text = $"Reloj global:{RelojGlobal.Minute}:{RelojGlobal.Second:00}";
            LblId.Text = chamba.Id.ToString();
            LblInstruccion.Text = "Instruccion: " + chamba.Instruccion;
            LblProgramador.Text = "Programador: " + chamba.Programador;
            LblTME.Text = "TME restante: " + chamba.Tme--.ToString();
            if (chamba.Tme == 0)
                Finalizados(chamba);
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
    }

}
