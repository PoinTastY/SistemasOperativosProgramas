using System.Collections.ObjectModel;// for observable collection

namespace Simulacion_Procesamiento_por_Lotes.Models
{
    public partial class Lote
    {
        //attributes
        private readonly int _capacidadMax;
        private int _procesosActuales;
        private ObservableCollection<Proceso> _procesos;

        //builder
        public Lote(int size)
        {
            _capacidadMax = size;
            _procesos = new();
            _procesosActuales = 0;
        }

        //add proceso
        public bool Add(Proceso proceso)
        {
            if (_procesosActuales < _capacidadMax)
            {
                Procesos.Add(proceso);
                _procesosActuales += 1;
                return true;
            }
            else
            {
                return false;
            }
        }

        //take first proceso
        public Proceso TakeFirst()
        {
            _procesosActuales--;//to continue w the index order
            Proceso primerProceso = Procesos[0]; // Tomar el primer proceso
            Procesos.RemoveAt(0); // Eliminar el primer proceso de la lista
            return primerProceso; // Elimina y devuelve el primer proceso
        }

        

        //returns current processes
        public ObservableCollection<Proceso> Procesos { get => _procesos; set => _procesos = value; }

        //overload to validate if lote is empty
        public static implicit operator bool(Lote lote)
        {
            return lote._procesosActuales > 0;
        }

    }
}
