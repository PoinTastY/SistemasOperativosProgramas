using Simulacion_Procesamiento_por_Lotes.Models;

namespace Simulacion_Procesamiento_por_Lotes
{
    public partial class MainPage : ContentPage
    {
        List<Lote> lotes = new();
        Lote lote;
        Proceso proceso;
        List<string> programadores = new()
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
        private void Run()
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
            DisplayAlert("Preparado","Todos los lotes listos", "Ok");
        }

        //generate more random stuff
        private int Randomizer(int x)
        {
            //it is max+1 bcs includes min and excludes max
            var rand = new Random().Next(x);
            return rand;
        }

        
    }

}
