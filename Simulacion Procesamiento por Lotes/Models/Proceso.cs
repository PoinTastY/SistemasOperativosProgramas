using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Simulacion_Procesamiento_por_Lotes.Models
{
    public partial class Proceso : ObservableObject
    {
        [ObservableProperty]
        private int _id;
        [ObservableProperty]
        private int _tmeoriginal;
        [ObservableProperty]
        private int _tme;
        [ObservableProperty]
        private string _instruccion;
        [ObservableProperty]
        private float? _resultado;
        [ObservableProperty]
        private string _programador;

        public Proceso(int id, int min, int max, string programador)
        {
            _id = id;
            _tme = Randomizer(min+1, max-1);
            _tmeoriginal = _tme;
            _programador = programador;
            _resultado = Procesamiento();
        }

        public Proceso() { }

        //get attributes

        //returns id
        //public int Id { get => _id; set => SetProperty(ref _id, value); }

        ////returns TME
        //public int Tme { get => _tme; set => SetProperty(ref _tme, value); }

        //public int TmeOriginal { get => _tmeoriginal; set => SetProperty(ref _tmeoriginal, value); }

        ////returns Instruccion
        //public string Instruccion { get => _instruccion; set => SetProperty(ref _instruccion, value); }

        ////Returns Resultado
        //public float? Resultado { get => _resultado; set => SetProperty(ref _resultado, value); }

        ////Returns assigned Programador
        //public string Programador { get => _programador; set => SetProperty(ref _programador, value); } 

        //Building Methods
        //Returns a random number between given range
        private int Randomizer(int min, int max)
        {
            //it is max+1 bcs includes min and excludes max
            var x = new Random().Next(min,max+1);
            return x;
        }

        private int? Procesamiento()
        {
            int a, b;
            a = Randomizer(1, 9);
            b = Randomizer(1, 9);
            switch (Randomizer(1, 4)){
                case 1:
                    Instruccion = $"{a} + {b}";
                    return a + b;
                case 2:
                    Instruccion = $"{a} * {b}";
                    return a * b;
                case 3:
                    Instruccion = $"{a} / {b}";
                    return a / b;
                case 4:
                    Instruccion = $"{a} - {b}";
                    return a - b;
                default:
                    return null;
            }

        }
    }
}
