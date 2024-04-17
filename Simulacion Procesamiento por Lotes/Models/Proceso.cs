namespace Simulacion_Procesamiento_por_Lotes.Models
{
    public partial class Proceso
    {
        //attributes
        private int _id;
        private int _tmeoriginal;
        private int _tme;
        private string _instruccion;
        private float? _resultado;
        private string _programador;

        //builders
        public Proceso(int id, int min, int max, string programador)
        {
            _id = id;
            _tme = Randomizer(min+1, max-1);
            _tmeoriginal = _tme;
            _programador = programador;
            _resultado = Procesamiento();
        }
        public Proceso() { }

        //public acces
        //returns id
        public int Id { get => _id; set => _id = value; }

        //returns TME
        public int Tme { get => _tme; set => _tme = value; }

        public int TmeOriginal { get => _tmeoriginal; set => _tmeoriginal = value; }

        //returns Instruccion
        public string Instruccion { get => _instruccion; set => _instruccion = value; }

        //Returns Resultado
        public float? Resultado { get => _resultado; set => _resultado = value; }

        //Returns assigned Programador
        public string Programador { get => _programador; set => _programador = value; }

        //Building Methods
        //Returns a random number between given range
        private int Randomizer(int min, int max)
        {
            //it is max+1 bcs includes min and excludes max
            var x = new Random().Next(min,max+1);
            return x;
        }
        
        //Returns a random operation, and also the result
        private float Procesamiento()
        {
            float a, b;
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
                    return 0;
            }

        }
    }
}
