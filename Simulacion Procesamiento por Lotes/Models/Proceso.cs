namespace Simulacion_Procesamiento_por_Lotes.Models
{
    public partial class Proceso
    {
        //attributes
        private int _id;
        private int _tmeoriginal;
        private int _tme;
        private string _instruccion;
        private string _resultado;
        private string _programador;
        private int _bloquado;

        //builders
        public Proceso(int id, int min, int max, string programador)
        {
            _id = id;
            _tme = Randomizer(min+1, max-1);
            _tmeoriginal = _tme;
            _programador = programador;
            _resultado = Procesamiento();
            _bloquado = 0;
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
        public string Resultado { get => _resultado; set => _resultado = value; }

        //Returns assigned Programador
        public string Programador { get => _programador; set => _programador = value; }

        //how much time in bloqueado
        public int Bloqueado { get => _bloquado; set => _bloquado = value; }


        public int Llegada { get; set; } = 0;
        public int Finalizacion { get; set; } = 0;
        public int Retorno { get; set; } = 0;
        public int Respuesta { get; set; } = 0;
        public int Espera { get; set; } = 0;
        public int Servicio { get; set; } = 0;

        //Building Methods
        //Returns a random number between given range
        private int Randomizer(int min, int max)
        {
            //it is max+1 bcs includes min and excludes max
            var x = new Random().Next(min,max+1);
            return x;
        }
        
        //Returns a random operation, and also the result
        private string Procesamiento()
        {
            float a, b;
            a = Randomizer(1, 9);
            b = Randomizer(1, 9);
            switch (Randomizer(1, 4)){
                case 1:
                    Instruccion = $"{a} + {b}";
                    return $"{a + b}";
                case 2:
                    Instruccion = $"{a} * {b}";
                    return $"{a * b}";
                case 3:
                    Instruccion = $"{a} / {b}";
                    return $"{a / b}";
                case 4:
                    Instruccion = $"{a} - {b}";
                    return $"{a - b}";
                default:
                    return null;
            }

        }
    }
}
