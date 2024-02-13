using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Simulacion_Procesamiento_por_Lotes.Models
{
    public partial class Lote
    {
        private readonly int _capacidadMax;
        private int _procesosActuales;
        private ObservableCollection<Proceso> _procesos;

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

        public Proceso TakeFirst()
        {
            _procesosActuales--;
            // Si hay procesos restantes en la lista:
            Proceso primerProceso = Procesos[0]; // Tomar el primer proceso
            Procesos.RemoveAt(0); // Eliminar el primer proceso de la lista
            return primerProceso; // Elimina y devuelve el primer proceso
        }

        public ObservableCollection<Proceso> Procesos { get => _procesos; set => _procesos = value; }


        //overload to validate if lote is empty
        public static implicit operator bool(Lote lote)
        {
            return lote._procesosActuales != 0;
        }

    }
}
