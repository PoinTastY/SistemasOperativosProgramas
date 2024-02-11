using System.Collections.ObjectModel;

namespace Simulacion_Procesamiento_por_Lotes.Models
{
    public class Proceso
    {
        private int _id;
        private int _tmeoriginal;
        private int _tme;
        private string _instruccion;
        private readonly float? _resultado;
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
        public int Id { get => _id; set => _id = value; }

        //returns TME
        public int Tme { get => _tme; set => _tme = value; }

        public int TmeOriginal { get => _tmeoriginal; }

        //returns Instruccion
        public string Instruccion { get => _instruccion; set => _instruccion = value; }

        //Returns Resultado
        public float? Resultado { get => _resultado; }

        //Returns assigned Programador
        public string Programador { get => _programador; } 

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
                    _instruccion = $"{a} + {b}";
                    return a + b;
                case 2:
                    _instruccion = $"{a} * {b}";
                    return a * b;
                case 3:
                    _instruccion = $"{a} / {b}";
                    return a / b;
                case 4:
                    _instruccion = $"{a} - {b}";
                    return a - b;
                default:
                    return null;
            }

        }
    }
}
